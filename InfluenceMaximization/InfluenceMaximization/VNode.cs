using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluenceMaximization
{
	public class VNode
	{
		public int id { set; get; }
		public double val { set; get; }
		public VNode(int id, double val)
		{
			this.id = id;
			this.val = val;
		}
	}

	public class VNodeComparer : IComparer<VNode>
	{
		public int Compare(VNode n1, VNode n2)
		{
			if (n1.val > n2.val)
				return 1;
			if (n1.val < n2.val)
				return -1;
			return 0;
		}
	}
}

