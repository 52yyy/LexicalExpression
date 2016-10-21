using System;

namespace LexicalExpression
{
	/// <summary>
	///		组合标记类   LabelGroup
	/// </summary>
	internal class LabelGroup : ILexWordMatching
	{
		public LabelGroup(bool isPercent, LabelExpression labelExpr)
		{
			this.IsPercent = isPercent;
			this.LabelExpr = labelExpr;
		}

		/// <summary>
		///		标识是否为 '%'
		/// </summary>
		public bool IsPercent { get; set; }

		/// <summary>
		///		标记表达式句柄
		/// </summary>
		public LabelExpression LabelExpr { get; set; } 

		public bool IsMatch(LexWord matchWord)
		{
			if (this.IsPercent)
			{
				return true;
			}
			else
			{
				return this.LabelExpr.IsMatch(matchWord);
			}
		}

		public override string ToString()
		{
			string output = "组合标记：" + Environment.NewLine;
			if (this.IsPercent)
			{
				output += "{ % }";
			}
			else
			{
				output += "{ " + this.LabelExpr.ToString() + " } ";
			}
			return output;
		}
	}
}