namespace JsonPatch
{
	public class JsonPatchRemove : JsonPatchBase
	{
		public JsonPath Path { get; set; }

		public override JsonDiffPatchOperation Operation
		{
			get { return JsonDiffPatchOperation.Remove; }
		}
	}
}