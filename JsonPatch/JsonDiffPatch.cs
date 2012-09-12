using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JsonPatch
{
	public class JsonDiffPatch
	{
		private readonly IJsonSerializer _serializer;

		public JsonDiffPatch()
			: this(new DefaultDeserializer())
		{ }

		public JsonDiffPatch(IJsonSerializer serializer)
		{
			Debug.Assert(serializer != null, "serializer != null");
			if (serializer == null)
				throw new ArgumentNullException("serializer");
			this._serializer = serializer;
		}

		public List<JsonPatchBase> GetPatch(string json1, string json2)
		{
			var patches = GetPatchInternal(json1, json2).ToList();
			if (patches.Count > 0)
			{
				return patches;
			}
			return null;
		}

		private IEnumerable<JsonPatchBase> GetPatchInternal(string json1, string json2)
		{
			var graph1 = this._serializer.Deserialize(json1);
			var graph2 = this._serializer.Deserialize(json2);

			if (graph1 != null && graph2 != null)
			{
				var queue = new Queue<KeyValuePair<JsonMemberInfo, JsonMemberInfo>>();
				queue.Enqueue(new KeyValuePair<JsonMemberInfo, JsonMemberInfo>(graph1, graph2));
				while (queue.Count > 0)
				{
					var dequeue = queue.Dequeue();
					var item1 = dequeue.Key;
					var item2 = dequeue.Value;
					if (item1.Path.Property == item2.Path.Property)
					{
						if (item1.Children == null)
						{
							if (item2.Children == null)
							{
								if (Comparer.Default.Compare(item1.Value, item2.Value) != 0)
								{
									yield return new JsonPatchBase(JsonDiffPatchOperation.Remove) { Path = item1.Path };
									if (item2.Parent.IsArray)
									{
										yield return new JsonPatchBase { Path = item2.Path.Prev, Element = Convert.ToInt32((string)item2.Path.Property), Value = item2.Value };
									}
									else
									{
										yield return new JsonPatchBase { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
									}
								}
							}
							else
							{
								yield return new JsonPatchBase(JsonDiffPatchOperation.Remove) { Path = item1.Path };
								if (item2.IsArray)
								{
									yield return new JsonPatchBase { Path = item2.Path.Prev, Element = Convert.ToInt32((string)item2.Path.Property), Value = item2.Value };
								}
								else
								{
									yield return new JsonPatchBase { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
								}
							}
						}
						else
						{
							if (item2.Children != null)
							{
								var intersect = item1.Children.Intersect(item2.Children, new JsonMembersComparer());
								foreach (var pair in intersect)
								{
									queue.Enqueue(new KeyValuePair<JsonMemberInfo, JsonMemberInfo>(item1.Children[pair.Key], item2.Children[pair.Key]));
								}
								var except = item2.Children.Except(item1.Children, new JsonMembersComparer());
								foreach (var pair in except)
								{
									var info = pair.Value;
									if (info.Parent.IsArray)
									{
										yield return new JsonPatchBase { Path = info.Path.Prev, Element = Convert.ToInt32((string)info.Path.Property), Value = info.Value };
									}
									else
									{
										yield return new JsonPatchBase { Path = info.Path.Prev, Element = info.Path.Property, Value = info.Value };
									}
								}
								var subtract = item1.Children.Except(item2.Children, new JsonMembersComparer());
								foreach (var pair in subtract)
								{
									yield return new JsonPatchBase(JsonDiffPatchOperation.Remove) { Path = pair.Value.Path };
								}
							}
							else
							{
								yield return new JsonPatchBase(JsonDiffPatchOperation.Remove) { Path = item1.Path };
								if (item2.IsArray)
								{
									yield return new JsonPatchBase { Path = item2.Path.Prev, Element = Convert.ToInt32((string)item2.Path.Property), Value = item2.Value };
								}
								else
								{
									yield return new JsonPatchBase { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
								}
							}
						}
					}
					else
					{
						yield return new JsonPatchBase(JsonDiffPatchOperation.Remove) { Path = item1.Path };
						if (item2.IsArray)
						{
							yield return new JsonPatchBase { Path = item2.Path.Prev, Element = Convert.ToInt32((string)item2.Path.Property), Value = item2.Value };
						}
						else
						{
							yield return new JsonPatchBase { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
						}
					}
				}
			}
		}

		public string ApplyPatch(IEnumerable<JsonPatchBase> patches, string json)
		{
			var graph = _serializer.Deserialize(json);

			if (patches == null)
			{
				return json;
			}

			foreach (var patch in patches)
			{
				if (patch.Operation == JsonDiffPatchOperation.Remove)
				{
					var root = patch.Path.GetRoot();
					var memberInfo = graph.FindByPath(root, patch.Path);
					if (memberInfo != null)
					{
						if (memberInfo.Parent != null)
						{
							memberInfo.Parent.Children.Remove(memberInfo.Parent.Children.Single(x => x.Value == memberInfo).Key);
						}
						else
						{
							if (memberInfo == graph)
							{
								graph = new JsonMemberInfo { Path = new JsonPath { Property = "$" } };
							}
							else
							{
								throw new Exception("Invalid JSON Graph Structure!");
							}
						}
					}
					else
					{
						throw new Exception("Invalid JSON Structure!");
					}
				}
				if (patch.Operation == JsonDiffPatchOperation.Add)
				{
					if (patch.Element is int)
					{
						var root = patch.Path.GetRoot();
						var memberInfo = graph.FindByPath(root, patch.Path);
						if (memberInfo != null)
						{
							if (memberInfo.IsArray)
							{
								var key = Convert.ToString(patch.Element);
								if (!memberInfo.Children.ContainsKey(key))
								{
									var info = new JsonMemberInfo
									           	{
									           		Parent = memberInfo,
									           		Path = new JsonPath { IsIndexer = true, Property = key },
									           		Value = patch.Value
									           	};
									info.Path.Prev = memberInfo.Path.Clone(info.Path);
									memberInfo.Children.Add(key, info);
								}
								else
								{
									throw new Exception("Invalid JSON Structure! Index already exists.");
								}
							}
							else
							{
								throw new Exception("Invalid JSON Structure! Not an Array.");
							}
						}
						else
						{
							throw new Exception("Invalid JSON Structure!");
						}
					}
					if (patch.Element is string)
					{
						var root = patch.Path.GetRoot();
						var memberInfo = graph.FindByPath(root, patch.Path);
						if (memberInfo != null)
						{
							if (memberInfo.IsObject)
							{
								var key = Convert.ToString(patch.Element);
								if (!memberInfo.Children.ContainsKey(key))
								{
									var info = new JsonMemberInfo
									           	{
									           		Parent = memberInfo,
									           		Path = new JsonPath { IsIndexer = false, Property = key },
									           		Value = patch.Value
									           	};
									info.Path.Prev = memberInfo.Path.Clone(info.Path);
									memberInfo.Children.Add(key, info);
								}
								else
								{
									throw new Exception("Invalid JSON Structure! Property already exists.");
								}
							}
							else
							{
								throw new Exception("Invalid JSON Structure! Not an Object.");
							}
						}
						else
						{
							throw new Exception("Invalid JSON Structure!");
						}
					}
				}
			}

			return graph.ToJson(this._serializer);
		}
	}
}
