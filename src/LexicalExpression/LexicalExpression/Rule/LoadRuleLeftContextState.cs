using System;

namespace LexicalExpression
{
	/// <summary>
	///		������ʽ����״̬
	/// </summary>
	internal class LoadRuleLeftContextState : RuleContextState
	{
		public override void Run(RuleContext context)
		{
			do
			{
				context.GetToken(); // ��ȡ '+' �������һ������
				context.State = new LoadRuleItemContextState();
				context.State.Run(context);
			}
			while (context.Token == Constants.Plus);
			// ���� Context State
		}
	}
}