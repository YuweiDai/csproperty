using System.ComponentModel;

namespace CSCZJ.Core.Domain.Properties
{
    /// <summary>
    /// 资产类别
    /// </summary>
    public enum PropertyType
    {
        [Description("安置房")]
        House = 0, //
        [Description("纯土地")]
        Land = 1, //
        [Description("农民迁建宅基地")]
        ZJD = 2, //
        [Description("非转经（财政局）")]
        CZJ = 3, //
        [Description("非转经（拆迁办）")]
        CQB = 4, //
        [Description("非转经（畜牧兽医）")]
        XMSY = 5, //
        [Description("非转经（村规站）")]
        CGZ = 6, //
        [Description("非转经（道路运输管理所）")]
        DLYSGLJ = 7, //
        [Description("非转经（发改局）")]
        FGJ = 8, //
        [Description("非转经（房管处）")]
        FGC = 9, //
        [Description("非转经（福利企业办公室）")]
        FLQYBGS = 10, //
        [Description("非转经（工商局）")]
        GSJ = 11, //
        [Description("非转经（公安局）")]
        GAJ = 12, //
        [Description("非转经（公路管理局）")]
        GLGLJ = 13, //
        [Description("非转经（广播电视局）")]
        GBDSJ = 14, //
        [Description("非转经（国企）")]
        GQ = 15, //
        [Description("非转经（行政服务中心）")]
        XZFWZX = 16, //
        [Description("非转经（环保局）")]
        HBJ = 17, //
        [Description("非转经（环卫所）")]
        HWS = 18, //
        [Description("非转经（机关事务局）")]
        JGSWJ = 19, //
        [Description("非转经（计生指导站、妇保院）")]
        JSZDZ = 20, //
        [Description("非转经（教育局）")]
        JYJ = 21, //
        [Description("非转经（经信局）")]
        JXJ = 22, //
        [Description("非转经（科技局）")]
        KJJ = 23, //
        [Description("非转经（劳动局）")]
        LDJ = 24, //
        [Description("非转经（老干部局）")]
        LGBJ = 25, //
        [Description("非转经（能源监测站）")]
        NYJCJ = 26, //
        [Description("非转经（农机管理站）")]
        NJGLZ = 27, //
        [Description("非转经（农业局）")]
        NYJ = 28, //
        [Description("非转经（人民医院）")]
        RMYY = 29, //
        [Description("非转经（人武部）")]
        RWB = 30, //
        [Description("非转经（商务和粮食局局）")]
        SWLSJ = 31, //
        [Description("非转经（水利局）")]
        SLJ = 32, //
        [Description("非转经（水土保持科学试验站）")]
        STBCKXZ = 33, //
        [Description("非转经（司法局）")]
        SFJ = 34, //
        [Description("非转经（文广新局）")]
        WGXJ = 35, //
        [Description("非转经（县残联）")]
        XCL = 36, //
        [Description("非转经（县林业局）")]
        XLYJ = 37, //
        [Description("非转经（县农发办）")]
        XNFB = 38, //
        [Description("非转经（油茶研究所）")]
        YCYJS = 39, //
        [Description("非转经（招商局）")]
        ZSJ = 40, //
        [Description("非转经（质量监督站）")]
        ZLJDZ = 41, //
        [Description("非转经（中型水库管理局）")]
        ZXSKGLJ = 42, //
        [Description("非转经（住建局）")]
        ZJJ = 43, //
        [Description("自有资产")]
        GYZC = 44, //
        [Description("国资注入")]
        GZZR = 45, //
        [Description("其他")]
        Others = 99 
    }
}
