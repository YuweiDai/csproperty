using CSCZJ.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class PropertyCreateModel : BaseQMEntityModel
    {
        public PropertyCreateModel()
        {

        }

       
        public string Name { get; set; }

        public int PropertyTypeId { get; set; }
     
        public string Address { get; set; }

        public float Floor { get; set; }

        /// <summary>
        /// 四至情况
        /// </summary>
        public string FourToStation { get; set; }

        public string GetedDate { get; set; }

        /// <summary>
        /// 获取方式
        /// </summary>
        public int GetModeId { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public bool IsAdmission { get; set; }

        /// <summary>
        /// 不动产证
        /// </summary>
        public string EstateId { get; set; }

        /// <summary>
        /// 不动产证发证时间
        /// </summary>
        public string EstateTime { get; set; }

        /// <summary>
        /// 房产证
        /// </summary>
        public string ConstructId { get; set; }

        /// <summary>
        /// 建筑面积
        /// </summary>
        public double ConstructArea { get; set; }

        /// <summary>
        /// 房产证发证时间
        /// </summary>
        public string ConstructTime { get; set; }

        /// <summary>
        /// 土地证号
        /// </summary>
        public string LandId { get; set; }

        /// <summary>
        /// 土地面积
        /// </summary>
        public double LandArea { get; set; }

        public string LandTime { get; set; }

        public int GovernmentId { get; set; }

        public string GovernmentName { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public int UseTypeId { get; set; }

        /// <summary>
        /// 使用现状
        /// </summary>
        public int CurrentTypeId { get; set; }

        /// <summary>
        /// 是否抵押
        /// </summary>
        public bool IsMortgage { get; set; }

        public string Description { get; set; }

        public string Logo { get; set; }

        public string LogoUrl { get; set; }

        public int LogoPictureId { get; set; }

        public string Location { get; set; }

        public string Extent { get; set; }

        public string UsedPeople { get; set; }

        public bool IsMain { get; set; }

        public int ParentPropertyId { get; set; }

        #region 暂时未用到
        public double CurrentUse_Self { get; set; }

        public double CurrentUse_Rent { get; set; }

        public double CurrentUse_Lend { get; set; }

        public double CurrentUse_Idle { get; set; }

        public bool RegisterEstate { get; set; }

        public bool Owner_self { get; set; }

        public bool Owner_children { get; set; }

        public double Price { get; set; }

        public string PropertyNature { get; set; }

        public string LandNature { get; set; }


        public int LifeTime { get; set; } 
        #endregion

        public bool Submit { get; set; }

        public virtual ICollection<PropertyPictureModel> Pictures { get; set; }

        public virtual ICollection<PropertyFileModel> Files { get; set; }
    }


    public class TestModel
    {
       public int Id { get; set; }

        public string Name { get; set; }

        public int PropertyTypeId { get; set; }
     
        public string Address { get; set; }

        public float Floor { get; set; }

        /// <summary>
        /// 四至情况
        /// </summary>
        public string FourToStation { get; set; }

        public string GetedDate { get; set; }

        /// <summary>
        /// 获取方式
        /// </summary>
        public int GetModeId { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public bool IsAdmission { get; set; }

        /// <summary>
        /// 不动产证
        /// </summary>
        public string EstateId { get; set; }

        /// <summary>
        /// 不动产证发证时间
        /// </summary>
        public string EstateTime { get; set; }

        /// <summary>
        /// 房产证
        /// </summary>
        public string ConstructId { get; set; }

        /// <summary>
        /// 建筑面积
        /// </summary>
        public double ConstructArea { get; set; }

        /// <summary>
        /// 房产证发证时间
        /// </summary>
        public string ConstructTime { get; set; }

        /// <summary>
        /// 土地证号
        /// </summary>
        public string LandId { get; set; }

        /// <summary>
        /// 土地面积
        /// </summary>
        public double LandArea { get; set; }

        public string LandTime { get; set; }

        public int GovernmentId { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public int UseTypeId { get; set; }

        /// <summary>
        /// 使用现状
        /// </summary>
        public int CurrentTypeId { get; set; }

        /// <summary>
        /// 是否抵押
        /// </summary>
        public bool IsMortgage { get; set; }

        public string Description { get; set; }

        public string Logo { get; set; }

        public string LogoUrl { get; set; }

        public int LogoPictureId { get; set; }

        public string Location { get; set; }

        public string Extent { get; set; }

        public string UsedPeople { get; set; }

        public bool IsMain { get; set; }

        public int ParentPropertyId { get; set; }

        #region 暂时未用到
        public double CurrentUse_Self { get; set; }

        public double CurrentUse_Rent { get; set; }

        public double CurrentUse_Lend { get; set; }

        public double CurrentUse_Idle { get; set; }

        public bool RegisterEstate { get; set; }

        public bool Owner_self { get; set; }

        public bool Owner_children { get; set; }

        public double Price { get; set; }

        public string PropertyNature { get; set; }

        public string LandNature { get; set; }


        public int LifeTime { get; set; }
        #endregion

        public bool Submit { get; set; }

        public virtual ICollection<PropertyPictureModel> Pictures { get; set; }

        public virtual ICollection<PropertyFileModel> Files { get; set; }
    }
}