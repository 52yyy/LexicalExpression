using System;

namespace LexicalExpression
{
	internal class RuleContext
	{
		public RuleContext()
		{
		}

		public RuleContext(string content)
		{
			this.RuleContent = content;
			this.CurRule = content;
			this.Rule = new Rule();
		}

		/// <summary>
		///		规则文本内容
		/// </summary>
		public string RuleContent { get; set; }

		/// <summary>
		///		单词对应的常量值
		/// </summary>
		public int Token { get; set; }

		/// <summary>
		///		数字串对应的数值
		/// </summary>
		public int Lexval { get; set; }

		/// <summary>
		///		当前的字符
		/// </summary>
		public int Chpos { get; set; }

		/// <summary>
		///		规则左式匹配的ID
		/// </summary>
		public int RuleItemId { get; set; }

		/// <summary>
		///		解析规则时的每一个单词
		/// </summary>
		public string Lex { get; set; }

		/// <summary>
		///		
		/// </summary>
		public string CurRule { get; set; }

		/// <summary>
		///		当前的rule item
		/// </summary>
		public IRuleItem CurrentRuleItem { get; set; }

		/// <summary>
		///		整条规则
		/// </summary>
		public Rule Rule { get; set; }

		/// <summary>
		///		当前上下文状态
		/// </summary>
		public RuleContextState State { get; set; }

		private bool _isFinished;

		internal void Finish()
		{
			this._isFinished = true;
		}

		public void Run()
		{
			while (!this._isFinished)
			{
				this.State.Run(this);
			}
		}

		public void GetToken()
		{
			int next;
			while (this.CurRule[this.Chpos] == ' ' || this.CurRule[this.Chpos] == '\t')
			{
				this.Chpos++;
			}
			if (this.CurRule[this.Chpos] == '\n')
			{
				// 规则结束
				this.Token = Constants.End;
				return;
			}
			else if (this.CurRule[this.Chpos] > 256)
			{
				// 汉字串
				this.Token = Constants.Hz;
				next = this.Chpos + 1;
				while (this.CurRule[next] > 256)
				{
					next++;
				}
				this.Lex = this.CurRule.Substring(this.Chpos, next - this.Chpos);
				this.Chpos = next;
				return;
			}
			else if (this.CurRule[this.Chpos] == '"')
			{
				this.Token = Constants.Eng;
				next = this.Chpos + 1;
				while (this.CurRule[next] != '"' && next < this.CurRule.Length)
				{
					next++;
				}
				this.Lex = this.CurRule.Substring(this.Chpos + 1, next - this.Chpos - 1);
				this.Chpos = next + 1;
				return;
			}
			//@@
			else if (Char.IsLetter(this.CurRule, this.Chpos))
			{
				// 英文字母串
				this.Token = Constants.Eng;
				next = this.Chpos + 1;
				while (Char.IsLetter(this.CurRule, next))
					next++;
				this.Lex = this.CurRule.Substring(this.Chpos, next - this.Chpos);

				this.Chpos = next;
				return;
			}
			else if (Char.IsDigit(this.CurRule, this.Chpos))
			{
				// 数字串
				this.Token = Constants.Number;
				next = this.Chpos + 1;
				while (Char.IsDigit(this.CurRule, next))
				{
					next++;
				}

				// 得到数字串
				this.Lex = this.CurRule.Substring(this.Chpos, next - this.Chpos);

				// 得到数字串对应的值
				this.Lexval = Int32.Parse(this.Lex);

				this.Chpos = next;
				return;
			}
			else if (this.CurRule[this.Chpos] == '^')
			{
				this.Token = Constants.Mark;
				this.Lex = "^";
				this.Chpos++;
				return;
			}
			else if (this.CurRule[this.Chpos] == '$')
			{
				this.Token = Constants.Dollar;
				this.Lex = "$";
				this.Chpos++;
				return;
			}
			//@@
			else
			{
				if (this.CurRule[this.Chpos] == '#')
				{
					if (this.CurRule[this.Chpos + 1] == 'L')
					{
						this.Lex = "#L";
						this.Token = Constants.Lhash;
						this.Chpos += 2;
						return;

					}
					else if (this.CurRule[this.Chpos + 1] == 'R')
					{
						this.Lex = "#R";
						this.Token = Constants.Rhash;
						this.Chpos += 2;
						return;
					}
					else
					{
						this.Lex = "#";
						this.Token = Constants.Hashtag;
						this.Chpos++;
						return;
					}
				}

				// 取单个字符为单词
				this.Lex = this.CurRule.Substring(this.Chpos, 1);

				switch (this.CurRule[this.Chpos])
				{
					case '*':
						this.Token = Constants.Star;
						break;
					case '%':
						this.Token = Constants.Percent;
						break;
					case '/':
						this.Token = Constants.Slash;
						break;
					case '=':
						this.Token = Constants.Equal;
						break;
					case '+':
						this.Token = Constants.Plus;
						break;
					case '|':
						this.Token = Constants.Or;
						break;
					case '&':
						this.Token = Constants.And;
						break;
					case '!':
						this.Token = Constants.Not;
						break;
					case '(':
						this.Token = Constants.Lparen;
						break;
					case ')':
						this.Token = Constants.Rparen;
						break;
					case '[':
						this.Token = Constants.Lbracket;
						break;
					case ']':
						this.Token = Constants.Rbracket;
						break;
					case '{':
						this.Token = Constants.Lbrace;
						break;
					case '}':
						this.Token = Constants.Rbrace;
						break;
					case ':':
						this.Token = Constants.Colon;
						break;
					case '@':
						this.Token = Constants.Address;
						break;
					case '-':
						this.Token = Constants.Minus;
						break;
					case ',':
						this.Token = Constants.Comma;
						break;
					case ';':
						this.Token = Constants.Eng;
						break;
					case ' ':
						this.Token = Constants.Space;
						break;
					case '^':
						this.Token = Constants.Mark;
						break;
					case '$':
						this.Token = Constants.Dollar;
						break;
					default:
						this.Token = Constants.Error;
						break;
				}

				this.Chpos++;
				return;
			}
		}
	}
}