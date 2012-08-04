using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LitJson;
using NUnit.Framework;

namespace JsonPatch
{
	public class LitJsonDeserializer : IJsonSerializer
	{
		public JsonMemberInfo Deserialize(string json)
		{
			var data = JsonMapper.ToObject(json);
			var root = GetMemberInfo(new JsonPath { Property = "$" }, data, null);
			return root;
		}

		protected JsonMemberInfo GetMemberInfo(JsonPath path, JsonData data, JsonMemberInfo parent)
		{
			var info = new JsonMemberInfo { Path = path, Value = data.ToJson(), Parent = parent };
			if (data.IsArray)
			{
				info.Children = new SortedDictionary<string, JsonMemberInfo>();
				info.IsArray = true;
				var idx = 0;
				foreach (var entry in ((IList)data))
				{
					var key = idx.ToString(CultureInfo.InvariantCulture);
					var newPath = new JsonPath { Prev = path, Property = key, IsIndexer = true };
					if (entry is JsonData)
					{
						info.Children.Add(key, GetMemberInfo(newPath, (JsonData)entry, info));
					}
					else
					{
						info.Children.Add(key, new JsonMemberInfo { Path = newPath, Value = entry.ToString() });
					}
					idx++;
				}
			}
			if (data.IsObject)
			{
				info.Children = new SortedDictionary<string, JsonMemberInfo>();
				info.IsObject = true;
				foreach (DictionaryEntry entry in ((IDictionary)data))
				{
					var newPath = new JsonPath { Prev = path, Property = entry.Key.ToString(), IsIndexer = false };
					if (entry.Value is JsonData)
					{
						info.Children.Add(entry.Key.ToString(), GetMemberInfo(newPath, (JsonData)entry.Value, info));
					}
					else
					{
						info.Children.Add(entry.Key.ToString(), new JsonMemberInfo { Path = newPath, Value = entry.Value.ToString() });
					}
				}
			}
			if (data.IsBoolean || data.IsDouble || data.IsInt || data.IsLong || data.IsString)
			{
				info.Value = data.ToString();
			}
			return info;
		}

		public string Serialize(object obj)
		{
			return JsonMapper.ToJson(obj);
		}
	}
}