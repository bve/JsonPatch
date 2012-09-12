using System.Text;

namespace JsonPatch
{
	public class JsonPatchBase
	{
		public JsonPatchBase()
			: this(JsonDiffPatchOperation.Add)
		{

		}

		public JsonPatchBase(JsonDiffPatchOperation operation)
		{
			this.Operation = operation;
		}

		public string PathString
		{
			get
			{
				var sb = new StringBuilder();
				if (this.Path != null)
				{
					var node = this.Path.GetRoot();
					while (node != null)
					{
						if (sb.Length == 0)
						{
							sb.Append(node.Property);
						}
						else
						{
							if (node.IsIndexer)
							{
								sb.AppendFormat("[{0}]", node.Property);
							}
							else
							{
								sb.AppendFormat(".{0}", node.Property);
							}
						}
						if (node == this.Path)
						{
							break;
						}
						node = node.Next;
					}
					return sb.ToString();
				}
				return string.Empty;
			}
			set
			{
				var root = new JsonPath { Property = "$" };

				var strings = value.Split('.', '[');

				var current = root;

				foreach (var str in strings)
				{
					if (str == "$")
					{
						continue;
					}

					var next = new JsonPath();

					if (str.EndsWith("]"))
					{
						next.Property = str.TrimEnd(']');
						next.IsIndexer = true;
					}
					else
					{
						next.Property = str;
					}

					next.Prev = current;
					current.Next = next;

					current = next;
				}

				this.Path = current;
			}
		}

		public JsonPath Path { get; set; }

		public object Element { get; set; }

		public object Value { get; set; }

		public JsonDiffPatchOperation Operation { get; set; }
	}
}
