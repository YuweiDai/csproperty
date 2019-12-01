using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Core.Domain.Panoramas
{
    /// <summary>
    /// 街景类型
    /// </summary>
    public enum PanoramaType
    {
        [Description("空中")]
        Sky = 0,
        [Description("地面")]
        Street = 1,
    }
}
