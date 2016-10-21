namespace LexicalExpression
{
	/// <summary>
	///		词项类 WordItem
	/// </summary>
	internal class WordItem : ILexWordMatching
	{
		public WordItem(int wordKind, bool notFlag, WordAtom wordAtom, WordExpression wordExpr)
		{
			this.WordKind = wordKind;
			this.NotFlag = notFlag;
			this.WordAtom = wordAtom;
			this.WordExpr = wordExpr;
		}

		/// <summary>
		///		标识该词项的内容是："词子", "！词表达式", "（词表达式）"
		/// </summary>
		public int WordKind { get; set; }

		/// <summary>
		///		标识是否为非项
		/// </summary>
		public bool NotFlag { get; set; }

		/// <summary>
		///		词子句柄
		/// </summary>
		public WordAtom WordAtom { get; set; }

		/// <summary>
		///		词表达式句柄
		/// </summary>
		public WordExpression WordExpr { get; set; }

		public bool IsMatch(LexWord matchWord)
		{
			bool match = false;
			if (this.WordKind == Constants.IsWordAtom) // 词子项
			{
				match = this.WordAtom.IsMatch(matchWord);
			}
			else
			{
				match = this.WordExpr.IsMatch(matchWord);
			}
			return this.NotFlag ? (!match) : match;
		}

		public override string ToString()
		{
			string output = "词项：";
			if (this.WordKind == Constants.IsWordAtom)
			{
				output += this.WordAtom.ToString();
			}
			else
			{
				if (this.NotFlag)
					output += "! {" + this.WordExpr.ToString() + "}";
				else
					output += "( {" + this.WordExpr.ToString() + ") }";
			}

			return output;
		}
	}
}