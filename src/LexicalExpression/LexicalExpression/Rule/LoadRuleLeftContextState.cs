using System;

namespace LexicalExpression
{
	/// <summary>
	///		规则左式加载状态
	/// </summary>
	internal class LoadRuleLeftContextState : RuleContextState
	{
		public override void Run(RuleContext context)
		{
			do
			{
				context.GetToken(); // 读取 '+' 后面的下一个符号
				context.State = new LoadRuleItemContextState();
				context.State.Run(context);
			}
			while (context.Token == Constants.Plus);
			// 设置 Context State
		}
	}
}