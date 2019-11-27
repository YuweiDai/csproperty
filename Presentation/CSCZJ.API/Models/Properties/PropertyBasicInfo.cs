﻿using CSCZJ.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class PropertyBasicInfo : BaseQMEntityModel
    {
        public PropertyBasicInfo()
        {
        }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 资产类别
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// 资产类别枚举
        /// </summary>
        public int PropertyTypeId { get; set; }

        /// <summary>
        /// 资产所处区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 资产所处区域枚举
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 建筑面积
        /// </summary>
        public float ConstructArea { get; set; }

        /// <summary>
        /// 土地面积
        /// </summary>
        public float LandArea { get; set; }

        /// <summary>
        /// 产权证号
        /// </summary>
        public string PropertyID { get; set; }

        public string EsatetId { get; set; }

        public string ConstructId { get; set; }

        public string LandId { get; set; }

        /// <summary>
        /// 房产性质
        /// </summary>
        public string PropertyNature { get; set; }

        /// <summary>
        /// 土地性质
        /// </summary>
        public string LandNature { get; set; }

        /// <summary>
        /// 账面价格，单位万元
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// 取得时间
        /// </summary>
        public string GetedDate { get; set; }

        /// <summary>
        /// 使用年限
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        /// 使用方
        /// </summary>
        public string UsedPeople { get; set; }

        /// <summary>
        /// 自用面积
        /// </summary>
        public float CurrentUse_Self { get; set; }

        /// <summary>
        /// 出租面积
        /// </summary>
        public float CurrentUse_Rent { get; set; }

        /// <summary>
        /// 出借面积
        /// </summary>
        public float CurrentUse_Lend { get; set; }

        /// <summary>
        /// 闲置面积
        /// </summary>
        public float CurrentUse_Idle { get; set; }

        /// <summary>
        /// 坐落位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 范围，房产不需要
        /// </summary>
        public string Extent { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// 资产所有方
        /// </summary>
        public int GovernmentId { get; set; }

        public string GovernmentName { get; set; }

        public bool Locked { get; set; }
    }
}