using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Properties
{
    public enum PropertyKind
    {
        [Description("房产")]
        House = 0,   //
        [Description("土地")]
        Land = 1,     //
        [Description("房产对应土地")]
        LandUnderHouse = 2, //
        [Description("其他")]
        Others = 3, //
    }
}
