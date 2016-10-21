using System;

namespace LexicalExpression
{
	/// <summary>
	///		组合词类    WordGroup
	/// </summary>
	internal class WordGroup : ILexWordMatching
	{
		public WordGroup(bool isStar, WordExpression wordExpr)
		{
			this.IsStar = isStar;
			this.WordExpr = wordExpr;
		}

		/// <summary>
		///		标识是否为 '*'
		/// </summary>
		public bool IsStar { get; set; }

		/// <summary>
		///		词表达式句柄
		/// </summary>
		public WordExpression WordExpr { get; set; }

		public bool IsMatch(LexWord matchWord)
		{
			if (this.IsStar)
			{
				return true;
			}

			return this.WordExpr.IsMatch(matchWord);
		}

		public override string ToString()
		{
			string output = "组合词：" + Environment.NewLine;
			if (this.IsStar)
			{
				output += "*";
			}
			else
			{
				output += "{" + this.WordExpr.ToString() + "}";
			}
			return output;
		}
	}
}