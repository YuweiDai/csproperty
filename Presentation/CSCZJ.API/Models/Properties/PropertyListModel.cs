using CSCZJ.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class PropertyListModel: BaseQMEntityModel
    {
        /// <summary>
        /// 资产名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 资产类别
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// 获取方式
        /// </summary>
        public string GetMode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 建筑面积
        /// </summary>
        public double ConstructArea { get; set; }

        /// <summary>
        /// 土地面积
        /// </summary>
        public double LandArea { get; set; }

        /// <summary>
        /// 层数
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// 产权证号
        /// </summary>
        public string PropertyID { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public string IsAdmission { get; set; }

        /// <summary>
        /// 取得时间
        /// </summary>
        public string GetedDate { get; set; }

        /// <summary>
        /// 使用方
        /// </summary>
        public string UsedPeople { get; set; }

        /// <summary>
        /// 四至情况
        /// </summary>
        public string FourToStation { get; set; }

        /// <summary>
        /// 不动产证
        /// </summary>
        public string EstateId { get; set; }

        /// <summary>
        /// 房产证
        /// </summary>
        public string ConstructId { get; set; }
        /// <summary>
        /// 房产证发证时间
        /// </summary>
        public string ConstructTime { get; set; }

        /// <summary>
        /// 土地证号
        /// </summary>
        public string LandId { get; set; }

        public string LandTime { get; set; }

        /// <summary>
        /// 使用现状
        /// </summary>
        public string CurrentType { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string UseType { get; set; }

        /// <summary>
        /// 是否抵押
        /// </summary>
        public string IsMortgage { get; set; }


        public int GovernmentId { get; set; }

        public string GovernmentName { get; set; }

        public bool CanEditDelete { get; set; }
    }
}