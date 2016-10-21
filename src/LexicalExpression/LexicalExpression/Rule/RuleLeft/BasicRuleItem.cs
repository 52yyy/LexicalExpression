using System;

namespace LexicalExpression
{
	/// <summary>
	///		基本项   BasicRuleItem
	/// </summary>
	internal class BasicRuleItem : IRuleItem
	{
		public WordGroup WordGroup { get; set; }   // 词表达式句柄
		public LabelGroup LabelGroup { get; set; } // 标记表达式句柄        

		public BasicRuleItem(WordGroup wordGroup, LabelGroup labelGroup)
		{
			this.WordGroup = wordGroup;
			this.LabelGroup = labelGroup;
		}

		public bool IsMatch(LexWord matchWord)
		{
			bool b1 = this.WordGroup.IsMatch(matchWord);
			if (b1 == false)
			{
				return false;
			}
			b1 = this.LabelGroup.IsMatch(matchWord);
			return b1;
		}

		public int Match(LexSentence sent, int startIndex)
		{
			if (startIndex < 0 || startIndex > sent.Words.Count - 1)
			{
				return -1;
			}

			LexWord curWord = sent.Words[startIndex];
			if (this.IsMatch(curWord))
			{
				return startIndex + 1;
			}
			else
			{
				return -1;
			}
		}

		public override string ToString()
		{
			string output = "【基本项】：" + Environment.NewLine;
			output += this.WordGroup.ToString() + Environment.NewLine + this.LabelGroup.ToString();
			return output;
		}
	}
}