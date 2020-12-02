using CSCZJ.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Properties
{
    public class PropertyPatrolModel : BaseQMEntityModel
    {
        public string PatrolDate { get; set; }

        public string People { get; set; }
     
        public string Content { get; set; }
     
        public string Tel { get; set; }

        public string Title { get; set; }
        public int Property_Id { get; set; }

        public virtual ICollection<PropertyPatrolPictureModel> PatrolPictures { get; set; }

    }

    public class PropertyPatrolCreateModel : BaseQMEntityModel
    {

        public string Content { get; set; }

        public string Title { get; set; }

        public int Property_Id { get; set; }

        public List<string> PatrolPictures { get; set; }
    }
}