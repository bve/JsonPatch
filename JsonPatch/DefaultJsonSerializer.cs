using System;
using System.Web.Script.Serialization;

namespace JsonPatch
{
	public class DefaultJsonSerializer : IJsonSerializer
	{
		private readonly JavaScriptSerializer _serializer;

		public DefaultJsonSerializer()
		{
			_serializer = new JavaScriptSerializer();
		}

		public JsonMemberInfo Deserialize(string json)
		{
			var obj = _serializer.DeserializeObject(json);
			throw new NotImplementedException();
		}

		public string Serialize(object obj)
		{
			return _serializer.Serialize(obj);
		}
	}
}