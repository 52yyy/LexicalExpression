using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace LexicalExpression
{
	/// <summary>
	///		整条规则 Rule
	/// </summary>
	public class Rule
    {
		public Rule(RuleLeft ruleLeft, RuleRight ruleRight)
		{
			this.RuleLeft = ruleLeft;
			this.RuleRight = ruleRight;
		}

		public Rule()
		{
			RuleLeft = new RuleLeft();
			RuleRight = new RuleRight();
		}

		public RuleLeft RuleLeft { get; set; }
		public RuleRight RuleRight { get; set; }

		public ILexicalExpressionMatcher CreateLexicalExpressionMatcher()
		{
			return new LexicalExpressionMatcher(this);
		}
    }
}
