using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LexicalExpression
{
	[Serializable]
	public class Match
	{
		internal static Match _empty = new Match();

		internal int _matchCollectionCount
		{
			get
			{
				return MatchCollection.Count;
			}
		}

		internal Lexex _lexex;

		internal Rule _matchRule { get; set; }

		internal Dictionary<int, int[]> _matchValues = new Dictionary<int, int[]>();

		public Dictionary<string, List<LexSentence>> MatchCollection { get; set; }

		public LexSentence Text { get; set; }

		public int BeginIndex { get; set; }

		public int EndIndex { get; set; }

		public bool Success
		{
			get
			{
				return this._matchCollectionCount != 0;
			}
		}

		static Match()
		{
		}

		/// <summary>
		/// Gets the empty group. All failed matches return this empty match.
		/// </summary>
		/// 
		/// <returns>
		/// An empty match.
		/// </returns>
		public static Match Empty
		{
			get
			{
				return Match._empty;
			}
		}

		public Match()
		{
			this._matchValues = new Dictionary<int, int[]>();
			this.MatchCollection = new Dictionary<string, List<LexSentence>>();
		}

		public Match GetNext()
		{
			return this._lexex.Run(this.Text, EndIndex + 1);
		}
	}
}