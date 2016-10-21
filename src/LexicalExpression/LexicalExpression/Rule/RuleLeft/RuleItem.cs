namespace LexicalExpression
{
	/// <summary>
	///		规则元素类  RuleItem
	/// </summary>
	public class RuleItem : IRuleItem
	{
		public int RuleKind;               // 标识是哪种规则。1：基本项；2：越过项；3：重复可选项
		public int id;
		public IRuleItem _ruleItem { get; private set; }

		public RuleItem(int ruleKind, IRuleItem ruleItem)
		{
			this.RuleKind = ruleKind;
			this._ruleItem = ruleItem;
		}

		public bool IsMatch(LexWord matchWord)
		{
			return this._ruleItem.IsMatch(matchWord);
		}

		public int Match(LexSentence sent, int startIndex)
		{
			return this._ruleItem.Match(sent, startIndex);
		}

		public override string ToString()
		{
			return this._ruleItem.ToString();
		}
	}
}