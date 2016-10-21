namespace LexicalExpression
{
	/// <summary>
	///		标记因子类   LabelAtom
	/// </summary>
	internal class LabelAtom : ILexWordMatching
	{
		public LabelAtom(int labelKind, bool notFlag, string labelStr, LabelExpression labelExpr)
		{
			this.LabelKind = labelKind;
			this.NotFlag = notFlag;
			this.LabelStr = labelStr;
			this.LabelExpr = labelExpr;
		}

		/// <summary>
		///		标识该标记的内容是单标记、！标记表达式、（标记表达式）
		/// </summary>
		public int LabelKind { get; set; } 

		/// <summary>
		///		标识是否为非项
		/// </summary>
		public bool NotFlag { get; set; }  

		/// <summary>
		///		单标记字符串
		/// </summary>
		public string LabelStr { get; set; } 

		/// <summary>
		///		标记表达式句柄
		/// </summary>
		public LabelExpression LabelExpr { get; set; } 


		public bool IsMatch(LexWord matchWord)
		{
			bool match = false;
			if (this.LabelKind == Constants.IsLabelStr) // ´Ê×ÓÏî
			{
				if (this.LabelStr == "^")
				{
					if (matchWord.IsStart)
					{
						match = true;
					}
					else
					{
						match = false;
					}
				}
				else if (this.LabelStr == "$")
				{
					if (matchWord.IsEnd)
					{
						match = true;
					}
					else
					{
						match = false;
					}
				}
				else
				{
					if (this.LabelStr == matchWord.Pos)
					{
						match = true;
					}
					else
					{
						match = false;
					}
				}

			}
			else
			{
				match = this.LabelExpr.IsMatch(matchWord);
			}

			return this.NotFlag ? (!match) : match;
		}

		public override string ToString()
		{
			string output = "±ê¼ÇÒò×Ó£º";
			if (this.LabelKind == 1)
				output += "<" + this.LabelStr + ">";
			else
			{
				if (this.NotFlag)
					output += "!" + "(" + this.LabelExpr.ToString() + ")";
				else
					output += "(" + this.LabelExpr.ToString() + ")";
			}
			return output;
		}
	}
}