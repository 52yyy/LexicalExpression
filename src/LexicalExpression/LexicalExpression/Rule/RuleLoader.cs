using System;
using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		规则加载器 （RuleLoader）
	/// </summary>
	internal class RuleLoader : IRuleLoader
	{
		private int _token;         // 解析规则时，单词对应的常量值
		private int _lexval;        // 解析规则时，数字串对应的数值
		private int _chpos;          // 解析规则时，当前的字符
		private string _lex;         // 解析规则时的每一个单词
		private string _curRule;

		#region 规则解析函数

		/// <summary>
		///		解析并加载一条规则到内存中
		/// </summary>
		/// <param name="ruleStr"></param>
		/// <returns></returns>
		public Rule LoadOneRule(string ruleStr)
		{
			try
			{
				this._curRule = ruleStr + "\n\n";         // 加上两个换行符，为了在解析字符串时不越界
				this._chpos = 0;                          // 指向规则的开始
				this.GetToken();                         // 读第一个token

				Rule ParserRule = new Rule();

				ParserRule.RuleLeft = this.LoadRuleLeft();
				if (this._token != Constants.Equal) // 规则左部后面应该是 '='
				{
					return null;
				}
				this.GetToken();                         // 读下一个token

				ParserRule.RuleRight = this.LoadRuleRight();

				return ParserRule;
			}
			catch (Exception e)
			{
				Exception new_e = new Exception(e.Message + "\r\n" + ruleStr);
				throw new_e;
			}
		}

		/// <summary>
		///		解析并加载规则左部到内存中
		/// </summary>
		/// <returns></returns>
		private RuleLeft LoadRuleLeft()
		{
			try
			{
				RuleLeft ruleLeft = new RuleLeft();
				RuleItem item;
				int i = 0;
				item = this.LoadRuleItem();         // 读取一个规则元素项
				item.id = i;
				i++;
				ruleLeft.ruleItemList.Add(item);

				while (true)
				{
					if (this._token == Constants.Plus)
					{
						this.GetToken();         // 读取 '+' 后面的下一个符号
						item = this.LoadRuleItem();     // 读取下一个规则元素项
						item.id = i;
						i++;
						ruleLeft.ruleItemList.Add(item);
					}
					else
					{
						break;
					}
				}
				return ruleLeft;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		///		解析并加载规则左部的一个规则项（元素项）
		/// </summary>
		/// <returns></returns>
		private RuleItem LoadRuleItem()
		{
			RuleItem item = null;
			IRuleItem ruleItem;
			try
			{
				if (this._token == Constants.Hashtag)
				{
					// 加载 越过项 #m[:n] '['<基本项>']'
					ruleItem = this.LoadSkipItem();
					item = new RuleItem(Constants.SkipRuleItem,ruleItem);
					return item;
				}
				else if (this._token == Constants.Lbrace)
				{
					// 加载 重复可选项 '{'<基本项>'}'
					ruleItem = this.LoadOptionItem();
					item = new RuleItem(Constants.OptionRuleItem, ruleItem);
					return item;
				}
				else
				{
					// 加载<基本项>
					ruleItem = this.LoadBasicItem();
					item = new RuleItem(Constants.BasicRuleItem, ruleItem);
					return item;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw e;
			}
		}

		/// <summary>
		///		解析并加载基本项
		/// </summary>
		/// <returns></returns>
		private BasicRuleItem LoadBasicItem()
		{
			WordGroup wordGroup = null;
			LabelGroup labelGroup = null;

			// 读取组合词并建立组合词的节点
			wordGroup = this.LoadWordGroup();

			if (this._token != Constants.Slash)
			{
				// showError("单词和标记之间缺少'/'" + Environment.NewLine + lex");
				return null;
			}
			this.GetToken();             // 跳过 '/'

			// 读取组合词并建立组合词的节点
			labelGroup = this.LoadLabelGroup();

			BasicRuleItem basicItem = new BasicRuleItem(wordGroup, labelGroup);
			return basicItem;
		}

		/// <summary>
		///		解析并加载越过项
		/// </summary>
		/// <returns></returns>
		private SkipRuleItem LoadSkipItem()
		{
			int low, high;
			BasicRuleItem basicItem = null;

			// 越过项 #m[:n] '['<基本项>']'
			this.GetToken();         // 读取 '#' 后面的第一个数字串
			if (this._token != Constants.Number)
			{
				low = 0;
				high = 65000;
			}
			else
			{
				// 当读第一个数字时，首先假定high的值就是low的值
				low = this._lexval;
				high = 65000;

				this.GetToken();         // 读取第一个数字串的符号
				if (this._token == Constants.Colon)
				{
					this.GetToken();         // 读取第二个数字串
					if (this._token != Constants.Number)
					{
						Console.WriteLine("越过项 '#n', 冒号后面缺少数字");
						return null;
					}

					// 读第二个数字后，赋给high值
					high = this._lexval;
					this.GetToken();         // 读取第二个数字串后面的符号
				}
			}
			if (this._token == Constants.Lbracket)
			{
				// 越过项有限制条件（在方括号里面）
				this.GetToken();     // 读取第一个方括号后面的符号

				// 读取方括号中的限制条件，是一个基本项
				basicItem = this.LoadBasicItem();

				// 读取完方括号中的基本项后，下一个符号应该是 ']'，否则就出错
				if (this._token != Constants.Rbracket)
				{
					Console.WriteLine("越过项 '#n:['，缺右方括号");
					return null;
				}
				else
					this.GetToken();     // 读完越过项
			}

			// 创建越过项
			SkipRuleItem skipItem = new SkipRuleItem(low, high, basicItem);
			return skipItem;

		}

		/// <summary>
		///		解析并加载重复可选项
		/// </summary>
		/// <returns></returns>
		private OptionalRuleItem LoadOptionItem()
		{
			BasicRuleItem basicItem = null;

			// 重复可选项的标识是 '{'
			this.GetToken();             // 跳过 '{'

			// 读取大括号中的限制条件，是一个基本项
			basicItem = this.LoadBasicItem();

			// 读取完大括号中的基本项后，下一个符号应该是 '}'，否则就出错
			if (this._token != Constants.Rbrace)
			{
				Console.WriteLine("重复可选项，缺右方括号");
				return null;
			}
			else
			{
				this.GetToken();     // 跳过 '}', 读完重复可选项
			}

			// 创建重复可选项
			OptionalRuleItem optionItem = new OptionalRuleItem(basicItem);
			return optionItem;
		}
		#endregion

		#region 组合词部分

		/// <summary>
		///		解析并加载-------组合词
		/// </summary>
		/// <returns></returns>
		private WordGroup LoadWordGroup()
		{
			WordGroup wg = new WordGroup(true, null);
			if (this._token != Constants.Star)
			{
				wg.IsStar = false;
				wg.WordExpr = this.LoadWordExpression();
			}
			else
				this.GetToken();

			return wg;
		}

		/// <summary>
		///		解析并加载------词表达式
		/// </summary>
		/// <returns></returns>
		private WordExpression LoadWordExpression()
		{
			List<WordItem> list = new List<WordItem>();  // 组成词表达式的词项列表

			WordItem wordItem = this.LoadWordItem();
			list.Add(wordItem);

			while (this._token == Constants.Or)
			{
				this.GetToken();
				wordItem = this.LoadWordItem();
				list.Add(wordItem);
			}

			WordExpression wordExpr = new WordExpression(list);

			return wordExpr;
		}

		/// <summary>
		///		解析并加载------词项
		/// </summary>
		/// <returns></returns>
		private WordItem LoadWordItem()
		{
			WordItem wordItem;
			if (this._token == Constants.Not)  // 检查是否是非标记
			{
				this.GetToken();
				WordExpression wordExpression = this.LoadWordExpression();
				wordItem = new WordItem(Constants.IsWordExpr, true, null, wordExpression);
			}
			else if (this._token == Constants.Lparen)  // 检查是否是左括号
			{
				this.GetToken();
				WordExpression wordExpression = this.LoadWordExpression();

				if (this._token != Constants.Rparen)  // 检查括号是否匹配
				{
					//showError("表达式括号不匹配");
					return null;
				}
				else
				{
					this.GetToken();
					wordItem = new WordItem(Constants.IsWordExpr, false, null, wordExpression);
				}
			}
			else
			{
				WordAtom wordAtom = this.LoadWordAtom();
				wordItem = new WordItem(Constants.IsWordAtom, false, wordAtom, null);
			}
			return wordItem;
		}

		/// <summary>
		///		解析并加载------词项
		/// </summary>
		/// <returns></returns>
		private WordAtom LoadWordAtom()
		{
			WordAtom wordAtom;

			string Lex = "";
			while (this._token == Constants.Eng || this._token == Constants.Hz || this._token == Constants.Number)
			{
				Lex += this._lex;
				this.GetToken();
			}
			wordAtom = new WordAtom(Lex);

			return wordAtom;
		}

		#endregion

		#region 组合标记部分

		/// <summary>
		///		解析并加载------组合标记
		/// </summary>
		/// <returns></returns>
		private LabelGroup LoadLabelGroup()
		{
			LabelGroup labelGroup = new LabelGroup(true, null);
			if (this._token != Constants.Percent)
			{
				labelGroup.IsPercent = false;
				labelGroup.LabelExpr = this.LoadLabelExpression();
			}
			else
				this.GetToken();

			return labelGroup;
		}

		/// <summary>
		///		解析并加载------标记表达式
		/// </summary>
		/// <returns></returns>
		private LabelExpression LoadLabelExpression()
		{
			List<LabelItem> list = new List<LabelItem>();
			LabelItem labelItem = this.LoadLabelItem();
			list.Add(labelItem);

			while (this._token == Constants.Or)
			{
				this.GetToken();
				labelItem = this.LoadLabelItem();
				list.Add(labelItem);
			}

			LabelExpression labelExpression = new LabelExpression(list);
			return labelExpression;
		}

		/// <summary>
		///		解析并加载------标记项
		/// </summary>
		/// <returns></returns>
		private LabelItem LoadLabelItem()
		{
			List<LabelAtom> list = new List<LabelAtom>();

			LabelAtom labelAtom = this.LoadLabelAtom();
			list.Add(labelAtom);

			while (this._token == Constants.And)
			{
				this.GetToken();
				labelAtom = this.LoadLabelAtom();
				list.Add(labelAtom);
			}

			LabelItem labelItem = new LabelItem(list);
			return labelItem;
		}

		/// <summary>
		///		解析并加载------标记因子
		/// </summary>
		/// <returns></returns>
		private LabelAtom LoadLabelAtom()
		{
			LabelAtom labelAtom;

			if (this._token == Constants.Not)
			{
				this.GetToken();
				LabelExpression labelExpression = this.LoadLabelExpression();
				labelAtom = new LabelAtom(Constants.IsLabelExpr, true, null, labelExpression);
			}
			else if (this._token == Constants.Lparen)
			{
				this.GetToken();
				LabelExpression labelExpression = this.LoadLabelExpression();

				if (this._token != Constants.Rparen)
				{
					//showError("表达式括号不匹配！");
					return null;
				}
				else
				{
					labelAtom = new LabelAtom(Constants.IsLabelExpr, false, null, labelExpression);
					this.GetToken();
				}
			}
			else
			{
				if (this._token == Constants.Eng || this._token == Constants.Number
					||this._token==Constants.Mark||this._token==Constants.Dollar)
				{
					labelAtom = new LabelAtom(Constants.IsLabelStr, false, this._lex, null);
					this.GetToken();
				}
				else
				{
					//showError("标记应为英文字符");
					return null;
				}
			}
			return labelAtom;
		}

		#endregion

		#region 规则右部

		/// <summary>
		///		解析并加载规则右部 
		/// </summary>
		/// <returns></returns>
		private RuleRight LoadRuleRight()
		{
			if (this._token == Constants.End)
			{
				//showError("缺少右部项");
				return null;
			}
			else
			{
				RuleRight ruleRight = null;
				List<RightRuleItem> rightRuleItemList = new List<RightRuleItem>();
				if (this._token == Constants.Lbrace)
				{
					RightRuleItem rightRuleItem = this.LoadRightRuleItem();
					rightRuleItemList.Add(rightRuleItem);
					while (this._token == Constants.Plus)
					{
						this.GetToken();
						rightRuleItem = this.LoadRightRuleItem();
						rightRuleItemList.Add(rightRuleItem);
					}
					ruleRight = new RuleRight(rightRuleItemList);
					return ruleRight;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		///		加载规则右部项
		/// </summary>
		/// <returns></returns>
		public RightRuleItem LoadRightRuleItem()
		{
			RightRuleItem rightRuleItem = new RightRuleItem();
			if (this._token == Constants.Lbrace)
			{
				// 读 '{'
				this.GetToken();
				// 读数字串，以','分隔
				if (this._token == Constants.Eng || this._token == Constants.Hz)
				{
					rightRuleItem.name = this._lex;
					this.GetToken();
					if (this._token == Constants.Colon)
					{
						this.GetToken();
					}
					else
					{
						return null;
					}
					while (this._token == Constants.Number)
					{
						rightRuleItem.ids.Add(Convert.ToInt32(this._lex));
						this.GetToken();
						if (this._token == Constants.Comma)
						{
							this.GetToken();
						}
						else
						{
							if (this._token == Constants.Rbrace)
							{
								this.GetToken();
								return rightRuleItem;
							}
							else
							{
								return null;
							}
						}
					}
				}
				else if(this._token==Constants.Number)
				{
					rightRuleItem.nameIdList = new List<int>();
					while (this._token == Constants.Number)
					{
						rightRuleItem.nameIdList.Add(Convert.ToInt32(this._lex));
						this.GetToken();
						if(this._token==Constants.Comma)
						{
							this.GetToken();
						}
						else
						{
							break;
						}
					}
					if (this._token == Constants.Colon)
					{
						this.GetToken();
					}
					else
					{
						return null;
					}
					while (this._token == Constants.Number)
					{
						rightRuleItem.ids.Add(Convert.ToInt32(this._lex));
						this.GetToken();
						if (this._token == Constants.Comma)
						{
							this.GetToken();
						}
						else
						{
							if (this._token == Constants.Rbrace)
							{
								this.GetToken();
								return rightRuleItem;
							}
							else
							{
								return null;
							}
						}
					}
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
			return rightRuleItem;
		}
		#endregion

		/// <summary>
		///		获取下一个元素 
		/// </summary>
		private void GetToken()
		{
			int next;

			while (this._curRule[this._chpos] == ' ' || this._curRule[this._chpos] == '\t')
			{
				this._chpos++;
			}
			if (this._curRule[this._chpos] == '\n')
			{
				// 规则结束
				this._token = Constants.End;
				return;
			}
			else if (this._curRule[this._chpos] > 256)
			{
				// 汉字串
				this._token = Constants.Hz;
				next = this._chpos + 1;
				while (this._curRule[next] > 256)
				{
					next++;
				}
				this._lex = this._curRule.Substring(this._chpos, next - this._chpos);
				this._chpos = next;
				return;
			}
			else if (this._curRule[this._chpos] == '"')
			{
				this._token = Constants.Eng;
				next = this._chpos + 1;
				while (this._curRule[next] != '"' && next < this._curRule.Length)
				{
					next++;
				}
				this._lex = this._curRule.Substring(this._chpos + 1, next - this._chpos - 1);
				this._chpos = next + 1;
				return;
			}
				//@@
			else if (Char.IsLetter(this._curRule, this._chpos))
			{
				// 英文字母串
				this._token = Constants.Eng;
				next = this._chpos + 1;
				while (Char.IsLetter(this._curRule, next))
					next++;
				this._lex = this._curRule.Substring(this._chpos, next - this._chpos);

				this._chpos = next;
				return;
			}
			else if (Char.IsDigit(this._curRule, this._chpos))
			{
				// 数字串
				this._token = Constants.Number;
				next = this._chpos + 1;
				while (Char.IsDigit(this._curRule, next))
				{
					next++;
				}

				// 得到数字串
				this._lex = this._curRule.Substring(this._chpos, next - this._chpos);

				// 得到数字串对应的值
				this._lexval = Int32.Parse(this._lex);

				this._chpos = next;
				return;
			}
			else if (this._curRule[this._chpos] == '^')
			{
				this._token = Constants.Mark;
				this._lex = "^";
				this._chpos++;
				return;
			}
			else if (this._curRule[this._chpos] == '$')
			{
				this._token = Constants.Dollar;
				this._lex = "$";
				this._chpos++;
				return;
			}
				//@@
			else
			{
				if (this._curRule[this._chpos] == '#')
				{
					if (this._curRule[this._chpos + 1] == 'L')
					{
						this._lex = "#L";
						this._token = Constants.Lhash;
						this._chpos += 2;
						return;

					}
					else if (this._curRule[this._chpos + 1] == 'R')
					{
						this._lex = "#R";
						this._token = Constants.Rhash;
						this._chpos += 2;
						return;
					}
					else
					{
						this._lex = "#";
						this._token = Constants.Hashtag;
						this._chpos++;
						return;
					}
				}

				// 取单个字符为单词
				this._lex = this._curRule.Substring(this._chpos, 1);

				switch (this._curRule[this._chpos])
				{
					case '*': this._token = Constants.Star; break;
					case '%': this._token = Constants.Percent; break;
					case '/': this._token = Constants.Slash; break;
					case '=': this._token = Constants.Equal; break;
					case '+': this._token = Constants.Plus; break;
					case '|': this._token = Constants.Or; break;
					case '&': this._token = Constants.And; break;
					case '!': this._token = Constants.Not; break;
					case '(': this._token = Constants.Lparen; break;
					case ')': this._token = Constants.Rparen; break;
					case '[': this._token = Constants.Lbracket; break;
					case ']': this._token = Constants.Rbracket; break;
					case '{': this._token = Constants.Lbrace; break;
					case '}': this._token = Constants.Rbrace; break;
					case ':': this._token = Constants.Colon; break;
					case '@': this._token = Constants.Address; break;
					case '-': this._token = Constants.Minus; break;
					case ',': this._token = Constants.Comma; break;
					case ';': this._token = Constants.Eng; break;
					case ' ': this._token = Constants.Space; break;
					case '^': this._token = Constants.Mark; break;
					case '$': this._token = Constants.Dollar; break;
					default: this._token = Constants.Error; break;
				}

				this._chpos++;
				return;
			}
		}
	}
}