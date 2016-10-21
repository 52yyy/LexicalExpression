using System;

namespace LexicalExpression
{
	/// <summary>
	///		重复可选项   OptionalRuleItem
	/// </summary>
	internal class OptionalRuleItem : IRuleItem
	{
		public BasicRuleItem BasicRuleItem { get; set; }

		public OptionalRuleItem(BasicRuleItem basicRuleItem)
		{
			this.BasicRuleItem = basicRuleItem;
		}

		public bool IsMatch(LexWord matchWord)
		{
			return this.BasicRuleItem.IsMatch(matchWord);
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
				return startIndex;
			}
		}

		public int Match(LexSentence sent, int startIndex, RuleItem nextRuleItem)
		{
			if (startIndex < 0 || startIndex > sent.Words.Count - 1)
			{
				return -1;
			}

			LexWord curWord = sent.Words[startIndex];
			if (nextRuleItem == null)
			{
				if (this.IsMatch(sent.Words[startIndex]))
				{
					startIndex++;
				}
				return startIndex;
			}
			else if (nextRuleItem.RuleKind == Constants.OptionRuleItem
					|| (nextRuleItem.RuleKind == Constants.SkipRuleItem && ((SkipRuleItem)nextRuleItem._ruleItem).Low == 0))
			{
				if (this.IsMatch(sent.Words[startIndex]))
				{
					startIndex++;
				}
				return startIndex;
			}
			else
			{
				if (startIndex < sent.Words.Count - 1 && this.IsMatch(sent.Words[startIndex]) && !nextRuleItem.IsMatch(sent.Words[startIndex]))
				{
					startIndex++;
				}
				return startIndex;
			}
		}

		public override string ToString()
		{
			string output = "【重复可选项】：" + Environment.NewLine;
			output += this.BasicRuleItem.ToString();
			return output;
		}
	}
}