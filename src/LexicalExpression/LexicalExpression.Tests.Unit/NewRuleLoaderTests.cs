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
		}
	}
}