using NUnit.Framework;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.SemanticComparison;

namespace JsonPatch
{
	class Tests
	{
		private readonly IFixture _fixture = new Fixture().Customize(new MultipleCustomization());

		[Test]
		public void SimpleTest()
		{
			var testObj1 = _fixture.CreateAnonymous<TestClass>();
			var testObj2 = _fixture.CreateAnonymous<TestClass>();
			var deserializer = new DefaultDeserializer();
			var jsonDiff = new JsonDiffPatch(deserializer);
			var json1 = deserializer.Serialize(testObj1);
			var patch = jsonDiff.GetPatch(json1, deserializer.Serialize(testObj2));
			var json2 = jsonDiff.ApplyPatch(patch, json1);
			var testObj3 = JsonConvert.DeserializeObject<TestClass>(json2);
			new Likeness<TestClass, TestClass>(testObj2).ShouldEqual(testObj3);
		}
	}

	public class TestClass
	{
		public string String { get; set; }
		public int Int { get; set; }
	}
}
