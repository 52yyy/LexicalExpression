using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace LexicalExpression.Tests.Unit
{
	[TestFixture]
	public class NewRuleLoaderTests
	{
		[Test]
		public void Case()
		{
			IRuleLoader loader = new NewRuleLoader();
			Rule rule = loader.LoadOneRule("*/mainPoint + 一/m + 案/ng = {案由:1}");
			Console.WriteLine(rule);

			LexSentence sen = new LexSentence();
			sen.Words = new List<LexWord>();
			sen.Words.Add(new LexWord() { Content = "原告", Pos = "n" });
			sen.Words.Add(new LexWord() { Content = "与", Pos = "c" });
			sen.Words.Add(new LexWord() { Content = "被告", Pos = "n" });
			sen.Words.Add(new LexWord() { Content = "离婚纠纷", Pos = "mainPoint" });
			sen.Words.Add(new LexWord() { Content = "一", Pos = "m" });
			sen.Words.Add(new LexWord() { Content = "案", Pos = "ng" });
			sen.Words.Add(new LexWord() { Content = "，", Pos = "w" });

			Lexex lexex = new Lexex();
			lexex.AddRule(rule);
			Match causeCollection = lexex.Match(sen, 0);
			Console.WriteLine(causeCollection);
		}
	}
}