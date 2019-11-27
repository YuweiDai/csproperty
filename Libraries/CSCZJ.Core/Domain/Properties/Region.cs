using System.ComponentModel;

namespace CSCZJ.Core.Domain.Properties
{
    public enum Region
    {
        [Description("天马镇")]
        TMZ = 0,
        [Description("招贤镇")]
        ZSZ = 1,
        [Description("辉埠镇")]
        HBZ = 2,
        [Description("球川镇")]
        LQZ = 3,
        [Description("宋畈乡")]
        SBZ = 4,
        [Description("其他")]
        Others = 99
    }
}
