using System.ComponentModel;

namespace CSCZJ.Core.Domain.Properties
{
    public enum Region
    {
        [Description("天马街道")]
        TMJD = 0,
        [Description("紫港街道")]
        ZGJD = 1,
        [Description("白石镇")]
        BSZ = 2,
        [Description("芳村镇")]
        FCZ = 3,
        [Description("招贤镇")]
        ZXZ = 4,
        [Description("球川镇")]
        QCZ = 5,
         [Description("东案乡")]
        DAX = 6,
         [Description("何家乡")]
         HJX = 7,
         [Description("青石镇")]
         QSZ = 8,
         [Description("金川街道")]
         JCJD = 9,
         [Description("同弓乡")]
         TGX = 10,
         [Description("其他")]
         Others = 99
    }
}
