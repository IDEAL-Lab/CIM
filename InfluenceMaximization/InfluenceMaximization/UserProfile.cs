using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluenceMaximization
{
	public class UserProfile
	{
		public int id;
		public int type; // Type indicates which seed probability function is assigned to this user.
		public UserProfile (int id, int type)
		{
			this.id = id;
			this.type = type;
		}
	}
}

