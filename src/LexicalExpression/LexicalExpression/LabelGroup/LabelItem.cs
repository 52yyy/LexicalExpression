using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		标记类项    LabelItem
	/// </summary>
	internal class LabelItem : ILexWordMatching
	{
		public LabelItem(List<LabelAtom> labelAtomList)
		{
			this.LabelAtomList = labelAtomList;
		}

		public List<LabelAtom> LabelAtomList { get; set; }
		
		public bool IsMatch(LexWord matchWord)
		{
			bool match = true;
			foreach (LabelAtom label in this.LabelAtomList)
			{
				if (!label.IsMatch(matchWord))
				{
					// 多个标记因子项是“与”的关系，只要有一个标记因子项为false，则整个标记项为false
					match = false;
					break;
				}
			}
			return match;
		}

		public override string ToString()
		{
			string output = "标记项：";
			foreach (LabelAtom la in this.LabelAtomList)
			{
				output += la.ToString();
			}
			return output;
		}
	}
}