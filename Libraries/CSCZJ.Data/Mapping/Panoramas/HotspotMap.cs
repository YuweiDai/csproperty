using CSCZJ.Core.Domain.Panoramas;

namespace CSCZJ.Data.Mapping.Panoramas
{
    public class HotspotMap : EntityTypeConfiguration<Hotspot>
    {
        public HotspotMap()
        {
            this.ToTable("Hotspots");
            this.HasKey(t => t.Id);
            this.HasRequired(ps => ps.PanoramaScene).WithMany(pl => pl.Hotspots);

        }

    }
}
