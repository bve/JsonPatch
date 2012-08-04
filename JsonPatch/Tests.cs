using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LitJson;
using NUnit.Framework;

namespace JsonPatch
{
	class Tests
	{
		[Test]
		public void Test1()
		{
			var info = new LitJsonDeserializer().Deserialize("{'baz': 'qux','foo': 'bar'}");
			Debugger.Launch();
		}

		[Test]
		public void Test2()
		{
			var info = new JsonDiffPatch(new LitJsonDeserializer()).GetPatch("{'baz': 'qux','foo': 'bar', 'a': [ 0 ]}", "{'baz': 'qux1','foo': 'bar', 'a': [ 2, 1 ]}");
			var list = info.ToList();
			Debugger.Launch();
		}
	}
}
