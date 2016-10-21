using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		规则右部 RuleRight
	/// </summary>
	public class RuleRight
	{
		public List<RightRuleItem> rightRuleItems;

		public RuleRight(List<RightRuleItem> rightRuleItems)
		{
			this.rightRuleItems = rightRuleItems;
		}

		public RuleRight()
		{
			this.rightRuleItems = new List<RightRuleItem>();
		}
	}
}