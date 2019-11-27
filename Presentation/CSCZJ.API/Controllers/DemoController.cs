using Microsoft.SqlServer.Types;
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

        public DemoController()
        {

        }

        public DemoController(IAuthenticationService authenticationService, IAccountUserService customerService,
            IAccountUserRegistrationService customerRegistrationService,
        IGenericAttributeService genericAttributeService,
       IWorkflowMessageService workflowMessageService, IGovernmentService governmentService, IPropertyService propertyService,
       IPropertyAllotService propertyAllotService, IPropertyLendService propertyLendService, IPropertyNewCreateService propertyNewCreateService,
       IPropertyOffService propertyOffService, IPropertyRentService propertyRentService, IEncryptionService encryptionService, IPictureService pictureService,
        IPropertyEditService propertyEditService, ICopyPropertyService copyPropertyService,
        IWebHelper webHelper,
            IWorkContext workContext,
        AccountUserSettings customerSettings, CommonSettings commonSettings, SecuritySettings securitySettings, ISettingService settingService
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

            string jsonfile = "D://地块数据1.json";//JSON文件路径  

            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    var properties = _propertyService.GetAllProperties();
                    foreach (var property in o["features"])
                    {
                        switch (property["attributes"]["ID"].ToString())
                        {
                            case "1":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 139)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "2":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 140)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "3":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 125)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "4":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 126)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "5":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 127)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "6":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 128)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "7":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 129)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "8":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 130)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "9":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 131)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "10":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 132)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "11":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 133)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "12":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 134)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "13":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 135)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "14":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 136)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "15":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 137)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "16":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 138)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;
                            case "17":
                                foreach (var p in properties)
                                {
                                    if (p.Id == 124)
                                    {
                                        var wkt = property["geometry"]["rings"].ToString().Replace("\r", "").Replace("\n", "").Replace("[", "(").Replace("]", ")");
                                        var graphy = DbGeography.FromText(property["attributes"]["wkt"].ToString());
                                        p.Extent = graphy;
                                        _propertyService.UpdateProperty(p);

                                    }
                                }
                                break;  
                        }



                    }

                }
            }
            return Ok();
        }





        [HttpGet]
        [Route("Import")]
        public IHttpActionResult ImportCustomers()
        {
            return BadRequest("导入关闭");

            //var filePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/import.mdb");

            //var result = ReadXlsFile(filePath);


            var filePath = @"D:\资产表1.xls";


            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
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
            // var goverments = _governmentService.GetAllGovernmentUnits();

            for (var i = 0; i < table.Rows.Count; i++)
            {

                var row = table.Rows[i];
                var p = new Property();
                p.Name = row[0].ToString();

                switch (row[1].ToString()) {
                    case  "非转经（机关局）":
                        p.PropertyType = PropertyType.House;
                        break;
                    case "非转经（房管处）":
                        p.PropertyType = PropertyType.Land;
                        break;
                    case "自有":
                        p.PropertyType = PropertyType.LandUnderHouse;
                        break;
                    case "国资注入":
                        p.PropertyType = PropertyType.Others;
                        break;

                }
                switch (row[3].ToString())
                {
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
                p.Address = row[4].ToString();
                p.Floor = Convert.ToInt32(row[5]);
                p.PropertyID = row[6].ToString();

                switch (row[7].ToString())
                {
                    case "是":
                         p.IsAdmission =true;
                        break;
                    case "否":
                        p.IsAdmission = false;
                        break;
                }

               // p.IsAdmission = Convert.ToBoolean(row[7]);
                p.GetedDate = (row[8] == null) ? Convert.ToDateTime(row[8]) : DateTime.Now;
                p.UsedPeople = row[9].ToString();
                p.Description = row[10].ToString();
                p.FourToStation = row[11].ToString();
                p.Published = true;
                p.Government = _governmentService.GetGovernmentUnitByName("常山县城市投资运营有限责任公司");
                //p.EstateId= row[13].ToString();
                //p.EstateTime = (row[14] == null) ? Convert.ToDateTime(row[14]) : DateTime.Now;
                //p.ConstructId= row[15].ToString();
                //p.ConstructTime = (row[16] == null) ? Convert.ToDateTime(row[16]) : DateTime.Now;
                p.ConstructArea = Convert.ToDouble(row[17]);
                //p.LandId= row[18].ToString();
                p.LandArea = Convert.ToDouble(row[19]);
                // p.LandTime = (row[20] == null) ? Convert.ToDateTime(row[20]) : DateTime.Now;
                switch (row[21].ToString())
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
                switch (row[22].ToString())
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

                }
                p.IsMortgage = (row[23].ToString() == "是") ? true : false;
                

                //var points = row[24].ToString().Split(',');
                //var point1 = points[0].Substring(7);
                //var point2 = points[1].Substring(6);
                //var point3 = point2.Substring(0, point2.Length - 1);
                //var point = "POINT(" + point3 + " " + point1 + ")";
                //p.Location = DbGeography.FromText(point);

                //if (row[25].ToString() != "")
                //{

                //    var arr1 = row[22].ToString().Substring(1);
                //    var arr2 = row[22].ToString().Substring(1, arr1.Length - 1);
                //    var arr = arr2.Split(',');

                //    var wkt = "";

                //    for (var j = 0; j <= arr.Length - 1; j++)
                //    {
                //        if (j % 2 == 0)
                //        {
                //            wkt += arr[j + 1].Substring(6, arr[j + 1].Length - 7) + " " + arr[j].Substring(7) + ",";
                //        }


                //    }
                //    wkt = wkt + arr[1].Substring(6, arr[1].Length - 7) + " " + arr[0].Substring(7);
                //    wkt = "MULTIPOLYGON(((" + wkt + ")))";
                //    p.Extent = DbGeography.FromText(wkt);
                //}


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

    }
}