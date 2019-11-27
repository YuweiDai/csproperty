using CSCZJ.Core.Domain.Properties;

namespace CSCZJ.Data.Mapping.Properties
{
    public class PropertyMap : EntityTypeConfiguration<Property>
    {
        public PropertyMap()
        {
            this.ToTable("Property");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(255);
            this.Property(p => p.UsedPeople).IsRequired().HasMaxLength(255);
            
            this.HasMany(p => p.Lends).WithRequired(t=>t.Property);
            this.HasMany(p => p.Rents).WithRequired(t=>t.Property);
            this.HasMany(p => p.Allots).WithRequired(t=>t.Property);
            this.HasMany(p => p.Patrols).WithRequired(t => t.Property);
            this.HasMany(p => p.Pictures).WithRequired(pp => pp.Property);
            //this.Property(p => p.Location).IsRequired();


        }
    }
}
