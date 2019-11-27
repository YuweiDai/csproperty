using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Properties
{
   public  enum CurrentType
    {
        [Description("自用")]
        ZY = 0,
        [Description("拆除")]
        CC = 1,
        [Description("出租")]
        CZ = 2,
        [Description("出借")]
        CJ = 3,
        [Description("调配使用")]
        DPSY = 4,
        [Description("闲置")]
        XZ = 5

    }
}
