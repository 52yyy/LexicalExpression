using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace LexicalExpression
{
	/// <summary>
	///		词法表达式解析器
	/// </summary>
	public class Lexex
	{
		private List<Rule> _rules;

		public Lexex(string ruleFileName)
		{
			this.LoadRules(ruleFileName);
		}

		private void LoadRules(string ruleFileName)
		{
			this._rules = new List<Rule>();
			string fileName = ruleFileName;
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException("未能找到规则文件", ruleFileName);
			}

			IEnumerable<string> ruleStrs = File.ReadLines(fileName, Encoding.UTF8);
			IRuleLoader loader = new NewRuleLoader();
			foreach (string ruleStr in ruleStrs)
			{
				Rule loadOneRule = loader.LoadOneRule(ruleStr);
				if (loadOneRule == null)
				{
					continue;
				}
				this._rules.Add(loadOneRule);
			}
		}

		/// <summary>
		///		在指定的输入句子中搜索由Lexex构造函数指定的词法表达式匹配项。
		/// </summary>
		/// <param name="inputSentence">待搜索匹配项的句子</param>
		/// <returns></returns>
		public Match Match(LexSentence inputSentence)
		{
			return this.Match(inputSentence, 0);
		}

		/// <summary>
		///		在指定的输入句子中搜索由Lexex构造函数指定的词法表达式匹配项。
		/// </summary>
		/// <param name="inputSentence">待搜索匹配项的句子</param>
		/// <param name="startat">在句子中指定起始位置</param>
		/// <returns></returns>
		public Match Match(LexSentence inputSentence, int startat)
		{
			ILexicalExpressionMatcher matcher;

			foreach (Rule rule in _rules)
			{
				matcher = rule.CreateLexicalExpressionMatcher();
				if (matcher.Match(inputSentence, startat))
				{
					Match match = matcher.Run(inputSentence);
					match._lexex = this;
					return match;
				}
			}
			return new Match();
		}
	}
}
