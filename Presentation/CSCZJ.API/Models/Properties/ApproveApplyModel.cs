﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class ApproveApplyModel
    {
        public string ApproveType { get; set; }

        public bool Agree { get; set; }

        public string Suggestion { get; set; }
    }
}