using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Properties
{
    public enum GetMode
    {
        [Description("自建")]
        SelfBuilt = 0,
        [Description("购入")]
        Purchase = 1,
        [Description("调配")]
        Deploy = 2,
        [Description("划拨")]
        HB = 3,
    }
}
