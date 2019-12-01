using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace CSCZJ.Core.Domain.Panoramas
{
    /// <summary>
    /// 街景路线
    /// </summary>
    public class Street:BaseEntity
    {
        private ICollection<PanoramaLocation> _panoramaLocations;

        public Street()
        {
        }

        /// <summary>
        /// 街道名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 街道路径
        /// </summary>
        public DbGeography Path { get; set; }

        public virtual ICollection<PanoramaLocation> PanoramaLocations
        {
            get { return _panoramaLocations ?? (_panoramaLocations = new List<PanoramaLocation>()); }
            protected set { _panoramaLocations = value; }
        }
    }
}
