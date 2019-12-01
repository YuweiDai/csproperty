using CSCZJ.Core.Domain.Panoramas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Data.Mapping.Panoramas
{
    public class StreetMap : EntityTypeConfiguration<Street>
    {
        public StreetMap()
        {
            this.ToTable("Streets");
            this.HasKey(s => s.Id);

            this.HasMany(s => s.PanoramaLocations).WithOptional(pl => pl.Street);
        }
    }
}
