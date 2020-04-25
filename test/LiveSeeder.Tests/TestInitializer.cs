using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using LiveSeeder.Reader;
using LiveSeeder.Tests.TestArtifacts;
using LiveSeeder.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Z.Dapper.Plus;

namespace LiveSeeder.Tests
{
    [SetUpFixture]
    public class TestInitializer
    {
        public static IServiceProvider ServiceProvider;
        public static IDbConnection Connection;
        public static string ConnectionString;
        public static IConfigurationRoot Configuration;

        [OneTimeSetUp]
        public void Init()
        {
            SqlMapper.AddTypeHandler<Guid>(new GuidTypeHandler());
            RemoveTestsFilesDbs();

            var dir = $"{TestContext.CurrentContext.TestDirectory.HasToEndWith(@"/")}";

            var config = Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.License.json", optional: true, reloadOnChange: true)
                .Build();

            RegisterLicence();

            var connectionString = config.GetConnectionString("liveConnection")
                .Replace("#dir#", dir);
            ConnectionString = connectionString.ToOsStyle();
            Connection = new SqliteConnection(connectionString);
            var services = new ServiceCollection()
                .AddTransient<ISeedReader, CsvSeedReader>();
            ServiceProvider = services.BuildServiceProvider();

            DapperPlusManager.Entity<Car>()
                .Key(x => x.Id)
                .Table(nameof(Car));

            DapperPlusManager.Entity<TestCar>()
                .Key(x=>x.Id)
                .Table(nameof(TestCar));

            DapperPlusManager.Entity<Company>()
                .Key(x=>x.Id)
                .Table(nameof(Company));

            DapperPlusManager.Entity<County>()
                .Key(x=>x.Id)
                .Table(nameof(County));

        }

        public static void SeedData(params IEnumerable<object>[] entities)
        {
            Connection.BulkInsert(entities);
        }

        private void RegisterLicence()
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DAPPER_LICENSENAME")))
            {
                DapperPlusManager.AddLicense(Environment.GetEnvironmentVariable("DAPPER_LICENSENAME"),
                    Environment.GetEnvironmentVariable("DAPPER_LICENSEKEY"));
                if (!Z.Dapper.Plus.DapperPlusManager.ValidateLicense(out var licenseErrorMessage))
                    throw new Exception(licenseErrorMessage);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Configuration["DapperPlusManager:licenseName"]))
            {
                DapperPlusManager.AddLicense(Configuration["DapperPlusManager:licenseName"],
                    Configuration["DapperPlusManager:licenseKey"]);
                if (!Z.Dapper.Plus.DapperPlusManager.ValidateLicense(out var licenseErrorMessage))
                    throw new Exception(licenseErrorMessage);
            }
        }

        private void RemoveTestsFilesDbs()
        {
            string[] keyFiles =
                { "seed.db"};
            string[] keyDirs = { @"TestArtifacts/Database".ToOsStyle()};

            foreach (var keyDir in keyDirs)
            {
                DirectoryInfo di = new DirectoryInfo(keyDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (!keyFiles.Contains(file.Name))
                        file.Delete();
                }
            }
        }
    }
}
