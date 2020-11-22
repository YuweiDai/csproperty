using CSCZJ.Core;
using CSCZJ.Core.Domain.AccountUsers;
using CSCZJ.Core.Domain.Common;
using CSCZJ.Core.Domain.Media;
using CSCZJ.Core.Domain.Properties;
using CSCZJ.Core.Domain.Security;
using CSCZJ.Services.AccountUsers;
using CSCZJ.Services.Authentication;
using CSCZJ.Services.Common;
using CSCZJ.Services.Configuration;
using CSCZJ.Services.Media;
using CSCZJ.Services.Messages;
using CSCZJ.Services.Property;
using CSCZJ.Services.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;
using CSCZJ.Core.Domain.Panoramas;
using CSCZJ.API.Models.Properties;

namespace CSCZJ.API.Controllers
{
    [RoutePrefix("Demo")]
    public class DemoController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountUserService _accountUserService;
        private readonly IAccountUserRegistrationService _accountUserRegistrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IGovernmentService _governmentService;
        private readonly IPropertyService _propertyService;
        private readonly IPropertyAllotService _propertyAllotService;
        private readonly IPropertyLendService _propertyLendService;
        private readonly IPropertyNewCreateService _propertyNewCreateService;
        private readonly IPropertyEditService _propertyEditService;
        private readonly IPropertyOffService _propertyOffService;
        private readonly IPropertyRentService _propertyRentService;
        private readonly ICopyPropertyService _copyPropertyService;
        private readonly IEncryptionService _encryptionService;
        private readonly IPictureService _pictureService;

        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly AccountUserSettings _accountUserSettings;
        private readonly CommonSettings _commonSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly ISettingService _settingService;
        private readonly ISceneService _sceneService;
        private readonly ILocationService _locationService;

        public DemoController(IAuthenticationService authenticationService, IAccountUserService customerService,
            IAccountUserRegistrationService customerRegistrationService,
        IGenericAttributeService genericAttributeService,
       IWorkflowMessageService workflowMessageService, IGovernmentService governmentService, IPropertyService propertyService,
       IPropertyAllotService propertyAllotService, IPropertyLendService propertyLendService, IPropertyNewCreateService propertyNewCreateService,
       IPropertyOffService propertyOffService, IPropertyRentService propertyRentService, IEncryptionService encryptionService, IPictureService pictureService,
        IPropertyEditService propertyEditService, ICopyPropertyService copyPropertyService,
        IWebHelper webHelper,
            IWorkContext workContext,
        AccountUserSettings customerSettings, CommonSettings commonSettings, SecuritySettings securitySettings, ISettingService settingService, ISceneService sceneService, ILocationService locationService
            )
        {
            _authenticationService = authenticationService;
            _accountUserService = customerService;
            _accountUserRegistrationService = customerRegistrationService;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;
            _governmentService = governmentService;
            _propertyService = propertyService;
            _propertyAllotService = propertyAllotService;
            _propertyLendService = propertyLendService;
            _propertyNewCreateService = propertyNewCreateService;
            _propertyEditService = propertyEditService;
            _propertyOffService = propertyOffService;
            _propertyRentService = propertyRentService;
            _copyPropertyService = copyPropertyService;
            _encryptionService = encryptionService;
            _pictureService = pictureService;

            _webHelper = webHelper;
            _workContext = workContext;
            _accountUserSettings = customerSettings;

            _commonSettings = commonSettings;
            _securitySettings = securitySettings;
            _settingService = settingService;
            _sceneService = sceneService;
            _locationService = locationService;
        }

        [HttpGet]
        [Route("Demo")]
        public IHttpActionResult Demo()
        {
            return Ok("123");
        }

        [HttpGet]
        [Route("settings")]
        public IHttpActionResult GetAll()
        {
            _accountUserSettings.DefaultPasswordFormat = PasswordFormat.Encrypted;

            _settingService.SaveSetting<AccountUserSettings>(_accountUserSettings);

            _securitySettings.EncryptionKey = "qzczjwithqzghchy";
            _settingService.SaveSetting(_securitySettings);

            _commonSettings.TelAndMobliePartten = @"^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$|(^(13[0-9]|15[0|3|6|7|8|9]|18[0-9])\d{8}$)";
            _commonSettings.Time24Partten = @"^((1|0?)[0-9]|2[0-4]):([0-5][0-9])";

            _settingService.SaveSetting<CommonSettings>(_commonSettings);

            _settingService.SaveSetting(new MediaSettings
            {
                AvatarPictureSize = 120,
                ProductThumbPictureSize = 415,
                ProductDetailsPictureSize = 550,
                ProductThumbPictureSizeOnProductDetailsPage = 100,
                AssociatedProductPictureSize = 220,
                CategoryThumbPictureSize = 450,
                ManufacturerThumbPictureSize = 420,
                CartThumbPictureSize = 80,
                MiniCartThumbPictureSize = 70,
                AutoCompleteSearchThumbPictureSize = 20,
                MaximumImageSize = 1980,
                DefaultPictureZoomEnabled = false,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false
            });

            _settingService.SaveSetting(_accountUserSettings);

            return Ok("配置保存成功！");
        }
        [HttpGet]
        [Route("resetpwd")]
        public IHttpActionResult ResetPWD()
        {
            var users = _accountUserService.GetAllAccountUsers();
            foreach (var user in users)
            {
                user.PasswordFormat = PasswordFormat.Encrypted;
                user.Password = _encryptionService.EncryptText(user.Password);

                _accountUserService.UpdateAccountUser(user);
            }

            return Ok();
        }

        [HttpGet]
        [Route("HW")]
        public IHttpActionResult HW()
        {
            return Ok("123");

            //var user = _accountUserService.GetAccountUserById(1);

            
        }


        [HttpGet]
        [Route("SetLocation")]
        public IHttpActionResult Test()
        {

            var properties = _propertyService.GetAllProperties();

            foreach (var p in properties) {
                if (p.Extent != null) {

                    string extent = p.Extent.AsText();

                    extent = extent.Replace("MULTIPOLYGON (((", "POLYGON ((");
                    extent = extent.Replace(")))","))");

                    p.Extent = DbGeography.FromText(extent);
                    _propertyService.UpdateProperty(p);

                }

            }
            //var properties = _propertyService.GetAllProperties();
            //foreach (var property in properties)
            //{
            //    if (property.X == 0 || property.Y == 0) continue;
            //    //if (property.Extent != null)
            //    //    property.WKT = property.Extent.ToString();
            //    //else property.WKT = property.Location.ToString();

            //    property.Location = DbGeography.FromText("POINT(" + property.Y + " " + property.X + ")");

            //    _propertyService.UpdateProperty(property);
            //}

            // var governments = _governmentService.GetAllGovernmentUnits();
            //List<string> names = new List<string>() ;
            // foreach (var gov in governments)
            // {
            //     names.Add(gov.Name);
            // }

            // var filePath = @"D:\企业.xls";
            // string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
            // System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            // conn.Open();
            // string strExcel = "";
            // System.Data.OleDb.OleDbDataAdapter myCommand = null;
            // System.Data.DataSet ds = null;
            // strExcel = "select * from [五级$]";
            // myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            // ds = new System.Data.DataSet();
            // myCommand.Fill(ds, "table1");

            // var table = ds.Tables[0];

            // for (var i = 0; i < table.Rows.Count; i++) {

            //     var row = table.Rows[i];
            //     if (!names.Contains(row[0].ToString()))
            //     {
            //         GovernmentUnit goverment = new GovernmentUnit();
            //         var g = governments.Where(m => m.Name == row[1].ToString()).FirstOrDefault();
            //         goverment.Name = row[0].ToString();
            //         goverment.ParentGovernmentId = g.Id;

            //         _governmentService.InsertGovernmentUnit(goverment);
            //     }

            // }


            return Ok("导入完成");
        }

        [HttpGet]
        [Route("export")]
        public IHttpActionResult Export()
        {
            FileStream fs = new FileStream(@"G:\导出文件.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            var log = "";
            var properties = _propertyService.GetPropertiesByGId(155);
            foreach (var property in properties)
            {
                //var logCount = 0; var picCount=0;
                //var pictrues = property.Pictures;

                //foreach (var pictrue in pictrues) {
                //    if (pictrue.IsLogo == true) logCount++;
                //    else { picCount++; }
                //}
                var copy = _copyPropertyService.GetCopyPropertyByPropertyId(property.Id);

                if (copy == null)
                {
                    log = property.Id + "," + property.Name + "," + property.Address + "," + property.ConstructId + "," + property.EstateId + "," + property.LandId + "," + "0" + "," + "0" + "\r\n";
                }
                else
                {
                    if (copy.PrictureIds == null) copy.PrictureIds = "0";

                    log = property.Id + "," + property.Name + "," + property.Address + "," + property.ConstructId + "," + property.EstateId + "," + property.LandId + "," + copy.PrictureIds + "," + copy.LogoPicture_Id + "\r\n";
                }
                sw.Write(log);

            }
            sw.Flush();
            sw.Close();

            return Ok("导出成功");
        }

        [HttpGet]
        [Route("SetPropertyConut")]
        public IHttpActionResult SetPropertyConut()
        {

            var goverments = _governmentService.GetAllGovernmentUnits();
            var role = _accountUserService.GetAccountUserRoleBySystemName(SystemAccountUserRoleNames.ParentGovernmentorAuditor);
            foreach (var g in goverments)
            {
                g.PropertyConut = g.Properties.Count;
                if (g.ParentGovernmentId != 0)
                {
                    var parent = _governmentService.GetGovernmentUnitById(g.ParentGovernmentId);

                    g.ParentName = parent.Name;
                }

                _governmentService.UpdateGovernmentUnit(g);

                var users = g.Users;
                foreach (var user in users)
                {
                    if (g.ParentGovernmentId == 0)
                    {
                        if (user.AccountUserRoles.Where(ur => ur.Name == SystemAccountUserRoleNames.ParentGovernmentorAuditor).Count() == 0)
                        {

                            user.AccountUserRoles.Add(role);
                            _accountUserService.UpdateAccountUser(user);
                        }
                    }
                }

            }


            return Ok("赋值完成");
        }

        [HttpGet]
        [Route("SetRoles")]
        public IHttpActionResult SetRoles()
        {

          //  return BadRequest("角色配置 cloesd");
            #region 用户角色创建

            var crAdministrators = new AccountUserRole();
            var crRegistered = new AccountUserRole();

            var roleNames = new List<string> {
                SystemAccountUserRoleNames.Administrators,
                SystemAccountUserRoleNames.DataReviewer,
                SystemAccountUserRoleNames.GovAuditor,
                SystemAccountUserRoleNames.StateOwnerAuditor,
                SystemAccountUserRoleNames.ParentGovernmentorAuditor,
                SystemAccountUserRoleNames.Registered
            };

            foreach (var roleName in roleNames)
            {
                var role = _accountUserService.GetAccountUserRoleBySystemName(roleName);
                if (role == null)
                {
                    role = new AccountUserRole
                    {
                        Name = roleName,
                        Active = true,
                        IsSystemRole = true,
                        SystemName = roleName
                    };

                    _accountUserService.InsertAccountUserRole(role);


                }
                if (roleName == SystemAccountUserRoleNames.Administrators) crAdministrators = role;
                    if (roleName == SystemAccountUserRoleNames.Registered) crRegistered = role;
            }
            #endregion
            #region 测试组织机构

            var cz = new GovernmentUnit
            {
                Name = "县财政局",
                GovernmentType = GovernmentType.Government,
                Person = "联系人",
                Tel = "0570-5062456"
            };
            _governmentService.InsertGovernmentUnit(cz);

            #endregion

            #region 用户创建

            var user = new AccountUser()
            {
                UserName = "张三",
                AccountUserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOn = DateTime.Now,
                IsSystemAccount = false,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                LastActivityDate = DateTime.Now,
                Deleted = false,
                UpdatedOn = DateTime.Now,
                Government = cz
            };
            user.AccountUserRoles.Add(crAdministrators);
            user.AccountUserRoles.Add(crRegistered);
            _accountUserService.InsertAccountUser(user);

           user = new AccountUser()
            {
                UserName = "lisi",
                AccountUserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOn = DateTime.Now,
                IsSystemAccount = false,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                LastActivityDate = DateTime.Now,
                Deleted = false,
                UpdatedOn = DateTime.Now,
                Government = cz
            };
            _accountUserService.InsertAccountUser(user);

            #endregion

            return Ok("角色配置完成");
        }






        [HttpGet]
        [Route("ImportJson")]
        public IHttpActionResult ImportJson()
        {
            return Ok("已经更新");
            string jsonfile = "D://数据//范围面.json";//JSON文件路径  

            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    var properties = _propertyService.GetAllProperties();
                    foreach (var property in o["features"])
                    {
                        foreach(var p in properties){
                            if (property["attributes"]["_rootname"].ToString().Split('、').Contains(p.Number))
                            {
                              //  var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                var graphy = DbGeography.FromText(property["attributes"]["WKT"].ToString());
                                p.Extent = graphy;
                                _propertyService.UpdateProperty(p);
                            }
                        }

                    }

                }
            }
            return Ok();
        }

        [HttpGet]
        [Route("ImportPointJson")]
        public IHttpActionResult ImportPointJson()
        {
            return Ok("已经更新");
            string jsonfile = "D://数据//中心点.json";//JSON文件路径  

            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    var properties = _propertyService.GetAllProperties();
                    foreach (var property in o["features"])
                    {
                        foreach (var p in properties)
                        {
                            if (property["attributes"]["_rootname"].ToString().Split('、').Contains(p.Number))
                            {
                              //  var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                p.Location = graphy;
                                _propertyService.UpdateProperty(p);
                            }
                        }

                    }

                }
            }
            return Ok();
        }


        [HttpGet]
        [Route("updateData")]
        public IHttpActionResult UpdateData() {

            var properties = _propertyService.GetAllProperties();
            var IDList =new List<string>();

            foreach (var p in properties) {

                if (!IDList.Contains(p.PropertyID)) {
                    IDList.Add(p.PropertyID);
                }
            }

            foreach (var id in IDList) {

                var lambdaList = properties.Where(p => p.PropertyID == id).ToList();

                if (lambdaList.Count() > 1)
                {

                    for (var i = 1; i < lambdaList.Count(); i++) {
                        lambdaList[i].ParentPropertyId = lambdaList[0].Id;
                        _propertyService.UpdateProperty(lambdaList[i]);
                    }

                }
            
            }

          

          


                return Ok("数据跟新完成");
        
        }

     public delegate bool EqualsComparer<T>(T x, T y);

     public class Compare<T> : IEqualityComparer<T>
     {
         private EqualsComparer<T> _equalsComparer;

         public Compare(EqualsComparer<T> equalsComparer)
         {
             this._equalsComparer = equalsComparer;
         }

         public bool Equals(T x, T y)
         {
             if (null != this._equalsComparer)
                 return this._equalsComparer(x, y);
             else
                 return false;
         }

         public int GetHashCode(T obj)
         {
             return obj.ToString().GetHashCode();
         }
     }

        [HttpGet]
        [Route("Import")]
        public IHttpActionResult ImportCustomers()
        {

            return Ok("导入成功");
          //  return BadRequest("导入关闭");

            //var filePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/import.mdb");

            //var result = ReadXlsFile(filePath);


            var filePath = @"D://数据//资产表精简2.xls";
            var imageFilePath = @"D://数据//";


            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            System.Data.OleDb.OleDbDataAdapter myCommand = null;
            System.Data.DataSet ds = null;
            strExcel = "select * from [资产表$]";
            myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            ds = new System.Data.DataSet();
            myCommand.Fill(ds, "table1");

            var table = ds.Tables[0];
            // var goverments = _governmentService.GetAllGovernmentUnits();

            for (var i = 0; i < table.Rows.Count; i++)
            {

                var row = table.Rows[i];
                var p = new Property();
                p.Number = row[0].ToString();
                p.Name = row[2].ToString();
                #region 资产类别
                switch (row[3].ToString()) {
                    case "非转经（财政局）":
                        p.PropertyType = PropertyType.CZJ;
                        break;
                    case "非转经（拆迁办）":
                        p.PropertyType = PropertyType.CQB;
                        break;
                    case "非转经（畜牧兽医）":
                        p.PropertyType = PropertyType.XMSY;
                        break;
                    case "非转经（村规站）":
                        p.PropertyType = PropertyType.CGZ;
                        break;
                    case "非转经（道路运输管理所）":
                        p.PropertyType = PropertyType.DLYSGLJ;
                        break;
                    case "非转经（发改局）":
                        p.PropertyType = PropertyType.FGJ;
                        break;
                    case "非转经（房管处）":
                        p.PropertyType = PropertyType.FGC;
                        break;
                    case "非转经（福利企业办公室）":
                        p.PropertyType = PropertyType.FLQYBGS;
                        break;
                    case "非转经（工商局）":
                        p.PropertyType = PropertyType.GSJ;
                        break;
                    case "非转经（公安局）":
                        p.PropertyType = PropertyType.GAJ;
                        break;
                    case "非转经（公路管理局）":
                        p.PropertyType = PropertyType.GLGLJ;
                        break;
                    case "非转经（广播电视局）":
                        p.PropertyType = PropertyType.GBDSJ;
                        break;
                    case "非转经（国企）":
                        p.PropertyType = PropertyType.GQ;
                        break;
                    case "非转经（行政服务中心）":
                        p.PropertyType = PropertyType.XZFWZX;
                        break;
                    case "非转经（环保局）":
                        p.PropertyType = PropertyType.HBJ;
                        break;
                    case "非转经（环卫所）":
                        p.PropertyType = PropertyType.HWS;
                        break;
                    case "非转经（机关事务局）":
                        p.PropertyType = PropertyType.JGSWJ;
                        break;
                    case "非转经（计生指导站、妇保院）":
                        p.PropertyType = PropertyType.JSZDZ;
                        break;
                    case "非转经（教育局）":
                        p.PropertyType = PropertyType.JYJ;
                        break;
                    case "非转经（经信局）":
                        p.PropertyType = PropertyType.JXJ;
                        break;
                    case "非转经（科技局）":
                        p.PropertyType = PropertyType.KJJ;
                        break;
                    case "非转经（劳动局）":
                        p.PropertyType = PropertyType.LDJ;
                        break;
                    case "非转经（老干部局）":
                        p.PropertyType = PropertyType.LGBJ;
                        break;
                    case "非转经（能源监测站）":
                        p.PropertyType = PropertyType.NYJCJ;
                        break;
                    case "非转经（农机管理站）":
                        p.PropertyType = PropertyType.NJGLZ;
                        break;
                    case "非转经（农业局）":
                        p.PropertyType = PropertyType.NYJ;
                        break;
                    case "非转经（人民医院）":
                        p.PropertyType = PropertyType.RMYY;
                        break;
                    case "非转经（人武部）":
                        p.PropertyType = PropertyType.RWB;
                        break;
                    case "非转经（商务和粮食局局）":
                        p.PropertyType = PropertyType.SWLSJ;
                        break;
                    case "非转经（水利局）":
                        p.PropertyType = PropertyType.SLJ;
                        break;
                    case "非转经（水土保持科学试验站）":
                        p.PropertyType = PropertyType.STBCKXZ;
                        break;
                    case "非转经（司法局）":
                        p.PropertyType = PropertyType.SFJ;
                        break;
                    case "非转经（文广新局）":
                        p.PropertyType = PropertyType.WGXJ;
                        break;
                    case "非转经（县残联）":
                        p.PropertyType = PropertyType.XCL;
                        break;
                    case "非转经（县林业局）":
                        p.PropertyType = PropertyType.XLYJ;
                        break;
                    case "非转经（县农发办）":
                        p.PropertyType = PropertyType.XNFB;
                        break;
                    case "非转经（油茶研究所）":
                        p.PropertyType = PropertyType.YCYJS;
                        break;
                    case "非转经（招商局）":
                        p.PropertyType = PropertyType.ZSJ;
                        break;
                    case "非转经（质量监督站）":
                        p.PropertyType = PropertyType.ZLJDZ;
                        break;
                    case "非转经（中型水库管理局）":
                        p.PropertyType = PropertyType.ZXSKGLJ;
                        break;
                    case "非转经（住建局）":
                        p.PropertyType = PropertyType.ZJJ;
                        break;
                    case "非转经（自有资产）":
                        p.PropertyType = PropertyType.GYZC;
                        break;
                    case "非转经（国资注入）":
                        p.PropertyType = PropertyType.GZZR;
                        break;
                    case "其他":
                        p.PropertyType = PropertyType.Others;
                        break;
                }
                #endregion
                #region 资产区域
                switch (row[4].ToString()){
                    case "天马街道":
                        p.Region = Region.TMJD;
                        break;
                    case "紫港街道":
                        p.Region = Region.ZGJD;
                        break;
                    case "白石镇":
                        p.Region = Region.BSZ;
                        break;
                    case "芳村镇":
                        p.Region = Region.FCZ;
                        break;
                    case "招贤镇":
                        p.Region = Region.ZXZ;
                        break;
                    case "球川镇":
                        p.Region = Region.QCZ;
                        break;
                    case "东案乡":
                        p.Region = Region.DAX;
                        break;
                    case "何家乡":
                        p.Region = Region.HJX;
                        break;
                    case "青石镇":
                        p.Region = Region.QSZ;
                        break;
                    case "金川街道":
                        p.Region = Region.JCJD;
                        break;
                    case "同弓乡":
                        p.Region = Region.TGX;
                        break;
                    case "其他":
                        p.Region = Region.Others;
                        break;
                }
                #endregion
                #region 获取方式
                switch(row[5].ToString()){
                    case "自建":
                        p.GetMode = GetMode.SelfBuilt;
                        break;
                    case "购入":
                        p.GetMode = GetMode.Purchase;
                        break;
                    case "调配":
                        p.GetMode = GetMode.Deploy;
                        break;
                    case "划拨":
                        p.GetMode = GetMode.HB;
                        break;
                }
                #endregion
                p.Address = row[6].ToString();
                p.PropertyID = row[7].ToString();
                #region 是否入账
                switch (row[8].ToString())
                {
                    case "是":
                        p.IsAdmission = true;
                        break;
                    case "否":
                        p.IsAdmission = false;
                        break;
                }
                #endregion
                p.GetedDate = (row[9] == null) ? Convert.ToDateTime(row[9]) : DateTime.Now;
                p.UsedPeople = row[10].ToString();
                if (p.Government == null) {
                  //  var gs = _governmentService.GetAllGeoGovernmentUnits();
                    var g = _governmentService.GetGovernmentUnitByName("常山县城市投资运营有限责任公司");
                    p.Government = g;
                }
                p.LandArea = (row[12].ToString() == "") ? 0 : Convert.ToDouble(row[12].ToString()); 

                p.LandArea = (row[13].ToString() == "") ? 0 : Convert.ToDouble(row[13].ToString()); 
                #region 使用现状
                switch (row[14].ToString())
                {
                    case "拆除":
                        p.CurrentType = CurrentType.CC;
                        break;
                    case "出借":
                        p.CurrentType = CurrentType.CJ;
                        break;
                    case "出租":
                        p.CurrentType = CurrentType.CZ;
                        break;
                    case "调配使用":
                        p.CurrentType = CurrentType.DPSY;
                        break;
                    case "自用":
                        p.CurrentType = CurrentType.ZY;
                        break;
                    case "闲置":
                        p.CurrentType = CurrentType.XZ;
                        break;
                }
                #endregion
                #region 用途
                switch (row[15].ToString())
                {
                    case "办公":
                        p.UseType = UseType.BG;
                        break;
                    case "业务":
                        p.UseType = UseType.YW;
                        break;
                    case "住宅":
                        p.UseType = UseType.ZZ;
                        break;
                    case "其他":
                        p.UseType = UseType.Others;
                        break;
                    case "工业厂房":
                        p.UseType = UseType.GYCF;
                        break;
                    case "仓储":
                        p.UseType = UseType.CC;
                        break;
                    case "商业":
                        p.UseType = UseType.SY;
                        break;
                }
                #endregion
                p.IsMortgage = (row[16].ToString() == "是") ? true : false;
                p.Description = row[19].ToString();
                #region 资产封面导入
                if (row[17].ToString() != "")
                {
                    var filepath = "";
                    if (Directory.Exists(imageFilePath + row[17].ToString()))
                    {
                        filepath = imageFilePath + row[17].ToString();
                    }
                    else {
                        var fileArr = (imageFilePath + row[17].ToString()).Split(new string[] { "文件夹" }, StringSplitOptions.None);
                        filepath = fileArr[0] + "文件夹 " + fileArr[1];
                    }
                DirectoryInfo dir = new DirectoryInfo(filepath);
                FileInfo[] fileInfo = dir.GetFiles();
                var index = 0;
              
                    foreach (FileInfo item in fileInfo)
                    {

                        if (index == 0 && item != null)
                        {
                            var fileStream = new FileStream(item.FullName, FileMode.Open);
                            var fileBinary = new byte[fileStream.Length];
                            fileStream.Read(fileBinary, 0, fileBinary.Length);

                            var picture = _pictureService.InsertPicture(fileBinary, "image/jpeg", "", "", dir + "\\" + item.FullName);
                            var url = _pictureService.GetPictureUrl(picture);
                            var propertyPicture = new PropertyPicture
                            {
                                Picture = picture,
                                Property = p,
                                IsLogo = true
                            };
                            p.Pictures.Add(propertyPicture);
                            _propertyService.UpdateProperty(p);
                            fileStream.Flush();
                            fileStream.Close();
                            index++;
                        }

                    }
                }
               
                #endregion

                //其余照片
                if (row[18].ToString() !="")
                {
                    var filepath = "";
                    if (Directory.Exists(imageFilePath + row[18].ToString()))
                    {
                        filepath = imageFilePath + row[18].ToString();
                    }
                    else
                    {
                        var fileArr = (imageFilePath + row[18].ToString()).Split(new string[] { "文件夹" }, StringSplitOptions.None);
                        filepath = fileArr[0] + "文件夹 " + fileArr[1];
                    }
                var pictures = new List<Picture>();
                DirectoryInfo dir1 = new DirectoryInfo(filepath);
                FileInfo[] fileInfo1 = dir1.GetFiles();
              
                    foreach (FileInfo item in fileInfo1)
                    {
                        if (item != null)
                        {

                            var fileStream = new FileStream(item.FullName, FileMode.Open);
                            var fileBinary = new byte[fileStream.Length];
                            fileStream.Read(fileBinary, 0, fileBinary.Length);

                            var picture = _pictureService.InsertPicture(fileBinary, "image/jpeg", "", "", dir1 + "\\" + item.FullName);
                            var url = _pictureService.GetPictureUrl(picture);

                            pictures.Add(picture);


                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }

                    foreach (var pi in pictures)
                    {
                        var propertyPicture = new PropertyPicture
                        {
                            Picture = pi,
                            Property = p,
                            IsLogo = false
                        };
                        p.Pictures.Add(propertyPicture);
                        _propertyService.UpdateProperty(p);
                    }

                }
               

                _propertyService.InsertProperty(p);

            }





            //单位导入
            //for (var i = 0; i < table.Rows.Count; i++)
            //{

            //    var row = table.Rows[i];
            //    var goverment = new GovernmentUnit();

            //    goverment.Name = row[0].ToString();
            //    goverment.Address = row[4].ToString();
            //    goverment.Person = row[1].ToString();
            //    goverment.Tel = row[2].ToString();
            //    switch (row[3].ToString()) {
            //        case "行政机关":
            //            goverment.GovernmentType = GovernmentType.Government;
            //            break;
            //        case "事业单位":
            //            goverment.GovernmentType = GovernmentType.Institution;
            //            break;
            //        case "企业":
            //            goverment.GovernmentType = GovernmentType.Company;
            //            break;
            //    }
            //    _governmentService.InsertGovernmentUnit(goverment);

            //}


            #region old code
            #region 用户角色创建
            //var crAdministrators = new AccountUserRole
            //{
            //    Name = SystemAccountUserRoleNames.Administrators,
            //    Active = true,
            //    IsSystemRole = true,
            //    SystemName = "管理员"
            //};

            //var crRegistered = new AccountUserRole
            //{
            //    Name = SystemAccountUserRoleNames.Registered,
            //    Active = true,
            //    IsSystemRole = true,
            //    SystemName = "注册单位",
            //};
            //var crGuests = new AccountUserRole
            //{
            //    Name = SystemAccountUserRoleNames.Guests,
            //    Active = true,
            //    IsSystemRole = true,
            //    SystemName = "访客",
            //};

            //_accountUserService.InsertAccountUserRole(crAdministrators);
            //_accountUserService.InsertAccountUserRole(crRegistered);
            //_accountUserService.InsertAccountUserRole(crGuests);
            #endregion

            #region 测试组织机构
            //var csj = _governmentService.GetGovernmentUnitById(37);

            ////var cz = new GovernmentUnit
            ////{
            ////    Name = "市财政局",
            ////    GovernmentType = GovernmentType.Government,
            ////    Person = "联系人",
            ////    Tel="0570-5062456"
            ////};

            ////var ghj = new GovernmentUnit
            ////{
            ////    Name = "市规划局",
            ////    GovernmentType = GovernmentType.Government,
            ////    Person = "规划局",
            ////    Tel="0570-3021456"
            ////};

            ////var kcgh = new GovernmentUnit
            ////{
            ////    Name = "市规划局柯城分局",
            ////    GovernmentType = GovernmentType.Government,
            ////    Person = "市规划局柯城分局",
            ////    Tel = "0570-3021456"
            ////};

            ////var qjfj = new GovernmentUnit
            ////{
            ////    Name = "市规划局衢江分局",
            ////    GovernmentType = GovernmentType.Government,
            ////    Person = "市规划局衢江分局",
            ////    Tel = "0570-3021456"
            ////};

            ////var jjq = new GovernmentUnit
            ////{
            ////    Name = "市规划局绿色产业集聚区分局",
            ////    GovernmentType = GovernmentType.Government,
            ////    Person = "市规划局绿色产业集聚区分局",
            ////    Tel = "0570-3021456"
            ////};

            ////var xq = new GovernmentUnit
            ////{
            ////    Name = "市规划局西区分局",
            ////    GovernmentType = GovernmentType.Government,
            ////    Person = "市规划局西区分局",
            ////    Tel = "0570-3021456"
            ////};

            ////context.Set<GovernmentUnit>().AddOrUpdate(cz, ghj, kcgh, qjfj, jjq, xq);
            #endregion

            #region 用户创建
            //var user = new AccountUser()
            //{               
            //    UserName = "财政局",
            //    AccountUserGuid = Guid.NewGuid(),
            //    Active = true,
            //    CreatedOn = DateTime.Now,
            //    IsSystemAccount = false,
            //    Password = "123456",
            //    PasswordFormat = PasswordFormat.Clear,
            //    LastActivityDate = DateTime.Now,
            //    Deleted = false,
            //    UpdatedOn = DateTime.Now,
            //    Government=csj
            //};
            //user.AccountUserRoles.Add(crAdministrators);
            //user.AccountUserRoles.Add(crRegistered);
            //_accountUserService.InsertAccountUser(user);

            #endregion
            #endregion

            return Ok("导入结束\n");
        }



        [HttpPost]
        [Route("VtourString")]
        public IHttpActionResult InsertVtourString(VtourStringModel vtourModel) {
            var p = _propertyService.GetPropertyById(Convert.ToInt32(vtourModel.id));
            if (p != null) {
                p.vtourString = string.Join(";", vtourModel.vtourString);

                p.Ath = vtourModel.Ath;
                p.Atv = vtourModel.Atv;
                p.Xml = vtourModel.Xml;
                p.Scene = vtourModel.Scene;
                _propertyService.UpdateProperty(p);
            }
            return Ok();
        }


        [HttpGet]
        [Route("ImportRent")]
        public IHttpActionResult ImportRent() {

            var filePath = @"D:\数据\资产表精简1.xls";
            var imageFilePath = @"D:\数据\";


            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            System.Data.OleDb.OleDbDataAdapter myCommand = null;
            System.Data.DataSet ds = null;
            strExcel = "select * from [Sheet3$]";
            myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            ds = new System.Data.DataSet();
            myCommand.Fill(ds, "table1");

            var table = ds.Tables[0];

            var properties = _propertyService.GetAllProperties();
            for (var i = 0; i < table.Rows.Count; i++) {
                 var row = table.Rows[i];
                foreach (var p in properties) {

                    if (row[1].ToString() == p.Name) {

                        var rent = new PropertyRent();
                        rent.Name=row[2].ToString();
                        rent.RentTime= (row[3] == null) ? Convert.ToDateTime(row[3]) : DateTime.Now;
                        rent.BackTime= (row[4] == null) ? Convert.ToDateTime(row[4]) : DateTime.Now;
                        rent.ProcessDate= (row[5] == null) ? Convert.ToDateTime(row[5]) : DateTime.Now;
                        rent.Remark =row[6].ToString();
                        rent.RentArea = (row[7].ToString() == "") ? 0 : Convert.ToDouble(row[7].ToString()); 
                        rent.Property=p;
                        //导入出租图片
                        if (row[9].ToString() != "")
                        {
                            var filepath = "";
                            if (Directory.Exists(imageFilePath + row[9].ToString()))
                            {
                                filepath = imageFilePath + row[9].ToString();
                            }
                            else
                            {
                                var fileArr = (imageFilePath + row[9].ToString()).Split(new string[] { "文件夹" }, StringSplitOptions.None);
                                filepath = fileArr[0] + "文件夹 " + fileArr[1];
                            }
                            var pictures = new List<Picture>();
                            DirectoryInfo dir1 = new DirectoryInfo(filepath);
                            FileInfo[] fileInfo1 = dir1.GetFiles();

                            foreach (FileInfo item in fileInfo1)
                            {
                                if (item != null)
                                {

                                    var fileStream = new FileStream(item.FullName, FileMode.Open);
                                    var fileBinary = new byte[fileStream.Length];
                                    fileStream.Read(fileBinary, 0, fileBinary.Length);

                                    var picture = _pictureService.InsertPicture(fileBinary, "image/jpeg", "", "", dir1 + "\\" + item.FullName);
                                    var url = _pictureService.GetPictureUrl(picture);

                                    pictures.Add(picture);


                                    fileStream.Flush();
                                    fileStream.Close();
                                }
                            }

                            foreach (var pi in pictures)
                            {
                                var propertyPicture = new PropertyPicture
                                {
                                    Picture = pi,
                                    Property = p,
                                    IsLogo = false
                                };
                                p.Pictures.Add(propertyPicture);
                                _propertyService.UpdateProperty(p);
                            }

                        }
                       _propertyRentService.InsertPropertyRent(rent);
                    }
                }
            }


            return Ok("新增出租完成");
        }


        [HttpGet]
        [Route("UpdateRent")]
        public IHttpActionResult UpdateRent() {
            var filePath = @"D:\数据\资产表精简1.xls";
            

            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            System.Data.OleDb.OleDbDataAdapter myCommand = null;
            System.Data.DataSet ds = null;
            strExcel = "select * from [Sheet3$]";
            myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            ds = new System.Data.DataSet();
            myCommand.Fill(ds, "table1");

            var table = ds.Tables[0];

            var rents = _propertyRentService.GetAllRents();
            for (var i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];
                foreach(var rent in rents){
                    if (row[2].ToString() == rent.Name && row[2].ToString() != "")
                    { 
                        var rentTime = row[3].ToString().Split('.');
                        rent.RentTime = new DateTime(Convert.ToInt32( rentTime[0]),Convert.ToInt32( rentTime[1]),Convert.ToInt32( rentTime[2]));
                        var backTime = row[4].ToString().Split('.');
                        rent.BackTime = new DateTime(Convert.ToInt32(backTime[0]), Convert.ToInt32(backTime[1]), Convert.ToInt32(backTime[2]));
                        var processDate = row[5].ToString().Split('.');
                        rent.ProcessDate = new DateTime(Convert.ToInt32(processDate[0]), Convert.ToInt32(processDate[1]), Convert.ToInt32(processDate[2]));
                        rent.PriceString = "";
                        _propertyRentService.UpdatePropertyRent(rent);
                    }
                }
               
            
            
            }

            return Ok("更新完成");
        }

        [HttpGet]
        [Route("temp")]
        public IHttpActionResult Temp()
        {


            // return Ok("导入成功");

            var filePath = @"D:\aaa.xls";
            //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
            string strConn = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", filePath);
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            System.Data.OleDb.OleDbDataAdapter myCommand = null;
            System.Data.DataSet ds = null;
            strExcel = "select * from [Sheet1$]";
            myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            ds = new System.Data.DataSet();
            myCommand.Fill(ds, "table1");

            var table = ds.Tables[0];
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];

                var location = _locationService.GetLocationByName(row[0].ToString());

                if (location == null) continue;

                var scene = new PanoramaScene();
                scene.Views = 0;
                scene.ProductionDate = Convert.ToDateTime(row[1]);
                scene.LastViewDate = DateTime.Now;
                scene.PanoramaLocation = location;
                _sceneService.InsertPanoramaScene(scene);

                location.DefaultPanoramaSceneId = scene.Id;
                _locationService.UpdatePanoramaLocation(location);

            }
            return Ok("更新完成");

        }

        [HttpGet]
        [Route("temp1")]
        public IHttpActionResult AddLocations()
        {

            var filePath = @"D:\aa.xls";
            //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
            string strConn = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", filePath);
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            System.Data.OleDb.OleDbDataAdapter myCommand = null;
            System.Data.DataSet ds = null;
            strExcel = "select * from [Sheet4$]";
            myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            ds = new System.Data.DataSet();
            myCommand.Fill(ds, "table1");

            var table = ds.Tables[0];
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var location = new PanoramaLocation();
                location.Name = row[0].ToString();
                location.Lat = Convert.ToDouble(row[1]);
                location.Lng = Convert.ToDouble(row[2]);

                _locationService.InsertPanoramaLocation(location);
            }


            return Ok("更新完成");
        }

    }
}