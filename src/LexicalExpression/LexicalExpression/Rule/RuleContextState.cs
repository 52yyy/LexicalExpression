namespace LexicalExpression
{
	/// <summary>
	///		规则上下文状态
	/// </summary>
	internal abstract class RuleContextState
	{
		/// <summary>
		///		在当前状态下执行处理
		/// </summary>
		/// <param name="context">当前的上下文</param>
		public abstract void Run(RuleContext context);
	}
}