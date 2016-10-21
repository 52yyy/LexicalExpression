using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		标记表达式类  LabelExpression
	/// </summary>
	internal class LabelExpression : ILexWordMatching
	{
		public LabelExpression(List<LabelItem> labelItemList)
		{
			this.LabelItemList = labelItemList;
		}

		public List<LabelItem> LabelItemList { get; set; }

		public bool IsMatch(LexWord matchWord)
		{
			bool match = false;
			foreach (LabelItem item in this.LabelItemList)
			{
				if (item.IsMatch(matchWord))
				{
					// 多个标记项是“或”的关系，只要有一个标记项为true，则整个标记表达式为true
					match = true;
					break;
				}
			}
			return match;
		}

		public override string ToString()
		{
			string output = "标记表达式：";
			foreach (LabelItem le in this.LabelItemList)
			{
				output += "[" + le.ToString() + "]";
			}
			return output;
		}
	}
}