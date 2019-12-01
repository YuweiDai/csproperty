using CSCZJ.Core.Domain.Panoramas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Properties
{
    /// <summary>
    /// 资产的全景
    /// </summary>
    public class PropertyPanorama : BaseEntity
    {
        /// <summary>
        /// 所属资产
        /// </summary>
        public virtual Property Property { get; set; }

        /// <summary>
        /// 所属全景点位
        /// </summary>
        public virtual PanoramaLocation PanoramaLocation { get; set; }

        /// <summary>
        /// 水平角度
        /// </summary>
        public double Ath { get; set; }
        /// <summary>
        /// 垂直角度
        /// </summary>
        public double Atv { get; set; }

        public int Order { get; set; }
    }
}
