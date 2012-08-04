using System.Collections.Generic;

namespace JsonPatch
{
	public class JsonMembersComparer : IEqualityComparer<KeyValuePair<string, JsonMemberInfo>>
	{
		public bool Equals(KeyValuePair<string, JsonMemberInfo> x, KeyValuePair<string, JsonMemberInfo> y)
		{
			return x.Key.Equals(y.Key);
		}

		public int GetHashCode(KeyValuePair<string, JsonMemberInfo> obj)
		{
			return obj.Key.GetHashCode();
		}
	}
}