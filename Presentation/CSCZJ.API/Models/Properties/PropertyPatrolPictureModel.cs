using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class PropertyPatrolPictureModel
    {
        public int PictureId { get; set; }

        public int PropertyPatrolId { get; set; }

        public int DisplayOrder { get; set; }

        /// <summary>
        /// 图片SEO名称
        /// </summary>
        public string SeoFilename { get; set; }

        /// <summary>
        /// Gets or sets the "alt" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. property name)
        /// </summary>
        public string Alt { get; set; }

        /// <summary>
        /// Gets or sets the "title" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. property name)
        /// </summary>
        public string Title { get; set; }

        public string Href { get; set; }

    }
}