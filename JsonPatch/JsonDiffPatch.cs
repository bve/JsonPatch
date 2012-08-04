using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace JsonPatch
{
	public class JsonDiffPatch
	{
		private readonly IJsonSerializer _serializer;

		public JsonDiffPatch()
			: this(new DefaultJsonSerializer())
		{ }

		public JsonDiffPatch(IJsonSerializer serializer)
		{
			Debug.Assert(serializer != null, "serializer != null");
			if (serializer == null)
				throw new ArgumentNullException("serializer");
			_serializer = serializer;
		}

		public IEnumerable<JsonPatchBase> GetPatch(string json1, string json2)
		{
			var graph1 = _serializer.Deserialize(json1);
			var graph2 = _serializer.Deserialize(json2);

			if (graph1 != null && graph2 != null)
			{
				var queue = new Queue<Tuple<JsonMemberInfo, JsonMemberInfo>>();
				queue.Enqueue(new Tuple<JsonMemberInfo, JsonMemberInfo>(graph1, graph2));
				while (queue.Count > 0)
				{
					var dequeue = queue.Dequeue();
					var item1 = dequeue.Item1;
					var item2 = dequeue.Item2;
					if (item1.Path.Property == item2.Path.Property)
					{
						if (item1.Children == null)
						{
							if (item2.Children == null)
							{
								if (item1.Value != item2.Value)
								{
									yield return new JsonPatchRemove { Path = item1.Path };
									if (item2.IsArray)
									{
										yield return new JsonPatchAddToArray { Path = item2.Path.Prev, Element = Convert.ToInt32(item2.Path.Property), Value = item2.Value };
									}
									else
									{
										yield return new JsonPatchAddToObject { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
									}
								}
							}
							else
							{
								yield return new JsonPatchRemove { Path = item1.Path };
								if (item2.IsArray)
								{
									yield return new JsonPatchAddToArray { Path = item2.Path.Prev, Element = Convert.ToInt32(item2.Path.Property), Value = item2.Value };
								}
								else
								{
									yield return new JsonPatchAddToObject { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
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
									queue.Enqueue(new Tuple<JsonMemberInfo, JsonMemberInfo>(item1.Children[pair.Key], item2.Children[pair.Key]));
								}
								var except = item2.Children.Except(item1.Children, new JsonMembersComparer());
								foreach (var pair in except)
								{
									var info = pair.Value;
									if (info.IsArray)
									{
										yield return new JsonPatchAddToArray { Path = info.Path.Prev, Element = Convert.ToInt32(info.Path.Property), Value = info.Value };
									}
									else
									{
										yield return new JsonPatchAddToObject { Path = info.Path.Prev, Element = info.Path.Property, Value = info.Value };
									}
								}
							}
							else
							{
								yield return new JsonPatchRemove { Path = item1.Path };
								if (item2.IsArray)
								{
									yield return new JsonPatchAddToArray { Path = item2.Path.Prev, Element = Convert.ToInt32(item2.Path.Property), Value = item2.Value };
								}
								else
								{
									yield return new JsonPatchAddToObject { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
								}
							}
						}
					}
					else
					{
						yield return new JsonPatchRemove { Path = item1.Path };
						if (item2.IsArray)
						{
							yield return new JsonPatchAddToArray { Path = item2.Path.Prev, Element = Convert.ToInt32(item2.Path.Property), Value = item2.Value };
						}
						else
						{
							yield return new JsonPatchAddToObject { Path = item2.Path.Prev, Element = item2.Path.Property, Value = item2.Value };
						}
					}
				}
			}
		}

		public string ApplyPatch(IEnumerable<JsonPatchBase> patch, string json)
		{
			throw new NotImplementedException();
		}
	}
}
