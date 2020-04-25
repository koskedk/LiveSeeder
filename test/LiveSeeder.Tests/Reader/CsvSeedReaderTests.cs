using System.Linq;
using LiveSeeder.Reader;
using LiveSeeder.Tests.TestArtifacts;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LiveSeeder.Tests.Reader
{
    [TestFixture]
    public class CsvSeedReaderTests
    {
        private ISeedReader _carCsvSeedReader;

        [SetUp]
        public void Setup()
        {
            _carCsvSeedReader = TestInitializer.ServiceProvider.GetService<ISeedReader>();
        }

        [Test]
        public void should_Read()
        {
            var results = _carCsvSeedReader.Read<TestCar>();
            Assert.True(results.Any());
        }

        [Test]
        public void should_Read_Custom()
        {
            var results = _carCsvSeedReader.Read<Car>(typeof(Car).Assembly, "|","Seed/Other","allcars.csv");
            Assert.True(results.Any());
        }
    }
}
