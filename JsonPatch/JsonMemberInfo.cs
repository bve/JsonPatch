using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JsonPatch
{
	public class JsonMemberInfo
	{
		public JsonMemberInfo Parent { get; set; }
		public JsonPath Path { get; set; }
		public object Value { get; set; }
		public bool IsArray { get; set; }
		public bool IsObject { get; set; }
		public SortedDictionary<string, JsonMemberInfo> Children { get; set; }

		public string ToJson(IJsonSerializer serializer)
		{
			var sb = new StringBuilder();
			if (this.IsArray)
			{
				sb.Append("[");
				foreach (var child in this.Children.OrderBy(x => x.Key))
				{
					sb.Append((string)child.Value.ToJson(serializer));
					sb.Append(",");
				}
				if (this.Children.Count > 0)
				{
					sb.Remove(sb.Length - 1, 1);
				}
				sb.Append("]");
			}
			else if (this.IsObject)
			{
				sb.Append("{");
				foreach (var child in this.Children.OrderBy(x => x.Key))
				{
					sb.AppendFormat("\"{0}\"", (object)child.Key);
					sb.Append(":");
					sb.Append((string)child.Value.ToJson(serializer));
					sb.Append(",");
				}
				if (this.Children.Count > 0)
				{
					sb.Remove(sb.Length - 1, 1);
				}
				sb.Append("}");
			}
			else
			{
				if (this.Value is string)
				{
					sb.AppendFormat("\"{0}\"", this.Value);
				}
				else
				{
					if (this.Value is bool)
					{
						sb.Append(((bool)this.Value).ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
					}
					else
					{
						if (this.Value == null)
						{
							sb.Append("null");
						}
						else
						{
							sb.Append(this.Value);
						}
					}
				}
			}
			return sb.ToString();
		}

		public JsonMemberInfo FindByPath(JsonPath path, JsonPath target)
		{
			if (target != null && (path == target || (target.Prev != null && path == target.Prev.Next)))
			{
				return this;
			}

			if (path.Next == null)
			{
				return this;
			}

			var next = path.Next;

			if (this.IsArray)
			{
				if (next.IsIndexer)
				{
					JsonMemberInfo info;
					if (this.Children != null && this.Children.Count > next.GetIndex() && this.Children.TryGetValue(next.Property, out info))
					{
						return info.FindByPath(next, target);
					}
				}
			}
			if (this.IsObject)
			{
				if (!next.IsIndexer)
				{
					JsonMemberInfo info;
					if (this.Children != null && this.Children.TryGetValue(next.Property, out info))
					{
						return info.FindByPath(next, target);
					}
				}
			}

			return this;
		}
	}
}
