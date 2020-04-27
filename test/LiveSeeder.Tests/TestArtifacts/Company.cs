using System;
using System.ComponentModel.DataAnnotations;

namespace LiveSeeder.Tests.TestArtifacts
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} |{Id}";
        }

        protected bool Equals(Company other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Company) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
