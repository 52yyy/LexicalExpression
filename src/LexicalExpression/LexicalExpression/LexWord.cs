namespace LexicalExpression
{
	/// <summary>
	///		单词类
	/// </summary>
	public class LexWord
	{
		public LexWord()
		{
			Pos = "null";
			IsStart = false;
			IsEnd = false;
		}

		public LexWord(string input)
			: this()
		{
			Content = input;
		}

		/// <summary>
		///		词形
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		///		词号
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		///		词性
		/// </summary>
		public string Pos { get; set; }

		/// <summary>
		///		是否句首词，匹配^用
		/// </summary>
		public bool IsStart { get; set; }

		/// <summary>
		///		是否句尾词，匹配$用
		/// </summary>
		public bool IsEnd { get; set; }

		public int Start { get; set; }

		public int End { get; set; }

		public override string ToString()
		{
			return Content + "/" + Pos;
		}
	}
}
