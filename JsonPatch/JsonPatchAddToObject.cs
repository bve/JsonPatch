namespace JsonPatch
{
	public class JsonPatchAddToObject : JsonPatchBase
	{
		public JsonPath Path { get; set; }

		public string Element { get; set; }

		public object Value { get; set; } 

		public override JsonDiffPatchOperation Operation
		{
			get { return JsonDiffPatchOperation.Add; }
		}
	}
}