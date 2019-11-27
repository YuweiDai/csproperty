﻿using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Data.Mapping.Properties
{
    public class PropertyPictureMap : EntityTypeConfiguration<PropertyPicture>
    {
        public PropertyPictureMap()
        {
            this.ToTable("Property_Picture_Mapping");
            this.HasKey(pp => pp.Id);

            this.HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.Property)
                .WithMany(p => p.Pictures)
                .HasForeignKey(pp => pp.PropertyId);
        }
    }
}
