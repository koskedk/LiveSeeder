using System;
using System.Reflection;

namespace LiveSeeder.Utils
{
    public static class CustomExtensions
    {
        public static string HasToEndWith(this string value, string end)
        {
            if (value == null)
                return string.Empty;

            return value.EndsWith(end) ? value : $"{value}{end}";
        }

        public static string ToOsStyle(this string value)
        {
            if (value == null)
                return string.Empty;

            if(Environment.OSVersion.Platform==PlatformID.Unix||Environment.OSVersion.Platform==PlatformID.MacOSX)
                return value.Replace(@"\", @"/");

            return value.Replace(@"/", @"\");
        }

        public static string ToNamespace(this string value)
        {
            if (value == null)
                return string.Empty;

            return value.ToOsStyle()
                .Replace(@"/", @".")
                .Replace(@"\", @".");
        }

        public static string ToCleanName(this AssemblyName value)
        {
            if (value.Name == null)
                return string.Empty;

            var names = value.Name.Split(',');
            if (names.Length > 0)
                return names[0];

            return value.Name;
        }
    }
}
