namespace JsonPatch
{
	public class JsonPath
	{
		public string Property { get; set; }
		public bool IsIndexer { get; set; }
		public JsonPath Prev { get; set; }
		public JsonPath Next { get; set; }
	}
}