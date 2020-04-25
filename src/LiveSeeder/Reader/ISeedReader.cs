using System.Collections.Generic;
using System.Reflection;

namespace LiveSeeder.Reader
{
    public interface ISeedReader
    {
        IEnumerable<T> Read<T>() where T : class;

        IEnumerable<T> Read<T>(Assembly assembly, string delimiter = ",", string @namespace = "Seed",
            string fileName = "") where T : class;
    }
}
