using System;

namespace LexicalExpression
{
	/// <summary>
	///		加载越过项状态，如果当前Token是'#'字符则开启至该状态
	/// </summary>
	internal class LoadSkipItemContextState : RuleContextState
	{
		public override void Run(RuleContext context)
		{
			int low, high;

			// 越过项 #m[:n] '['<基本项>']'
			context.GetToken();         // 读取 '#' 后面的第一个数字串
			if (context.Token != Constants.Number)
			{
				low = 0;
				high = 65000;
			}
			else
			{
				// 当读第一个数字时，首先假定high的值就是low的值
				low = context.Lexval;
				high = 65000;

				context.GetToken();         // 读取第一个数字串的符号
				if (context.Token == Constants.Colon)
				{
					context.GetToken();         // 读取第二个数字串
					if (context.Token != Constants.Number)
					{
					}

					// 读第二个数字后，赋给high值
					high = context.Lexval;
					context.GetToken();         // 读取第二个数字串后面的符号
				}
			}
			if (context.Token == Constants.Lbracket)
			{
				// 越过项有限制条件（在方括号里面）
				context.GetToken();     // 读取第一个方括号后面的符号

				// 读取方括号中的限制条件，是一个基本项
				context.State = new LoadBasicItemContextState();
				context.State.Run(context);

				// 读取完方括号中的基本项后，下一个符号应该是 ']'，否则就出错
				if (context.Token != Constants.Rbracket)
				{
					Console.WriteLine("越过项 '#n:['，缺右方括号");
				}
				else
					context.GetToken();     // 读完越过项
			}

			// 创建越过项
			context.CurrentRuleItem = new SkipRuleItem(low, high, (BasicRuleItem)context.CurrentRuleItem);

		}
	}
}