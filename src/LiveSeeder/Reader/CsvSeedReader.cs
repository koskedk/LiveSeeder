using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using LiveSeeder.Utils;

namespace LiveSeeder.Reader
{
    public class CsvSeedReader : ISeedReader
    {
        public IEnumerable<T> Read<T>() where T : class
        {
            return Read<T>(Assembly.GetCallingAssembly());
        }

        public IEnumerable<T> Read<T>(Assembly assembly, string delimiter = ",", string @namespace = "Seed",
            string fileName = "") where T : class
        {
            List<T> records;

            var name = string.IsNullOrWhiteSpace(fileName) ? typeof(T).Name : fileName;
            var resourceName = $"{assembly.GetName().Name}.{@namespace.ToNamespace()}.{name.HasToEndWith(".csv")}";
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (null == stream)
                throw new Exception("Could not read resource!");

            using (StreamReader reader = new StreamReader(stream))
            {
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.Delimiter = delimiter;
                records = csv.GetRecords<T>().ToList();
            }

            return records;
        }
    }
}
