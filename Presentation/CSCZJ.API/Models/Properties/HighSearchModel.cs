using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class HighSearchModel
    {

        public string PropertyType { get; set; }
       public bool House { get; set;}
        public bool Land { get; set; }

        public bool TMJD { get; set; }
        public bool ZGJD { get; set; }
        public bool BSZ { get; set; }
        public bool FCZ { get; set; }
        public bool ZXZ { get; set; }
        public bool QCZ { get; set; }
        public bool DAX { get; set; }
        public bool HJX { get; set; }
        public bool QSZ { get; set; }
        public bool JCJD { get; set; }
        public bool TGX { get; set; }



        public bool ZY { get; set; }
        public bool CC { get; set; }
        public bool XZ { get; set; }
        public bool SYDP { get; set; }


        public bool All { get; set; }
        public bool isHouse { get; set; }
        public bool None { get; set; }
        public bool isLand { get; set; }
        public bool One { get; set; }
        public bool Two { get; set; }
        public bool Three { get; set; }
        public bool Four { get; set; }
        public bool Five { get; set; }

    }



}