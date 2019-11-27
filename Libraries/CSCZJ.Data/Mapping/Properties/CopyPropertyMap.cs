using CSCZJ.Core.Domain.Properties;

namespace CSCZJ.Data.Mapping.Properties
{
    public class CopyPropertyMap : EntityTypeConfiguration<CopyProperty>
    {
        public CopyPropertyMap()
        {
            this.ToTable("CopyProperty");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(255);
            this.Property(p => p.UsedPeople).IsRequired().HasMaxLength(255);
            
        }
    }
}
