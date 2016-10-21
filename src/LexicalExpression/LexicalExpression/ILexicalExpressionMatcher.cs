using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LexicalExpression
{
	public interface ILexicalExpressionMatcher
	{
		bool Match(LexSentence sent);

		bool Match(LexSentence sent, int startIndex);

		bool MatchOneRuleForSentence(int startRuleId, LexSentence sentence, int startPos);

		Dictionary<int, string> GetMatchValue(LexSentence sent, Dictionary<int, int[]> curMatchValueItem);

		Dictionary<int, LexSentence> GetMatchLexSentence(LexSentence sent, Dictionary<int, int[]> curMatchValueItem);

		NameValueCollection GetMatchResult(LexSentence sent);

		Match Run(LexSentence sent);
	}

	public class LexicalExpressionMatcher : ILexicalExpressionMatcher
	{
		public LexicalExpressionMatcher(Rule rule)
		{
			this._rule = rule;
		}

		private Rule _rule;


		public List<Dictionary<int, int[]>> MatchValues = new List<Dictionary<int, int[]>>();
		public Dictionary<int, int[]> curMatchValue = new Dictionary<int, int[]>();



		#region 检查是否匹配 bool Match(Sentence sent)
		public bool Match(LexSentence sent)
		{
			return Match(sent, 0);
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
					curMatchValue = curMatchValue.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
					this.MatchValues.Add(curMatchValue);
					int lastMatchIndex = -1;
					for (int dicPos = curMatchValue.Count - 1; dicPos >= 0; dicPos--)
					{
						lastMatchIndex = curMatchValue.ElementAt(dicPos).Value[1];
						if (lastMatchIndex != -1)
						{
							break;
						}
					}
					wordPos = lastMatchIndex + 1;
					curMatchValue = new Dictionary<int, int[]>();
				}
				else
				{
					wordPos++;
					continue;
				}
			}
			if (MatchValues.Count == 0)
			{
				curMatchValue = new Dictionary<int, int[]>();
				return false;
			}
			else
			{
				return true;
			}

		}
		#endregion

		#region 一次匹配 bool matchOneRuleForSentence(int startRuleId, Sentence sentence, int startPos)
		public bool MatchOneRuleForSentence(int startRuleId, LexSentence sentence, int startPos)
		{
			bool isMatch;
			//this.MatchValues = new Dictionary<int, int[]>();
			// 递归匹配结束条件。规则的全部节点都已经匹配成功。所以返回 true
			if (startRuleId >= _rule.RuleLeft.ruleItemList.Count)
			{
				return true;
			}
			// 递归匹配结束条件。已经匹配到句子末尾，但还有规则项未匹配，所以返回 false 
			if (startPos >= sentence.Words.Count)
			{
				return false;
			}
			// 当期待匹配的规则项
			RuleItem ruleItem = _rule.RuleLeft.ruleItemList[startRuleId];

			if (ruleItem.RuleKind == Constants.BasicRuleItem)
			{
				// -------- 1) 基本项的匹配 -------------
				//
				if (ruleItem.Match(sentence, startPos) != -1)
				{
					// 如果当前项匹配
					// 记录与当前规则项匹配的单词节点的序号（句子列表中的下标）
					if (curMatchValue.ContainsKey(startRuleId))
					{
						curMatchValue[startRuleId] = new int[] { startPos, startPos };
					}
					else
					{
						curMatchValue.Add(startRuleId, new int[] { startPos, startPos });
					}

					// 递归匹配下一个规则项
					return this.MatchOneRuleForSentence(startRuleId + 1, sentence, startPos + 1);
				}
				else
				{
					return false;   // 当前规则项是一个基本项，当前单词与之不匹配，故而该条规则在 startPos 位置不匹配
				}
			}
			else if (ruleItem.RuleKind == Constants.SkipRuleItem)
			{
				// 对短语规则而言，当前规则系统只有基本项和越过项
				// -------- 2) 越过项的匹配 -------------
				// 
				SkipRuleItem skipRuleItem = (SkipRuleItem)ruleItem._ruleItem; // 越过项规则节点
				int low = skipRuleItem.Low;                    // 最低越过数
				int high = skipRuleItem.High;                  // 最高越过数

				if (low == 1 && high == 1)
				{
					//  2.1 )越过项只有1项

					isMatch = skipRuleItem.Match(sentence, startPos) == -1 ? false : true;
					if (!isMatch)
					{
						return false;
					}
					// 记录与当前越过项匹配的单词节点的序号（句子列表中的下标）
					if (curMatchValue.ContainsKey(startRuleId))
					{
						curMatchValue[startRuleId] = new int[] { startPos, startPos };
					}
					else
					{
						curMatchValue.Add(startRuleId, new int[] { startPos, startPos });
					}


					// 递归匹配下一个规则项
					return this.MatchOneRuleForSentence(startRuleId + 1, sentence, startPos + 1);

				}
				else if (high >= low)
				{
					//  2.2 )越过项多于 1 项

					// 确定符合越过条件的实际项数
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

					// 如果实际可越过的项数小于最低越过数，则该规则在 startPos 位置不匹配，返回 false
					if (numItemsCanSkiped < low)
					{
						return false;
					}
					// 下面分别越过从 low 到 numItemsCanSkiped( low <= X <= high)个项数，递归匹配规则的其余部分
					// 确定在越过几个项后，整个规则得到匹配

					isMatch = false;
					int numRealSkiped = low;
					while (numRealSkiped <= numItemsCanSkiped)
					{
						if (startRuleId == _rule.RuleLeft.ruleItemList.Count - 1
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
						// 记录越过项的对应的句子节点的起始和结束序号
						if (numRealSkiped == 0)
						{
							if (curMatchValue.ContainsKey(startRuleId))
							{
								curMatchValue[startRuleId] = new int[] { -1, -1 };
							}
							else
							{
								curMatchValue.Add(startRuleId, new int[] { -1, -1 });
							}
						}
						else
						{
							if (curMatchValue.ContainsKey(startRuleId))
							{
								curMatchValue[startRuleId] = new int[] { startPos, startPos + numRealSkiped - 1 };
							}
							else
							{
								curMatchValue.Add(startRuleId, new int[] { startPos, startPos + numRealSkiped - 1 });
							}
						}
					}
					return isMatch;
				}
				else
				{
					// 越过项数目是 high < low 的情况。应该是规则加载的错误
					return false;
				}
			}
			else
			{
				// 除了基本项和越过项之外的其它情况。应该是规则加载的错误
				return false;
			}
		}

		#endregion

		#region 获取匹配结果 Dictionary<int,string> getMatchValue(Sentence sent)
		/// <summary>
		/// 获取Match匹配结果的索引值范围
		/// </summary>
		/// <param name="sent">输入句子</param>
		/// <returns>词典格式，保存各个规则项的匹配结果</returns>
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

		#region 获取规则右部匹配结果  NameValueCollection getMatchResult(Sentence sent)
		/// <summary>
		/// 获取规则右部匹配结果
		/// </summary>
		/// <param name="sent">输入句子</param>
		/// <returns>规则右部各项的匹配结果</returns>
		public NameValueCollection GetMatchResult(LexSentence sent)
		{
			Dictionary<int, string> matchValue = this.GetMatchValue(sent, MatchValues.ElementAt(0));
			NameValueCollection matchResult = new NameValueCollection();

			foreach (RightRuleItem rightRuleItem in _rule.RuleRight.rightRuleItems)
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
			match._matchValues = MatchValues.ElementAt(0);
			match._matchRule = this._rule;

			Dictionary<string, List<LexSentence>> matchResult = new Dictionary<string, List<LexSentence>>();
			Dictionary<int, LexSentence> matchValue = this.GetMatchLexSentence(sent, MatchValues.ElementAt(0));
			match.BeginIndex = match._matchValues.ElementAt(0).Value[0];
			match.EndIndex = match._matchValues.ElementAt(match._matchValues.Count - 1).Value[1];
			foreach (RightRuleItem rightRuleItem in _rule.RuleRight.rightRuleItems)
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
