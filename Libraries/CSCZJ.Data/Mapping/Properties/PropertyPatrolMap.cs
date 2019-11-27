using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Data.Mapping.Properties
{
    public class PropertyPatrolMap : EntityTypeConfiguration<PropertyPatrol>
    {

        public PropertyPatrolMap() {

            this.ToTable("PropertyPatrol");
            this.HasKey(p => p.Id);
        }

    }
}
