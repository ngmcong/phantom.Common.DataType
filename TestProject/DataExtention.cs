using NUnit.Framework;
using phantom;

namespace TestProject
{
    public class TestObject1
    {
        public string Name { get; set; }
    }
    public class TestObject2
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class DataExtention
    {
        [TestCase]
        public void CopyValueTo()
        {
            var obj1 = new TestObject1 { Name = "ACBC" };
            var obj2 = obj1.CopyValueTo<TestObject2>();
            Assert.That(true, Is.True, $"{obj2} should not be prime");
        }
    }
}