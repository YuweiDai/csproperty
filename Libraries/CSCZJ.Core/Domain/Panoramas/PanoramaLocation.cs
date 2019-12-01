using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Panoramas
{
    /// <summary>
    /// 全景点位
    /// </summary>
    public class PanoramaLocation : BaseEntity
    {
        private ICollection<PanoramaScene> _panoramaScenes;
        private ICollection<PropertyPanorama> _propertyPanoramas;

        public string Name { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 全景类型，空中 or 地面
        /// </summary>
        public PanoramaType PanoramaType { get; set; }

        /// <summary>
        /// 默认的全景SceneId
        /// </summary>
        public int DefaultPanoramaSceneId { get; set; }

        /// <summary>
        /// 所属街道id
        /// </summary>
        public int StreetLineId { get; set; }
        public virtual Street Street { get; set; }

        /// <summary>
        /// 一个点对应的全景集合
        /// </summary>
        public virtual ICollection<PanoramaScene> PanoramaScenes
        {
            get { return _panoramaScenes ?? (_panoramaScenes = new List<PanoramaScene>()); }
            protected set { _panoramaScenes = value; }
        }

        public virtual ICollection<PropertyPanorama> PropertyPanoramas
        {
            get { return _propertyPanoramas ?? (_propertyPanoramas = new List<PropertyPanorama>()); }
            protected set { _propertyPanoramas = value; }
        }
    }
}
