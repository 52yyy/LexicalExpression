using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LexicalExpression
{
	public class LexicalExpressionMatcher : ILexicalExpressionMatcher
	{
		public LexicalExpressionMatcher(Rule rule)
		{
			this._rule = rule;
		}

		private Rule _rule;


		public List<Dictionary<int, int[]>> MatchValues = new List<Dictionary<int, int[]>>();
		public Dictionary<int, int[]> curMatchValue = new Dictionary<int, int[]>();



		#region ����Ƿ�ƥ�� bool Match(Sentence sent)
		public bool Match(LexSentence sent)
		{
			return this.Match(sent, 0);
		}


		public bool Match(LexSentence sent, int startIndex)
		{
			this.MatchValues = new List<Dictionary<int, int[]>>();
			this.curMatchValue = new Dictionary<int, int[]>();
			int wordPos = startIndex;
			while (wordPos < sent.Words.Count)
			{
				if (this.MatchOneRuleForSentence(0, sent, wordPos))
				{
					this.curMatchValue = this.curMatchValue.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
					this.MatchValues.Add(this.curMatchValue);
					int lastMatchIndex = -1;
					for (int dicPos = this.curMatchValue.Count - 1; dicPos >= 0; dicPos--)
					{
						lastMatchIndex = this.curMatchValue.ElementAt(dicPos).Value[1];
						if (lastMatchIndex != -1)
						{
							break;
						}
					}
					wordPos = lastMatchIndex + 1;
					this.curMatchValue = new Dictionary<int, int[]>();
				}
				else
				{
					wordPos++;
					continue;
				}
			}
			if (this.MatchValues.Count == 0)
			{
				this.curMatchValue = new Dictionary<int, int[]>();
				return false;
			}
			else
			{
				return true;
			}

		}
		#endregion

		#region һ��ƥ�� bool matchOneRuleForSentence(int startRuleId, Sentence sentence, int startPos)
		public bool MatchOneRuleForSentence(int startRuleId, LexSentence sentence, int startPos)
		{
			bool isMatch;
			//this.MatchValues = new Dictionary<int, int[]>();
			// �ݹ�ƥ����������������ȫ���ڵ㶼�Ѿ�ƥ��ɹ������Է��� true
			if (startRuleId >= this._rule.RuleLeft.ruleItemList.Count)
			{
				return true;
			}
			// �ݹ�ƥ������������Ѿ�ƥ�䵽����ĩβ�������й�����δƥ�䣬���Է��� false 
			if (startPos >= sentence.Words.Count)
			{
				return false;
			}
			// ���ڴ�ƥ��Ĺ�����
			RuleItem ruleItem = this._rule.RuleLeft.ruleItemList[startRuleId];

			if (ruleItem.RuleKind == Constants.BasicRuleItem)
			{
				// -------- 1) �������ƥ�� -------------
				//
				if (ruleItem.Match(sentence, startPos) != -1)
				{
					// �����ǰ��ƥ��
					// ��¼�뵱ǰ������ƥ��ĵ��ʽڵ����ţ������б��е��±꣩
					if (this.curMatchValue.ContainsKey(startRuleId))
					{
						this.curMatchValue[startRuleId] = new int[] { startPos, startPos };
					}
					else
					{
						this.curMatchValue.Add(startRuleId, new int[] { startPos, startPos });
					}

					// �ݹ�ƥ����һ��������
					return this.MatchOneRuleForSentence(startRuleId + 1, sentence, startPos + 1);
				}
				else
				{
					return false;   // ��ǰ��������һ���������ǰ������֮��ƥ�䣬�ʶ����������� startPos λ�ò�ƥ��
				}
			}
			else if (ruleItem.RuleKind == Constants.SkipRuleItem)
			{
				// �Զ��������ԣ���ǰ����ϵͳֻ�л������Խ����
				// -------- 2) Խ�����ƥ�� -------------
				// 
				SkipRuleItem skipRuleItem = (SkipRuleItem)ruleItem._ruleItem; // Խ�������ڵ�
				int low = skipRuleItem.Low;                    // ���Խ����
				int high = skipRuleItem.High;                  // ���Խ����

				if (low == 1 && high == 1)
				{
					//  2.1 )Խ����ֻ��1��

					isMatch = skipRuleItem.Match(sentence, startPos) == -1 ? false : true;
					if (!isMatch)
					{
						return false;
					}
					// ��¼�뵱ǰԽ����ƥ��ĵ��ʽڵ����ţ������б��е��±꣩
					if (this.curMatchValue.ContainsKey(startRuleId))
					{
						this.curMatchValue[startRuleId] = new int[] { startPos, startPos };
					}
					else
					{
						this.curMatchValue.Add(startRuleId, new int[] { startPos, startPos });
					}


					// �ݹ�ƥ����һ��������
					return this.MatchOneRuleForSentence(startRuleId + 1, sentence, startPos + 1);

				}
				else if (high >= low)
				{
					//  2.2 )Խ������� 1 ��

					// ȷ������Խ��������ʵ������
					int numItemsCanSkiped = 0;
					while (numItemsCanSkiped < high && numItemsCanSkiped < sentence.Words.Count)
					{
						isMatch = (skipRuleItem.Match(sentence, startPos + numItemsCanSkiped) == -1
									|| skipRuleItem.Match(sentence, startPos + numItemsCanSkiped) == startPos + numItemsCanSkiped) ? false : true;

						if (isMatch == false)
						{
							break;
						}
						numItemsCanSkiped++;
					}

					// ���ʵ�ʿ�Խ��������С�����Խ��������ù����� startPos λ�ò�ƥ�䣬���� false
					if (numItemsCanSkiped < low)
					{
						return false;
					}
					// ����ֱ�Խ���� low �� numItemsCanSkiped( low <= X <= high)���������ݹ�ƥ���������ಿ��
					// ȷ����Խ�����������������õ�ƥ��

					isMatch = false;
					int numRealSkiped = low;
					while (numRealSkiped <= numItemsCanSkiped)
					{
						if (startRuleId == this._rule.RuleLeft.ruleItemList.Count - 1
							&& (skipRuleItem.Match(sentence, startPos + numRealSkiped) != -1 && skipRuleItem.Match(sentence, startPos + numRealSkiped) != startPos + numRealSkiped))
						{
							isMatch = true;
							numRealSkiped++;
						}
						else
						{
							if (this.MatchOneRuleForSentence(startRuleId + 1, sentence, startPos + numRealSkiped))
							{
								isMatch = true;
								break;
							}
							else
							{
								numRealSkiped++;
							}
						}
					}
					if (isMatch)
					{
						// ��¼Խ����Ķ�Ӧ�ľ��ӽڵ����ʼ�ͽ������
						if (numRealSkiped == 0)
						{
							if (this.curMatchValue.ContainsKey(startRuleId))
							{
								this.curMatchValue[startRuleId] = new int[] { -1, -1 };
							}
							else
							{
								this.curMatchValue.Add(startRuleId, new int[] { -1, -1 });
							}
						}
						else
						{
							if (this.curMatchValue.ContainsKey(startRuleId))
							{
								this.curMatchValue[startRuleId] = new int[] { startPos, startPos + numRealSkiped - 1 };
							}
							else
							{
								this.curMatchValue.Add(startRuleId, new int[] { startPos, startPos + numRealSkiped - 1 });
							}
						}
					}
					return isMatch;
				}
				else
				{
					// Խ������Ŀ�� high < low �������Ӧ���ǹ�����صĴ���
					return false;
				}
			}
			else
			{
				// ���˻������Խ����֮������������Ӧ���ǹ�����صĴ���
				return false;
			}
		}

		#endregion

		#region ��ȡƥ���� Dictionary<int,string> getMatchValue(Sentence sent)
		/// <summary>
		/// ��ȡMatchƥ����������ֵ��Χ
		/// </summary>
		/// <param name="sent">�������</param>
		/// <returns>�ʵ��ʽ����������������ƥ����</returns>
		public Dictionary<int, string> GetMatchValue(LexSentence sent, Dictionary<int, int[]> curMatchValueItem)
		{
			Dictionary<int, string> matchValue = new Dictionary<int, string>();
			foreach (var item in curMatchValueItem)
			{
				int start = item.Value[0];
				if (start == -1)
				{
					matchValue.Add(item.Key, "");
					continue;
				}
				int end = item.Value[1];
				int length = end - start + 1;
				matchValue.Add(item.Key, sent.SubSentence(start, length).ToWordsString());
			}
			matchValue = matchValue.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
			return matchValue;
		}

		public Dictionary<int, LexSentence> GetMatchLexSentence(LexSentence sent, Dictionary<int, int[]> curMatchValueItem)
		{
			Dictionary<int, LexSentence> matchValue = new Dictionary<int, LexSentence>();
			foreach (var item in curMatchValueItem)
			{
				int start = item.Value[0];
				if (start == -1)
				{
					matchValue.Add(item.Key, new LexSentence());
					continue;
				}
				int end = item.Value[1];
				int length = end - start + 1;
				matchValue.Add(item.Key, sent.SubSentence(start, length));
			}
			matchValue = matchValue.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
			return matchValue;
		}
		#endregion

		#region ��ȡ�����Ҳ�ƥ����  NameValueCollection getMatchResult(Sentence sent)
		/// <summary>
		/// ��ȡ�����Ҳ�ƥ����
		/// </summary>
		/// <param name="sent">�������</param>
		/// <returns>�����Ҳ������ƥ����</returns>
		public NameValueCollection GetMatchResult(LexSentence sent)
		{
			Dictionary<int, string> matchValue = this.GetMatchValue(sent, this.MatchValues.ElementAt(0));
			NameValueCollection matchResult = new NameValueCollection();

			foreach (RightRuleItem rightRuleItem in this._rule.RuleRight.rightRuleItems)
			{
				string name = rightRuleItem.name;
				if (String.IsNullOrEmpty(name))
				{
					foreach (int nameId in rightRuleItem.nameIdList)
					{
						name += matchValue[nameId - 1];
					}
				}
				var nameBuilder = new StringBuilder();
				foreach (int id in rightRuleItem.ids)
				{
					nameBuilder.Append(matchValue[id - 1]);
				}
				matchResult.Add(name, nameBuilder.ToString());
			}
			return matchResult;
		}

		public Match Run(LexSentence sent)
		{
			Match match = new Match();
			match.Text = sent;
			match._matchValues = this.MatchValues.ElementAt(0);
			match._matchRule = this._rule;

			Dictionary<string, List<LexSentence>> matchResult = new Dictionary<string, List<LexSentence>>();
			Dictionary<int, LexSentence> matchValue = this.GetMatchLexSentence(sent, this.MatchValues.ElementAt(0));
			match.BeginIndex = match._matchValues.ElementAt(0).Value[0];
			match.EndIndex = match._matchValues.ElementAt(match._matchValues.Count - 1).Value[1];
			foreach (RightRuleItem rightRuleItem in this._rule.RuleRight.rightRuleItems)
			{
				string name = rightRuleItem.name;
				if (String.IsNullOrEmpty(name))
				{
					foreach (int nameId in rightRuleItem.nameIdList)
					{
						name += matchValue[nameId - 1];
					}
				}
				LexSentence nameBuilder = new LexSentence();
				foreach (int id in rightRuleItem.ids)
				{
					nameBuilder.Words.AddRange(matchValue[id - 1].Words);
				}

				if (matchResult.ContainsKey(name))
				{
					matchResult[name].Add(nameBuilder);
				}
				else
				{
					matchResult[name] = new List<LexSentence>() { nameBuilder };
				}
			}
			match.MatchCollection = matchResult;

			return match;
		}
		#endregion
	}
}