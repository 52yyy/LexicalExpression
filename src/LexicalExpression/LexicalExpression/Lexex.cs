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
		public NameValueCollection GetMatchedItems(LexSentence inputSentence)
		{
			ILexicalExpressionMatcher matcher;
			foreach (Rule rule in _rules)
			{
				matcher = rule.CreateLexicalExpressionMatcher();
				if (matcher.Match(inputSentence))
				{
					return matcher.GetMatchResult(inputSentence);
				}
			}
			return new NameValueCollection { { "null", "null" } };
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
			RuleLoader loader = new RuleLoader();
			foreach (string ruleStr in ruleStrs)
			{
				Rule loadOneRule = loader.loadOneRule(ruleStr);
				if (loadOneRule == null)
				{
					continue;
				}
				this._rules.Add(loadOneRule);
			}
		}

		public Match Run(LexSentence inputSentence, int startIndex)
		{
			ILexicalExpressionMatcher matcher;

			foreach (Rule rule in _rules)
			{
				matcher = rule.CreateLexicalExpressionMatcher();
				if (matcher.Match(inputSentence, startIndex))
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
