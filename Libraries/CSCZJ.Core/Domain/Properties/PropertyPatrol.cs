using System;
using System.Collections.Generic;

namespace CSCZJ.Core.Domain.Properties
{
    public class PropertyPatrol : BaseEntity
    {
        private ICollection<PropertyPatrolPicture> _propertyPatrolPictures;

        public PropertyPatrol()
        {

        }
        /// <summary>
        /// 巡查时间
        /// </summary>
        public DateTime PatrolDate { get; set; }
        /// <summary>
        /// 巡查人
        /// </summary>
        public string People { get; set; }
        /// <summary>
        /// 巡查内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 巡查人电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>
        public string Title { get; set; }
        public virtual Property Property { get; set; }

        public virtual ICollection<PropertyPatrolPicture> PatrolPictures
        {
            get { return _propertyPatrolPictures ?? (_propertyPatrolPictures = new List<PropertyPatrolPicture>()); }
            protected set { _propertyPatrolPictures = value; }
        }


    }
}
