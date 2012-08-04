using System.Collections.Generic;

namespace JsonPatch
{
	public class JsonMemberInfo
	{
		public JsonMemberInfo Parent { get; set; }
		public JsonPath Path { get; set; }
		public string Value { get; set; }
		public bool IsArray { get; set; }
		public bool IsObject { get; set; }
		public SortedDictionary<string, JsonMemberInfo> Children { get; set; }
	}
}