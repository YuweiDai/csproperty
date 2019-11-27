using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Data.Mapping.Properties
{
    public class PropertyPatrolPictureMap : EntityTypeConfiguration<PropertyPatrolPicture>
    {
        public PropertyPatrolPictureMap() {

            this.ToTable("PropertyPatrol_Picture_Mapping");
            this.HasKey(pp => pp.Id);

            this.HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.PropertyPatrol)
                .WithMany(p => p.PatrolPictures)
                .HasForeignKey(pp => pp.PropertyPatrolId);
        
        }

    }
}
