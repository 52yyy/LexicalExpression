namespace LexicalExpression
{
	/// <summary>
	///		词子类 WordAtom
	/// </summary>
	internal class WordAtom : ILexWordMatching
	{
		public WordAtom(string word)
		{
			this.Word = word;
		}

		/// <summary>
		///		单词串
		/// </summary>
		public string Word { get; set; }

		public bool IsMatch(LexWord matchWord)
		{
			if (matchWord == null)
				return false;
			if (this.Word == matchWord.Content)
				return true;
			else
				return false;
		}

		public override string ToString()
		{
			string output = "词子：";
			output += "<" + this.Word + ">";
			return output;
		}
	}
}