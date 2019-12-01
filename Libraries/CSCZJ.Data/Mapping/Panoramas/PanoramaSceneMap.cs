using CSCZJ.Core.Domain.Panoramas;

namespace CSCZJ.Data.Mapping.Panoramas
{
    public class PanoramaSceneMap : EntityTypeConfiguration<PanoramaScene>
    {
        public PanoramaSceneMap()
        {
            this.ToTable("PanoramaScenes");
            this.HasKey(pl => pl.Id);

            this.HasRequired(ps => ps.PanoramaLocation).WithMany(pl => pl.PanoramaScenes);

        }
    }
}
