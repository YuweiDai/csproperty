using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class PropertyEditApproveListModel : PropertyApproveListModel
    {
        public string Name { get; set; }

        /// <summary>
        /// 出借面积
        /// </summary>
        public float LendArea { get; set; }
    }
}