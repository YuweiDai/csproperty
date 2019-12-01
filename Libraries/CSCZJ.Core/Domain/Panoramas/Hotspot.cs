using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Panoramas
{
    public class Hotspot : BaseEntity
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 水平角度
        /// </summary>
        public double Ath { get; set; }
        /// <summary>
        /// 垂直角度
        /// </summary>
        public double Atv { get; set; }

        public virtual PanoramaScene PanoramaScene { get; set; }

    }

}
