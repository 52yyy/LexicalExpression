using System;
using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		规则加载器 （RuleLoader）
	/// </summary>
	internal class RuleLoader
	{
		string ruleFilePath;
		string curRule;
		string lex;         // 解析规则时的每一个单词
		int token;         // 解析规则时，单词对应的常量值
		int lexval;        // 解析规则时，数字串对应的数值
		int chpos;          // 解析规则时，当前的字符

		// 构造函数
		public RuleLoader(string path)
		{
			this.ruleFilePath = path;
		}

		public RuleLoader()
		{

		}

		//=======================================
		// 规则解析函数
		//
		//=======================================

		#region 解析并加载一条规则到内存中    loadOneRule(string ruleStr)
		public Rule loadOneRule(string ruleStr)
		{
			try
			{
				this.curRule = ruleStr + "\n\n";         // 加上两个换行符，为了在解析字符串时不越界
				this.chpos = 0;                          // 指向规则的开始
				this.getToken();                         // 读第一个token

				Rule ParserRule = new Rule();

				ParserRule.RuleLeft = this.loadRuleLeft();
				if (this.token != Constants.Equal) // 规则左部后面应该是 '='
				{
					return null;
				}
				this.getToken();                         // 读下一个token

				ParserRule.RuleRight = this.loadRuleRight();

				return ParserRule;
			}
			catch (Exception e)
			{
				Exception new_e = new Exception(e.Message + "\r\n" + ruleStr);
				throw new_e;
			}
		}
		#endregion

		#region 解析并加载规则左部到内存中  loadRuleLeft()

		// 解析并加载规则左部到内存中
		//===================================================================
		RuleLeft loadRuleLeft()
		{
			try
			{
				RuleLeft ruleLeft = new RuleLeft();
				RuleItem item;
				int i = 0;
				item = this.loadRuleItem();         // 读取一个规则元素项
				item.id = i;
				i++;
				ruleLeft.ruleItemList.Add(item);

				while (true)
				{
					if (this.token == Constants.Plus)
					{
						this.getToken();         // 读取 '+' 后面的下一个符号
						item = this.loadRuleItem();     // 读取下一个规则元素项
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
		#endregion

		#region 解析并加载规则左部的一个规则项（元素项）     loadRuleItem()
		// 解析并加载规则左部的一个规则项（元素项）
		//===================================================================
		//
		RuleItem loadRuleItem()
		{
			RuleItem item = null;
			IRuleItem ruleItem;
			try
			{
				if (this.token == Constants.Hashtag)
				{
					// 加载 越过项 #m[:n] '['<基本项>']'
					ruleItem = this.loadSkipItem();
					item = new RuleItem(Constants.SkipRuleItem,ruleItem);
					return item;
				}
				else if (this.token == Constants.Lbrace)
				{
					// 加载 重复可选项 '{'<基本项>'}'
					ruleItem = this.loadOptionItem();
					item = new RuleItem(Constants.OptionRuleItem, ruleItem);
					return item;
				}
				else
				{
					// 加载<基本项>
					ruleItem = this.loadBasicItem();
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
		#endregion

		#region 解析并加载基本项 loadBasicItem()
		BasicRuleItem loadBasicItem()
		{
			WordGroup wordGroup = null;
			LabelGroup labelGroup = null;

			// 读取组合词并建立组合词的节点
			wordGroup = this.loadWordGroup();

			if (this.token != Constants.Slash)
			{
				// showError("单词和标记之间缺少'/'" + Environment.NewLine + lex");
				return null;
			}
			this.getToken();             // 跳过 '/'

			// 读取组合词并建立组合词的节点
			labelGroup = this.loadLabelGroup();

			BasicRuleItem basicItem = new BasicRuleItem(wordGroup, labelGroup);
			return basicItem;
		}
		#endregion

		#region 解析并加载越过项   loadSkipItem()
		// 解析并加载越过项
		//===================================================================
		//
		SkipRuleItem loadSkipItem()
		{
			int low, high;
			BasicRuleItem basicItem = null;

			// 越过项 #m[:n] '['<基本项>']'
			this.getToken();         // 读取 '#' 后面的第一个数字串
			if (this.token != Constants.Number)
			{
				low = 0;
				high = 65000;
			}
			else
			{
				// 当读第一个数字时，首先假定high的值就是low的值
				low = this.lexval;
				high = 65000;

				this.getToken();         // 读取第一个数字串的符号
				if (this.token == Constants.Colon)
				{
					this.getToken();         // 读取第二个数字串
					if (this.token != Constants.Number)
					{
						Console.WriteLine("越过项 '#n', 冒号后面缺少数字");
						return null;
					}

					// 读第二个数字后，赋给high值
					high = this.lexval;
					this.getToken();         // 读取第二个数字串后面的符号
				}
			}
			if (this.token == Constants.Lbracket)
			{
				// 越过项有限制条件（在方括号里面）
				this.getToken();     // 读取第一个方括号后面的符号

				// 读取方括号中的限制条件，是一个基本项
				basicItem = this.loadBasicItem();

				// 读取完方括号中的基本项后，下一个符号应该是 ']'，否则就出错
				if (this.token != Constants.Rbracket)
				{
					Console.WriteLine("越过项 '#n:['，缺右方括号");
					return null;
				}
				else
					this.getToken();     // 读完越过项
			}

			// 创建越过项
			SkipRuleItem skipItem = new SkipRuleItem(low, high, basicItem);
			return skipItem;

		}
		#endregion

		#region  解析并加载重复可选项     loadOptionItem()
		OptionalRuleItem loadOptionItem()
		{
			BasicRuleItem basicItem = null;

			// 重复可选项的标识是 '{'
			this.getToken();             // 跳过 '{'

			// 读取大括号中的限制条件，是一个基本项
			basicItem = this.loadBasicItem();

			// 读取完大括号中的基本项后，下一个符号应该是 '}'，否则就出错
			if (this.token != Constants.Rbrace)
			{
				Console.WriteLine("重复可选项，缺右方括号");
				return null;
			}
			else
			{
				this.getToken();     // 跳过 '}', 读完重复可选项
			}

			// 创建重复可选项
			OptionalRuleItem optionItem = new OptionalRuleItem(basicItem);
			return optionItem;
		}
		#endregion
		//===================================================================//
		//                                                                                         |                                                                       //
		//                                                                      --------组合词部分--------                                                    //
		//                                                                                         |                                                                       //
		//===================================================================//

		#region 解析并加载-------组合词      loadWordGroup()

		WordGroup loadWordGroup()
		{
			WordGroup wg = new WordGroup(true, null);
			if (this.token != Constants.Star)
			{
				wg.IsStar = false;
				wg.WordExpr = this.loadWordExpression();
			}
			else
				this.getToken();

			return wg;
		}
		#endregion

		#region 解析并加载------词表达式   loadWordExpression()

		WordExpression loadWordExpression()
		{
			List<WordItem> list = new List<WordItem>();  // 组成词表达式的词项列表

			WordItem wordItem = this.loadWordItem();
			list.Add(wordItem);

			while (this.token == Constants.Or)
			{
				this.getToken();
				wordItem = this.loadWordItem();
				list.Add(wordItem);
			}

			WordExpression wordExpr = new WordExpression(list);

			return wordExpr;
		}
		#endregion

		#region 解析并加载------词项  loadWordItem()
		WordItem loadWordItem()
		{
			WordItem wordItem;
			if (this.token == Constants.Not)  // 检查是否是非标记
			{
				this.getToken();
				WordExpression wordExpression = this.loadWordExpression();
				wordItem = new WordItem(Constants.IsWordExpr, true, null, wordExpression);
			}
			else if (this.token == Constants.Lparen)  // 检查是否是左括号
			{
				this.getToken();
				WordExpression wordExpression = this.loadWordExpression();

				if (this.token != Constants.Rparen)  // 检查括号是否匹配
				{
					//showError("表达式括号不匹配");
					return null;
				}
				else
				{
					this.getToken();
					wordItem = new WordItem(Constants.IsWordExpr, false, null, wordExpression);
				}
			}
			else
			{
				WordAtom wordAtom = this.loadWordAtom();
				wordItem = new WordItem(Constants.IsWordAtom, false, wordAtom, null);
			}
			return wordItem;
		}
		#endregion

		#region 解析并加载------词项  loadWordAtom()

		WordAtom loadWordAtom()
		{
			WordAtom wordAtom;

			string Lex = "";
			while (this.token == Constants.Eng || this.token == Constants.Hz || this.token == Constants.Number)
			{
				Lex += this.lex;
				this.getToken();
			}
			wordAtom = new WordAtom(Lex);

			return wordAtom;
		}
		#endregion

		//===================================================================//
		//                                                                                         |                                                                       //
		//                                                                    --------组合标记部分--------                                                    //
		//                                                                                         |                                                                       //
		//===================================================================//

		#region 解析并加载------组合标记   loadLabelGroup( )

		LabelGroup loadLabelGroup()
		{
			LabelGroup labelGroup = new LabelGroup(true, null);
			if (this.token != Constants.Percent)
			{
				labelGroup.IsPercent = false;
				labelGroup.LabelExpr = this.loadLabelExpression();
			}
			else
				this.getToken();

			return labelGroup;
		}
		#endregion

		#region 解析并加载------标记表达式 loadLabelExpression()

		LabelExpression loadLabelExpression()
		{
			List<LabelItem> list = new List<LabelItem>();
			LabelItem labelItem = this.loadLabelItem();
			list.Add(labelItem);

			while (this.token == Constants.Or)
			{
				this.getToken();
				labelItem = this.loadLabelItem();
				list.Add(labelItem);
			}

			LabelExpression labelExpression = new LabelExpression(list);
			return labelExpression;
		}
		#endregion

		#region 解析并加载------标记项 loadLabelItem( )

		LabelItem loadLabelItem()
		{
			List<LabelAtom> list = new List<LabelAtom>();

			LabelAtom labelAtom = this.loadLabelAtom();
			list.Add(labelAtom);

			while (this.token == Constants.And)
			{
				this.getToken();
				labelAtom = this.loadLabelAtom();
				list.Add(labelAtom);
			}

			LabelItem labelItem = new LabelItem(list);
			return labelItem;
		}

		#endregion

		#region 解析并加载------标记因子   loadLabelAtom()

		LabelAtom loadLabelAtom()
		{
			LabelAtom labelAtom;

			if (this.token == Constants.Not)
			{
				this.getToken();
				LabelExpression labelExpression = this.loadLabelExpression();
				labelAtom = new LabelAtom(Constants.IsLabelExpr, true, null, labelExpression);
			}
			else if (this.token == Constants.Lparen)
			{
				this.getToken();
				LabelExpression labelExpression = this.loadLabelExpression();

				if (this.token != Constants.Rparen)
				{
					//showError("表达式括号不匹配！");
					return null;
				}
				else
				{
					labelAtom = new LabelAtom(Constants.IsLabelExpr, false, null, labelExpression);
					this.getToken();
				}
			}
			else
			{
				if (this.token == Constants.Eng || this.token == Constants.Number
					||this.token==Constants.Mark||this.token==Constants.Dollar)
				{
					labelAtom = new LabelAtom(Constants.IsLabelStr, false, this.lex, null);
					this.getToken();
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


		//===================================================================//
		//                                                                                         |                                                                       //
		//                                                                          ------规则右部--------                                                      //
		//                                                                                         |                                                                       //
		//===================================================================//

		#region 解析并加载规则右部       loadRuleRight()
		RuleRight loadRuleRight()
		{
			if (this.token == Constants.End)
			{
				//showError("缺少右部项");
				return null;
			}
			else
			{
				RuleRight ruleRight = null;
				List<RightRuleItem> rightRuleItemList = new List<RightRuleItem>();
				if (this.token == Constants.Lbrace)
				{
					RightRuleItem rightRuleItem = this.loadRightRuleItem();
					rightRuleItemList.Add(rightRuleItem);
					while (this.token == Constants.Plus)
					{
						this.getToken();
						rightRuleItem = this.loadRightRuleItem();
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
		#endregion

		#region 加载规则右部项  loadRightRuleItem()
		public RightRuleItem loadRightRuleItem()
		{
			RightRuleItem rightRuleItem = new RightRuleItem();
			if (this.token == Constants.Lbrace)
			{
				// 读 '{'
				this.getToken();
				// 读数字串，以','分隔
				if (this.token == Constants.Eng || this.token == Constants.Hz)
				{
					rightRuleItem.name = this.lex;
					this.getToken();
					if (this.token == Constants.Colon)
					{
						this.getToken();
					}
					else
					{
						return null;
					}
					while (this.token == Constants.Number)
					{
						rightRuleItem.ids.Add(Convert.ToInt32(this.lex));
						this.getToken();
						if (this.token == Constants.Comma)
						{
							this.getToken();
						}
						else
						{
							if (this.token == Constants.Rbrace)
							{
								this.getToken();
								return rightRuleItem;
							}
							else
							{
								return null;
							}
						}
					}
				}
				else if(this.token==Constants.Number)
				{
					rightRuleItem.nameIdList = new List<int>();
					while (this.token == Constants.Number)
					{
						rightRuleItem.nameIdList.Add(Convert.ToInt32(this.lex));
						this.getToken();
						if(this.token==Constants.Comma)
						{
							this.getToken();
						}
						else
						{
							break;
						}
					}
					if (this.token == Constants.Colon)
					{
						this.getToken();
					}
					else
					{
						return null;
					}
					while (this.token == Constants.Number)
					{
						rightRuleItem.ids.Add(Convert.ToInt32(this.lex));
						this.getToken();
						if (this.token == Constants.Comma)
						{
							this.getToken();
						}
						else
						{
							if (this.token == Constants.Rbrace)
							{
								this.getToken();
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

		#region 获取下一个元素 getToken()
		void getToken()
		{
			int next;

			while (this.curRule[this.chpos] == ' ' || this.curRule[this.chpos] == '\t')
			{
				this.chpos++;
			}
			if (this.curRule[this.chpos] == '\n')
			{
				// 规则结束
				this.token = Constants.End;
				return;
			}
			else if (this.curRule[this.chpos] > 256)
			{
				// 汉字串
				this.token = Constants.Hz;
				next = this.chpos + 1;
				while (this.curRule[next] > 256)
				{
					next++;
				}
				this.lex = this.curRule.Substring(this.chpos, next - this.chpos);
				this.chpos = next;
				return;
			}
			else if (this.curRule[this.chpos] == '"')
			{
				this.token = Constants.Eng;
				next = this.chpos + 1;
				while (this.curRule[next] != '"' && next < this.curRule.Length)
				{
					next++;
				}
				this.lex = this.curRule.Substring(this.chpos + 1, next - this.chpos - 1);
				this.chpos = next + 1;
				return;
			}
				//@@
			else if (Char.IsLetter(this.curRule, this.chpos))
			{
				// 英文字母串
				this.token = Constants.Eng;
				next = this.chpos + 1;
				while (Char.IsLetter(this.curRule, next))
					next++;
				this.lex = this.curRule.Substring(this.chpos, next - this.chpos);

				this.chpos = next;
				return;
			}
			else if (Char.IsDigit(this.curRule, this.chpos))
			{
				// 数字串
				this.token = Constants.Number;
				next = this.chpos + 1;
				while (Char.IsDigit(this.curRule, next))
				{
					next++;
				}

				// 得到数字串
				this.lex = this.curRule.Substring(this.chpos, next - this.chpos);

				// 得到数字串对应的值
				this.lexval = Int32.Parse(this.lex);

				this.chpos = next;
				return;
			}
			else if (this.curRule[this.chpos] == '^')
			{
				this.token = Constants.Mark;
				this.lex = "^";
				this.chpos++;
				return;
			}
			else if (this.curRule[this.chpos] == '$')
			{
				this.token = Constants.Dollar;
				this.lex = "$";
				this.chpos++;
				return;
			}
				//@@
			else
			{
				if (this.curRule[this.chpos] == '#')
				{
					if (this.curRule[this.chpos + 1] == 'L')
					{
						this.lex = "#L";
						this.token = Constants.Lhash;
						this.chpos += 2;
						return;

					}
					else if (this.curRule[this.chpos + 1] == 'R')
					{
						this.lex = "#R";
						this.token = Constants.Rhash;
						this.chpos += 2;
						return;
					}
					else
					{
						this.lex = "#";
						this.token = Constants.Hashtag;
						this.chpos++;
						return;
					}
				}

				// 取单个字符为单词
				this.lex = this.curRule.Substring(this.chpos, 1);

				switch (this.curRule[this.chpos])
				{
					case '*': this.token = Constants.Star; break;
					case '%': this.token = Constants.Percent; break;
					case '/': this.token = Constants.Slash; break;
					case '=': this.token = Constants.Equal; break;
					case '+': this.token = Constants.Plus; break;
					case '|': this.token = Constants.Or; break;
					case '&': this.token = Constants.And; break;
					case '!': this.token = Constants.Not; break;
					case '(': this.token = Constants.Lparen; break;
					case ')': this.token = Constants.Rparen; break;
					case '[': this.token = Constants.Lbracket; break;
					case ']': this.token = Constants.Rbracket; break;
					case '{': this.token = Constants.Lbrace; break;
					case '}': this.token = Constants.Rbrace; break;
					case ':': this.token = Constants.Colon; break;
					case '@': this.token = Constants.Address; break;
					case '-': this.token = Constants.Minus; break;
					case ',': this.token = Constants.Comma; break;
					case ';': this.token = Constants.Eng; break;
					case ' ': this.token = Constants.Space; break;
					case '^': this.token = Constants.Mark; break;
					case '$': this.token = Constants.Dollar; break;
					default: this.token = Constants.Error; break;
				}

				this.chpos++;
				return;
			}
		}
		#endregion

	}
}