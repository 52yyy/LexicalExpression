using System;
using System.Collections.Generic;

namespace LexicalExpression
{
	/// <summary>
	///		规则右部项  RightRuleItem
	/// </summary>
	public class RightRuleItem
	{
		public List<int> ids;
		public List<int> nameIdList;
		public string name;
		public string rightRuleKind;

		public RightRuleItem()
		{
			this.ids = new List<int>();
			this.name = String.Empty;
			this.nameIdList = new List<int>();
			this.rightRuleKind = String.Empty;
		}

		public RightRuleItem(List<int> ids, string name)
		{
			this.ids = ids;
			this.name = name;
		}

		public RightRuleItem(List<int> ids,List<int> nameIdList)
		{
			this.ids = ids;
			this.nameIdList = nameIdList;
		}
	}
}