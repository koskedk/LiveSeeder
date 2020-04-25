using System;

namespace LiveSeeder.Tests.TestArtifacts
{
    public class TestCar
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public TestCar()
        {
        }

        public override string ToString()
        {
            return $"{Name} |{Id}";
        }

        protected bool Equals(TestCar other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestCar) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
