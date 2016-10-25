using System.Collections.Generic;
using System.Collections.Specialized;

namespace LexicalExpression
{
	public interface ILexicalExpressionMatcher
	{
		bool Match(LexSentence sent);

		bool Match(LexSentence sent, int startIndex);

		bool MatchOneRuleForSentence(int startRuleId, LexSentence sentence, int startPos);

		Dictionary<int, string> GetMatchValue(LexSentence sent, Dictionary<int, int[]> curMatchValueItem);

		Dictionary<int, LexSentence> GetMatchLexSentence(LexSentence sent, Dictionary<int, int[]> curMatchValueItem);

		NameValueCollection GetMatchResult(LexSentence sent);

		Match Run(LexSentence sent);
	}
}
