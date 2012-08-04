namespace JsonPatch
{
	public interface IJsonSerializer
	{
		JsonMemberInfo Deserialize(string json);
		string Serialize(object obj);
	}
}