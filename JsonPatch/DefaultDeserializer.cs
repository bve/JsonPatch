using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonPatch
{
	public class DefaultDeserializer : IJsonSerializer
	{
		public JsonMemberInfo Deserialize(string json)
		{
			var data = JsonConvert.DeserializeObject<JToken>(json);
			var root = this.GetMemberInfo(new JsonPath { Property = "$" }, data, null);
			return root;
		}

		protected JsonMemberInfo GetMemberInfo(JsonPath path, JToken data, JsonMemberInfo parent)
		{
			var info = new JsonMemberInfo { Value = data.ToString(), Parent = parent, Path = path };
		
			if (data.Type == JTokenType.Array)
			{
				info.Children = new SortedDictionary<string, JsonMemberInfo>();
				info.IsArray = true;
				var idx = 0;
				foreach (var entry in ((IList<JToken>)data))
				{
					var key = idx.ToString(CultureInfo.InvariantCulture);
					var newPath = new JsonPath { Property = key, IsIndexer = true };
					newPath.Prev = path.Clone(newPath);
					info.Children.Add(key, this.GetMemberInfo(newPath, entry, info));
					idx++;
				}
			}
			if (data.Type == JTokenType.Object)
			{
				info.Children = new SortedDictionary<string, JsonMemberInfo>();
				info.IsObject = true;
				foreach (var entry in ((IDictionary<string, JToken>)data))
				{
					var newPath = new JsonPath { Property = entry.Key, IsIndexer = false };
					newPath.Prev = path.Clone(newPath);
					info.Children.Add(entry.Key, this.GetMemberInfo(newPath, entry.Value, info));
				}
			}
			if (data.Type == JTokenType.Boolean)
			{
				info.Value = Convert.ToBoolean(data.ToString());
			}
			if (data.Type == JTokenType.Float)
			{
				info.Value = Convert.ToDouble(data.ToString());
			}
			if (data.Type == JTokenType.Integer)
			{
				info.Value = Convert.ToInt32(data.ToString());
			}
			if (data.Type == JTokenType.Guid)
			{
				info.Value = new Guid(data.ToString());
			}
			if (data.Type == JTokenType.String)
			{
				info.Value = Convert.ToString(data.ToString());
			}
			if (data.Type == JTokenType.Null)
			{
				info.Value = null;
			}
			if (data.Type == JTokenType.Date)
			{
				info.Value = DateTime.Parse(data.ToString());
			}
			return info;
		}

		public string Serialize(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
	}
}
