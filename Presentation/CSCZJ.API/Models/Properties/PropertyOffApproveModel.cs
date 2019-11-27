﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class PropertyOffApproveModel
    {
        public PropertyOffModel PropertyOff { get; set; }

        public PropertyBasicInfo Property { get; set; }

        /// <summary>
        /// 是否拥有审批权限
        /// </summary>
        public bool CanApprove { get; set; }

        /// <summary>
        /// 是否可以删除和修改
        /// </summary>
        public bool CanEditAndDelete { get; set; }

        public string LinkMan { get; set; }
        public string LinkTel { get; set; }
    }
}