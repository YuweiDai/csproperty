using CSCZJ.Web.Framework.Mvc;

namespace CSCZJ.API.Models.Properties
{
    public class SameIdPropertyModel : BaseQMEntityModel
    {
        public string Name { get; set; }

        public bool IsMain { get; set; }
    }
}