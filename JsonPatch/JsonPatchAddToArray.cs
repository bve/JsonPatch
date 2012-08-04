namespace JsonPatch
{
	public class JsonPatchAddToArray : JsonPatchBase
	{
		public JsonPath Path { get; set; }

		public int Element { get; set; }

		public object Value { get; set; }

		public override JsonDiffPatchOperation Operation
		{
			get { return JsonDiffPatchOperation.Add; }
		}
	}
}