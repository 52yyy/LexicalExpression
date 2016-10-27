using System;

namespace LexicalExpression
{
	/// <summary>
	///		加载重复可选项状态，如果当前Token是'{'字符则开启至该状态
	/// </summary>
	internal class LoadOptionItemContextState : RuleContextState
	{
		public override void Run(RuleContext context)
		{

			// 重复可选项的标识是 '{'
			context.GetToken();             // 跳过 '{'

			// 读取大括号中的限制条件，是一个基本项
			context.State = new LoadBasicItemContextState();
			context.State.Run(context);

			// 读取完大括号中的基本项后，下一个符号应该是 '}'，否则就出错
			if (context.Token != Constants.Rbrace)
			{
				Console.WriteLine("重复可选项，缺右方括号");
			}
			else
			{
				context.GetToken();     // 跳过 '}', 读完重复可选项
			}

			// 创建重复可选项
			context.CurrentRuleItem = new OptionalRuleItem((BasicRuleItem)context.CurrentRuleItem);
		}
	}
}