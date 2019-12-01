using CSCZJ.Core.Domain.Panoramas;

namespace CSCZJ.Data.Mapping.Panoramas
{
    public class PanoramaLocationMap : EntityTypeConfiguration<PanoramaLocation>
    {
        public PanoramaLocationMap()
        {
            this.ToTable("PanoramaLocations");
            this.HasKey(pl => pl.Id);

            
        }
    }
}
