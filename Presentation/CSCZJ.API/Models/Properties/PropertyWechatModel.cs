using CSCZJ.Web.Framework.Mvc;

namespace CSCZJ.API.Models.Properties
{
    /// <summary>
    /// 用于微信小程序端的模型数据
    /// </summary>
    public class PropertyWechatModel : BaseQMEntityModel
    {
        public PropertyWechatModel()
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
        /// 资产所处区域
        /// </summary>
        public string Region { get; set; }

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

        public string EstateId { get; set; }

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
        /// 下步使用或处置建议
        /// </summary>
        public string NextStepUsage { get; set; }

        /// <summary>
        /// 下步使用或处置建议枚举
        /// </summary>
        public int NextStepUsageId { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double x { get; set; }

        public double y { get; set; }

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

        public string LogoUrl { get; set; }

        /// <summary>
        /// 获取方式
        /// </summary>
        public string GetMode { get; set; }

        /// <summary>
        /// 层数
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public bool IsAdmission { get; set; }

        /// <summary>
        /// 四至情况
        /// </summary>
        public string FourToStation { get; set; }

        /// <summary>
        /// 房产证发证时间
        /// </summary>
        public string ConstructTime { get; set; }

        public string LandTime { get; set; }

        public string EstateTime { get; set; }

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
        public bool IsMortgage { get; set; }

        public int ParentPropertyId { get; set; }
    }

    public class PropertySimpleWechatModel : BaseQMEntityModel
    {
        public string Name { get; set; }

        public string PropertyType { get; set; }

        public string Address { get; set; }

        public string Logo { get; set; }

        public string GovernmentName { get; set; }
    }
}