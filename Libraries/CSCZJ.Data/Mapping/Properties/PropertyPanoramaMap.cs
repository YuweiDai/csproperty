using CSCZJ.Core.Domain.Properties;

namespace CSCZJ.Data.Mapping.Properties
{
    public class PropertyPanoramaMap : EntityTypeConfiguration<PropertyPanorama>
    {
        public PropertyPanoramaMap()
        {
            this.ToTable("Property_Panorama_Mapping");
            this.HasKey(p => p.Id);
            this.HasRequired(pp => pp.Property).WithMany(p => p.PropertyPanoramas);
            this.HasRequired(pp => pp.PanoramaLocation).WithMany(p => p.PropertyPanoramas);
        }
    }
}
