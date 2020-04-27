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
        protected bool Equals(Car other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Car) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
