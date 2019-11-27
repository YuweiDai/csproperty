using System.ComponentModel;

namespace CSCZJ.Core.Domain.Properties
{
    /// <summary>
    /// 资产类别
    /// </summary>
    public enum PropertyType
    {
        [Description("非转让(局机关)")]
        House = 0,   //
        [Description("非转让(房管处)")]
        Land = 1,     //
        [Description("自有")]
        LandUnderHouse = 2, //
        [Description("国资注入")]
        Others = 99 
    }
}
