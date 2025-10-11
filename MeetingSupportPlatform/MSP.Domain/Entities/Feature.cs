using MSP.Domain.Base;

namespace MSP.Domain.Entities
{
    public class Feature : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }
        public bool IsUnlimited { get; set; }
        public int? LimitValue { get; set; }
        public string? LimitUnit { get; set; }

        public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
