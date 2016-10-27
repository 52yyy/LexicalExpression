using System;
using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		加载基本项状态，如果当前Token是字符则开启至该状态
	/// </summary>
	internal class LoadBasicItemContextState : RuleContextState
	{
		public override void Run(RuleContext context)
		{
			// 读取组合词并建立组合词的节点
			WordGroup wordGroup = this.LoadWordGroup(context);

			if (context.Token != Constants.Slash)
			{
				// showError("单词和标记之间缺少'/'" + Environment.NewLine + lex");
			}
			context.GetToken(); // 跳过 '/'

			// 读取组合词并建立组合词的节点
			LabelGroup labelGroup = this.LoadLabelGroup(context);

			context.CurrentRuleItem = new BasicRuleItem(wordGroup, labelGroup);
		}

		# region Word Loader

		/// <summary>
		///		解析并加载-------组合词
		/// </summary>
		/// <returns></returns>
		private WordGroup LoadWordGroup(RuleContext context)
		{
			WordGroup wg = new WordGroup(true, null);
			if (context.Token != Constants.Star)
			{
				wg.IsStar = false;
				wg.WordExpr = this.LoadWordExpression(context);
			}
			else
				context.GetToken();

			return wg;
		}

		/// <summary>
		///		解析并加载------词表达式
		/// </summary>
		/// <returns></returns>
		private WordExpression LoadWordExpression(RuleContext context)
		{
			List<WordItem> list = new List<WordItem>();  // 组成词表达式的词项列表

			WordItem wordItem = this.LoadWordItem(context);
			list.Add(wordItem);

			while (context.Token == Constants.Or)
			{
				context.GetToken();
				wordItem = this.LoadWordItem(context);
				list.Add(wordItem);
			}

			WordExpression wordExpr = new WordExpression(list);

			return wordExpr;
		}

		/// <summary>
		///		解析并加载------词项
		/// </summary>
		/// <returns></returns>
		private WordItem LoadWordItem(RuleContext context)
		{
			WordItem wordItem;
			if (context.Token == Constants.Not)  // 检查是否是非标记
			{
				context.GetToken();
				WordExpression wordExpression = this.LoadWordExpression(context);
				wordItem = new WordItem(Constants.IsWordExpr, true, null, wordExpression);
			}
			else if (context.Token == Constants.Lparen)  // 检查是否是左括号
			{
				context.GetToken();
				WordExpression wordExpression = this.LoadWordExpression(context);

				if (context.Token != Constants.Rparen)  // 检查括号是否匹配
				{
					//showError("表达式括号不匹配");
					return null;
				}
				else
				{
					context.GetToken();
					wordItem = new WordItem(Constants.IsWordExpr, false, null, wordExpression);
				}
			}
			else
			{
				WordAtom wordAtom = this.LoadWordAtom(context);
				wordItem = new WordItem(Constants.IsWordAtom, false, wordAtom, null);
			}
			return wordItem;
		}

		/// <summary>
		///		解析并加载------词项
		/// </summary>
		/// <returns></returns>
		private WordAtom LoadWordAtom(RuleContext context)
		{
			WordAtom wordAtom;

			string Lex = "";
			while (context.Token == Constants.Eng || context.Token == Constants.Hz || context.Token == Constants.Number)
			{
				Lex += context.Lex;
				context.GetToken();
			}
			wordAtom = new WordAtom(Lex);

			return wordAtom;
		}

		#endregion

		# region Label Loader


		/// <summary>
		///		解析并加载------组合标记
		/// </summary>
		/// <returns></returns>
		private LabelGroup LoadLabelGroup(RuleContext context)
		{
			LabelGroup labelGroup = new LabelGroup(true, null);
			if (context.Token != Constants.Percent)
			{
				labelGroup.IsPercent = false;
				labelGroup.LabelExpr = this.LoadLabelExpression(context);
			}
			else
			{
				context.GetToken();
			}

			return labelGroup;
		}

		/// <summary>
		///		解析并加载------标记表达式
		/// </summary>
		/// <returns></returns>
		private LabelExpression LoadLabelExpression(RuleContext context)
		{
			List<LabelItem> list = new List<LabelItem>();
			LabelItem labelItem = this.LoadLabelItem(context);
			list.Add(labelItem);

			while (context.Token == Constants.Or)
			{
				context.GetToken();
				labelItem = this.LoadLabelItem(context);
				list.Add(labelItem);
			}

			LabelExpression labelExpression = new LabelExpression(list);
			return labelExpression;
		}

		/// <summary>
		///		解析并加载------标记项
		/// </summary>
		/// <returns></returns>
		private LabelItem LoadLabelItem(RuleContext context)
		{
			List<LabelAtom> list = new List<LabelAtom>();

			LabelAtom labelAtom = this.LoadLabelAtom(context);
			list.Add(labelAtom);

			while (context.Token == Constants.And)
			{
				context.GetToken();
				labelAtom = this.LoadLabelAtom(context);
				list.Add(labelAtom);
			}

			LabelItem labelItem = new LabelItem(list);
			return labelItem;
		}

		/// <summary>
		///		解析并加载------标记因子
		/// </summary>
		/// <returns></returns>
		private LabelAtom LoadLabelAtom(RuleContext context)
		{
			LabelAtom labelAtom;

			if (context.Token == Constants.Not)
			{
				context.GetToken();
				LabelExpression labelExpression = this.LoadLabelExpression(context);
				labelAtom = new LabelAtom(Constants.IsLabelExpr, true, null, labelExpression);
			}
			else if (context.Token == Constants.Lparen)
			{
				context.GetToken();
				LabelExpression labelExpression = this.LoadLabelExpression(context);

				if (context.Token != Constants.Rparen)
				{
					//showError("表达式括号不匹配！");
					return null;
				}
				else
				{
					labelAtom = new LabelAtom(Constants.IsLabelExpr, false, null, labelExpression);
					context.GetToken();
				}
			}
			else
			{
				if (context.Token == Constants.Eng || context.Token == Constants.Number
					|| context.Token == Constants.Mark || context.Token == Constants.Dollar)
				{
					labelAtom = new LabelAtom(Constants.IsLabelStr, false, context.Lex, null);
					context.GetToken();
				}
				else
				{
					//showError("标记应为英文字符");
					return null;
				}
			}
			return labelAtom;
		}

		
		# endregion
	}
}