using System;

namespace LiveSeeder.Tests.TestArtifacts
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} |{Id}";
        }
    }
}