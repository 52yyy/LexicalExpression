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
			Rule rule = loader.LoadOneRule("*/mainPoint + һ/m + ��/ng = {����:1}");
			Console.WriteLine(rule);

			LexSentence sen = new LexSentence();
			sen.Words = new List<LexWord>();
			sen.Words.Add(new LexWord() { Content = "ԭ��", Pos = "n" });
			sen.Words.Add(new LexWord() { Content = "��", Pos = "c" });
			sen.Words.Add(new LexWord() { Content = "����", Pos = "n" });
			sen.Words.Add(new LexWord() { Content = "������", Pos = "mainPoint" });
			sen.Words.Add(new LexWord() { Content = "һ", Pos = "m" });
			sen.Words.Add(new LexWord() { Content = "��", Pos = "ng" });
			sen.Words.Add(new LexWord() { Content = "��", Pos = "w" });

			Lexex lexex = new Lexex();
			lexex.AddRule(rule);
			Match causeCollection = lexex.Match(sen, 0);
			Console.WriteLine(causeCollection);
		}
	}
}