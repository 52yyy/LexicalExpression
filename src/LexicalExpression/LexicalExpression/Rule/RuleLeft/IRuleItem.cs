namespace LexicalExpression
{
	/// <summary>
	///		规则项接口
	/// </summary>
	public interface IRuleItem
	{
		bool IsMatch(LexWord matchWord);

		int Match(LexSentence sent, int startIndex);

		string ToString();
	}
}