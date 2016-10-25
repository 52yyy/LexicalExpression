using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace LexicalExpression.Tests.Unit
{
	[TestFixture]
    public class Class1
    {
		[Test]
		public void ToyCase()
		{
			Lexex ruleParser = new Lexex("Rule_Cause");
			LexSentence sen = new LexSentence();
			sen.Words = new List<LexWord>();
			sen.Words.Add(new LexWord() { Content = "原告", Pos = "n" });
			sen.Words.Add(new LexWord() { Content = "与", Pos = "c" });
			sen.Words.Add(new LexWord() { Content = "被告", Pos = "n" });
			sen.Words.Add(new LexWord() { Content = "离婚纠纷", Pos = "mainPoint" });
			sen.Words.Add(new LexWord() { Content = "一案", Pos = "m" });
			sen.Words.Add(new LexWord() { Content = "，", Pos = "w" });
			Match causeCollection = ruleParser.Match(sen, 0);

			LexSentence[] causes = causeCollection.MatchCollection["案由"].ToArray();
			Assert.IsTrue(causes.Length == 1);
			Assert.IsTrue(causes[0].Content == "离婚纠纷");
		}
    }
}
