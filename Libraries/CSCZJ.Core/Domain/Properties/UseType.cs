using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Properties
{
   public  enum UseType
    {
        [Description("办公")]
        BG = 0,
        [Description("住宅")]
        ZZ = 1,
        [Description("业务")]
        YW = 2,
        [Description("工业厂房")]
        GYCF = 3,
        [Description("仓储")]
        CC = 4,
        [Description("其他")]
        Others = 99
    }
}
