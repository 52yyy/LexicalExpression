using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		规则左部 RuleLeft
	/// </summary>
	public class RuleLeft
	{
		public List<RuleItem> ruleItemList;
		public RuleLeft()
		{
			this.ruleItemList = new List<RuleItem>();
		}
	}
}