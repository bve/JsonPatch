namespace JsonPatch
{
	public abstract class JsonPatchBase
	{
		public abstract JsonDiffPatchOperation Operation { get; }
	}
}