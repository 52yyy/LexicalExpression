using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		词表达式类   WordExpression
	/// </summary>
	internal class WordExpression : ILexWordMatching
	{
		public WordExpression(List<WordItem> wordItemList)
		{
			this.WordItemList = wordItemList;
		}

		public List<WordItem> WordItemList { get; set; }

		public bool IsMatch(LexWord matchWord)
		{
			bool match = false;
			foreach (WordItem item in this.WordItemList)
			{
				if (item.IsMatch(matchWord))
				{
					// 多个词项是“或”的关系，只要有一个词项为true，则整个词表达式为true
					match = true;
					break;
				}
			}
			return match;
		}

		public override string ToString()
		{
			string output = "词表达式：";
			foreach (WordItem wi in this.WordItemList)
			{
				output += "[" + wi.ToString() + "]";
			}
			return output;
		}
	}
}