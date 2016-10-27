using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace LexicalExpression
{
	public interface IRuleLoader
	{
		Rule LoadOneRule(string ruleStr);
	}

	public class NewRuleLoader : IRuleLoader
	{
		public Rule LoadOneRule(string ruleStr)
		{
			RuleContext context = new RuleContext(ruleStr + "\n\n");
			context.Chpos = 0;

			context.State = new LoadRuleLeftContextState();
			context.State.Run(context);
			if (context.Token != Constants.Equal) // 规则左部后面应该是 '='
			{
				return null;
			}
			context.State = new LoadRuleRightContextState();
			context.State.Run(context);

			return context.Rule;
		}
	}

	/// <summary>
	///		右侧表达式加载状态
	/// </summary>
	internal class LoadRuleRightContextState:RuleContextState
	{
		public override void Run(RuleContext context)
		{
			if (context.Token == Constants.End)
			{
				return;
			}
			RuleRight ruleRight = null;
			List<RightRuleItem> rightRuleItemList = new List<RightRuleItem>();

			do
			{
				context.GetToken();
				RightRuleItem rightRuleItem = this.LoadRightRuleItem(context);
				rightRuleItemList.Add(rightRuleItem);
			}
			while (context.Token == Constants.Plus);

			ruleRight = new RuleRight(rightRuleItemList);
			context.Rule.RuleRight = ruleRight;
		}

		private RightRuleItem LoadRightRuleItem(RuleContext context)
		{
			RightRuleItem rightRuleItem = new RightRuleItem();
			if (context.Token == Constants.Lbrace)
			{
				// 读 '{'
				context.GetToken();
				// 读数字串，以','分隔
				if (context.Token == Constants.Eng || context.Token == Constants.Hz)
				{
					rightRuleItem.name = context.Lex;
					context.GetToken();
					if (context.Token == Constants.Colon)
					{
						context.GetToken();
					}
					else
					{
						return null;
					}
					while (context.Token == Constants.Number)
					{
						rightRuleItem.ids.Add(Convert.ToInt32(context.Lex));
						context.GetToken();
						if (context.Token == Constants.Comma)
						{
							context.GetToken();
						}
						else
						{
							if (context.Token == Constants.Rbrace)
							{
								context.GetToken();
								return rightRuleItem;
							}
							else
							{
								return null;
							}
						}
					}
				}
				else if (context.Token == Constants.Number)
				{
					rightRuleItem.nameIdList = new List<int>();
					while (context.Token == Constants.Number)
					{
						rightRuleItem.nameIdList.Add(Convert.ToInt32(context.Lex));
						context.GetToken();
						if (context.Token == Constants.Comma)
						{
							context.GetToken();
						}
						else
						{
							break;
						}
					}
					if (context.Token == Constants.Colon)
					{
						context.GetToken();
					}
					else
					{
						return null;
					}
					while (context.Token == Constants.Number)
					{
						rightRuleItem.ids.Add(Convert.ToInt32(context.Lex));
						context.GetToken();
						if (context.Token == Constants.Comma)
						{
							context.GetToken();
						}
						else
						{
							if (context.Token == Constants.Rbrace)
							{
								context.GetToken();
								return rightRuleItem;
							}
							else
							{
								return null;
							}
						}
					}
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
			return rightRuleItem;
		}
	}
}