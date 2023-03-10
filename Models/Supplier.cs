namespace CarDealer.Models
{
    using System.Collections.Generic;

    public class Supplier
    {
        public Supplier()
        {
            this.Parts = new HashSet<Part>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsImporter { get; set; }

        public ICollection<Part> Parts { get; set; }

        public override bool Equals(object obj)
        {
           var other= obj as Supplier;
            if (other != null && this.Id == other.Id)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
