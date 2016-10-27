namespace LexicalExpression
{
	/// <summary>
	///		规则项加载状态
	/// </summary>
	internal class LoadRuleItemContextState : RuleContextState
	{
		public override void Run(RuleContext context)
		{
			RuleItem item;
			if (context.Token == Constants.Hashtag)
			{
				// 加载 越过项 #m[:n] '['<基本项>']'
				context.State = new LoadSkipItemContextState();
				context.State.Run(context);
				item = new RuleItem(Constants.SkipRuleItem, context.CurrentRuleItem);
			}
			else if (context.Token == Constants.Lbrace)
			{
				// 加载 重复可选项 '{'<基本项>'}'
				context.State = new LoadOptionItemContextState();
				context.State.Run(context);
				item = new RuleItem(Constants.OptionRuleItem, context.CurrentRuleItem);
			}
			else
			{
				// 加载<基本项>
				context.State = new LoadBasicItemContextState();
				context.State.Run(context);
				item = new RuleItem(Constants.BasicRuleItem, context.CurrentRuleItem);
			}

			item.id = context.RuleItemId;
			context.RuleItemId++;
			context.Rule.RuleLeft.ruleItemList.Add(item);
		}
	}
}