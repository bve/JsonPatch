using System;

namespace JsonPatch
{
	public class JsonPath
	{
		public string Property { get; set; }

		public bool IsIndexer { get; set; }

		public JsonPath Prev { get; set; }

		public JsonPath Next { get; set; }

		public JsonPath GetRoot()
		{
			var current = this;
			while (current.Prev != null)
			{
				current = current.Prev;
			}
			return current;
		}

		public JsonPath Clone(JsonPath next, int level = 0)
		{
			var path = new JsonPath { IsIndexer = this.IsIndexer, Property = this.Property };
			if (next != null)
			{
				path.Next = next.Clone(next.Next, level + 1);
			}
			if (this.Prev != null)
			{
				path.Prev = this.Prev.Clone(path, level - 1);
			}
			if (path.Property == null)
			{
				throw new Exception("AAAAAAAAA");
			}
			return path;
		}

		public int GetIndex()
		{
			int index;
			if (this.IsIndexer && int.TryParse(this.Property, out index))
			{
				return index;
			}
			throw new InvalidCastException(this.Property);
		}
	}
}
