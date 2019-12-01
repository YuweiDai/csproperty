using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Panoramas
{
    /// <summary>
    /// 全景拍摄点
    /// </summary>
    public class PanoramaScene : BaseEntity
    {
        private ICollection<Hotspot> _hotspots;
        public PanoramaScene()
        { }


        /// <summary>
        /// 水平角度
        /// </summary>
        public double Ath { get; set; }
        /// <summary>
        /// 垂直角度
        /// </summary>
        public double Atv { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 缩略图路径，用于CSS模式的展示
        /// </summary>
        public string ThumPath { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int Views { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime ProductionDate { get; set; }

        /// <summary>
        /// 最后一次访问日期
        /// </summary>
        public DateTime LastViewDate { get; set; }


        public virtual PanoramaLocation PanoramaLocation { get; set; }

        public virtual ICollection<Hotspot> Hotspots
        {
            get { return _hotspots ?? (_hotspots = new List<Hotspot>()); }
            protected set { _hotspots = value; }
        }

    }
}