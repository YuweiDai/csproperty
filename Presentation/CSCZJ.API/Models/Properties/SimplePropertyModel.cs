using CSCZJ.Web.Framework.Mvc;
using System.Collections.Generic;

namespace CSCZJ.API.Models.Properties
{
    public class SimplePropertyModel: BaseQMEntityModel
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public bool Locked { get; set; }

     //   public string GovernmentName { get; set; }
    }

    public class VtourStringModel
    {

       public  string id { get; set; }

       public   List<string> vtourString { get; set; }

        public double Ath { get; set; }
    
        public double Atv { get; set; }
   
        public string Xml { get; set; }
     
        public string Scene { get; set; }
    }



}