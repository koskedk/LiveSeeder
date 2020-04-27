using System;
using System.ComponentModel.DataAnnotations;

namespace LiveSeeder.Tests.TestArtifacts
{
    public class Car
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} |{Id}";
        }
    }
}
