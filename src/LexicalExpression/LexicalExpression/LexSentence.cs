using System.Collections.Generic;
using System.Text;

namespace LexicalExpression
{
	/// <summary>
	///		整句类
	/// </summary>
	public class LexSentence
	{
		public LexSentence()
		{
			Words = new List<LexWord>();
		}

		/// <summary>
		///		句子内容
		/// </summary>
		public string Content
		{
			get
			{
				return ToWordsString();
			}
		}

		/// <summary>
		///		词的集合
		/// </summary>
		public List<LexWord> Words { get; set; } 
		
		public LexSentence SubSentence(int startIndex, int length)
		{
			LexSentence subSent = new LexSentence();
			for (int i = 0; i < length; i++)
			{
				LexWord tempWord = this.Words[startIndex + i];
				tempWord.Id = i;
				subSent.Words.Add(tempWord);
			}
			return subSent;
		}

		public LexSentence SubSentence(int startIndex)
		{
			int length = Words.Count - startIndex;
			return SubSentence(startIndex, length);
		}

		public override string ToString()
		{
			StringBuilder output = new StringBuilder();
			foreach (LexWord w in this.Words)
			{
				output.Append(w.ToString());
				output.Append("  ");
			}
			return output.ToString();
		}

		public string ToWordsString()
		{
			StringBuilder output = new StringBuilder();
			foreach (LexWord w in this.Words)
			{
				output.Append(w.Content);
			}
			return output.ToString();
		}

		public string GetWordsString(int start, int end)
		{
			if (start < end)
			{
				return "NULL";
			}

			StringBuilder sb = new StringBuilder();
			for (; start <= end; start++)
			{
				sb.Append(Words[start].Content);
			}
			return sb.ToString();
		}
	}
}
