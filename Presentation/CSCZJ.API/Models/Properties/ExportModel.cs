using CSCZJ.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class ExportModel: BaseQMEntityModel
    {

         public string  propertyids{ get; set; }
         public string govermentids{ get; set; }
        public bool isName { get; set; }
        public bool  isAddress { get; set; }
        public bool  isGoverment { get; set; }
        public bool  isPropertyType { get; set; }
        public bool isRegion { get; set; }
        public bool isGetMode { get; set; }
        public bool isPropertyID { get; set; }
        public bool isUsedPeople { get; set; }
        public bool isFourToStation { get; set; }
        public bool isEstateId { get; set; }
        public bool isConstructArea { get; set; }
        public bool isConstructId { get; set; }
        public bool isLandArea { get; set; }
        public bool isCurrentType { get; set; }
        public bool isUsedType { get; set; }

    }
}