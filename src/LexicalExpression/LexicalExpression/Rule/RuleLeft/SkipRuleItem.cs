using System;

namespace LexicalExpression
{
	/// <summary>
	///		越过项 SkipRuleItem
	/// </summary>
	internal class SkipRuleItem : IRuleItem
	{
		public int Low { get; set; }            // 下限值
		public int High { get; set; }          // 上限值
		public BasicRuleItem BasicRuleItem { get; set; }

		public SkipRuleItem(int low, int high, BasicRuleItem basicRuleItem)
		{
			this.Low = low;
			this.High = high;
			this.BasicRuleItem = basicRuleItem;
		}

		public bool IsMatch(LexWord matchWord)
		{
			if (this.BasicRuleItem == null)
			{
				return true;
			}
			else
			{
				return this.BasicRuleItem.IsMatch(matchWord);
			}
		}

		public int Match(LexSentence sent, int startIndex)
		{
			if (startIndex < 0 || startIndex > sent.Words.Count - 1)
			{
				return -1;
			}
			if (startIndex + this.Low > sent.Words.Count - 1)
			{
				return -1;
			}
			int step = 0;
			while (step < this.Low)
			{
				if (this.BasicRuleItem.IsMatch(sent.Words[startIndex + step]))
				{
					step++;
				}
				else
				{
					return -1;
				}
			}
			startIndex = startIndex + this.Low;

			while (startIndex < sent.Words.Count - 1 && this.IsMatch(sent.Words[startIndex]))
			{
				startIndex++;
			}
			return startIndex;
		}

		public int Match(LexSentence sent, int startIndex, RuleItem nextRuleItem)
		{
			if (startIndex < 0 || startIndex > sent.Words.Count - 1)
			{
				return -1;
			}
			if (startIndex + this.Low > sent.Words.Count - 1)
			{
				return -1;
			}
			int step = 0;
			while (step < this.Low)
			{
				if (this.BasicRuleItem.IsMatch(sent.Words[startIndex + step]))
				{
					step++;
				}
				else
				{
					return -1;
				}
			}
			startIndex = startIndex + this.Low;
			if (nextRuleItem == null)
			{
				while (startIndex < sent.Words.Count - 1 && this.IsMatch(sent.Words[startIndex]))
				{
					startIndex++;
				}
				return startIndex;
			}
			else if (nextRuleItem.RuleKind == Constants.OptionRuleItem
					|| (nextRuleItem.RuleKind == Constants.SkipRuleItem && ((SkipRuleItem)nextRuleItem._ruleItem).Low == 0))
			{
				while (startIndex < sent.Words.Count - 1 && this.IsMatch(sent.Words[startIndex]))
				{
					startIndex++;
				}
				return startIndex;
			}
			else
			{
				while (startIndex < sent.Words.Count - 1 && this.IsMatch(sent.Words[startIndex]) && !nextRuleItem.IsMatch(sent.Words[startIndex]))
				{
					startIndex++;
				}
				return startIndex;
			}
		}

		public override string ToString()
		{
			string output = "【越过项】" + Environment.NewLine;
			output += "下限值：" + this.Low.ToString() + Environment.NewLine;
			output += "上限值：" + this.High.ToString() + Environment.NewLine;
			output += "限制条件：" + this.BasicRuleItem.ToString() + Environment.NewLine;
			return output;
		}
	}
}