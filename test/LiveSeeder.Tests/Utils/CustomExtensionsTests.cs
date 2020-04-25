using LiveSeeder.Utils;
using NUnit.Framework;

namespace LiveSeeder.Tests.Utils
{
    [TestFixture]
    public class CustomExtensionsTests
    {
        [Test]
        public void should_ensure_string_Ends()
        {
            string a = "A";
            Assert.AreEqual("A-", a.HasToEndWith("-"));
        }

        [Test]
        public void should_change_To_OsPath()
        {
            var dir = TestContext.CurrentContext.WorkDirectory.ToOsStyle();
            Assert.False(string.IsNullOrWhiteSpace(dir));
        }

        [Test]
        public void should_change_Folder_To_Namepsace()
        {
            var dir = "Seed/Other";
            Assert.AreEqual("Seed.Other",dir.ToNamespace());
        }
    }
}
