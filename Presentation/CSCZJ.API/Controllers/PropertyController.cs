using Microsoft.Win32;
using CSCZJ.API.Models.Media;
using CSCZJ.API.Models.Properties;
using CSCZJ.Core;
using CSCZJ.Core.Domain.Media;
using CSCZJ.Core.Domain.Properties;
using CSCZJ.Services.AccountUsers;
using CSCZJ.Services.ExportImport;
using CSCZJ.Services.Logging;
using CSCZJ.Services.Media;
using CSCZJ.Services.Property;
using CSCZJ.Web.Api.Extensions;
using CSCZJ.Web.Framework.Controllers;
using CSCZJ.Web.Framework.Response;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Collections;

namespace CSCZJ.API.Controllers
{
  
    [RoutePrefix("Properties")]
    public class PropertyController : ApiController //BaseAdminApiController
    {
        private readonly IPropertyService _propertyService;
        private readonly IGovernmentService _governmentService;
        private readonly IAccountUserService _accountUserService;
        private readonly IAccountUserActivityService _accountUserActivityService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IPropertyNewCreateService _propertyNewCreateService;
        private readonly IPropertyLendService _propertyLendService;
        private readonly IPropertyRentService _propertyRentService;
        private readonly IPropertyAllotService _propertyAllotService;
        private readonly IPropertyOffService _propertyOffService;
        private readonly IPropertyEditService _propertyEditService;
        private readonly ICopyPropertyService _copyPropertyService;
        private readonly IPictureService _pictureService;
        private readonly IFileService _fileService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly IMonthTotalService _monthTotalService;
        private readonly ISubmitRecordService _submitRecordService;
        private readonly IPropertyPatrolService _propertyPatrolService;

        public PropertyController(
            IPropertyService propertyService,
            IAccountUserService accountUserService,
        IGovernmentService governmentService,
            IAccountUserActivityService accountUserActivityService,
             IWebHelper webHelper,
             IPropertyNewCreateService propertyNewCreateService,
             IPropertyLendService propertyLendService,
             IPropertyRentService propertyRentService,
            IPropertyPatrolService propertyPatrolService,
             IPropertyAllotService propertyAllotService, IPropertyOffService propertyOffService,
              IPropertyEditService propertyEditService, ICopyPropertyService copyPropertyService,
              IPictureService pictureService, IFileService fileService, IExportManager exportManager,
              IImportManager importManager,IMonthTotalService monthTotalService,
              ISubmitRecordService submitRecordService,
        IWorkContext workContext)
        {
            _propertyService = propertyService;
            _accountUserService = accountUserService;
            _accountUserActivityService = accountUserActivityService;
            _governmentService = governmentService;
            _propertyNewCreateService = propertyNewCreateService;
            _propertyLendService = propertyLendService;
            _propertyRentService = propertyRentService;
            _propertyAllotService = propertyAllotService;
            _propertyOffService = propertyOffService;
            _webHelper = webHelper;
            _workContext = workContext;
            _propertyEditService = propertyEditService;
            _copyPropertyService = copyPropertyService;
            _pictureService = pictureService;
            _fileService = fileService;
            _exportManager = exportManager;
            _importManager = importManager;
            _monthTotalService = monthTotalService;
            _submitRecordService = submitRecordService;
            _propertyPatrolService = propertyPatrolService;
           // _exportMonthTotalManager = exportMonthTotalManager;
        }

        #region utility     

        /// <summary>
        /// 模型检查
        /// </summary>
        /// <param name="propertyCreateModel"></param>
        /// <returns></returns>
        [NonAction]

        protected virtual string PropertyCreateModelValid(PropertyCreateModel propertyCreateModel)
        {

            var sum = propertyCreateModel.CurrentUse_Idle + propertyCreateModel.CurrentUse_Lend + propertyCreateModel.CurrentUse_Rent + propertyCreateModel.CurrentUse_Self;
            if (propertyCreateModel.PropertyTypeId == 0)
            {
                if (propertyCreateModel.ConstructArea <sum)
                {
                    return "建筑面积应大于自用、出租、出借、闲置面积之和";
                }
            }

            if (propertyCreateModel.PropertyTypeId == 1)
            {
                if (propertyCreateModel.LandArea <sum)
                {
                    return "土地面积应大于自用、出租、出借、闲置面积之和";
                }
            }

            if (string.IsNullOrEmpty(propertyCreateModel.Logo) && string.IsNullOrEmpty(propertyCreateModel.LogoUrl) ||
                propertyCreateModel.Logo == "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAUAAAAFACAYAAADNkKWqAAAJPElEQVR4Xu3UAREAAAgCMelf2iA/GzA8do4AAQJRgUVzi02AAIEzgJ6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToCAAfQDBAhkBQxgtnrBCRAwgH6AAIGsgAHMVi84AQIG0A8QIJAVMIDZ6gUnQMAA+gECBLICBjBbveAECBhAP0CAQFbAAGarF5wAAQPoBwgQyAoYwGz1ghMgYAD9AAECWQEDmK1ecAIEDKAfIEAgK2AAs9ULToDAA2UOAUG2GG9iAAAAAElFTkSuQmCC")
                return "请上传资产现场照片";

            return "";
        }

        /// <summary>
        /// 资产模型通用性处理
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyModel"></param>
        [NonAction]

        protected virtual void PrepareProperty(Property property, PropertyCreateModel propertyCreateModel)
        {
            #region 日期处理
            if (!string.IsNullOrEmpty(propertyCreateModel.GetedDate))
            {
                try
                {
                    property.GetedDate = Convert.ToDateTime(propertyCreateModel.GetedDate);
                }
                catch
                {
                }
            }
            else property.GetedDate = null;
            if (!string.IsNullOrEmpty(propertyCreateModel.EstateTime))
            {
                try
                {
                    property.EstateTime = Convert.ToDateTime(propertyCreateModel.EstateTime);
                }
                catch
                {
                }
            }
            else property.EstateTime = null;
            if (!string.IsNullOrEmpty(propertyCreateModel.ConstructTime))
            {
                try
                {
                    property.ConstructTime = Convert.ToDateTime(propertyCreateModel.ConstructTime);
                }
                catch
                {
                }
            }
            else property.ConstructTime = null;
            if (!string.IsNullOrEmpty(propertyCreateModel.LandTime))
            {
                try
                {
                    property.LandTime = Convert.ToDateTime(propertyCreateModel.LandTime);
                }
                catch
                {
                }
            }
            else property.LandTime = null;
            #endregion

            if(propertyCreateModel.RegisterEstate)
            {
                property.ConstructId = "";
                property.ConstructTime = null;
                property.LandId = "";
                property.LandTime = null;
            }
            else
            {
                property.EstateId = "";
                property.EstateTime = null;
            }

            //产权单位处理
            if (propertyCreateModel.GovernmentId == 0) property.Government = _workContext.CurrentAccountUser.Government;
            else
            {
                var g = _governmentService.GetGovernmentUnitById(propertyCreateModel.GovernmentId);
                if (g == null) throw new Exception("产权单位无效");
                property.Government = g;
            }

            if (!string.IsNullOrEmpty(propertyCreateModel.Location)) property.Location = DbGeography.FromText(propertyCreateModel.Location);
            else throw new Exception("空间位置不能为空");

            //本地电脑未装SQL 会出现的BUG
            try
            {
                if (!string.IsNullOrEmpty(propertyCreateModel.Extent)) property.Extent = DbGeography.FromText(propertyCreateModel.Extent);
            }
            catch
            {

            }
       
            property.Region = Region.TMZ;

            if (propertyCreateModel.PropertyTypeId == 2) propertyCreateModel.Floor = 0;


         //   property.HasConstructID = !string.IsNullOrEmpty(property.EstateId) || !string.IsNullOrEmpty(property.ConstructId);  //是否拥有房产证
        //    property.HasLandID = !string.IsNullOrEmpty(property.EstateId) || !string.IsNullOrEmpty(property.LandId);  //是否土地证
        }

        /// <summary>
        /// 高级搜索参数赋值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        [NonAction]
        protected virtual PropertyAdvanceConditionRequest PrepareAdvanceCondition(PropertyAdvanceConditionModel propertyAdvanceConditionModel)
        {
            var advanceCondition = new PropertyAdvanceConditionRequest();

            advanceCondition.SerachParentGovernement = propertyAdvanceConditionModel.Government.Manage;
            if (propertyAdvanceConditionModel.Government.SelectedId > 0)
                advanceCondition.GovernmentId = propertyAdvanceConditionModel.Government.SelectedId;

            if (propertyAdvanceConditionModel.Government.IsCompany) advanceCondition.GovernmentTypes.Add((int)GovernmentType.Company);
            if (propertyAdvanceConditionModel.Government.IsGovernment) advanceCondition.GovernmentTypes.Add((int)GovernmentType.Government);
            if (propertyAdvanceConditionModel.Government.IsInstitution) advanceCondition.GovernmentTypes.Add((int)GovernmentType.Institution);
            if (advanceCondition.Region.Count == 3) advanceCondition.GovernmentTypes = new List<int>();

            advanceCondition.PropertyType = new List<int>();
            if (propertyAdvanceConditionModel.PropertyType.Construct) advanceCondition.PropertyType.Add((int)PropertyType.House);
            if (propertyAdvanceConditionModel.PropertyType.Land) advanceCondition.PropertyType.Add((int)PropertyType.Land);
            if (propertyAdvanceConditionModel.PropertyType.ConstructOnLand) advanceCondition.PropertyType.Add((int)PropertyType.LandUnderHouse);
            if (advanceCondition.PropertyType.Count == 3) advanceCondition.PropertyType = new List<int>();

            advanceCondition.Region = new List<int>();
            //if (propertyAdvanceConditionModel.Region.Old) advanceCondition.Region.Add((int)Region.OldCity);
            //if (propertyAdvanceConditionModel.Region.West) advanceCondition.Region.Add((int)Region.West);
            //if (propertyAdvanceConditionModel.Region.Jjq) advanceCondition.Region.Add((int)Region.Clusters);
            //if (propertyAdvanceConditionModel.Region.Kc) advanceCondition.Region.Add((int)Region.KC);
            //if (propertyAdvanceConditionModel.Region.Qj) advanceCondition.Region.Add((int)Region.QJ);
            if (propertyAdvanceConditionModel.Region.Other) advanceCondition.Region.Add((int)Region.Others);
            if (advanceCondition.Region.Count == 6) advanceCondition.Region = new List<int>();

            advanceCondition.Certificate_Both = propertyAdvanceConditionModel.Certificate.Both;
            advanceCondition.Certificate_Construct = propertyAdvanceConditionModel.Certificate.Construct;
            advanceCondition.Certificate_Land = propertyAdvanceConditionModel.Certificate.Land;
            advanceCondition.Certificate_None = propertyAdvanceConditionModel.Certificate.None;

            advanceCondition.Current_Self = propertyAdvanceConditionModel.Current.Self;
            advanceCondition.Current_Idle = propertyAdvanceConditionModel.Current.Idle;
            advanceCondition.Current_Lend = propertyAdvanceConditionModel.Current.Lend;
            advanceCondition.Current_Rent = propertyAdvanceConditionModel.Current.Rent;

            advanceCondition.NextStep = new List<int>();
            switch (propertyAdvanceConditionModel.NextStep.MapType)
            {
                case 1:
                    if (propertyAdvanceConditionModel.NextStep.Ct) advanceCondition.NextStep.Add((int)NextStepType.InjectionCT);
                    if (propertyAdvanceConditionModel.NextStep.Jt) advanceCondition.NextStep.Add((int)NextStepType.InjectionJT);
                    if (propertyAdvanceConditionModel.NextStep.Jk) advanceCondition.NextStep.Add((int)NextStepType.InjectionJK);
                    if (advanceCondition.NextStep.Count == 0) advanceCondition.NextStep.Add(-1);
                    break;
                case 2:
                    advanceCondition.NextStep.Add((int)NextStepType.Auction);
                    break;
                case 3:
                    advanceCondition.NextStep.Add((int)NextStepType.Storeup);
                    break;
            }
            if (propertyAdvanceConditionModel.NextStep.Self) advanceCondition.NextStep.Add((int)NextStepType.Self);
            if (propertyAdvanceConditionModel.NextStep.StoreUp) advanceCondition.NextStep.Add((int)NextStepType.Storeup);

            if (propertyAdvanceConditionModel.NextStep.Greenland) advanceCondition.NextStep.Add((int)NextStepType.Greenland);
            if (propertyAdvanceConditionModel.NextStep.Auction) advanceCondition.NextStep.Add((int)NextStepType.Auction);
            if (propertyAdvanceConditionModel.NextStep.Adjust) advanceCondition.NextStep.Add((int)NextStepType.Adjust);
            if (propertyAdvanceConditionModel.NextStep.House) advanceCondition.NextStep.Add((int)NextStepType.House);
            if (advanceCondition.NextStep.Count == 8) advanceCondition.NextStep = new List<int>();

            advanceCondition.ConstructArea = new List<List<double>>();
            foreach (var range in propertyAdvanceConditionModel.ConstructArea.Ranges)
            {
                advanceCondition.ConstructArea.Add(new List<double>() { range.Min, range.Max });
            }

            advanceCondition.LandArea = new List<List<double>>();
            foreach (var range in propertyAdvanceConditionModel.LandArea.Ranges)
            {
                advanceCondition.LandArea.Add(new List<double>() { range.Min, range.Max });
            }

            advanceCondition.Price = new List<List<double>>();
            foreach (var range in propertyAdvanceConditionModel.Price.Ranges)
            {
                advanceCondition.Price.Add(new List<double>() { range.Min, range.Max });
            }

            advanceCondition.LifeTime = new List<double>();
            advanceCondition.LifeTime.Add(propertyAdvanceConditionModel.LifeTime.Min);
            advanceCondition.LifeTime.Add(propertyAdvanceConditionModel.LifeTime.Max);

            advanceCondition.GetedDate = new List<double>();
            //if (propertyAdvanceConditionModel.GetedDate.Min != 0 && propertyAdvanceConditionModel.GetedDate.Max != 0)
            //{
            //    advanceCondition.GetedDate.Add(propertyAdvanceConditionModel.GetedDate.Min);
            //    advanceCondition.GetedDate.Add(propertyAdvanceConditionModel.GetedDate.Max);
            //}

            if (!string.IsNullOrEmpty(propertyAdvanceConditionModel.Extent.Geo))
            {
                try
                {
                    advanceCondition.Extent = DbGeography.FromText(propertyAdvanceConditionModel.Extent.Geo);
                }
                catch
                {
                    throw new CSCZJException("传入的范围参数不是一个正确的WKT格式的数据");
                }
            }
            return advanceCondition;

        }

        /// <summary>
        /// 区间参数配置
        /// </summary>
        /// <param name="rangeStr"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual RangeList PrepareRanges(string rangeStr)
        {
            var rangeList = new RangeList() { Ranges = new List<Range>() };
            if (!string.IsNullOrEmpty(rangeStr))
            {
                var rangeArr = rangeStr.Split(';');
                foreach (var range in rangeArr)
                {
                    var minAndMax = range.Split('_');
                    if (minAndMax.Length == 2)
                    {
                        var r = new Range
                        {
                            Min = Convert.ToDouble(minAndMax[0]),
                            Max = Convert.ToDouble(minAndMax[1])
                        };
                        rangeList.Ranges.Add(r);
                    }
                }
            }
            return rangeList;
        }

        [NonAction]
        protected virtual Range PrepareRange(string rangeStr)
        {
            var minAndMax = rangeStr.Split('_');
            if (minAndMax.Length == 2)
            {
                var r = new Range
                {
                    Min = Convert.ToDouble(minAndMax[0]),
                    Max = Convert.ToDouble(minAndMax[1])
                };
                return r;
            }
            return new Range { Min = 0, Max = 0 };
        }

        /// <summary>
        /// 资产是否属于当前用户的部门
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual bool PropertyBelongCurrentUser(Property property, bool exceptGovernmentHasUsers = false)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var ownerGovernment = property.Government;
            if (ownerGovernment.Id == currentUser.Government.Id) return true;  //本单位
            else
            {
                var childrenGovernmentIds = _governmentService.GetGovernmentIdsByParentId(currentUser.Government.Id, exceptGovernmentHasUsers);
                return childrenGovernmentIds.Contains(ownerGovernment.Id);
            }
        }

        /// <summary>
        /// 处置申请是否可以审批
        /// </summary>
        /// <param name="state"></param>
        /// <param name="suggestionId"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual bool PropertyCanApprove(PropertyApproveState state, int suggestionId)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var g = _governmentService.GetGovernmentUnitById(suggestionId);
            if (g == null) return false;

            //部门审核阶段
            if (state == PropertyApproveState.DepartmentApprove)
            {
                //必须是主管单位才能审核
                var childrenGovernmentIds = _governmentService.GetGovernmentIdsByParentId(currentUser.Government.Id);
                if (!childrenGovernmentIds.Contains(currentUser.Government.Id)) childrenGovernmentIds.Add(currentUser.Government.Id);
                return childrenGovernmentIds.Contains(suggestionId) && currentUser.IsParentGovernmentorAuditor();
                //return currentUser.Government.ParentGovernmentId == 0 && currentUser.Government.Id == g.ParentGovernmentId;
            }
            else if (state == PropertyApproveState.AdminApprove)
            {
                if (g.GovernmentType == GovernmentType.Company) return currentUser.IsStateOwnerAuditor();
                else return currentUser.IsGovAuditor();
            }

            return false;
        }

        /// <summary>
        /// 是否可以编辑
        /// </summary>
        /// <param name="state"></param>
        /// <param name="suggestionId"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual bool PropertyApproveCanEditDeleteAndSubmit(PropertyApproveState state, int suggestionId)
        {
            var currentUser = _workContext.CurrentAccountUser;

            if (state != PropertyApproveState.Start) return false;

            //必须是申请的人才有权限
            return suggestionId == currentUser.Government.Id;
        }

        /// <summary>
        /// 资产是否可以查看
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual bool PropertyCanView(Property property)
        {
            var currentUser = _workContext.CurrentAccountUser;

            if (currentUser.IsGovAuditor())
                return property.Government.GovernmentType == GovernmentType.Government
                    || property.Government.GovernmentType == GovernmentType.Institution;
            else if (currentUser.IsStateOwnerAuditor())
                return property.Government.GovernmentType == GovernmentType.Company;
            else
                return PropertyBelongCurrentUser(property, false);
        }


        /// <summary>
        /// 资产是否可以编辑，针对未入库的
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual bool PropertyCanEditDelete(Property property)
        {
            var currentUser = _workContext.CurrentAccountUser;
            if (currentUser.IsAdmin() || currentUser.IsGovAuditor() || currentUser.IsStateOwnerAuditor() || currentUser.IsDataReviewer()) return false;  //超管等角色不能进行资产处置

            if (PropertyBelongCurrentUser(property, true))
            {
                var newCreate = _propertyNewCreateService.GetPropertyNewCreateByPropertyId(property.Id);
                if (newCreate == null)
                    return !property.Published && !property.Off;
                else
                    return newCreate.State == PropertyApproveState.Start && !property.Published && !property.Off;
            }

            return false;
        }

        /// <summary>
        /// 资产是否可以变更，针对已入库的
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual bool PropertyCanChange(Property property)
        {
            var currentUser = _workContext.CurrentAccountUser;
            if (currentUser.IsAdmin() || currentUser.IsGovAuditor() || currentUser.IsStateOwnerAuditor() || currentUser.IsDataReviewer()) return false;  //超管等角色不能进行资产处置

            if (PropertyBelongCurrentUser(property, true))
            {
                var edits = _propertyEditService.GetPropertyEditByPropertyId(property.Id);

                if (edits.Count > 0)
                {
                    var startEdits = edits.Where(e => e.State == PropertyApproveState.Start);
                    var finishEdits = edits.Where(e => e.State == PropertyApproveState.Finish);

                    if (edits.Count == finishEdits.Count() + startEdits.Count())
                    {
                        return (startEdits.Count() == 0 || startEdits.Count() == 1) && property.Published;
                    }
                }
                else
                {
                    return property.Published && !property.Locked;
                }
            }

            return false;
        }

        [NonAction]
        protected virtual CopyProperty CopyPropertyChange(CopyProperty copy, PropertyCreateModel propertyCreateModel, Property property, Picture logoPicture)
        {
            copy.GetedDate = Convert.ToDateTime(propertyCreateModel.GetedDate);
            copy.PropertyID = property.PropertyID;
            copy.Government_Id = property.Government.Id;

            var dbLocation = DbGeography.FromText(propertyCreateModel.Location);
            copy.Region = _propertyService.GetPropertyRegion(dbLocation);          

            //  copy.Region = propertyCreateModel.re
            copy.Property_Id = property.Id;

            copy.PrictureIds = string.Join("_", propertyCreateModel.Pictures.Select(p => p.PictureId).ToArray());
            copy.FileIds = string.Join("_", propertyCreateModel.Files.Select(p => p.FileId).ToArray());
            if (logoPicture != null) copy.LogoPicture_Id = logoPicture.Id;
            return copy;
        }

        [NonAction]
        protected virtual void PreparePropertyPictures(PropertyCreateModel propertyCreateModel)
        {
            propertyCreateModel.Pictures = propertyCreateModel.Pictures.Where(p => !p.IsLogo).ToList();

            foreach (var propertyPictureModel in propertyCreateModel.Pictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            propertyCreateModel.Pictures = propertyCreateModel.Pictures.OrderBy(pp => pp.DisplayOrder).ToList();
        }

        [NonAction]
        protected virtual void PreparePropertyPictures(PropertyModel propertyModel)
        {
            propertyModel.Pictures = propertyModel.Pictures.Where(p => !p.IsLogo).ToList();

            foreach (var propertyPictureModel in propertyModel.Pictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            propertyModel.Pictures = propertyModel.Pictures.OrderBy(pp => pp.DisplayOrder).ToList();
        }

        [NonAction]
        protected virtual void PreparePropertyFiles(PropertyModel propertyModel)
        {
            foreach (var propertyFileModel in propertyModel.Files)
            {
                propertyFileModel.Src = _fileService.GetFileUrl(propertyFileModel.FileId);
                propertyFileModel.Uploaded = true;
                propertyFileModel.Percentage = 100;
            }
        }

        [NonAction]
        protected virtual void PreparePropertyFiles(PropertyCreateModel propertyCreateModel)
        {
            foreach (var propertyFileModel in propertyCreateModel.Files)
            {
                propertyFileModel.Src = _fileService.GetFileUrl(propertyFileModel.FileId);
                propertyFileModel.Uploaded = true;
                propertyFileModel.Percentage = 100;
            }
        }

        #region 图片处理
        [NonAction]
        protected virtual void SavePropertyPictures(Property property, IEnumerable<PropertyPictureModel> propertyPictures)
        {
            var existingPropertyPictures = property.Pictures.ToList();
            var propertyPicturesToRemove = new List<PropertyPicture>();

            //获取要删除的图片
            foreach (var existingPropertyPicture in existingPropertyPictures)
            {
                if (existingPropertyPicture.IsLogo) continue;  //不要删logo

                if (propertyPictures.Where(pp => pp.Id == existingPropertyPicture.Id).Count() == 0)
                {
                    propertyPicturesToRemove.Add(existingPropertyPicture);
                }
            }

            foreach (var newPropertyPictureModel in propertyPictures)
            {
                var propertyPicture = new PropertyPicture
                {
                    PictureId = newPropertyPictureModel.PictureId,
                    IsLogo = false,
                    DisplayOrder = 0
                };

                if (newPropertyPictureModel.Id == 0) property.Pictures.Add(propertyPicture);
            }

            //删除不存在的图片
            foreach (var propertyPicture in propertyPicturesToRemove)
            {
                _propertyService.DeletePropertyPicture(propertyPicture);
                //property.Pictures.Remove(propertyPicture);
            }
        }

        [NonAction]
        protected virtual void SavePropertyFiles(Property property, IEnumerable<PropertyFileModel> propertyFiles)
        {
            var existingPropertyFiles = property.Files.ToList();
            var propertyFilesToRemove = new List<PropertyFile>();

            //获取要删除的文件
            foreach (var existingPropertyFile in existingPropertyFiles)
            {
                if (propertyFiles.Where(pp => pp.Id == existingPropertyFile.Id).Count() == 0)
                {
                    propertyFilesToRemove.Add(existingPropertyFile);
                }
            }

            foreach (var newPropertyFileModel in propertyFiles)
            {
                var propertyFile = new PropertyFile
                {
                    FileId = newPropertyFileModel.FileId,
                    DisplayOrder = 0
                };
                if (newPropertyFileModel.Id == 0) property.Files.Add(propertyFile);
            }

            //删除不存在的图片
            foreach (var propertyFile in propertyFilesToRemove)
            {
                _propertyService.DeletePropertyFile(propertyFile);
            }
        }

        [NonAction]
        protected virtual void SavePropertyLendPictures(PropertyLend propertyLend, IEnumerable<PropertyLendPictureModel> propertyLendPictures)
        {
            var existingPropertyLendPictures = propertyLend.LendPictures.ToList();
            var propertyPicturesToRemove = new List<PropertyLendPicture>();

            //获取要删除的图片
            foreach (var existingPropertyLendPicture in existingPropertyLendPictures)
            {
                if (propertyLendPictures.Where(pp => pp.Id == existingPropertyLendPicture.Id).Count() == 0)
                {
                    propertyPicturesToRemove.Add(existingPropertyLendPicture);
                }
            }

            foreach (var newPropertyLendPictureModel in propertyLendPictures)
            {
                var propertyPicture = newPropertyLendPictureModel.ToEntity();

                if (newPropertyLendPictureModel.Id == 0) propertyLend.LendPictures.Add(propertyPicture);
            }

            //删除不存在的图片
            foreach (var propertyLendPicture in propertyPicturesToRemove)
            {
                _propertyLendService.DeletePropertyLendPicture(propertyLendPicture);
            }
        }

        [NonAction]
        protected virtual void SavePropertyLendFiles(PropertyLend property, IEnumerable<PropertyLendFileModel> propertyLendFiles)
        {
            var existingPropertyLendFiles = property.LendFiles.ToList();
            var propertyLendFilesToRemove = new List<PropertyLendFile>();

            //获取要删除的文件
            foreach (var existingPropertyLendFile in existingPropertyLendFiles)
            {
                if (propertyLendFiles.Where(pp => pp.Id == existingPropertyLendFile.Id).Count() == 0)
                {
                    propertyLendFilesToRemove.Add(existingPropertyLendFile);
                }
            }

            foreach (var newPropertyFileModel in propertyLendFiles)
            {
                var propertyFile = newPropertyFileModel.ToEntity();

                if (newPropertyFileModel.Id == 0) property.LendFiles.Add(propertyFile);
            }

            //删除不存在的图片
            foreach (var propertyLendFile in propertyLendFilesToRemove)
            {
                _propertyLendService.DeletePropertyLendFile(propertyLendFile);
            }
        }

        [NonAction]
        protected virtual void SavePropertyRentPictures(PropertyRent propertyRent, IEnumerable<PropertyRentPictureModel> propertyRentPictures)
        {
            var existingPropertyRentPictures = propertyRent.RentPictures.ToList();
            var propertyPicturesToRemove = new List<PropertyRentPicture>();

            //获取要删除的图片
            foreach (var existingPropertyRentPicture in existingPropertyRentPictures)
            {
                if (propertyRentPictures.Where(pp => pp.Id == existingPropertyRentPicture.Id).Count() == 0)
                {
                    propertyPicturesToRemove.Add(existingPropertyRentPicture);
                }
            }

            foreach (var newPropertyRentPictureModel in propertyRentPictures)
            {
                var propertyPicture = newPropertyRentPictureModel.ToEntity();

                if (newPropertyRentPictureModel.Id == 0) propertyRent.RentPictures.Add(propertyPicture);
            }

            //删除不存在的图片
            foreach (var propertyRentPicture in propertyPicturesToRemove)
            {
                _propertyRentService.DeletePropertyRentPicture(propertyRentPicture);
            }
        }

        [NonAction]
        protected virtual void SavePropertyRentFiles(PropertyRent property, IEnumerable<PropertyRentFileModel> propertyRentFiles)
        {
            var existingPropertyRentFiles = property.RentFiles.ToList();
            var propertyRentFilesToRemove = new List<PropertyRentFile>();

            //获取要删除的文件
            foreach (var existingPropertyRentFile in existingPropertyRentFiles)
            {
                if (propertyRentFiles.Where(pp => pp.Id == existingPropertyRentFile.Id).Count() == 0)
                {
                    propertyRentFilesToRemove.Add(existingPropertyRentFile);
                }
            }

            foreach (var newPropertyFileModel in propertyRentFiles)
            {
                var propertyFile = newPropertyFileModel.ToEntity();

                if (newPropertyFileModel.Id == 0) property.RentFiles.Add(propertyFile);
            }

            //删除不存在的图片
            foreach (var propertyRentFile in propertyRentFilesToRemove)
            {
                _propertyRentService.DeletePropertyRentFile(propertyRentFile);
            }
        }

        [NonAction]
        protected virtual void SavePropertyAllotPictures(PropertyAllot propertyAllot, IEnumerable<PropertyAllotPictureModel> propertyAllotPictures)
        {
            var existingPropertyAllotPictures = propertyAllot.AllotPictures.ToList();
            var propertyPicturesToRemove = new List<PropertyAllotPicture>();

            //获取要删除的图片
            foreach (var existingPropertyAllotPicture in existingPropertyAllotPictures)
            {
                if (propertyAllotPictures.Where(pp => pp.Id == existingPropertyAllotPicture.Id).Count() == 0)
                {
                    propertyPicturesToRemove.Add(existingPropertyAllotPicture);
                }
            }

            foreach (var newPropertyAllotPictureModel in propertyAllotPictures)
            {
                var propertyPicture = newPropertyAllotPictureModel.ToEntity();

                if (newPropertyAllotPictureModel.Id == 0) propertyAllot.AllotPictures.Add(propertyPicture);
            }

            //删除不存在的图片
            foreach (var propertyAllotPicture in propertyPicturesToRemove)
            {
                _propertyAllotService.DeletePropertyAllotPicture(propertyAllotPicture);
            }
        }

        [NonAction]
        protected virtual void SavePropertyAllotFiles(PropertyAllot property, IEnumerable<PropertyAllotFileModel> propertyAllotFiles)
        {
            var existingPropertyAllotFiles = property.AllotFiles.ToList();
            var propertyAllotFilesToRemove = new List<PropertyAllotFile>();

            //获取要删除的文件
            foreach (var existingPropertyAllotFile in existingPropertyAllotFiles)
            {
                if (propertyAllotFiles.Where(pp => pp.Id == existingPropertyAllotFile.Id).Count() == 0)
                {
                    propertyAllotFilesToRemove.Add(existingPropertyAllotFile);
                }
            }

            foreach (var newPropertyFileModel in propertyAllotFiles)
            {
                var propertyFile = newPropertyFileModel.ToEntity();

                if (newPropertyFileModel.Id == 0) property.AllotFiles.Add(propertyFile);
            }

            //删除不存在的图片
            foreach (var propertyAllotFile in propertyAllotFilesToRemove)
            {
                _propertyAllotService.DeletePropertyAllotFile(propertyAllotFile);
            }
        }

        [NonAction]
        protected virtual void SavePropertyOffPictures(PropertyOff propertyOff, IEnumerable<PropertyOffPictureModel> propertyOffPictures)
        {
            var existingPropertyOffPictures = propertyOff.OffPictures.ToList();
            var propertyPicturesToRemove = new List<PropertyOffPicture>();

            //获取要删除的图片
            foreach (var existingPropertyOffPicture in existingPropertyOffPictures)
            {
                if (propertyOffPictures.Where(pp => pp.Id == existingPropertyOffPicture.Id).Count() == 0)
                {
                    propertyPicturesToRemove.Add(existingPropertyOffPicture);
                }
            }

            foreach (var newPropertyOffPictureModel in propertyOffPictures)
            {
                var propertyPicture = newPropertyOffPictureModel.ToEntity();

                if (newPropertyOffPictureModel.Id == 0) propertyOff.OffPictures.Add(propertyPicture);
            }

            //删除不存在的图片
            foreach (var propertyOffPicture in propertyPicturesToRemove)
            {
                _propertyOffService.DeletePropertyOffPicture(propertyOffPicture);
            }
        }

        [NonAction]
        protected virtual void SavePropertyOffFiles(PropertyOff property, IEnumerable<PropertyOffFileModel> propertyOffFiles)
        {
            var existingPropertyOffFiles = property.OffFiles.ToList();
            var propertyOffFilesToRemove = new List<PropertyOffFile>();

            //获取要删除的文件
            foreach (var existingPropertyOffFile in existingPropertyOffFiles)
            {
                if (propertyOffFiles.Where(pp => pp.Id == existingPropertyOffFile.Id).Count() == 0)
                {
                    propertyOffFilesToRemove.Add(existingPropertyOffFile);
                }
            }

            foreach (var newPropertyFileModel in propertyOffFiles)
            {
                var propertyFile = newPropertyFileModel.ToEntity();

                if (newPropertyFileModel.Id == 0) property.OffFiles.Add(propertyFile);
            }

            //删除不存在的图片
            foreach (var propertyOffFile in propertyOffFilesToRemove)
            {
                _propertyOffService.DeletePropertyOffFile(propertyOffFile);
            }
        }
        #endregion

        [NonAction]
        protected virtual void PreparePropertyLendPicturesAndFiles(PropertyLendModel propertyLendModel)
        {
            propertyLendModel.LendPictures = propertyLendModel.LendPictures.ToList();

            foreach (var propertyPictureModel in propertyLendModel.LendPictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            propertyLendModel.LendPictures = propertyLendModel.LendPictures.OrderBy(pp => pp.DisplayOrder).ToList();

            foreach (var propertyLendFileModel in propertyLendModel.LendFiles)
            {
                propertyLendFileModel.Src = _fileService.GetFileUrl(propertyLendFileModel.FileId);
                propertyLendFileModel.Uploaded = true;
                propertyLendFileModel.Percentage = 100;
            }
        }

        [NonAction]
        protected virtual void PreparePropertyRentPicturesAndFiles(PropertyRentModel propertyRentModel)
        {
            propertyRentModel.RentPictures = propertyRentModel.RentPictures.ToList();

            foreach (var propertyPictureModel in propertyRentModel.RentPictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            propertyRentModel.RentPictures = propertyRentModel.RentPictures.OrderBy(pp => pp.DisplayOrder).ToList();

            foreach (var propertyRentFileModel in propertyRentModel.RentFiles)
            {
                propertyRentFileModel.Src = _fileService.GetFileUrl(propertyRentFileModel.FileId);
                propertyRentFileModel.Uploaded = true;
                propertyRentFileModel.Percentage = 100;
            }
        }

        [NonAction]
        protected virtual void PreparePropertyPatrolPictures(PropertyPatrolModel propertyPatrolModel)
        {
            propertyPatrolModel.PatrolPictures = propertyPatrolModel.PatrolPictures.ToList();

            foreach (var propertyPictureModel in propertyPatrolModel.PatrolPictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            propertyPatrolModel.PatrolPictures = propertyPatrolModel.PatrolPictures.OrderBy(pp => pp.DisplayOrder).ToList();          
        }

        [NonAction]
        protected virtual void PreparePropertyAllotPicturesAndFiles(PropertyAllotModel propertyAllotModel)
        {
            propertyAllotModel.AllotPictures = propertyAllotModel.AllotPictures.ToList();

            foreach (var propertyPictureModel in propertyAllotModel.AllotPictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            propertyAllotModel.AllotPictures = propertyAllotModel.AllotPictures.OrderBy(pp => pp.DisplayOrder).ToList();

            foreach (var propertyAllotFileModel in propertyAllotModel.AllotFiles)
            {
                propertyAllotFileModel.Src = _fileService.GetFileUrl(propertyAllotFileModel.FileId);
                propertyAllotFileModel.Uploaded = true;
                propertyAllotFileModel.Percentage = 100;
            }
        }

        [NonAction]
        protected virtual void PreparePropertyOffPicturesAndFiles(PropertyOffModel propertyOffModel)
        {
            propertyOffModel.OffPictures = propertyOffModel.OffPictures.ToList();

            foreach (var propertyPictureModel in propertyOffModel.OffPictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            propertyOffModel.OffPictures = propertyOffModel.OffPictures.OrderBy(pp => pp.DisplayOrder).ToList();

            foreach (var propertyOffFileModel in propertyOffModel.OffFiles)
            {
                propertyOffFileModel.Src = _fileService.GetFileUrl(propertyOffFileModel.FileId);
                propertyOffFileModel.Uploaded = true;
                propertyOffFileModel.Percentage = 100;
            }
        }

        [NonAction]
        protected virtual PropertyPicture GetLogoPicture(Property property)
        {

            //获取logo图片
            var logoPicture = property.Pictures.Where(sp => sp.IsLogo).SingleOrDefault();

            return logoPicture;
        }

        [NonAction]
        protected virtual string GetLogoUrl(Property property)
        {
            string logoUrl = string.Empty;
            //获取logo图片
            var logoPicture = GetLogoPicture(property);
            if (logoPicture != null)
            {
                logoUrl = _pictureService.GetPictureUrl(logoPicture.PictureId);
            }

            return logoUrl;
        }

        /// <summary>
        /// 资产锁定状态切换
        /// </summary>
        /// <param name="locked"></param>
        /// <param name="property"></param>
        [NonAction]
        protected virtual void SwitchPropertyLockState(bool locked, Property property)
        {
            if (property.Locked != locked)
            {
                property.Locked = locked; //资产被锁定，无法进行其他操作
                _propertyService.UpdateProperty(property);

                //activity log
                _accountUserActivityService.InsertActivity("LockPropertySwitch", "{1} 名为 {0} 的资产", property.Name, locked ? "锁定" : "解锁");
            }
        }

        [NonAction]
        //解压文件代码
        protected virtual bool UnRAR(string path, string rarPath, string rarName)
        {
            bool flag = false;
            string rarexe;
            RegistryKey regkey;
            Object regvalue;
            string cmd;
            ProcessStartInfo startinfo;
            Process process;
            try
            {
                regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                regvalue = regkey.GetValue("");
                rarexe = regvalue.ToString();
                regkey.Close();
                //rarexe = rarexe.Substring(1, rarexe.Length - 7);

                Directory.CreateDirectory(path);
                //解压缩命令，相当于在要压缩文件(rarName)上点右键->WinRAR->解压到当前文件夹
                //cmd = string.Format(path + "\\" + rarName,
                //                    rarName,
                //                    path);
                cmd = string.Format("x {0} {1} -y",
                        rarName,
                        path);

                startinfo = new ProcessStartInfo();
                startinfo.FileName = rarexe;
                startinfo.Arguments = cmd;
                startinfo.WindowStyle = ProcessWindowStyle.Hidden;

                startinfo.WorkingDirectory = rarPath;
                process = new Process();
                process.StartInfo = startinfo;
                process.Start();
                process.WaitForExit();

                if (process.HasExited)
                {
                    flag = true;
                }
                process.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return flag;
        }

        public string GetXml(string shppath)
        {
            string shpName = "";
            DirectoryInfo dir = new DirectoryInfo(shppath);
            FileInfo[] files = dir.GetFiles("资产导入表.xlsx");

            if (files.Length < 1)
            {
                string[] folders = Directory.GetDirectories(shppath);
                foreach (var folder in folders)
                {
                    return GetXml(folder);
                }
            }
            else
            {
                foreach (var file in files)
                {
                    shpName = file.Directory.ToString() + "\\" + file.Name;
                }
            }
            return shpName;
        }



        #endregion

        #region 资产API
        [HttpGet]
        [Route("Unique/{name}")]
        public IHttpActionResult UniqueCheck(string name)
        {
            var result = !_propertyService.NameUniqueCheck(name);

            return Ok(result);
        }
        
        [HttpGet]
        [Route("MonthTotal")]
        public IHttpActionResult GetMonthTotal() {
            var currentUser = _workContext.CurrentAccountUser;

            var response = _propertyService.GetMonthTotalPropertyProcess(currentUser.Government.Id).Select(p=>
            {
                var property = p.ToModel();
                property.LogoUrl = GetLogoUrl(p);
                property.MonthTotalModel = _monthTotalService.GetPropertyMonthTotal(property.Id, DateTime.Now.Year.ToString() +";"+ DateTime.Now.Month.ToString()).Select(m=>
                {
                    return m.ToModel();
                }
                ).ToList();
                return property;
            }).ToList();

            return Ok(response);
        }

        [HttpGet]
        [Route("GetRecord")]
        public IHttpActionResult GetRecord() {
            var record = false;
            var user = _workContext.CurrentAccountUser;
            var submitRecord = _submitRecordService.GetSubmitRecordByGId(user.Government.Id,DateTime.Now.Year.ToString()+DateTime.Now.Month.ToString());

            if (submitRecord != null) {
                record = true;
            }

            return Ok(record);
        }


        [HttpGet]
        [Route("CurrentUserId")]
        public IHttpActionResult GetCurrentUserId() {

            var currentUser = _workContext.CurrentAccountUser;

            return Ok(currentUser.Id);

        }


        [HttpPost]
        [Route("createmonthtotal")]
        public IHttpActionResult CreateMonthTotal(MonthTotalModel monthTotalModel){
            var monthTotal = new MonthTotal();
            monthTotal = monthTotalModel.ToEntity();
            monthTotal.Month = DateTime.Now;
            _monthTotalService.InsertMonthTotal(monthTotal);

            return Ok();
        }

        [HttpPost]
        [Route("createall")]
        public IHttpActionResult CreateAll(IList <MonthTotalModel> All)
        {
            var user = _workContext.CurrentAccountUser;
            foreach (var model in All) {
                var monthTotal = new MonthTotal();
                monthTotal = model.ToEntity();
                monthTotal.Month = DateTime.Now;
                _monthTotalService.InsertMonthTotal(monthTotal);
            }

            var submitRecord = new SubmitRecord();
            submitRecord.Goverment_ID = user.Government.Id;
            submitRecord.RecordDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
            _submitRecordService.InsertSubmitRecord(submitRecord);
            return Ok();
        }


        [HttpGet]
        [Route("{propertyId:int}")]
        public IHttpActionResult Get(int propertyId, bool simple = false)
        {
            //var currentUser = _workContext.CurrentAccountUser;

            var property = _propertyService.GetPropertyById(propertyId);
            if (property == null || property.Deleted)
                return NotFound();

            //if (!(currentUser.IsAdmin()|| currentUser.IsDataReviewer() || PropertyCanView(property))) 
            //    return NotFound();

            if (simple)
            {                
                var model = property.ToSimpleModel();

                //activity log
                _accountUserActivityService.InsertActivity("GetpropertyInfo", "获取 名为 {0} 的资产简单信息", property.Name);
                return Ok(model);
            }
            else
            {

                var model = property.ToModel();
                model.LogoUrl = GetLogoUrl(property);
                model.NewCreate = _propertyNewCreateService.GetPropertyNewCreateByPropertyId(model.Id).ToModel();
                model.Edits = model.Edits.Where(m => m.Deleted != true).ToList();
                model.Rents = model.Rents.Where(m => m.Deleted != true).ToList();
                model.Patrols = model.Patrols.ToList();
                model.Lends = model.Lends.Where(m => m.Deleted).ToList();

                model.CanEditDelete = false;// PropertyCanEditDelete(property);
                model.CanChange = false;// PropertyCanChange(property);
                var propertyOff = _propertyOffService.GetPropertyOffByPropertyId(model.Id).ToModel();
                if (propertyOff != null) model.PropertyOff = propertyOff;

                //获取图片
                PreparePropertyPictures(model);
                //获取文件
                PreparePropertyFiles(model);

                //获取出租信息
                foreach(var rentModel in model.Rents)
                {
                    var rent = _propertyRentService.GetPropertyRentById(rentModel.Id);
                    if (rent == null || rent.Deleted) throw new Exception("无法找到出租信息资源");                   

                    rentModel.Valid = DateTime.Now >= rent.RentTime && DateTime.Now < rent.BackTime;

                    if (rentModel.PriceString.EndsWith(";")) rentModel.PriceString = rentModel.PriceString.TrimEnd(';');

                    PreparePropertyRentPicturesAndFiles(rentModel);                   
                }

                foreach(var patrolModel in model.Patrols){
                   // var patrol = _propertyPatrolService.GetPropertyPatrolById(patrolModel.Id);
                    PreparePropertyPatrolPictures(patrolModel);
                }
 

                //activity log
                _accountUserActivityService.InsertActivity("GetpropertyInfo", "获取 名为 {0} 的资产信息", property.Name);
                return Ok(model);
            }
        }

        /// <summary>
        /// 获取要更新的资产
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Update/{propertyId:int}")]
        public IHttpActionResult GetUpdatedProperty(int propertyId)
        {
            //var currentUser = _workContext.CurrentAccountUser;

            var property = _propertyService.GetPropertyById(propertyId);
            if (property == null || property.Deleted)
                return NotFound();

            //if (!PropertyCanEditDelete(property) && !PropertyCanChange(property)) return NotFound();// "资产无法编辑或变更！");

            var propertyCreatModel = new PropertyCreateModel();

            var edit = property.Edits.Where(e => e.State == PropertyApproveState.Start && !e.Deleted).FirstOrDefault();
            if (edit != null) {
                var copyproperty = _copyPropertyService.GetCopyPropertyById(edit.CopyProperty_Id);
                propertyCreatModel = copyproperty.ToCreateModel();

               // propertyCreatModel.Owner_self = propertyCreatModel.GovernmentId == currentUser.Government.Id;
                propertyCreatModel.Owner_children = propertyCreatModel.Owner_self;

                #region 新数据库对象
                #region 获取图片
                propertyCreatModel.Pictures = new List<PropertyPictureModel>();
                foreach (var pid in copyproperty.PrictureIds.Split('_'))
                {
                    if (string.IsNullOrWhiteSpace(pid)) continue;
                    var picture = _pictureService.GetPictureById(Convert.ToInt32(pid));
                    if (picture == null) continue;

                    var propertyPictureModel = new PropertyPictureModel
                    {
                        PictureId = picture.Id,
                        PropertyId = copyproperty.Id,
                        Href = _pictureService.GetPictureUrl(picture.Id),
                        Title = picture.TitleAttribute
                    };

                    propertyCreatModel.Pictures.Add(propertyPictureModel);
                }
                #endregion

                #region 文件更新
                propertyCreatModel.Files = new List<PropertyFileModel>();
                foreach (var fid in copyproperty.FileIds.Split('_'))
                {
                    if (string.IsNullOrWhiteSpace(fid)) continue;
                    var file = _fileService.GetFileById(Convert.ToInt32(fid));
                    if (file == null) continue;

                    var propertyFileModel = new PropertyFileModel
                    {
                        FileId = file.Id,
                        PropertyId = copyproperty.Id,
                        Src = _fileService.GetFileUrl(file),
                        Title = file.Name,
                        Percentage = 100,
                        Uploaded = true
                    };

                    propertyCreatModel.Files.Add(propertyFileModel);
                }

                #endregion
                var logoUrl= _pictureService.GetPictureUrl(copyproperty.LogoPicture_Id);
                propertyCreatModel.LogoUrl = string.IsNullOrEmpty(logoUrl) ? null : logoUrl;
                propertyCreatModel.Logo = null;
                propertyCreatModel.LogoPictureId = copyproperty.LogoPicture_Id;
                #endregion
            }   
            else 
            {
                //if (!currentUser.IsAdmin() && !(property.Government.Id == currentUser.Government.Id || property.Government.ParentGovernmentId == currentUser.Government.Id)
                //    return NotFound();

                propertyCreatModel = property.ToCreateModel();
               // propertyCreatModel.Owner_self = propertyCreatModel.GovernmentId == currentUser.Government.Id;
                propertyCreatModel.Owner_children = propertyCreatModel.Owner_self;
                propertyCreatModel.LogoUrl = string.IsNullOrEmpty(GetLogoUrl(property)) ? null : GetLogoUrl(property);
                var propertyLogoPicture = GetLogoPicture(property);
                propertyCreatModel.LogoPictureId = propertyLogoPicture == null ? 0 : propertyLogoPicture.PictureId;
                propertyCreatModel.Logo = null;
                //获取图片
                PreparePropertyPictures(propertyCreatModel);

                //获取文件
                PreparePropertyFiles(propertyCreatModel);

                //activity log
                _accountUserActivityService.InsertActivity("GetpropertyInfo", "获取 名为 {0} 的资产信息", property.Name);
            }

            return Ok(propertyCreatModel);
        }

        [HttpGet]
        [Route("Rent/{rentId:int}")]
        public IHttpActionResult GetRentById(int rentId)
        {
            var rent = _propertyRentService.GetPropertyRentById(rentId);

            var rentModel = rent.ToModel();

            foreach (var propertyPictureModel in rentModel.RentPictures)
            {
                propertyPictureModel.Href = _pictureService.GetPictureUrl(propertyPictureModel.PictureId);
            }

            rentModel.RentPictures = rentModel.RentPictures.OrderBy(pp => pp.DisplayOrder).ToList();

            foreach (var propertyFileModel in rentModel.RentFiles)
            {
                propertyFileModel.Src = _fileService.GetFileUrl(propertyFileModel.FileId);
                propertyFileModel.Uploaded = true;
                propertyFileModel.Percentage = 100;
            }

            return Ok(rentModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex">页码从1开始</param>
        /// <param name="showHidden"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(string query = "", string sort = "", int pageSize = Int32.MaxValue, int pageIndex = 1, bool showHidden = false, long time = 0
            //bool manage = false, bool isGovernment = false, bool isInstitution = false, bool isCompany = false, int selectedId = 0,
            //bool construct = false, bool land = false, bool constructOnLand = false,//资产类型
            //bool old = false, bool west = false, bool jjq = false, bool kc = false, bool qj = false, bool other = false, //区域
            //bool certi_both = false, bool certi_land = false, bool certi_construct = false, bool certi_none = false, //证书情况
            //bool current_self = false, bool current_rent = false, bool current_lend = false, bool currnet_idle = false, //使用现状
            //bool auction = false, bool ct = false, bool jt = false, bool jk = false, bool self = false, bool storeUp = false, bool adjust = false, bool greenland = false, bool house = false,//下步使用
            //bool constructArea_L=false, bool constructArea_M = false, bool constructArea_H = false, bool constructArea_T = false,
            //bool landArea_L = false, bool landArea_M = false, bool landArea_H = false, bool landArea_T = false,
            //bool price_L = false, bool price_M = false, bool price_H = false, bool price_T = false,
            //string lifeTime_min="",string lifeTime_max=""
            )
        {
            //var currentUser = _workContext.CurrentAccountUser;

            //showHidden = currentUser.IsRegistered() && !(currentUser.IsAdmin() || currentUser.IsGovAuditor() || currentUser.IsStateOwnerAuditor() || currentUser.IsDataReviewer());   //只是注册单位可以获取未发布的

            //页码-1
            if (pageIndex <= 0) pageIndex = 1;
            //初始化排序条件
            var sortConditions = PropertySortCondition.Instance(sort);

            //特殊字段排序调整
            if (sort.ToLower().StartsWith("governmentname")) sortConditions[0].PropertyName = "Government";

            ////高级搜索参数设置
            //PropertyAdvanceConditionModel advance = new PropertyAdvanceConditionModel
            //{
            //    Government = new GovernmentFilterModel { Manage = manage, IsCompany = isCompany, IsGovernment = isGovernment, IsInstitution = isInstitution, SelectedId = selectedId },
            //    PropertyType = new PropertyTypeModel { Construct = construct, Land = land, ConstructOnLand = constructOnLand },
            //    Region = new RegionModel { West = west, Jjq = jjq, Kc = kc, Old = old, Other = other, Qj = qj },
            //    Certificate = new Certificate { Both = certi_both, Construct = certi_construct, Land = certi_land, None = certi_land },
            //    Current = new CurrentModel { Idle = currnet_idle, Lend = current_lend, Rent = current_rent, Self = current_self },
            //    NextStep = new NextStepModel { Adjust = adjust, Auction = auction, Ct = ct, Jt = jt, Jk = jk, Greenland = greenland, House = house, Self = self, StoreUp = storeUp },
            //    Extent = new ExtentModel { Geo = "", Type = "" },
            //    ConstructArea = new RangeList { Ranges = new List<Range>() },
            //    LandArea = new RangeList { Ranges = new List<Range>() },
            //    Price = new RangeList { Ranges = new List<Range>() },
            //    LifeTime = new Range { Min = Convert.ToDouble(lifeTime_min), Max = Convert.ToDouble(lifeTime_max) }
            //};

            ////建筑面积
            //if (constructArea_L) advance.ConstructArea.Ranges.Add(new Range { Min = 0, Max = 5000 });
            //if (constructArea_M) advance.ConstructArea.Ranges.Add(new Range { Min = 5001, Max = 10000 });
            //if (constructArea_H) advance.ConstructArea.Ranges.Add(new Range { Min = 10001, Max = 20000 });
            //if (constructArea_T) advance.ConstructArea.Ranges.Add(new Range { Min = 20001, Max = 0 });

            ////土地面积
            //if (landArea_L) advance.LandArea.Ranges.Add(new Range { Min = 0, Max = 51 * 666.67 - 1 });
            //if (landArea_M) advance.LandArea.Ranges.Add(new Range { Min = 51 * 666.67, Max = 301 * 666.67-1 });
            //if (landArea_H) advance.LandArea.Ranges.Add(new Range { Min = 301 * 666.67, Max = 501 * 666.67-1 });
            //if (landArea_T) advance.LandArea.Ranges.Add(new Range { Min = 501 * 666.67, Max = 0 });

            ////账面价值
            //if (price_L) advance.Price.Ranges.Add(new Range { Min = 0, Max = 500 });
            //if (price_M) advance.Price.Ranges.Add(new Range { Min = 501, Max = 5000 });
            //if (price_H) advance.Price.Ranges.Add(new Range { Min = 5001, Max = 10000 });
            //if (price_T) advance.Price.Ranges.Add(new Range { Min = 10001, Max = 0 });                      


            //高级搜索参数设置
            //PropertyAdvanceConditionRequest request = PrepareAdvanceCondition(advance);

            var governmentIds = new List<int>();//  _governmentService.GetGovernmentIdsByCurrentUser();  //获取当前账户的可查询的资产


            var properties = _propertyService.GetAllProperties(governmentIds, query, pageIndex-1, pageSize,
               showHidden, null, sortConditions);

            var response = new ListResponse<PropertyListModel>
            {
                Time = time,
                Paging = new Paging
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Total = properties.TotalCount,
                    FilterCount = string.IsNullOrEmpty(query) ? properties.TotalCount : properties.Count,
                },
                Data = properties.Select(s =>
                {
                    var propertyModel = s.ToListModel();                    
                    if (s.Off)
                        propertyModel.Name = propertyModel.Name + "（已核销）";
                    else if (!s.Published)
                        propertyModel.Name = propertyModel.Name + "（未发布）";


                    propertyModel.CanEditDelete = false;// PropertyCanEditDelete(s);

                    return propertyModel;
                })
            };

            //activity log
            _accountUserActivityService.InsertActivity("GetpropertyList", "获取资产列表信息");

            return Ok(response);
        }


        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAll(PropertyAdvanceConditionModel advance)
        {
            var currentUser = _workContext.CurrentAccountUser;

      var      showHidden = currentUser.IsRegistered() && !(currentUser.IsAdmin() || currentUser.IsGovAuditor() || currentUser.IsStateOwnerAuditor() || currentUser.IsDataReviewer());   //只是注册单位可以获取未发布的

            //初始化排序条件
            var sortConditions = PropertySortCondition.Instance(advance.Sort);

            //特殊字段排序调整
            if (advance.Sort.ToLower().StartsWith("governmentname")) sortConditions[0].PropertyName = "Government";

            //高级搜索参数设置
            PropertyAdvanceConditionRequest request = PrepareAdvanceCondition(advance);

            var governmentIds = _governmentService.GetGovernmentIdsByCurrentUser();  //获取当前账户的可查询的资产

            var properties = _propertyService.GetAllProperties(governmentIds,advance.Query, advance.PageIndex, advance.PageSize,
                showHidden, request, sortConditions);

            var response = new ListResponse<PropertyModel>
            {
                Time = advance.Time,
                Paging = new Paging
                {
                    PageIndex = advance.PageIndex,
                    PageSize = advance.PageSize,
                    Total = properties.TotalCount,
                    FilterCount = string.IsNullOrEmpty(advance.Query) ? properties.TotalCount : properties.Count,
                },
                Data = properties.Select(s =>
                {
                    var propertyModel = s.ToModel();
                    if (!propertyModel.Published) propertyModel.Name += "（未发布）";
                    else if (!s.Off) propertyModel.Name += "（已核销）";
                    return propertyModel;
                })
            };

            //activity log
            _accountUserActivityService.InsertActivity("GetpropertyList", "获取资产列表信息");

            return Ok(response);
        }

        /// <summary>
        /// 地图中的搜索
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("geo")]
        public IHttpActionResult GetAllInMap(PropertyAdvanceConditionModel advance)
        {
            var currentUser = _workContext.CurrentAccountUser;

            var showHidden = currentUser.IsRegistered() && currentUser.AccountUserRoles.Count == 1;  //只是注册单位可以获取未发布的

            //初始化排序条件
            var sortConditions = PropertySortCondition.Instance(advance.Sort);

            //特殊字段排序调整
            if (advance.Sort.ToLower().StartsWith("governmentname")) sortConditions[0].PropertyName = "Government";

            //高级搜索参数设置
            PropertyAdvanceConditionRequest request = PrepareAdvanceCondition(advance);
            var governmentIds = _governmentService.GetGovernmentIdsByCurrentUser();  //获取当前账户的可查询的资产
            var properties = _propertyService.GetAllProperties(governmentIds, advance.Query, 0, int.MaxValue, showHidden, request, sortConditions);

            var response = new ListResponse<GeoPropertyModel>
            {
                Time = advance.Time,
                Paging = new Paging
                {
                    PageIndex = 0,
                    PageSize = int.MaxValue,
                    Total = properties.TotalCount,
                    FilterCount = string.IsNullOrEmpty(advance.Query) ? properties.TotalCount : properties.Count,
                },
                Data = properties.Select(s =>
                {
                    var geoPropertyModel = s.ToGeoModel();
                    if (!s.Published) geoPropertyModel.Name += "（未发布）";
                    else
                    {
                        if (s.Off) geoPropertyModel.Name += "（已核销）";
                    }
                    return geoPropertyModel;
                })
            };

            //activity log
            _accountUserActivityService.InsertActivity("GetpropertyList", "获取资产列表信息");

            return Ok(response);
        }

        /// <summary>
        /// 获取地图大数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("geo/bigdata")]
        public IHttpActionResult GetMapBigData()
        {
            //var currentUser = _workContext.CurrentAccountUser;
            //var showHidden = currentUser.IsRegistered() && currentUser.AccountUserRoles.Count == 1;
            //var governmentIds = _governmentService.GetGovernmentIdsByCurrentUser();
            //var properties = _propertyService.GetAllProperties(governmentIds, showHidden);
            var properties = _propertyService.GetAllProperties();
            var mapProperties = properties.ToList().Select(p => {
                var geoModel = p.ToGeoModel();
                geoModel.X =Convert.ToDouble( geoModel.Location.Split(' ')[2].Substring(0, geoModel.Location.Split(' ')[2].Length - 1));
                geoModel.Y = Convert.ToDouble(geoModel.Location.Split(' ')[1].Substring(1, geoModel.Location.Split(' ')[1].Length - 1));

                return geoModel; });

            //activity log
            _accountUserActivityService.InsertActivity("GetGeopropertyList", "地图获取资产列表信息");

            return Ok(mapProperties);
        }
        [HttpGet]
        [Route("search")]
        public IHttpActionResult GetSearchKey(string search) {

            var response = _propertyService.GetKeyProperties(search).ToList().Select(p =>
            {
                return p.ToSimpleModel();
            });

            return Ok(response);
        }

        /// <summary>
        /// 搜索联想提示
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("geo/suggestion")]
        public IHttpActionResult Suggestion(string query = "",long time=0, int pageSize = 10, bool showHidden = false)
        {
            var currentUser = _workContext.CurrentAccountUser;

            var governmentIds = _governmentService.GetGovernmentIdsByCurrentUser();

            var properties = _propertyService.GetAllProperties(governmentIds, query, 0, pageSize, false);

            var response = new ListResponse<GeoPropertyModel>
            {
                Time = time,
                Paging = new Paging
                {
                    PageIndex = 0,
                    PageSize = int.MaxValue,
                    Total = properties.TotalCount,
                    FilterCount = properties.TotalCount,
                },
                Data = properties.Select(s =>
                {
                    return s.ToGeoModel();
                })
            };

            //activity log
            _accountUserActivityService.InsertActivity("PropertySuggestion", "资产信息关键字联想");

            return Ok(response);
        }

        /// <summary>
        /// 获取相同不动产证号 或者 房产证的 资产
        /// </summary>
        /// <param name="numberId">证据号码</param>
        /// <param name="typeId">证件类型，0为不动产证 1为房产证</param>
        /// <returns></returns>
        [HttpGet]
        [Route("samenumber")]
        public IHttpActionResult GetPropertiesBySameNumberId(string numberId, string typeId, int id = 0)
        {
            if (typeId != "0" && typeId != "1") return BadRequest();

            var response = _propertyService.GetPropertiesBySameNumberId(numberId, typeId).Select(p =>
             {
                 var pp = p.ToSameIdModel();

                 if (!p.Published)
                     pp.Name += "(未发布)";
                 return pp;
             }).ToList();



            if (id != 0)
            {
                var currentProperty = response.Where(p => p.Id == id).SingleOrDefault();
                if (currentProperty != null)
                    response.Remove(currentProperty);               
            }

 
        

            return Ok(response);
        }


        /// <summary>
        /// 新增资产
        /// </summary>
        /// <param name="propertyModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(PropertyCreateModel propertyCreateModel)
        {
            var checkMessage = PropertyCreateModelValid(propertyCreateModel);

            if (!string.IsNullOrEmpty(checkMessage)) throw new Exception(checkMessage);
           
            var property = propertyCreateModel.ToEntity();       

            PrepareProperty(property, propertyCreateModel);

            //一证多资产处理
            var orginParentPropertyId = propertyCreateModel.IsMain ? propertyCreateModel.ParentPropertyId : 0;

 

            _propertyService.InsertProperty(property);

 

            //activity log
            _accountUserActivityService.InsertActivity("AddNewproperty", "增加 名为 {0} 的资产", property.Name);

            #region 现场照片处理

            var picture = _pictureService.GetPictureById(propertyCreateModel.LogoPictureId);
            if (picture == null) throw new Exception("未上传资产现场照片");

            //关联
            var logoPicture = new PropertyPicture
            {
                IsLogo = true,
                Picture = picture,
                Property = property,
                DisplayOrder = 0
            };

            _propertyService.InsertPropertyPicture(logoPicture);
            //activity log
            _accountUserActivityService.InsertActivity("AddPropertyLogo", "增加 资产名为 {0} 的现场照片", property.Name);
            #endregion

            #region 附件处理

            //图片更新
            SavePropertyPictures(property, propertyCreateModel.Pictures);

            //文件更新
            SavePropertyFiles(property, propertyCreateModel.Files);

            #endregion

            //添加一个资产插入申请
            var propertyNewRecord = new PropertyNewCreate()
            {
                Property = property,
                Title = property.Name,
                State = propertyCreateModel.Submit ? PropertyApproveState.DepartmentApprove : PropertyApproveState.Start,
                ProcessDate = DateTime.Now,
                SuggestGovernmentId =  0//_workContext.CurrentAccountUser.Government.Id
            };

            //如果当前用户是主管部门，则跳过主管部门审核环节
            if (propertyCreateModel.Submit )//&& _workContext.CurrentAccountUser.Government.ParentGovernmentId == 0)
            {
                propertyNewRecord.State = PropertyApproveState.AdminApprove;
                propertyNewRecord.DSuggestion = "同意";
                propertyNewRecord.DApproveDate = DateTime.Now;
            }

            _propertyNewCreateService.InsertPropertyNewCreate(propertyNewRecord);

            //activity log
            _accountUserActivityService.InsertActivity("AddNewproperty", "增加 名为 {0} 的资产创建申请", property.Name);

            SwitchPropertyLockState(true, property);

            //_propertyService.UpdateProperty(property);
            return Ok(property.ToModel());
        }

        /// <summary>
        /// 更新资产
        /// </summary>
        /// <param name="propertyModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{propertyId:int}")]
        public IHttpActionResult UpdateProperty(int propertyId, PropertyCreateModel propertyCreateModel)
        {
            var property = _propertyService.GetPropertyById(propertyId);
            var parentid = property.ParentPropertyId;

            var logoPicture = _pictureService.GetPictureById(propertyCreateModel.LogoPictureId);

            var imgs = property.Pictures;
            property = propertyCreateModel.ToEntity(property);
            PrepareProperty(property, propertyCreateModel);

            #region 现场照片处理
            var logoPropertyPicture = property.Pictures.Where(p => p.IsLogo).SingleOrDefault();
            if (logoPropertyPicture == null)
            {
                //关联
                logoPropertyPicture = new PropertyPicture
                {
                    IsLogo = true,
                    Picture = logoPicture,
                    Property = property,
                    DisplayOrder = 0
                };

                _propertyService.InsertPropertyPicture(logoPropertyPicture);
            }
            else
            {
                if (logoPicture != null && logoPicture.Id != logoPropertyPicture.PictureId)
                {
                    logoPropertyPicture.Picture = logoPicture;
                    _propertyService.UpdatePropertyPicture(logoPropertyPicture);
                }
            }

            #endregion

            //图片更新
            SavePropertyPictures(property, propertyCreateModel.Pictures);

            //文件更新
            SavePropertyFiles(property, propertyCreateModel.Files);
            property.ParentPropertyId = parentid;
           

            //保存资产
            _propertyService.UpdateProperty(property);


 

                return Ok(property.ToModel());
                
                
        }

        /// <summary>
        /// 更新出租
        /// </summary>
        /// <param name="rentId"></param>
        /// <param name="propertyRentModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateRent/{rentId:int}")]
        public IHttpActionResult UpdateRent(int rentId, PropertyRentModel propertyRentModel)
        {
            var rent = _propertyRentService.GetPropertyRentById(rentId);
            rent = propertyRentModel.ToEntity(rent);

            #region 图片更新

            var existingPropertyPictures = rent.RentPictures.ToList();
            var rentPicturesToRemove = new List<PropertyRentPicture>();

            //获取要删除的图片
            foreach (var existingPropertyPicture in existingPropertyPictures)
            {

                if (rent.RentPictures.Where(pp => pp.Id == existingPropertyPicture.Id).Count() == 0)
                {
                    rentPicturesToRemove.Add(existingPropertyPicture);
                }
            }

            foreach (var newPropertyPictureModel in rent.RentPictures)
            {
                var rentPicture = new PropertyRentPicture
                {
                    PictureId = newPropertyPictureModel.PictureId,
                    DisplayOrder = 0
                };

                if (newPropertyPictureModel.Id == 0) rent.RentPictures.Add(rentPicture);
            }

            //删除不存在的图片
            foreach (var propertyPicture in rentPicturesToRemove)
            {
                _propertyRentService.DeletePropertyRentPicture(propertyPicture);
            }

            #endregion

            #region 文件更新
            var existingPropertyFiles = rent.RentFiles.ToList();
            var propertyFilesToRemove = new List<PropertyRentFile>();

            //获取要删除的文件
            foreach (var existingPropertyFile in existingPropertyFiles)
            {
                if (rent.RentFiles.Where(pp => pp.Id == existingPropertyFile.Id).Count() == 0)
                {
                    propertyFilesToRemove.Add(existingPropertyFile);
                }
            }

            foreach (var newPropertyFileModel in rent.RentFiles)
            {
                var propertyFile = new PropertyRentFile
                {
                    FileId = newPropertyFileModel.FileId,
                    DisplayOrder = 0
                };
                if (newPropertyFileModel.Id == 0) rent.RentFiles.Add(propertyFile);
            }

            //删除不存在的图片
            foreach (var propertyFile in propertyFilesToRemove)
            {
                _propertyRentService.DeletePropertyRentFile(propertyFile);
            }
            #endregion

            _propertyRentService.UpdatePropertyRent(rent);

            return Ok(rent.ToModel());
        }


        [HttpDelete]
        [Route("{propertyId:int}")]
        public IHttpActionResult DeleteProperty(int propertyId)
        {

            var property = _propertyService.GetPropertyById(propertyId);
            if (property == null || property.Deleted) return NotFound();

            _propertyService.DeleteProperty(property);

            //activity log
            _accountUserActivityService.InsertActivity("Deleteproperty", "删除 名为 {0} 的资产", property.Name);

            var newCreate = property.PropertyNewCreate;
            if(newCreate==null) newCreate = _propertyNewCreateService.GetPropertyNewCreateByPropertyId(property.Id);
            _propertyNewCreateService.DeletePropertyNewCreate(newCreate);            

            //通知
            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));

            return Ok();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="propertyIdString"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{propertyIdString}")]
        public IHttpActionResult Deleteproperty(string propertyIdString)
        {
            var idStringArr = propertyIdString.Split('_');
            foreach (var idStr in idStringArr)
            {
                int id = 0;
                if (!int.TryParse(idStr, out id)) continue;

                var property = _propertyService.GetPropertyById(id);
                if (property == null) continue;

                _propertyService.DeleteProperty(property);

                var newCreate = property.PropertyNewCreate;
                _propertyNewCreateService.DeletePropertyNewCreate(newCreate);
            }

            //活动日志
            _accountUserActivityService.InsertActivity("Deletepropertys", "批量删除 Id为 {0} 的资产", propertyIdString);
           
            //通知
            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));

            return Ok();
        }


        [HttpGet]
        [Route("Governments/{governmentId}")]
        public IHttpActionResult GetPropertiesByGovernment(int governmentId, bool loadChildren = true)
        {
            var governmentIdList = new List<int>();

            if(loadChildren)
            {
                var childrenGovernments = _governmentService.GetGovernmentIdsByParentId(governmentId);
                governmentIdList.AddRange(childrenGovernments);
            }

            governmentIdList.Add(governmentId);

            var properties = _propertyService.GetPropertiesByGovernmentId(governmentIdList).ToList().Select(p =>
            {
                return new SimplePropertyModel
                {
                    Id = p.Id,
                    Name = p.Name
                 //   GovernmentName = p.Government.Name
                };
            });

            return Ok(properties);
        }
        #endregion

        #region 资产处置

        //获取资产巡查列表
        [HttpGet]
        [Route("patrollist")]
        public IHttpActionResult GetPatrolList(int page = 0, int results = int.MaxValue, string sortField = "", string sortOrder = "", string tabKey = "今年巡查")
        {
            var sortConditions = PropertySortCondition.Instance(sortField);
            var allRecords = _propertyPatrolService.GetPatrolListRecords(page, results, sortField, sortOrder, tabKey);
            var response = new ListResponse<PropertyPatrolModel>
            {

                Paging = new Paging
                {
                    PageIndex = page,
                    PageSize = results,
                    Total = allRecords.TotalCount,
                    FilterCount = allRecords.Count,
                },
                Data = allRecords.Select(pcr =>
                {
                    var pcrl = pcr.ToListModel();

                    pcrl.PatrolDate = pcr.PatrolDate.ToString("yyyy/MM/dd");                 
                    pcrl.Property_Id = pcr.Property.Id;
                    return pcrl;
                })
            };

            return Ok(response);

        }


        //获取资产出租列表
        [HttpGet]
        [Route("rentlist")]
        public IHttpActionResult GetRentList(int page = 0, int results = int.MaxValue, string sortField = "", string sortOrder = "",string tabKey="即将过期") {
         
            //初始化排序条件
            var sortConditions = PropertySortCondition.Instance(sortField);

            var allRecords = _propertyRentService.GetRentListRecords(page,results,sortField,sortOrder,tabKey);
            var response = new ListResponse<PropertyRentApproveListModel>
            {

                Paging = new Paging
                {
                    PageIndex = page,
                    PageSize = results,
                    Total = allRecords.TotalCount,
                    FilterCount = allRecords.Count,
                },
                Data = allRecords.Select(pcr =>
                {
                    var pcrl = pcr.ToListModel();

                    pcrl.RentTime = pcr.RentTime.ToString("yyyy/MM/dd") + " - " + pcr.BackTime.ToString("yyyy/MM/dd");
                    pcrl.PriceString = "";
                    var priceList = pcr.PriceString.Split(';');
                    var index = 1;
                    var ysindex = 1;
                    var wsindex = 1;
                    foreach (var price in priceList)
                    {
                        if (price.Contains("true"))
                        {
                            if (ysindex > 2)
                            {
                                pcrl.PriceString += "...";
                                break;
                            }
                            pcrl.PriceString += string.Format("第{0}年租金{1}元;", index, price.Substring(0,price.Length-5));
                            ysindex++;
                        }
                        if (price.Contains("false"))
                        {
                            if (wsindex > 2)
                            {
                                pcrl.UnPriceString += "...";
                                break;
                            }
                            pcrl.UnPriceString += string.Format("第{0}年租金{1}元;", index, price.Substring(0, price.Length - 6));
                            wsindex++;
                        }
                       
                        index++;
                    }

                    pcrl.Property_Id = pcr.Property.Id;
                  //  pcrl.CanApprove = PropertyCanApprove(pcr.State, pcr.SuggestGovernmentId);

                //    pcrl.CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(pcr.State, pcr.SuggestGovernmentId);

                    return pcrl;
                })
            };

            return Ok(response);
        }


        //[AllowAnonymous]
        [HttpGet]
        [Route("PropertyProcess/{name}")]
        public IHttpActionResult GetPropertyProcess(string name="") {

            //var currentAccount = _workContext.CurrentAccountUser;
            var governmentIds = new List<int>();// _governmentService.GetGovernmentIdsByCurrentUser(true);
           
            var properties = _propertyService.GetProcessProperties(name,governmentIds).Select(sp => {
                return new
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Address = sp.Address,
                    Locked=sp.Locked
                };

                //return simplePropertyModel;
            });

            return Ok(properties);
        }
   

        [HttpGet]
        [Route("Process/Records")]
        public IHttpActionResult GetAllApproveRecords(string approveType = "newCreate",string checkState = "", 
            string query = "", string sort = "", int pageSize = Int32.MaxValue, int pageIndex = 0, long time = 0)
        {
            var currentUser = _workContext.CurrentAccountUser;          
            var government = currentUser.Government;

            IList<int> targetGovIds = new List<int>();

            if (currentUser.IsGovAuditor())
            {
                targetGovIds = _governmentService.GetAllGovernmentUnitsByType(
                    new GovernmentType[2] { GovernmentType.Government, GovernmentType.Institution }).Select(g => g.Id).ToList();
            }
            else if (currentUser.IsStateOwnerAuditor())
            {
                targetGovIds = _governmentService.GetAllGovernmentUnitsByType(
                    new GovernmentType[1] { GovernmentType.Company }).Select(g => g.Id).ToList();
            }
            else
            {
                targetGovIds = _governmentService.GetGovernmentIdsByParentId(government.Id, false);
                if (!targetGovIds.Contains(currentUser.Government.Id)) targetGovIds.Add(currentUser.Government.Id);
            }

            //初始化排序条件
            var sortConditions = PropertySortCondition.Instance(sort);

            switch (approveType)
            {
                case "newCreate":
                    {
                        #region 获取新增审批记录
                        var allRecords = _propertyNewCreateService.GetAllNewCreateRecords(targetGovIds, checkState, query, pageIndex,
                    pageSize, sortConditions);
                        var response = new ListResponse<PropertyNewCreateApproveListModel>
                        {
                            Time = time,
                            Paging = new Paging
                            {
                                PageIndex = pageIndex,
                                PageSize = pageSize,
                                Total = allRecords.TotalCount,
                                FilterCount = string.IsNullOrEmpty(query) ? allRecords.TotalCount : allRecords.Count,
                            },
                            Data = allRecords.Select(pcr =>
                            {
                                var pcrl = pcr.ToListModel();
                                pcrl.ApproveType = approveType;
                                pcrl.CanApprove = PropertyCanApprove(pcr.State, pcr.SuggestGovernmentId);

                                pcrl.CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(pcr.State, pcr.SuggestGovernmentId);

                                return pcrl;
                            })
                        };
                      
                        if (currentUser.IsGovAuditor())
                        {
                         
                        }

                        //activity log
                        _accountUserActivityService.InsertActivity("GetAllNewCreateRecords", "获取相关资产新增审批列表信息");

                        return Ok(response);
                        #endregion
                    }
                case "edit":
                    {
                        #region 获取变跟审批记录
                        var allRecords = _propertyEditService.GetAllEditRecords(targetGovIds, checkState, query, pageIndex,
                    pageSize, sortConditions);
                        var response = new ListResponse<PropertyEditApproveListModel>
                        {
                            Time = time,
                            Paging = new Paging
                            {
                                PageIndex = pageIndex,
                                PageSize = pageSize,
                                Total = allRecords.TotalCount,
                                FilterCount = string.IsNullOrEmpty(query) ? allRecords.TotalCount : allRecords.Count,
                            },
                            Data = allRecords.Select(pcr =>
                            {
                                var pcrl = pcr.ToListModel();
                                pcrl.Property_Id = pcr.Property.Id;
                                pcrl.ApproveType = approveType;
                                pcrl.CanApprove = PropertyCanApprove(pcr.State, pcr.SuggestGovernmentId);

                                pcrl.CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(pcr.State, pcr.SuggestGovernmentId);

                                return pcrl;
                            })
                        };

                        //activity log
                        _accountUserActivityService.InsertActivity("GetAllNewCreateRecords", "获取相关资产新增审批列表信息");

                        return Ok(response);
                        #endregion
                    }
                case "lend":
                    {
                        #region 获取出借审批记录
                        var allRecords = _propertyLendService.GetAllLendRecords(targetGovIds, checkState, query, pageIndex,
                    pageSize, sortConditions);
                        var response = new ListResponse<PropertyLendApproveListModel>
                        {
                            Time = time,
                            Paging = new Paging
                            {
                                PageIndex = pageIndex,
                                PageSize = pageSize,
                                Total = allRecords.TotalCount,
                                FilterCount = string.IsNullOrEmpty(query) ? allRecords.TotalCount : allRecords.Count,
                            },
                            Data = allRecords.Select(pcr =>
                            {
                                var pcrl = pcr.ToListModel();
                                pcrl.Property_Id = pcr.Property.Id;
                                pcrl.ApproveType = approveType;
                                
                                pcrl.CanApprove = PropertyCanApprove(pcr.State, pcr.SuggestGovernmentId);

                                pcrl.CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(pcr.State, pcr.SuggestGovernmentId);

                                return pcrl;
                            })
                        };

                        //activity log
                        _accountUserActivityService.InsertActivity("GetAllLendRecords", "获取相关资产出借审批列表信息");

                        return Ok(response);
                        #endregion
                    }
                case "rent":
                    {
                        #region 获取出租审批记录
                        var allRecords = _propertyRentService.GetAllRentRecords(targetGovIds, checkState, query, pageIndex,
                    pageSize, sortConditions);
                        var response = new ListResponse<PropertyRentApproveListModel>
                        {
                            Time = time,
                            Paging = new Paging
                            {
                                PageIndex = pageIndex,
                                PageSize = pageSize,
                                Total = allRecords.TotalCount,
                                FilterCount = string.IsNullOrEmpty(query) ? allRecords.TotalCount : allRecords.Count,
                            },
                            Data = allRecords.Select(pcr =>
                            {
                                var pcrl = pcr.ToListModel();

                                pcrl.RentTime = pcr.RentTime.ToString("yyyy/MM/dd") +" - "+pcr.BackTime.ToString("yyyy/MM/dd");
                                pcrl.PriceString = "";
                                var priceList = pcr.PriceString.Split(';');
                                var index = 1;
                                foreach(var price in priceList)
                                {
                                    if (index > 2) {
                                        pcrl.PriceString += "...";
                                        break;
                                    }
                                    pcrl.PriceString += string.Format("第{0}年租金{1}元;", index, price);
                                    index++;
                                }

                                pcrl.Property_Id = pcr.Property.Id;
                                pcrl.ApproveType = approveType;
                                pcrl.CanApprove = PropertyCanApprove(pcr.State, pcr.SuggestGovernmentId);

                                pcrl.CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(pcr.State, pcr.SuggestGovernmentId);

                                return pcrl;
                            })
                        };

                        //activity log
                        _accountUserActivityService.InsertActivity("GetAllRentRecords", "获取相关资产出租审批列表信息");

                        return Ok(response);
                        #endregion
                    }
                case "allot":
                    {
                        #region 获取划拨审批记录
                        var allRecords = _propertyAllotService.GetAllAllotRecords(targetGovIds, checkState, query, pageIndex,
                    pageSize, sortConditions);
                        var response = new ListResponse<PropertyAllotApproveListModel>
                        {
                            Time = time,
                            Paging = new Paging
                            {
                                PageIndex = pageIndex,
                                PageSize = pageSize,
                                Total = allRecords.TotalCount,
                                FilterCount = string.IsNullOrEmpty(query) ? allRecords.TotalCount : allRecords.Count,
                            },
                            Data = allRecords.Select(pcr =>
                            {
                                var pcrl = pcr.ToListModel();
                                pcrl.Property_Id = pcr.Property.Id;

                                pcrl.ApproveType = approveType;
                                pcrl.CanApprove = PropertyCanApprove(pcr.State, pcr.SuggestGovernmentId);

                                pcrl.CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(pcr.State, pcr.SuggestGovernmentId);

                                return pcrl;
                            })
                        };

                        //activity log
                        _accountUserActivityService.InsertActivity("GetAllAllotRecords", "获取相关资产划拨审批列表信息");

                        return Ok(response);
                        #endregion
                    }
                case "off":
                    {
                        #region 获取核销审批记录
                        var allRecords = _propertyOffService.GetAllOffRecords(targetGovIds, checkState, query, pageIndex,
                     pageSize, sortConditions);
                        var response = new ListResponse<PropertyOffApproveListModel>
                        {
                            Time = time,
                            Paging = new Paging
                            {
                                PageIndex = pageIndex,
                                PageSize = pageSize,
                                Total = allRecords.TotalCount,
                                FilterCount = string.IsNullOrEmpty(query) ? allRecords.TotalCount : allRecords.Count,
                            },
                            Data = allRecords.Select(pcr =>
                            {
                                var pcrl = pcr.ToListModel();
                                pcrl.Property_Id = pcr.Property.Id;

                                pcrl.ApproveType = approveType;
                                pcrl.CanApprove = PropertyCanApprove(pcr.State, pcr.SuggestGovernmentId);

                                pcrl.CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(pcr.State, pcr.SuggestGovernmentId);

                                return pcrl;
                            })
                        };

                        //activity log
                        _accountUserActivityService.InsertActivity("GetAllOffRecords", "获取相关资产核销审批列表信息");

                        return Ok(response);
                        #endregion
                    }
                default:
                    throw new Exception("没有找到相应的处置类型");
            }
        }

        [HttpPost]
        [Route("Lend")]   //Url 的动词名称意义直观 修改则为UpdateLend，审批为ApproveLend
        public IHttpActionResult CreateLendRecord(PropertyLendModel propertyLendModel)
        {
            //var currentUser = _workContext.CurrentAccountUser;
            //var suggestGovernmentId = currentUser.Government.Id;
            if (propertyLendModel.Ids == "") throw new Exception("请选择需要处置的资产");
            var lendIds = propertyLendModel.Ids.Split(';');
            foreach (var id in lendIds) {
                if (id != "") {
                    var property = _propertyService.GetPropertyById(Convert.ToInt32(id));
                    if (property == null) continue;  //防止错误的PropertyId传入
                    if (property.Locked || property.Deleted || !property.Published)
                        throw new Exception("无法对该资产进行操作或该资产已经不存在");

                    if (!PropertyBelongCurrentUser(property, true)) throw new Exception("没有操作权限");

                    PropertyLend propertyLendRecord = new PropertyLend();
                    propertyLendRecord.Name = propertyLendModel.Name;
                    propertyLendRecord.Title = property.Name;
                    propertyLendRecord.LendArea = propertyLendModel.LendArea;
                    propertyLendRecord.LendTime = Convert.ToDateTime(propertyLendModel.LendTime);
                    propertyLendRecord.BackTime = DateTime.MaxValue;// Convert.ToDateTime(propertyLendModel.BackTime);
                    propertyLendRecord.Property = property;
                    propertyLendRecord.State = propertyLendModel.Submit ? PropertyApproveState.DepartmentApprove : PropertyApproveState.Start;
                 
                    propertyLendRecord.ProcessDate = DateTime.Now;
                    propertyLendRecord.SuggestGovernmentId = 0;// suggestGovernmentId; temp
               
                    //如果当前用户是主管部门，则跳过主管部门审核环节
                    if (propertyLendModel.Submit && _workContext.CurrentAccountUser.IsParentGovernmentorAuditor())
                    {
                        propertyLendRecord.State = PropertyApproveState.AdminApprove;
                        propertyLendRecord.DSuggestion = "同意";
                        propertyLendRecord.DApproveDate = DateTime.Now;
                    }

                    _propertyLendService.InsertPropertyLend(propertyLendRecord);
                    //activity log
                    _accountUserActivityService.InsertActivity("AddNewpropertyLendRecord", "增加 名为 {0} 的资产出借申请", property.Name);

                    #region 附件处理

                    //图片更新
                    SavePropertyLendPictures(propertyLendRecord, propertyLendModel.LendPictures);

                    //文件更新
                    SavePropertyLendFiles(propertyLendRecord, propertyLendModel.LendFiles);

                    _propertyLendService.UpdatePropertyLend(propertyLendRecord);

                    #endregion

                    SwitchPropertyLockState(propertyLendModel.Submit, property);
                }             
            }

            return Ok();
        }

        [HttpPost]
        [Route("Rent")]
        public IHttpActionResult CreateRentRecord(PropertyRentModel propertyRentModel)
        {
            if (propertyRentModel.Ids == "") throw new Exception("请选择需要处置的资产");
            var rentIds = propertyRentModel.Ids.Split(';');
            rentIds= rentIds.Where(s => !string.IsNullOrEmpty(s)).ToArray();  //删除空要素
            var currentUser = _workContext.CurrentAccountUser;
            foreach (var id in rentIds)
            {
                if (id != "")
                {
                    var property = _propertyService.GetPropertyById(Convert.ToInt32(id));
                    if (property == null) continue;  //防止错误的PropertyId传入
                    if (property.Locked || property.Deleted || !property.Published)
                        throw new Exception("无法对该资产进行操作或该资产已经不存在");

                    //if (!PropertyBelongCurrentUser(property, true)) throw new Exception("没有操作权限");  temp

                    PropertyRent rent = new PropertyRent();
                    rent.Name = propertyRentModel.Name;
                    rent.Title = property.Name;
                    rent.RentArea = propertyRentModel.RentArea;
                    rent.RentMonth = propertyRentModel.RentMonth;
                    if (propertyRentModel.PriceString.EndsWith(";")) rent.PriceString = propertyRentModel.PriceString.TrimEnd(';');
                    rent.RentPrice = propertyRentModel.RentPrice;
                    rent.RentTime = Convert.ToDateTime(propertyRentModel.RentTime);
                    rent.BackTime = Convert.ToDateTime(propertyRentModel.BackTime);

                    if (rent.BackTime <= rent.RentTime) throw new Exception("出租时间不能晚于或等于归还时间");

                    rent.Property = property;
                    rent.State = propertyRentModel.Submit ? PropertyApproveState.DepartmentApprove : PropertyApproveState.Start;
                    rent.ProcessDate = DateTime.Now;
                    rent.SuggestGovernmentId = 0;// currentUser.Government.Id; temp

                    //如果当前用户是主管部门，则跳过主管部门审核环节
                    //if (propertyRentModel.Submit && _workContext.CurrentAccountUser.IsParentGovernmentorAuditor())
                    //{
                    rent.State = PropertyApproveState.AdminApprove;
                    rent.DSuggestion = "同意";
                    rent.DApproveDate = DateTime.Now;
                    //}

                    _propertyRentService.InsertPropertyRent(rent);
                    _accountUserActivityService.InsertActivity("AddNewpropertyLendRecord", "增加 名为 {0} 的资产出借申请", property.Name);

                    #region 附件处理

                    //图片更新
                    SavePropertyRentPictures(rent, propertyRentModel.RentPictures);

                    //文件更新
                    SavePropertyRentFiles(rent, propertyRentModel.RentFiles);
                    _propertyRentService.UpdatePropertyRent(rent);
                    #endregion

                    //
                    SwitchPropertyLockState(true, property);

                }

            }
            return Ok();
        }

        [HttpPost]
        [Route("Allot")]
        public IHttpActionResult CreateAllotRecord(PropertyAllotModel propertyAllotModel)
        {
            if (propertyAllotModel.Ids == "") throw new Exception("请选择需要处置的资产");
            var AllotIds = propertyAllotModel.Ids.Split(';');
            var currentUser = _workContext.CurrentAccountUser;
            var prePropertyOwner = currentUser.Government.Name;
            foreach (var id in AllotIds)
            {
                if (id != "")
                {
                    var property = _propertyService.GetPropertyById(Convert.ToInt32(id));
                 
                    if (property == null) continue;  //防止错误的PropertyId传入
                    if (property.Locked || property.Deleted || !property.Published)
                        throw new Exception("无法对该资产进行操作或该资产已经不存在");

                    if (!PropertyBelongCurrentUser(property, true)) throw new Exception("没有操作权限");

                    var nowGovernmentOwner = _governmentService.GetGovernmentUnitByName(propertyAllotModel.NowPropertyOwner).Name;
                    if (nowGovernmentOwner == null) throw new Exception("目标产权单位无效");

                    PropertyAllot allot = new PropertyAllot();
                    allot.PrePropertyOwner = prePropertyOwner;
                    allot.NowPropertyOwner = propertyAllotModel.NowPropertyOwner;
                    allot.NowGovernmentId = _governmentService.GetGovernmentUnitByName(propertyAllotModel.NowPropertyOwner).Id;
                    allot.Title = property.Name;
                    allot.AllotTime =Convert.ToDateTime( propertyAllotModel.AllotTime);
                    allot.Property = property;
                    allot.State = propertyAllotModel.Submit ? PropertyApproveState.DepartmentApprove : PropertyApproveState.Start;
                    allot.ProcessDate = DateTime.Now;
                    allot.SuggestGovernmentId = currentUser.Government.Id;

                    //如果当前用户是主管部门，则跳过主管部门审核环节
                    if (propertyAllotModel.Submit && _workContext.CurrentAccountUser.IsParentGovernmentorAuditor())
                    {
                        allot.State = PropertyApproveState.AdminApprove;
                        allot.DSuggestion = "同意";
                        allot.DApproveDate = DateTime.Now;
                    }

                    _propertyAllotService.InsertPropertyAllot(allot);
                    _accountUserActivityService.InsertActivity("AddNewpropertyLendRecord", "增加 名为 {0} 的资产出借申请", property.Name);


                    SavePropertyAllotPictures(allot, propertyAllotModel.AllotPictures);
                    SavePropertyAllotFiles(allot, propertyAllotModel.AllotFiles);

                    _propertyAllotService.UpdatePropertyAllot(allot);

                    SwitchPropertyLockState(true, property);
                }
            }
            return Ok();
        }

        [HttpPost]
        [Route("Off")]
        public IHttpActionResult CreateOffRecord(PropertyOffModel propertyOffModel)
        {
            if (propertyOffModel.Ids == "") throw new Exception("请选择需要处置的资产");
            var offIds = propertyOffModel.Ids.Split(';');
            var currentUser = _workContext.CurrentAccountUser;
            var sid = 0;// currentUser.Government.Id;
            foreach (var id in offIds)
            {
                if (id != "")
                {
                    var property = _propertyService.GetPropertyById(Convert.ToInt32(id));

                    if (property == null) continue;  //防止错误的PropertyId传入
                    if (property.Locked || property.Deleted || !property.Published)
                        throw new Exception("无法对该资产进行操作或该资产已经不存在");

                    //if (!PropertyBelongCurrentUser(property, true)) throw new Exception("没有操作权限");  temp

                    PropertyOff off = new PropertyOff();
                    off.Reason = propertyOffModel.Reason;
                    off.OffTime = Convert.ToDateTime(propertyOffModel.OffTime);
                    off.Price = propertyOffModel.Price;
                    off.Title = property.Name;
                    off.Property = property;
                    
                    off.SuggestGovernmentId = sid;
                    off.State = propertyOffModel.Submit ? PropertyApproveState.DepartmentApprove : PropertyApproveState.Start;
                    off.ProcessDate = DateTime.Now;
                    switch (propertyOffModel.OffType) {
                        case "0":
                            off.OffType = OffType.Auction;
                            break;
                        case "1":
                            off.OffType = OffType.Remove;
                            break;
                        case "2":
                            off.OffType = OffType.Storeup;
                            break;
                    }



                    //如果当前用户是主管部门，则跳过主管部门审核环节
                    //if (propertyOffModel.Submit && _workContext.CurrentAccountUser.IsParentGovernmentorAuditor())
                    //{
                        off.State = PropertyApproveState.AdminApprove;
                        off.DSuggestion = "同意";
                        off.DApproveDate = DateTime.Now;
                    //}

                    _propertyOffService.InsertPropertyOff(off);
                    _accountUserActivityService.InsertActivity("AddNewpropertyLendRecord", "增加 名为 {0} 的资产出借申请", property.Name);

                    #region 附件处理

                    //图片更新
                    SavePropertyOffPictures(off, propertyOffModel.OffPictures);

                    //文件更新
                    SavePropertyOffFiles(off, propertyOffModel.OffFiles);

                    _propertyOffService.UpdatePropertyOff(off);
                    #endregion

                    SwitchPropertyLockState(true, property);
                }
            }

            return Ok();
        }

        [HttpPut]
        [Route("UpdateLend/{id}")]   
        public IHttpActionResult UpdateLendRecord(int id,PropertyLendModel propertyLendModel)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var lend = _propertyLendService.GetPropertyLendById(id);
            
            if (!PropertyApproveCanEditDeleteAndSubmit(lend.State, lend.SuggestGovernmentId)) throw new Exception("该项目已无法编辑");

            lend = propertyLendModel.ToEntity(lend);

            #region 附件处理

            //图片更新
            SavePropertyLendPictures(lend, propertyLendModel.LendPictures);

            //文件更新
            SavePropertyLendFiles(lend, propertyLendModel.LendFiles);

            #endregion


            if (propertyLendModel.Submit)
            {
                lend.State = PropertyApproveState.DepartmentApprove;

                //如果当前用户是主管部门，则跳过主管部门审核环节
                if (currentUser.IsParentGovernmentorAuditor())
                {
                    lend.State = PropertyApproveState.AdminApprove;
                    lend.DSuggestion = "同意";
                    lend.DApproveDate = DateTime.Now;
                }
            }

                _propertyLendService.UpdatePropertyLend(lend);

            SwitchPropertyLockState(true , lend.Property);  //锁定资产

            return Ok();
        }

        [HttpPut]
        [Route("UpdateRent1111/{id}")]
        public IHttpActionResult UpdateRentRecord(int id,PropertyRentModel propertyRentModel)
        {

            var currentUser = _workContext.CurrentAccountUser;
            var rent = _propertyRentService.GetPropertyRentById(id);
            if (!PropertyApproveCanEditDeleteAndSubmit(rent.State, rent.SuggestGovernmentId)) throw new Exception("该项目已无法编辑");

              rent = propertyRentModel.ToEntity(rent);

            if (rent.BackTime <= rent.RentTime) throw new Exception("出租时间不能晚于或等于归还时间");

            #region 附件处理

            //图片更新
            SavePropertyRentPictures(rent, propertyRentModel.RentPictures);

            //文件更新
            SavePropertyRentFiles(rent, propertyRentModel.RentFiles);

            #endregion

            if (propertyRentModel.Submit)
            {
                rent.State = PropertyApproveState.DepartmentApprove;

                //如果当前用户是主管部门，则跳过主管部门审核环节
                if (currentUser.IsParentGovernmentorAuditor())
                {
                    rent.State = PropertyApproveState.AdminApprove;
                    rent.DSuggestion = "同意";
                    rent.DApproveDate = DateTime.Now;
                }
            }
            else {
                rent.State = PropertyApproveState.Start;
            }

        _propertyRentService.UpdatePropertyRent(rent);

            SwitchPropertyLockState(true, rent.Property);

            return Ok();
        }

        [HttpPut]
        [Route("UpdateAllot/{id}")]
        public IHttpActionResult UpdateAllotRecord(int id,PropertyAllotModel propertyAllotModel)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var allot = _propertyAllotService.GetPropertyAllotById(id);
            if (!PropertyApproveCanEditDeleteAndSubmit(allot.State, allot.SuggestGovernmentId)) throw new Exception("该项目已无法编辑");
            allot = propertyAllotModel.ToEntity(allot);
            allot.NowGovernmentId = _governmentService.GetGovernmentUnitByName(propertyAllotModel.NowPropertyOwner).Id;
            SavePropertyAllotPictures(allot, propertyAllotModel.AllotPictures);
            SavePropertyAllotFiles(allot, propertyAllotModel.AllotFiles);

            if (propertyAllotModel.Submit)
            {
                allot.State = PropertyApproveState.DepartmentApprove;

                //如果当前用户是主管部门，则跳过主管部门审核环节
                if (currentUser.IsParentGovernmentorAuditor())
                {
                    allot.State = PropertyApproveState.AdminApprove;
                    allot.DSuggestion = "同意";
                    allot.DApproveDate = DateTime.Now;
                }
            }

            else {
                allot.State = PropertyApproveState.Start;
            }

            _propertyAllotService.UpdatePropertyAllot(allot);

            SwitchPropertyLockState(true, allot.Property);

            return Ok();
        }

        [HttpPut]
        [Route("UpdateOff/{id}")]
        public IHttpActionResult UpdateOffRecord(int id,PropertyOffModel propertyOffModel)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var off = _propertyOffService.GetPropertyOffById(id);

            if (!PropertyApproveCanEditDeleteAndSubmit(off.State, off.SuggestGovernmentId)) throw new Exception("该项目已无法编辑");
            off = propertyOffModel.ToEntity(off);
            switch (propertyOffModel.OffType)
            {
                case "0":
                    off.OffType = OffType.Auction;
                    break;
                case "1":
                    off.OffType = OffType.Remove;
                    break;
                case "2":
                    off.OffType = OffType.Storeup;
                    break;
            }

            #region 附件处理

            //图片更新
            SavePropertyOffPictures(off, propertyOffModel.OffPictures);

            //文件更新
            SavePropertyOffFiles(off, propertyOffModel.OffFiles);

            #endregion

            if (propertyOffModel.Submit)
            {
                off.State = PropertyApproveState.DepartmentApprove;

                //如果当前用户是主管部门，则跳过主管部门审核环节
                if (currentUser.IsParentGovernmentorAuditor())
                {
                    off.State = PropertyApproveState.AdminApprove;
                    off.DSuggestion = "同意";
                    off.DApproveDate = DateTime.Now;
                }
            }

            _propertyOffService.UpdatePropertyOff(off);

            SwitchPropertyLockState(true, off.Property);
            return Ok();
        }

        [HttpGet]
        [Route("NewCreate/{id}")]
        public IHttpActionResult GetNewCreate(int id)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var government = currentUser.Government;

            var newCreate = _propertyNewCreateService.GetPropertyNewCreateById(id);
            if (newCreate == null || newCreate.Deleted) throw new Exception("无法找到资源");

            var property = _propertyService.GetPropertyById(newCreate.Property_Id);

            //if (!PropertyBelongCurrentUser(property, false)) throw new Exception("没有操作权限");

            var response = new PropertyNewCreateApproveModel
            {
                Property = newCreate.Property.ToModel(),
                PropertyNewCreate = newCreate.ToModel(),
                CanApprove = PropertyCanApprove(newCreate.State,newCreate.SuggestGovernmentId),
                CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(newCreate.State, newCreate.SuggestGovernmentId),
                LinkMan = newCreate.Property.Government.Person,
                LinkTel = newCreate.Property.Government.Tel
            };

            #region 原数据库对象
            response.Property.LogoUrl = GetLogoUrl(newCreate.Property);
            //获取图片
            PreparePropertyPictures(response.Property);
            //获取文件
            PreparePropertyFiles(response.Property);
            #endregion            
 
            return Ok(response);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IHttpActionResult GetEdit(int id)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var government = currentUser.Government;

            var edit = _propertyEditService.GetPropertyEditById(id);
            if (edit == null || edit.Deleted) throw new Exception("无法找到资源");

            var property = _propertyService.GetPropertyById(edit.Property.Id);
            //if (!PropertyBelongCurrentUser(property, false)) throw new Exception("没有操作权限");

            var copyProperty = _copyPropertyService.GetCopyPropertyById(edit.CopyProperty_Id);
            var govermentName = _governmentService.GetGovernmentUnitById(copyProperty.Government_Id).Name;

            var response = new PropertyEditApproveModel
            {
                CopyProperty = copyProperty.ToModel(),               
                PropertyEdit = edit.ToModel(),
                CanApprove = PropertyCanApprove(edit.State, edit.SuggestGovernmentId),
                CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(edit.State, edit.SuggestGovernmentId),
                LinkMan = edit.Property.Government.Person,
                LinkTel = edit.Property.Government.Tel
            };
            response.CopyProperty.GovernmentName = govermentName;            

            #region 新数据库对象
            #region 获取图片
            response.CopyProperty.Pictures = new List<PropertyPictureModel>();
            foreach (var pid in copyProperty.PrictureIds.Split('_'))
            {
                if (string.IsNullOrWhiteSpace(pid)) continue;
                var picture = _pictureService.GetPictureById(Convert.ToInt32(pid));
                if (picture == null) continue;

                var propertyPictureModel = new PropertyPictureModel
                {
                    PictureId = picture.Id,
                    PropertyId = copyProperty.Id,
                    Href = _pictureService.GetPictureUrl(picture.Id),
                    Title = picture.TitleAttribute
                };

                response.CopyProperty.Pictures.Add(propertyPictureModel);
            }
            #endregion

            #region 文件更新
            response.CopyProperty.Files = new List<PropertyFileModel>();
            foreach (var fid in copyProperty.FileIds.Split('_'))
            {
                if (string.IsNullOrWhiteSpace(fid)) continue;
                var file = _fileService.GetFileById(Convert.ToInt32(fid));
                if (file == null) continue;

                var propertyFileModel = new PropertyFileModel
                {
                    FileId = file.Id,
                    PropertyId = copyProperty.Id,
                    Src = _fileService.GetFileUrl(file),
                    Title = file.Name
                };

                response.CopyProperty.Files.Add(propertyFileModel);
            }

            #endregion
            var logoUrl = _pictureService.GetPictureUrl(copyProperty.LogoPicture_Id);
            response.CopyProperty.LogoUrl = string.IsNullOrEmpty(logoUrl) ? null : logoUrl;
            #endregion

            if(edit.State== PropertyApproveState.Finish && edit.OriginCopyProperty_Id!=0)
            {
                var originCopyProperty = _copyPropertyService.GetCopyPropertyById(edit.OriginCopyProperty_Id);
                var originGovermentName = _governmentService.GetGovernmentUnitById(originCopyProperty.Government_Id).Name;

                response.Property = originCopyProperty.ToModel();
                response.Property.GovernmentName = originGovermentName;

                #region 备份的原数据库对象
                #region 获取图片
                response.Property.Pictures = new List<PropertyPictureModel>();
                foreach (var pid in originCopyProperty.PrictureIds.Split('_'))
                {
                    if (string.IsNullOrWhiteSpace(pid)) continue;
                    var picture = _pictureService.GetPictureById(Convert.ToInt32(pid));
                    if (picture == null) continue;

                    var propertyPictureModel = new PropertyPictureModel
                    {
                        PictureId = picture.Id,
                        PropertyId = originCopyProperty.Id,
                        Href = _pictureService.GetPictureUrl(picture.Id),
                        Title = picture.TitleAttribute
                    };

                    response.Property.Pictures.Add(propertyPictureModel);
                }
                #endregion

                #region 文件更新
                response.Property.Files = new List<PropertyFileModel>();
                foreach (var fid in originCopyProperty.FileIds.Split('_'))
                {
                    if (string.IsNullOrWhiteSpace(fid)) continue;
                    var file = _fileService.GetFileById(Convert.ToInt32(fid));
                    if (file == null) continue;

                    var propertyFileModel = new PropertyFileModel
                    {
                        FileId = file.Id,
                        PropertyId = originCopyProperty.Id,
                        Src = _fileService.GetFileUrl(file),
                        Title = file.Name
                    };

                    response.Property.Files.Add(propertyFileModel);
                }

                #endregion
                var originLogoUrl = _pictureService.GetPictureUrl(originCopyProperty.LogoPicture_Id);
                response.Property.LogoUrl = string.IsNullOrEmpty(originLogoUrl) ? null : originLogoUrl;
                #endregion
            }
            else
            {
                response.Property = edit.Property.ToModel();

                #region 原数据库对象
                response.Property.LogoUrl = GetLogoUrl(edit.Property);
                //获取图片
                PreparePropertyPictures(response.Property);
                //获取文件
                PreparePropertyFiles(response.Property);
                #endregion
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("Lend/{id}")]
        public IHttpActionResult GetLendDetail(int id) {

            var currentUser = _workContext.CurrentAccountUser;
            var government = currentUser.Government;

            var lend = _propertyLendService.GetPropertyLendById(id);
            if (lend == null || lend.Deleted) throw new Exception("无法找到资源");

            //if (!PropertyBelongCurrentUser(lend.Property, false)) throw new Exception("没有操作权限");

            var response = new PropertyLendApproveModel
            {
                Property = lend.Property.ToBasicModel(),
                PropertyLend = lend.ToModel(),
                CanApprove = PropertyCanApprove(lend.State,lend.SuggestGovernmentId),
                CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(lend.State, lend.SuggestGovernmentId),
                LinkMan = lend.Property.Government.Person,
                LinkTel = lend.Property.Government.Tel
            };

            PreparePropertyLendPicturesAndFiles(response.PropertyLend);

            return Ok(response);
        }

        [HttpGet]
        [Route("Rent/{id}")]
        public IHttpActionResult GetRentDetail(int id)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var government = currentUser.Government;

            var rent = _propertyRentService.GetPropertyRentById(id);
            if (rent == null || rent.Deleted) throw new Exception("无法找到资源");

            //if (!PropertyBelongCurrentUser(rent.Property, false)) throw new Exception("没有操作权限");

            var response = new PropertyRentApproveModel
            {
                Property = rent.Property.ToBasicModel(),
                PropertyRent = rent.ToModel(),
                CanApprove = PropertyCanApprove(rent.State,rent.SuggestGovernmentId),
                CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(rent.State, rent.SuggestGovernmentId),
                LinkMan = rent.Property.Government.Person,
                LinkTel = rent.Property.Government.Tel
            };

            response.PropertyRent.Valid = DateTime.Now >= rent.RentTime && DateTime.Now < rent.BackTime;

            if (response.PropertyRent.PriceString.EndsWith(";")) response.PropertyRent.PriceString = response.PropertyRent.PriceString.TrimEnd(';');            

            PreparePropertyRentPicturesAndFiles(response.PropertyRent);

            return Ok(response);
        }

        [HttpGet]
        [Route("Allot/{id}")]
        public IHttpActionResult GetAllotDetail(int id)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var government = currentUser.Government;

            var allot = _propertyAllotService.GetPropertyAllotById(id);
            if (allot == null || allot.Deleted) throw new Exception("无法找到资源");

            //if (!PropertyBelongCurrentUser(allot.Property, false)) throw new Exception("没有操作权限");

            var response = new PropertyAllotApproveModel
            {
                Property = allot.Property.ToBasicModel(),
                PropertyAllot = allot.ToModel(),
                CanApprove = PropertyCanApprove(allot.State,allot.SuggestGovernmentId),
                CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(allot.State, allot.SuggestGovernmentId),
                LinkMan = allot.Property.Government.Person,
                LinkTel = allot.Property.Government.Tel
            };
            PreparePropertyAllotPicturesAndFiles(response.PropertyAllot);
            return Ok(response);
        }

        [HttpGet]
        [Route("Off/{id}")]
        public IHttpActionResult GetOffDetail(int id)
        {
            var currentUser = _workContext.CurrentAccountUser;
            var government = currentUser.Government;

            var off = _propertyOffService.GetPropertyOffById(id);
            if (off == null || off.Deleted) throw new Exception("无法找到资源");
            //if (!PropertyBelongCurrentUser(off.Property, false)) throw new Exception("没有操作权限");

            var response = new PropertyOffApproveModel
            {
                Property = off.Property.ToBasicModel(),
                PropertyOff = off.ToModel(),
                CanApprove = PropertyCanApprove(off.State,off.SuggestGovernmentId),
                CanEditAndDelete = PropertyApproveCanEditDeleteAndSubmit(off.State, off.SuggestGovernmentId),
                LinkMan = off.Property.Government.Person,
                LinkTel = off.Property.Government.Tel
            };
            PreparePropertyOffPicturesAndFiles(response.PropertyOff);
            return Ok(response);
        }

        /// <summary>
        /// 删除资产处置申请，除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approveType"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Approve/{id}")]
        public IHttpActionResult DeleteApprove(int id, string approveType)
        {
            if (string.IsNullOrEmpty(approveType)) throw new Exception("找不到要处理的类型");

            switch (approveType)
            {
                case "edit":
                    #region 变更审批
                    {
                        var edit = _propertyEditService.GetPropertyEditById(id);
                        if (edit == null) return NotFound();
                        
                        if (!PropertyApproveCanEditDeleteAndSubmit(edit.State, edit.SuggestGovernmentId)) throw new Exception("无法删除");

                        _propertyEditService.DeletePropertyEdit(edit);

                        var copyProperty = _copyPropertyService.GetCopyPropertyByPropertyId(edit.Property.Id);
                        if (copyProperty != null)
                        {
                            copyProperty.Published = true;
                            _copyPropertyService.UpdateCopyProperty(copyProperty);
                        }

                        //活动日志
                        _accountUserActivityService.InsertActivity("deleteEditApprove", string.Format("删除id为 {0} 的资产变更的处置", id));

                        //解除锁定
                        edit.Property.Locked = false;
                        _propertyService.UpdateProperty(edit.Property);

                        break;
                    }
                    #endregion
                case "lend":
                    #region 出借审批
                    {
                        var lend = _propertyLendService.GetPropertyLendById(id);
                        if (lend == null) return NotFound();
                        if (!PropertyApproveCanEditDeleteAndSubmit(lend.State, lend.SuggestGovernmentId)) throw new Exception("无法删除");
                        _propertyLendService.DeletePropertyLend(lend);

                        //活动日志
                        _accountUserActivityService.InsertActivity("deleteLendApprove", string.Format("删除id为 {0} 的资产出借的处置", id));

                        //解除锁定
                        lend.Property.Locked = false;
                        _propertyService.UpdateProperty(lend.Property);

                        break;
                    }
                #endregion
                case "rent":
                    #region 出租审批
                    {
                        var rent = _propertyRentService.GetPropertyRentById(id);
                        if (rent == null) return NotFound();
                        if (!PropertyApproveCanEditDeleteAndSubmit(rent.State, rent.SuggestGovernmentId)) throw new Exception("无法删除");
                        _propertyRentService.DeletePropertyRent(rent);

                        //活动日志
                        _accountUserActivityService.InsertActivity("deleteLendApprove", string.Format("删除id为 {0} 的资产出租的处置", id));
                        //解除锁定
                        rent.Property.Locked = false;
                        _propertyService.UpdateProperty(rent.Property);
                        break;
                    }
                #endregion
                case "allot":
                    #region 划拨审批
                    {
                        var allot = _propertyAllotService.GetPropertyAllotById(id);
                        if (allot == null) return NotFound();
                        if (!PropertyApproveCanEditDeleteAndSubmit(allot.State, allot.SuggestGovernmentId)) throw new Exception("无法删除");
                        _propertyAllotService.DeletePropertyAllot(allot);

                        //活动日志
                        _accountUserActivityService.InsertActivity("deleteLendApprove", string.Format("删除id为 {0} 的资产划拨的处置", id));
                        //解除锁定
                        allot.Property.Locked = false;
                        _propertyService.UpdateProperty(allot.Property);
                        break;
                    }
                #endregion
                case "off":
                    #region 核销审批
                    {
                        var off = _propertyOffService.GetPropertyOffById(id);
                        var property = off.Property;

                        if (off == null) return NotFound();
                        if (!PropertyApproveCanEditDeleteAndSubmit(off.State, off.SuggestGovernmentId)) throw new Exception("无法删除");
                        _propertyOffService.DeletePropertyOff(off);

                        //活动日志
                        _accountUserActivityService.InsertActivity("deleteLendApprove", string.Format("删除id为 {0} 的资产核销的处置", id));
                        //解除锁定
                        property.Locked = false;
                        _propertyService.UpdateProperty(property);
                        break;
                    }
                    #endregion
            }

            return Ok();
        }

        /// <summary>
        /// 提交审批申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approveApplyModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SubmitApprove/{id}")]
        public IHttpActionResult SumbitApprove(int id,string approveType="")
        {
          
            var currentUser = _workContext.CurrentAccountUser;

            switch (approveType)
            {
                case "newCreate":
                    #region 新增审批
                    {
                        var newCreate = _propertyNewCreateService.GetPropertyNewCreateById(id);
                        if (newCreate == null || newCreate.Deleted) throw new Exception("找不到资源");

                        if (PropertyApproveCanEditDeleteAndSubmit(newCreate.State, newCreate.SuggestGovernmentId))
                        {
                            newCreate.State = PropertyApproveState.DepartmentApprove;

                            if (currentUser.IsParentGovernmentorAuditor())  //当前用户既是主管部门又是报送部门   // if (currentUser.Government.ParentGovernmentId == 0)
                            {
                                newCreate.State = PropertyApproveState.AdminApprove;
                                newCreate.DApproveDate = DateTime.Now;
                                newCreate.DSuggestion = "同意";
                            }

                            _propertyNewCreateService.UpdatePropertyNewCreate(newCreate);

                            //活动日志
                            _accountUserActivityService.InsertActivity("submitNewCreateApprove", string.Format("提交id为 {0} 的新增资产的处置申请", id));

                            SwitchPropertyLockState(true, newCreate.Property);
                        }

                        break;
                    }
                #endregion
                case "edit":
                    #region 编辑审批
                    {
                        var edit = _propertyEditService.GetPropertyEditById(id);
                        if (edit == null || edit.Deleted) throw new Exception("找不到资源");

                        if (PropertyApproveCanEditDeleteAndSubmit(edit.State, edit.SuggestGovernmentId))
                        {
                            edit.State = PropertyApproveState.DepartmentApprove;

                            if (currentUser.IsParentGovernmentorAuditor())
                            {
                                edit.State = PropertyApproveState.AdminApprove;
                                edit.DApproveDate = DateTime.Now;
                                edit.DSuggestion = "同意";
                            }

                            _propertyEditService.UpdatePropertyEdit(edit);

                            //活动日志
                            _accountUserActivityService.InsertActivity("submitNewCreateApprove", string.Format("提交id为 {0} 的新增资产的处置申请", id));
                        }

                        break;
                    }
                #endregion
                case "lend":
                    #region 出借审批
                    {
                        var lend = _propertyLendService.GetPropertyLendById(id);
                        if (lend == null || lend.Deleted) throw new Exception("找不到资源");

                        if (PropertyApproveCanEditDeleteAndSubmit(lend.State, lend.SuggestGovernmentId))
                        {
                            lend.State = PropertyApproveState.DepartmentApprove;
                            if (currentUser.IsParentGovernmentorAuditor())
                            {
                                lend.State = PropertyApproveState.AdminApprove;
                                lend.DApproveDate = DateTime.Now;
                                lend.DSuggestion = "同意";
                            }

                            _propertyLendService.UpdatePropertyLend(lend);
                            //活动日志
                            _accountUserActivityService.InsertActivity("submitLendApprove", string.Format("提交id为 {0} 的资产出借的处置申请", id));
                        }

                        break;
                    }
                #endregion
                case "rent":
                    #region 出租审批
                    {
                        var rent = _propertyRentService.GetPropertyRentById(id);
                        if (rent == null || rent.Deleted) throw new Exception("找不到资源");

                        if (PropertyApproveCanEditDeleteAndSubmit(rent.State, rent.SuggestGovernmentId))
                        {
                            rent.State = PropertyApproveState.DepartmentApprove;
                            if (currentUser.IsParentGovernmentorAuditor())
                            {
                                rent.State = PropertyApproveState.AdminApprove;
                                rent.DApproveDate = DateTime.Now;
                                rent.DSuggestion = "同意";
                            }

                            _propertyRentService.UpdatePropertyRent(rent);

                            //活动日志
                            _accountUserActivityService.InsertActivity("submitRentApprove", string.Format("提交id为 {0} 的新增资产的处置申请", id));
                        }

                        break;
                    }
                #endregion
                case "allot":
                    #region 划拨审批
                    {
                        var allot = _propertyAllotService.GetPropertyAllotById(id);
                        if (allot == null || allot.Deleted) throw new Exception("找不到资源");

                        if (PropertyApproveCanEditDeleteAndSubmit(allot.State, allot.SuggestGovernmentId))
                        {
                            allot.State = PropertyApproveState.DepartmentApprove;
                            if (currentUser.IsParentGovernmentorAuditor())
                            {
                                allot.State = PropertyApproveState.AdminApprove;
                                allot.DApproveDate = DateTime.Now;
                                allot.DSuggestion = "同意";
                            }
                            _propertyAllotService.UpdatePropertyAllot(allot);

                            //活动日志
                            _accountUserActivityService.InsertActivity("submitAllotApprove", string.Format("提交id为 {0} 的资产划拨的处置申请", id));
                        }

                        break;
                    }
                #endregion
                case "off":
                    #region 核销审批
                    {
                        var off = _propertyOffService.GetPropertyOffById(id);
                        if (off == null || off.Deleted) throw new Exception("找不到资源");

                        if (PropertyApproveCanEditDeleteAndSubmit(off.State, off.SuggestGovernmentId))
                        {
                            off.State = PropertyApproveState.DepartmentApprove;
                            if (currentUser.IsParentGovernmentorAuditor())
                            {
                                off.State = PropertyApproveState.AdminApprove;
                                off.DApproveDate = DateTime.Now;
                                off.DSuggestion = "同意";
                            }
                            _propertyOffService.UpdatePropertyOff(off);

                            //活动日志
                            _accountUserActivityService.InsertActivity("submitOffApprove", string.Format("提交id为 {0} 的资产核销的处置申请", id));
                        }

                        break;
                    }
                    #endregion
            }
            return Ok();
        }
         
        /// <summary>
        /// 批量提交申请
        /// </summary>
        /// <param name="idsString"></param>
        /// <param name="approveType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SubmitApprove/Multi/{idsString}")]
        public IHttpActionResult Multi_SumbitApprove(string idsString, string approveType = "")
        {
            var result = new StringBuilder();
            var currentUser = _workContext.CurrentAccountUser;

            var idArr = idsString.Split(';');

            foreach(var idString in idArr)
            {
                int id = 0;
                if (int.TryParse(idString, out id))
                {
                    try
                    {
                        #region 处理
                        switch (approveType)
                        {
                            case "newCreate":
                                #region 新增审批
                                {
                                    var newCreate = _propertyNewCreateService.GetPropertyNewCreateById(id);
                                    if (newCreate == null || newCreate.Deleted) throw new Exception(string.Format("找不到id为 {0} 新增资产处置申请"));

                                    if (PropertyApproveCanEditDeleteAndSubmit(newCreate.State, newCreate.SuggestGovernmentId))
                                    {
                                        newCreate.State = PropertyApproveState.DepartmentApprove;

                                        if (currentUser.IsParentGovernmentorAuditor())  //当前用户既是主管部门又是报送部门   // if (currentUser.Government.ParentGovernmentId == 0)
                                        {
                                            newCreate.State = PropertyApproveState.AdminApprove;
                                            newCreate.DApproveDate = DateTime.Now;
                                            newCreate.DSuggestion = "同意";
                                        }

                                        _propertyNewCreateService.UpdatePropertyNewCreate(newCreate);

                                        //活动日志
                                        _accountUserActivityService.InsertActivity("submitNewCreateApprove", string.Format("提交id为 {0} 的新增资产的处置申请", id));

                                        SwitchPropertyLockState(true, newCreate.Property);
                                    }

                                    break;
                                }
                            #endregion
                            case "edit":
                                #region 编辑审批
                                {
                                    var edit = _propertyEditService.GetPropertyEditById(id);
                                    if (edit == null || edit.Deleted) throw new Exception(string.Format("找不到id为 {0} 资产变更处置申请"));

                                    if (PropertyApproveCanEditDeleteAndSubmit(edit.State, edit.SuggestGovernmentId))
                                    {
                                        edit.State = PropertyApproveState.DepartmentApprove;

                                        if (currentUser.IsParentGovernmentorAuditor())
                                        {
                                            edit.State = PropertyApproveState.AdminApprove;
                                            edit.DApproveDate = DateTime.Now;
                                            edit.DSuggestion = "同意";
                                        }

                                        _propertyEditService.UpdatePropertyEdit(edit);

                                        //活动日志
                                        _accountUserActivityService.InsertActivity("submitNewCreateApprove", string.Format("提交id为 {0} 的资产变更的处置申请", id));
                                    }

                                    break;
                                }
                            #endregion
                            case "lend":
                                #region 出借审批
                                {
                                    var lend = _propertyLendService.GetPropertyLendById(id);
                                    if (lend == null || lend.Deleted) throw new Exception(string.Format("找不到id为 {0} 资产出借处置申请"));

                                    if (PropertyApproveCanEditDeleteAndSubmit(lend.State, lend.SuggestGovernmentId))
                                    {
                                        lend.State = PropertyApproveState.DepartmentApprove;
                                        if (currentUser.IsParentGovernmentorAuditor())
                                        {
                                            lend.State = PropertyApproveState.AdminApprove;
                                            lend.DApproveDate = DateTime.Now;
                                            lend.DSuggestion = "同意";
                                        }

                                        _propertyLendService.UpdatePropertyLend(lend);
                                        //活动日志
                                        _accountUserActivityService.InsertActivity("submitLendApprove", string.Format("提交id为 {0} 的资产出借的处置申请", id));
                                    }

                                    break;
                                }
                            #endregion
                            case "rent":
                                #region 出租审批
                                {
                                    var rent = _propertyRentService.GetPropertyRentById(id);
                                    if (rent == null || rent.Deleted) throw new Exception(string.Format("找不到id为 {0} 资产出租处置申请"));

                                    if (PropertyApproveCanEditDeleteAndSubmit(rent.State, rent.SuggestGovernmentId))
                                    {
                                        rent.State = PropertyApproveState.DepartmentApprove;
                                        if (currentUser.IsParentGovernmentorAuditor())
                                        {
                                            rent.State = PropertyApproveState.AdminApprove;
                                            rent.DApproveDate = DateTime.Now;
                                            rent.DSuggestion = "同意";
                                        }

                                        _propertyRentService.UpdatePropertyRent(rent);

                                        //活动日志
                                        _accountUserActivityService.InsertActivity("submitRentApprove", string.Format("提交id为 {0} 的资产出租的处置申请", id));
                                    }

                                    break;
                                }
                            #endregion
                            case "allot":
                                #region 划拨审批
                                {
                                    var allot = _propertyAllotService.GetPropertyAllotById(id);
                                    if (allot == null || allot.Deleted) throw new Exception(string.Format("找不到id为 {0} 资产划拨处置申请"));

                                    if (PropertyApproveCanEditDeleteAndSubmit(allot.State, allot.SuggestGovernmentId))
                                    {
                                        allot.State = PropertyApproveState.DepartmentApprove;
                                        if (currentUser.IsParentGovernmentorAuditor())
                                        {
                                            allot.State = PropertyApproveState.AdminApprove;
                                            allot.DApproveDate = DateTime.Now;
                                            allot.DSuggestion = "同意";
                                        }
                                        _propertyAllotService.UpdatePropertyAllot(allot);

                                        //活动日志
                                        _accountUserActivityService.InsertActivity("submitAllotApprove", string.Format("提交id为 {0} 的资产划拨的处置申请", id));
                                    }

                                    break;
                                }
                            #endregion
                            case "off":
                                #region 核销审批
                                {
                                    var off = _propertyOffService.GetPropertyOffById(id);
                                    if (off == null || off.Deleted) throw new Exception(string.Format("找不到id为 {0} 资产核销处置申请"));

                                    if (PropertyApproveCanEditDeleteAndSubmit(off.State, off.SuggestGovernmentId))
                                    {
                                        off.State = PropertyApproveState.DepartmentApprove;
                                        if (currentUser.IsParentGovernmentorAuditor())
                                        {
                                            off.State = PropertyApproveState.AdminApprove;
                                            off.DApproveDate = DateTime.Now;
                                            off.DSuggestion = "同意";
                                        }
                                        _propertyOffService.UpdatePropertyOff(off);

                                        //活动日志
                                        _accountUserActivityService.InsertActivity("submitOffApprove", string.Format("提交id为 {0} 的资产核销的处置申请", id));
                                    }

                                    break;
                                }
                                #endregion
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        result.AppendLine(string.Format("id 为 {0} 的资产处置未提交成功，错误原因为：{1}", id, e.Message));
                    }
                }
            }
            
            return Ok(result.ToString());
        }

        /// <summary>
        /// 审批处置申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approveApplyModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("ApplyApprove/{id}")]
        public IHttpActionResult ApplyApprove(int id, ApproveApplyModel approveApplyModel)
        {
            bool agree = approveApplyModel.Agree;
            string suggestion = approveApplyModel.Suggestion;
            string approveType = approveApplyModel.ApproveType;

            if (agree && string.IsNullOrEmpty(suggestion))
                suggestion = "同意";

            switch (approveType)
            {
                case "newCreate":
                    #region 新增审批
                    {
                        var newCreate = _propertyNewCreateService.GetPropertyNewCreateById(id);
                        if (newCreate == null || newCreate.Deleted) throw new Exception("找不到资源");

                        if (!PropertyCanApprove(newCreate.State,newCreate.SuggestGovernmentId)) throw new Exception("没有审批权限");

                        if (newCreate.State == PropertyApproveState.DepartmentApprove)
                        {
                            newCreate.DApproveDate = DateTime.Now;
                            newCreate.DSuggestion = suggestion;
                        }
                        else if (newCreate.State == PropertyApproveState.AdminApprove)
                        {
                            newCreate.AApproveDate = DateTime.Now;
                            newCreate.ASuggestion = suggestion;
                        }                      

                        if (agree)
                        {

                            if (newCreate.State == PropertyApproveState.Start)
                            {
                                newCreate.State = PropertyApproveState.DepartmentApprove;
                            }
                            else if (newCreate.State == PropertyApproveState.DepartmentApprove) //主管部门审核阶段
                            {
                                newCreate.State = PropertyApproveState.AdminApprove;
                            }
                            else if (newCreate.State == PropertyApproveState.AdminApprove)
                            {
                                newCreate.State = PropertyApproveState.Finish;
                                newCreate.Property.Published = true;
                                SwitchPropertyLockState(false, newCreate.Property);

                                //    SwitchPropertyLockState(false, newCreate.Property);
                            }
                        }
                        else
                        {
                            newCreate.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                        }

                        _propertyNewCreateService.UpdatePropertyNewCreate(newCreate);

                        //活动日志
                        _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的新增资产的处置", id));

                        break;
                    }
                #endregion
                case "edit":
                    #region 编辑审批
                    {
                        var edit = _propertyEditService.GetPropertyEditById(id);
                        if (edit == null || edit.Deleted) throw new Exception("找不到资源");
                        if (!PropertyCanApprove(edit.State, edit.SuggestGovernmentId)) throw new Exception("没有审批权限");
                        if (edit.State == PropertyApproveState.DepartmentApprove)
                        {
                            edit.DApproveDate = DateTime.Now;
                            edit.DSuggestion = suggestion;
                        }
                        else if (edit.State == PropertyApproveState.AdminApprove)
                        {
                            edit.AApproveDate = DateTime.Now;
                            edit.ASuggestion = suggestion;
                        }

                        if (agree)
                        {

                            if (edit.State == PropertyApproveState.Start)
                            {
                                edit.State = PropertyApproveState.DepartmentApprove;
                            }
                            else if (edit.State == PropertyApproveState.DepartmentApprove) //主管部门审核阶段
                            {
                                edit.State = PropertyApproveState.AdminApprove;
                            }
                            else if (edit.State == PropertyApproveState.AdminApprove)
                            {
                                var property = _propertyService.GetPropertyById(edit.Property.Id);
                                var copyproperty = _copyPropertyService.GetCopyPropertyById(edit.CopyProperty_Id);

                                #region 资产原值存储
                                var originCopyProperty = new CopyProperty();
                                originCopyProperty.Name = property.Name;
                                originCopyProperty.PropertyType = property.PropertyType;
                                originCopyProperty.Region = property.Region;
                                originCopyProperty.Address = property.Address;
                                originCopyProperty.ConstructArea = property.ConstructArea;
                                originCopyProperty.LandArea = property.LandArea;
                                originCopyProperty.PropertyID = property.PropertyID;
                              //  originCopyProperty.HasConstructID = property.HasConstructID;
                              //  originCopyProperty.HasLandID = property.HasLandID;
                             //   originCopyProperty.PropertyNature = property.PropertyNature;
                             //   originCopyProperty.LandNature = property.LandNature;
                             //   originCopyProperty.Price = property.Price;
                                originCopyProperty.GetedDate = property.GetedDate;
                             //   originCopyProperty.LifeTime = property.LifeTime;
                                originCopyProperty.UsedPeople = property.UsedPeople;
                             //   originCopyProperty.CurrentUse_Self = property.CurrentUse_Self;
                             //   originCopyProperty.CurrentUse_Rent = property.CurrentUse_Rent;
                              //  originCopyProperty.CurrentUse_Lend = property.CurrentUse_Lend;
                              //  originCopyProperty.CurrentUse_Idle = property.CurrentUse_Idle;
                             //   originCopyProperty.NextStepUsage = property.NextStepUsage;
                                originCopyProperty.Location = property.Location == null ? "" : property.Location.AsText();
                                originCopyProperty.Extent = property.Extent == null ? "" : property.Extent.AsText();
                                originCopyProperty.Description = property.Description;
                                originCopyProperty.EstateId = property.EstateId;
                                originCopyProperty.ConstructId = property.ConstructId;
                                originCopyProperty.LandId = property.LandId;
                                originCopyProperty.Government_Id = property.Government.Id;

                                originCopyProperty.PrictureIds = string.Join("_", property.Pictures.Select(p => p.PictureId).ToArray());
                                originCopyProperty.FileIds = string.Join("_", property.Files.Select(p => p.FileId).ToArray());
                                var originPropertyLogoPicture = property.Pictures.Where(pp => pp.IsLogo).FirstOrDefault();
                                if (originPropertyLogoPicture != null) originCopyProperty.LogoPicture_Id = originPropertyLogoPicture.PictureId;

                                _copyPropertyService.InsertCopyProperty(originCopyProperty);
                                #endregion

                                #region 资产赋新值                           
                                property.Name = copyproperty.Name;
                                property.PropertyType = copyproperty.PropertyType;
                                property.Region = copyproperty.Region;
                                property.Address = copyproperty.Address;
                                property.ConstructArea = copyproperty.ConstructArea;
                                property.LandArea = copyproperty.LandArea;
                                property.PropertyID = copyproperty.PropertyID;
                            //    property.HasConstructID = copyproperty.HasConstructID;
                            //    property.HasLandID = copyproperty.HasLandID;
                             //   property.PropertyNature = copyproperty.PropertyNature;
                             //   property.LandNature = copyproperty.LandNature;
                             //   property.Price = copyproperty.Price;
                                property.GetedDate = copyproperty.GetedDate;
                            //    property.LifeTime = copyproperty.LifeTime;
                                property.UsedPeople = copyproperty.UsedPeople;
                          //      property.CurrentUse_Self = copyproperty.CurrentUse_Self;
                            //    property.CurrentUse_Rent = copyproperty.CurrentUse_Rent;
                           //     property.CurrentUse_Lend = copyproperty.CurrentUse_Lend;
                           //     property.CurrentUse_Idle = copyproperty.CurrentUse_Idle;
                            //   property.NextStepUsage = copyproperty.NextStepUsage;
                                if (!string.IsNullOrEmpty(copyproperty.Location))
                                    property.Location = DbGeography.FromText(copyproperty.Location);
                                else throw new Exception("空间位置未赋值");
                                if (!string.IsNullOrEmpty(copyproperty.Extent))
                                    property.Extent = DbGeography.FromText(copyproperty.Extent);
                                property.Description = copyproperty.Description;
                                property.EstateId = copyproperty.EstateId;
                                property.ConstructId = copyproperty.ConstructId;
                                property.LandId = copyproperty.LandId;
                                if (property.Government.Id != copyproperty.Government_Id)
                                    property.Government = _governmentService.GetGovernmentUnitById(copyproperty.Government_Id);

                                #region 图片更新
                                var propertyPictureModels = new List<PropertyPictureModel>();

                                foreach (var pid in copyproperty.PrictureIds.Split('_'))
                                {
                                    if (string.IsNullOrWhiteSpace(pid)) continue;
                                    var picture = _pictureService.GetPictureById(Convert.ToInt32(pid));

                                    if (picture == null) continue;

                                    var propertyPictureModel = new PropertyPictureModel
                                    {
                                        PictureId = picture.Id,
                                        PropertyId = copyproperty.Id
                                    };

                                    propertyPictureModels.Add(propertyPictureModel);
                                }
                                //图片更新
                                SavePropertyPictures(property, propertyPictureModels);
                                #endregion

                                #region logo更新
                                var logoPicture = _pictureService.GetPictureById(copyproperty.LogoPicture_Id);
                                if (logoPicture != null)
                                {
                                    var propertyLogoPicture = property.Pictures.Where(p => p.IsLogo).SingleOrDefault();

                                    if (propertyLogoPicture != null)
                                    {
                                        if (propertyLogoPicture.PictureId != copyproperty.LogoPicture_Id)
                                        {
                                            propertyLogoPicture.Picture = logoPicture;
                                            _propertyService.UpdatePropertyPicture(propertyLogoPicture);
                                        }
                                    }
                                    else
                                    {
                                        propertyLogoPicture = new PropertyPicture
                                        {
                                            Property = property,
                                            IsLogo = true,
                                            Picture = logoPicture
                                        };

                                        property.Pictures.Add(propertyLogoPicture);
                                    }
                                }
                                #endregion

                                #region 文件更新
                                var propertyFileModels = new List<PropertyFileModel>();
                                foreach (var fid in copyproperty.FileIds.Split('_'))
                                {
                                    if (string.IsNullOrWhiteSpace(fid)) continue;
                                    var file = _pictureService.GetPictureById(Convert.ToInt32(fid));
                                    if (file == null) continue;

                                    var propertyFileModel = new PropertyFileModel
                                    {
                                        FileId = file.Id,
                                        PropertyId = copyproperty.Id
                                    };

                                    propertyFileModels.Add(propertyFileModel);
                                }

                                SavePropertyFiles(property, propertyFileModels);
                                #endregion

                                #endregion

                                edit.State = PropertyApproveState.Finish;
                                edit.OriginCopyProperty_Id = originCopyProperty.Id;  //历史Property记录
                                property.Published = true;
                                copyproperty.Published = true;

                                SwitchPropertyLockState(false, property);
                                _propertyService.UpdateProperty(property);
                                _copyPropertyService.UpdateCopyProperty(copyproperty);
                            }
                        }
                        else
                        {
                            edit.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                        }

                        _propertyEditService.UpdatePropertyEdit(edit);

                        //活动日志
                        _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的新增资产的处置", id));

                        break;
                    }
                #endregion
                case "lend":
                    #region 出借审批
                    {
                        var lend = _propertyLendService.GetPropertyLendById(id);
                        if (lend == null || lend.Deleted) throw new Exception("找不到资源");
                        if (!PropertyCanApprove(lend.State, lend.SuggestGovernmentId)) throw new Exception("没有审批权限");
                        if (lend.State == PropertyApproveState.DepartmentApprove)
                        {
                            lend.DApproveDate = DateTime.Now;
                            lend.DSuggestion = suggestion;
                        }
                        else if (lend.State == PropertyApproveState.AdminApprove)
                        {
                            lend.AApproveDate = DateTime.Now;
                            lend.ASuggestion = suggestion;
                        }

                        if (agree)
                        {
                            if (lend.State == PropertyApproveState.Start)
                            {
                                lend.State = PropertyApproveState.DepartmentApprove;
                            }
                            else if (lend.State == PropertyApproveState.DepartmentApprove)                             //主管部门审核阶段
                            {
                                lend.State = PropertyApproveState.AdminApprove;
                            }
                            else if (lend.State == PropertyApproveState.AdminApprove)
                            {
                                lend.State = PropertyApproveState.Finish;
                                SwitchPropertyLockState(false, lend.Property);
                            }
                        }
                        else
                        {
                            lend.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                        }

                        _propertyLendService.UpdatePropertyLend(lend);

                        //活动日志
                        _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产出借的处置", id));

                        break;
                    }
                #endregion
                case "rent":
                    #region 出租审批
                    {
                        var rent = _propertyRentService.GetPropertyRentById(id);
                        if (rent == null || rent.Deleted) throw new Exception("找不到资源");
                        if (!PropertyCanApprove(rent.State, rent.SuggestGovernmentId)) throw new Exception("没有审批权限");
                        if (rent.State == PropertyApproveState.DepartmentApprove)
                        {
                            rent.DApproveDate = DateTime.Now;
                            rent.DSuggestion = suggestion;
                        }
                        else if (rent.State == PropertyApproveState.AdminApprove)
                        {
                            rent.AApproveDate = DateTime.Now;
                            rent.ASuggestion = suggestion;
                        }

                        if (agree)
                        {
                            //主管部门审核阶段
                            if (rent.State == PropertyApproveState.Start)
                            {
                                rent.State = PropertyApproveState.DepartmentApprove;
                            }
                            else if (rent.State == PropertyApproveState.DepartmentApprove)
                            {
                                rent.State = PropertyApproveState.AdminApprove;
                            }
                            else if (rent.State == PropertyApproveState.AdminApprove)
                            {
                                rent.State = PropertyApproveState.Finish;
                                SwitchPropertyLockState(false, rent.Property);
                            }
                        }
                        else
                        {
                            rent.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                        }

                        _propertyRentService.UpdatePropertyRent(rent);

                        //活动日志
                        _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产出租的处置", id));

                        break;
                    }
                #endregion
                case "allot":
                    #region 划拨审批
                    {
                        var allot = _propertyAllotService.GetPropertyAllotById(id);
                        if (allot == null || allot.Deleted) throw new Exception("找不到资源");
                        if (!PropertyCanApprove(allot.State, allot.SuggestGovernmentId)) throw new Exception("没有审批权限");
                        if (allot.State == PropertyApproveState.DepartmentApprove)
                        {
                            allot.DApproveDate = DateTime.Now;
                            allot.DSuggestion = suggestion;
                        }
                        else if (allot.State == PropertyApproveState.AdminApprove)
                        {
                            allot.AApproveDate = DateTime.Now;
                            allot.ASuggestion = suggestion;
                        }

                        if (agree)
                        {
                            //主管部门审核阶段
                            if (allot.State == PropertyApproveState.Start)
                            {
                                allot.State = PropertyApproveState.DepartmentApprove;
                            }
                            else if (allot.State == PropertyApproveState.DepartmentApprove)
                            {
                                allot.State = PropertyApproveState.AdminApprove;
                            }
                            else if (allot.State == PropertyApproveState.AdminApprove)
                            {
                                allot.State = PropertyApproveState.Finish;
                                var newGovernment = _governmentService.GetGovernmentUnitById(allot.NowGovernmentId);
                                if (newGovernment == null) throw new Exception("找不到新的权属单位");
                                allot.Property.Government = newGovernment;

                                SwitchPropertyLockState(false, allot.Property);
                                
                            }
                        }
                        else
                        {
                            allot.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                        }

                        _propertyAllotService.UpdatePropertyAllot(allot);

                        //活动日志
                        _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产的划拨处置", id));

                        break;
                    }
                #endregion
                case "off":
                    #region 划拨审批
                    {
                        var off = _propertyOffService.GetPropertyOffById(id);
                        if (off == null || off.Deleted) throw new Exception("找不到资源");
                        if (!PropertyCanApprove(off.State, off.SuggestGovernmentId)) throw new Exception("没有审批权限");
                        if (off.State == PropertyApproveState.Start)
                        {
                            off.State = PropertyApproveState.DepartmentApprove;
                        }
                        else if (off.State == PropertyApproveState.DepartmentApprove)
                        {
                            off.DApproveDate = DateTime.Now;
                            off.DSuggestion = suggestion;
                        }
                        else if (off.State == PropertyApproveState.AdminApprove)
                        {
                            off.AApproveDate = DateTime.Now;
                            off.ASuggestion = suggestion;
                        }

                        if (agree)
                        {
                            //主管部门审核阶段
                            if (off.State == PropertyApproveState.DepartmentApprove)
                            {
                                off.State = PropertyApproveState.AdminApprove;
                            }
                            else if (off.State == PropertyApproveState.AdminApprove)
                            {
                                off.State = PropertyApproveState.Finish;
                                off.Property.Off = true;
                                off.Property.Published = false;
                                SwitchPropertyLockState(false, off.Property);
                            }
                        }
                        else
                        {
                            off.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                        }

                        _propertyOffService.UpdatePropertyOff(off);

                        //活动日志
                        _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产核销处置", id));

                        break;
                    }
                    #endregion
            }
            return Ok();
        }

        /// <summary>
        /// 批量审批
        /// </summary>
        /// <param name="idsString"></param>
        /// <param name="approveApplyModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("ApplyApprove/Multi/{idsString}")]
        public IHttpActionResult Multi_ApplyApprove(string idsString, ApproveApplyModel approveApplyModel)
        {
            var result = new StringBuilder();
            var currentUser = _workContext.CurrentAccountUser;

            var idArr = idsString.Split(';');

            bool agree = approveApplyModel.Agree;
            string suggestion = approveApplyModel.Suggestion;
            string approveType = approveApplyModel.ApproveType;

            if (agree && string.IsNullOrEmpty(suggestion))
                suggestion = "同意";

            foreach (var idString in idArr)
            {
                int id = 0;
                if (int.TryParse(idString, out id))
                {
                    try
                    {
                        #region 处理
                        switch (approveType)
                        {
                            case "newCreate":
                                #region 新增审批
                                {
                                    var newCreate = _propertyNewCreateService.GetPropertyNewCreateById(id);
                                    if (newCreate == null || newCreate.Deleted) throw new Exception("找不到资源");

                                    if (!PropertyCanApprove(newCreate.State, newCreate.SuggestGovernmentId)) throw new Exception("没有审批权限");

                                    if (newCreate.State == PropertyApproveState.DepartmentApprove)
                                    {
                                        newCreate.DApproveDate = DateTime.Now;
                                        newCreate.DSuggestion = suggestion;
                                    }
                                    else if (newCreate.State == PropertyApproveState.AdminApprove)
                                    {
                                        newCreate.AApproveDate = DateTime.Now;
                                        newCreate.ASuggestion = suggestion;
                                    }

                                    if (agree)
                                    {

                                        if (newCreate.State == PropertyApproveState.Start)
                                        {
                                            newCreate.State = PropertyApproveState.DepartmentApprove;
                                        }
                                        else if (newCreate.State == PropertyApproveState.DepartmentApprove) //主管部门审核阶段
                                        {
                                            newCreate.State = PropertyApproveState.AdminApprove;
                                        }
                                        else if (newCreate.State == PropertyApproveState.AdminApprove)
                                        {
                                            newCreate.State = PropertyApproveState.Finish;
                                            newCreate.Property.Published = true;
                                            SwitchPropertyLockState(false, newCreate.Property);

                                            //    SwitchPropertyLockState(false, newCreate.Property);
                                        }
                                    }
                                    else
                                    {
                                        newCreate.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                                    }

                                    _propertyNewCreateService.UpdatePropertyNewCreate(newCreate);

                                    //活动日志
                                    _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的新增资产的处置", id));

                                    break;
                                }
                            #endregion
                            case "edit":
                                #region 编辑审批
                                {
                                    var edit = _propertyEditService.GetPropertyEditById(id);
                                    if (edit == null || edit.Deleted) throw new Exception("找不到资源");
                                    if (!PropertyCanApprove(edit.State, edit.SuggestGovernmentId)) throw new Exception("没有审批权限");
                                    if (edit.State == PropertyApproveState.DepartmentApprove)
                                    {
                                        edit.DApproveDate = DateTime.Now;
                                        edit.DSuggestion = suggestion;
                                    }
                                    else if (edit.State == PropertyApproveState.AdminApprove)
                                    {
                                        edit.AApproveDate = DateTime.Now;
                                        edit.ASuggestion = suggestion;
                                    }

                                    if (agree)
                                    {

                                        if (edit.State == PropertyApproveState.Start)
                                        {
                                            edit.State = PropertyApproveState.DepartmentApprove;
                                        }
                                        else if (edit.State == PropertyApproveState.DepartmentApprove) //主管部门审核阶段
                                        {
                                            edit.State = PropertyApproveState.AdminApprove;
                                        }
                                        else if (edit.State == PropertyApproveState.AdminApprove)
                                        {
                                            var property = _propertyService.GetPropertyById(edit.Property.Id);
                                            var copyproperty = _copyPropertyService.GetCopyPropertyById(edit.CopyProperty_Id);

                                            #region 资产原值存储
                                            var originCopyProperty = new CopyProperty();
                                            originCopyProperty.Name = property.Name;
                                            originCopyProperty.PropertyType = property.PropertyType;
                                            originCopyProperty.Region = property.Region;
                                            originCopyProperty.Address = property.Address;
                                            originCopyProperty.ConstructArea = property.ConstructArea;
                                            originCopyProperty.LandArea = property.LandArea;
                                            originCopyProperty.PropertyID = property.PropertyID;
                                          //  originCopyProperty.HasConstructID = property.HasConstructID;
                                        //    originCopyProperty.HasLandID = property.HasLandID;
                                         //   originCopyProperty.PropertyNature = property.PropertyNature;
                                        //    originCopyProperty.LandNature = property.LandNature;
                                         //   originCopyProperty.Price = property.Price;
                                            originCopyProperty.GetedDate = property.GetedDate;
                                         //   originCopyProperty.LifeTime = property.LifeTime;
                                            originCopyProperty.UsedPeople = property.UsedPeople;
                                        //    originCopyProperty.CurrentUse_Self = property.CurrentUse_Self;
                                        //    originCopyProperty.CurrentUse_Rent = property.CurrentUse_Rent;
                                         //   originCopyProperty.CurrentUse_Lend = property.CurrentUse_Lend;
                                        //    originCopyProperty.CurrentUse_Idle = property.CurrentUse_Idle;
                                       //     originCopyProperty.NextStepUsage = property.NextStepUsage;
                                            originCopyProperty.Location = property.Location == null ? "" : property.Location.AsText();
                                            originCopyProperty.Extent = property.Extent == null ? "" : property.Extent.AsText();
                                            originCopyProperty.Description = property.Description;
                                            originCopyProperty.EstateId = property.EstateId;
                                            originCopyProperty.ConstructId = property.ConstructId;
                                            originCopyProperty.LandId = property.LandId;
                                            originCopyProperty.Government_Id = property.Government.Id;

                                            originCopyProperty.PrictureIds = string.Join("_", property.Pictures.Select(p => p.PictureId).ToArray());
                                            originCopyProperty.FileIds = string.Join("_", property.Files.Select(p => p.FileId).ToArray());
                                            var originPropertyLogoPicture = property.Pictures.Where(pp => pp.IsLogo).FirstOrDefault();
                                            if (originPropertyLogoPicture != null) originCopyProperty.LogoPicture_Id = originPropertyLogoPicture.PictureId;

                                            _copyPropertyService.InsertCopyProperty(originCopyProperty);
                                            #endregion

                                            #region 资产赋新值                           
                                            property.Name = copyproperty.Name;
                                            property.PropertyType = copyproperty.PropertyType;
                                            property.Region = copyproperty.Region;
                                            property.Address = copyproperty.Address;
                                            property.ConstructArea = copyproperty.ConstructArea;
                                            property.LandArea = copyproperty.LandArea;
                                            property.PropertyID = copyproperty.PropertyID;
                                        //    property.HasConstructID = copyproperty.HasConstructID;
                                        //    property.HasLandID = copyproperty.HasLandID;
                                         //   property.PropertyNature = copyproperty.PropertyNature;
                                        //    property.LandNature = copyproperty.LandNature;
                                         //   property.Price = copyproperty.Price;
                                            property.GetedDate = copyproperty.GetedDate;
                                       //     property.LifeTime = copyproperty.LifeTime;
                                            property.UsedPeople = copyproperty.UsedPeople;
                                       //     property.CurrentUse_Self = copyproperty.CurrentUse_Self;
                                        //    property.CurrentUse_Rent = copyproperty.CurrentUse_Rent;
                                        //    property.CurrentUse_Lend = copyproperty.CurrentUse_Lend;
                                        //    property.CurrentUse_Idle = copyproperty.CurrentUse_Idle;
                                       //     property.NextStepUsage = copyproperty.NextStepUsage;
                                            if (!string.IsNullOrEmpty(copyproperty.Location))
                                                property.Location = DbGeography.FromText(copyproperty.Location);
                                            else throw new Exception("空间位置未赋值");
                                            if (!string.IsNullOrEmpty(copyproperty.Extent))
                                                property.Extent = DbGeography.FromText(copyproperty.Extent);
                                            property.Description = copyproperty.Description;
                                            property.EstateId = copyproperty.EstateId;
                                            property.ConstructId = copyproperty.ConstructId;
                                            property.LandId = copyproperty.LandId;
                                            if (property.Government.Id != copyproperty.Government_Id)
                                                property.Government = _governmentService.GetGovernmentUnitById(copyproperty.Government_Id);

                                            #region 图片更新
                                            var propertyPictureModels = new List<PropertyPictureModel>();

                                            foreach (var pid in copyproperty.PrictureIds.Split('_'))
                                            {
                                                if (string.IsNullOrWhiteSpace(pid)) continue;
                                                var picture = _pictureService.GetPictureById(Convert.ToInt32(pid));

                                                if (picture == null) continue;

                                                var propertyPictureModel = new PropertyPictureModel
                                                {
                                                    PictureId = picture.Id,
                                                    PropertyId = copyproperty.Id
                                                };

                                                propertyPictureModels.Add(propertyPictureModel);
                                            }
                                            //图片更新
                                            SavePropertyPictures(property, propertyPictureModels);
                                            #endregion

                                            #region logo更新
                                            var logoPicture = _pictureService.GetPictureById(copyproperty.LogoPicture_Id);
                                            if (logoPicture != null)
                                            {
                                                var propertyLogoPicture = property.Pictures.Where(p => p.IsLogo).SingleOrDefault();

                                                if (propertyLogoPicture != null)
                                                {
                                                    if (propertyLogoPicture.PictureId != copyproperty.LogoPicture_Id)
                                                    {
                                                        propertyLogoPicture.Picture = logoPicture;
                                                        _propertyService.UpdatePropertyPicture(propertyLogoPicture);
                                                    }
                                                }
                                                else
                                                {
                                                    propertyLogoPicture = new PropertyPicture
                                                    {
                                                        Property = property,
                                                        IsLogo = true,
                                                        Picture = logoPicture
                                                    };

                                                    property.Pictures.Add(propertyLogoPicture);
                                                }
                                            }
                                            #endregion

                                            #region 文件更新
                                            var propertyFileModels = new List<PropertyFileModel>();
                                            foreach (var fid in copyproperty.FileIds.Split('_'))
                                            {
                                                if (string.IsNullOrWhiteSpace(fid)) continue;
                                                var file = _pictureService.GetPictureById(Convert.ToInt32(fid));
                                                if (file == null) continue;

                                                var propertyFileModel = new PropertyFileModel
                                                {
                                                    FileId = file.Id,
                                                    PropertyId = copyproperty.Id
                                                };

                                                propertyFileModels.Add(propertyFileModel);
                                            }

                                            SavePropertyFiles(property, propertyFileModels);
                                            #endregion

                                            #endregion

                                            edit.State = PropertyApproveState.Finish;
                                            edit.OriginCopyProperty_Id = originCopyProperty.Id;  //历史Property记录
                                            property.Published = true;
                                            copyproperty.Published = true;

                                            SwitchPropertyLockState(false, property);
                                            _propertyService.UpdateProperty(property);
                                            _copyPropertyService.UpdateCopyProperty(copyproperty);
                                        }
                                    }
                                    else
                                    {
                                        edit.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                                    }

                                    _propertyEditService.UpdatePropertyEdit(edit);

                                    //活动日志
                                    _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的新增资产的处置", id));

                                    break;
                                }
                            #endregion
                            case "lend":
                                #region 出借审批
                                {
                                    var lend = _propertyLendService.GetPropertyLendById(id);
                                    if (lend == null || lend.Deleted) throw new Exception("找不到资源");
                                    if (!PropertyCanApprove(lend.State, lend.SuggestGovernmentId)) throw new Exception("没有审批权限");
                                    if (lend.State == PropertyApproveState.DepartmentApprove)
                                    {
                                        lend.DApproveDate = DateTime.Now;
                                        lend.DSuggestion = suggestion;
                                    }
                                    else if (lend.State == PropertyApproveState.AdminApprove)
                                    {
                                        lend.AApproveDate = DateTime.Now;
                                        lend.ASuggestion = suggestion;
                                    }

                                    if (agree)
                                    {
                                        if (lend.State == PropertyApproveState.Start)
                                        {
                                            lend.State = PropertyApproveState.DepartmentApprove;
                                        }
                                        else if (lend.State == PropertyApproveState.DepartmentApprove)                             //主管部门审核阶段
                                        {
                                            lend.State = PropertyApproveState.AdminApprove;
                                        }
                                        else if (lend.State == PropertyApproveState.AdminApprove)
                                        {
                                            lend.State = PropertyApproveState.Finish;
                                            SwitchPropertyLockState(false, lend.Property);
                                        }
                                    }
                                    else
                                    {
                                        lend.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                                    }

                                    _propertyLendService.UpdatePropertyLend(lend);

                                    //活动日志
                                    _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产出借的处置", id));

                                    break;
                                }
                            #endregion
                            case "rent":
                                #region 出租审批
                                {
                                    var rent = _propertyRentService.GetPropertyRentById(id);
                                    if (rent == null || rent.Deleted) throw new Exception("找不到资源");
                                    if (!PropertyCanApprove(rent.State, rent.SuggestGovernmentId)) throw new Exception("没有审批权限");
                                    if (rent.State == PropertyApproveState.DepartmentApprove)
                                    {
                                        rent.DApproveDate = DateTime.Now;
                                        rent.DSuggestion = suggestion;
                                    }
                                    else if (rent.State == PropertyApproveState.AdminApprove)
                                    {
                                        rent.AApproveDate = DateTime.Now;
                                        rent.ASuggestion = suggestion;
                                    }

                                    if (agree)
                                    {
                                        //主管部门审核阶段
                                        if (rent.State == PropertyApproveState.Start)
                                        {
                                            rent.State = PropertyApproveState.DepartmentApprove;
                                        }
                                        else if (rent.State == PropertyApproveState.DepartmentApprove)
                                        {
                                            rent.State = PropertyApproveState.AdminApprove;
                                        }
                                        else if (rent.State == PropertyApproveState.AdminApprove)
                                        {
                                            rent.State = PropertyApproveState.Finish;
                                            SwitchPropertyLockState(false, rent.Property);
                                        }
                                    }
                                    else
                                    {
                                        rent.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                                    }

                                    _propertyRentService.UpdatePropertyRent(rent);

                                    //活动日志
                                    _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产出租的处置", id));

                                    break;
                                }
                            #endregion
                            case "allot":
                                #region 划拨审批
                                {
                                    var allot = _propertyAllotService.GetPropertyAllotById(id);
                                    if (allot == null || allot.Deleted) throw new Exception("找不到资源");
                                    if (!PropertyCanApprove(allot.State, allot.SuggestGovernmentId)) throw new Exception("没有审批权限");
                                    if (allot.State == PropertyApproveState.DepartmentApprove)
                                    {
                                        allot.DApproveDate = DateTime.Now;
                                        allot.DSuggestion = suggestion;
                                    }
                                    else if (allot.State == PropertyApproveState.AdminApprove)
                                    {
                                        allot.AApproveDate = DateTime.Now;
                                        allot.ASuggestion = suggestion;
                                    }

                                    if (agree)
                                    {
                                        //主管部门审核阶段
                                        if (allot.State == PropertyApproveState.Start)
                                        {
                                            allot.State = PropertyApproveState.DepartmentApprove;
                                        }
                                        else if (allot.State == PropertyApproveState.DepartmentApprove)
                                        {
                                            allot.State = PropertyApproveState.AdminApprove;
                                        }
                                        else if (allot.State == PropertyApproveState.AdminApprove)
                                        {
                                            allot.State = PropertyApproveState.Finish;
                                            var newGovernment = _governmentService.GetGovernmentUnitById(allot.NowGovernmentId);
                                            if (newGovernment == null) throw new Exception("找不到新的权属单位");
                                            allot.Property.Government = newGovernment;

                                            SwitchPropertyLockState(false, allot.Property);

                                        }
                                    }
                                    else
                                    {
                                        allot.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                                    }

                                    _propertyAllotService.UpdatePropertyAllot(allot);

                                    //活动日志
                                    _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产的划拨处置", id));

                                    break;
                                }
                            #endregion
                            case "off":
                                #region 划拨审批
                                {
                                    var off = _propertyOffService.GetPropertyOffById(id);
                                    if (off == null || off.Deleted) throw new Exception("找不到资源");
                                    if (!PropertyCanApprove(off.State, off.SuggestGovernmentId)) throw new Exception("没有审批权限");
                                    if (off.State == PropertyApproveState.Start)
                                    {
                                        off.State = PropertyApproveState.DepartmentApprove;
                                    }
                                    else if (off.State == PropertyApproveState.DepartmentApprove)
                                    {
                                        off.DApproveDate = DateTime.Now;
                                        off.DSuggestion = suggestion;
                                    }
                                    else if (off.State == PropertyApproveState.AdminApprove)
                                    {
                                        off.AApproveDate = DateTime.Now;
                                        off.ASuggestion = suggestion;
                                    }

                                    if (agree)
                                    {
                                        //主管部门审核阶段
                                        if (off.State == PropertyApproveState.DepartmentApprove)
                                        {
                                            off.State = PropertyApproveState.AdminApprove;
                                        }
                                        else if (off.State == PropertyApproveState.AdminApprove)
                                        {
                                            off.State = PropertyApproveState.Finish;
                                            off.Property.Off = true;
                                            off.Property.Published = false;
                                            SwitchPropertyLockState(false, off.Property);
                                        }
                                    }
                                    else
                                    {
                                        off.State = PropertyApproveState.Start;  //直接退回到申请单位                                      
                                    }

                                    _propertyOffService.UpdatePropertyOff(off);

                                    //活动日志
                                    _accountUserActivityService.InsertActivity("applyNewCreateApprove", string.Format("执行id为 {0} 的资产核销处置", id));

                                    break;
                                }
                                #endregion
                        } 
                        #endregion
                    }
                    catch (Exception e)
                    {
                        result.AppendLine(string.Format("id 为 {0} 的资产处置未审批成功，错误原因为：{1}", id, e.Message));
                    }
                }
            }

            return Ok(result.ToString());
        }


        [HttpGet]
        [Route("Approve/Statistics")]
        public IHttpActionResult ApproveStatistics()
        {
            var currentUser = _workContext.CurrentAccountUser;
            var government = currentUser.Government;
            var targetGovIds = new List<int>();
            if (currentUser.IsGovAuditor())
            {
                targetGovIds = _governmentService.GetAllGovernmentUnitsByType(
                    new GovernmentType[2] { GovernmentType.Government, GovernmentType.Institution }).Select(g => g.Id).ToList();
            }
            else if (currentUser.IsStateOwnerAuditor())
            {
                targetGovIds = _governmentService.GetAllGovernmentUnitsByType(
                    new GovernmentType[1] { GovernmentType.Company }).Select(g => g.Id).ToList();
            }
            else
            {
                targetGovIds = _governmentService.GetChildrenGovernmentIds(government.Id);
            }
            string sort = "processDate,desc;";
            var condition = PropertySortCondition.Instance(sort);
            var response = new ApproveStatisticsModel();
            response.NewCreate = _propertyNewCreateService.GetAllNewCreateRecords(targetGovIds, "unchecked", "", 0, int.MaxValue, condition).Count;
            response.Edit = _propertyEditService.GetAllEditRecords(targetGovIds, "unchecked", "", 0, int.MaxValue, condition).Count;
            response.Lend = _propertyLendService.GetAllLendRecords(targetGovIds, "unchecked", "", 0, int.MaxValue, condition).Count;
            response.Rent = _propertyRentService.GetAllRentRecords(targetGovIds, "unchecked", "", 0, int.MaxValue, condition).Count;
            response.Allot = _propertyAllotService.GetAllAllotRecords(targetGovIds, "unchecked", "", 0, int.MaxValue, condition).Count;
            response.Off = _propertyOffService.GetAllOffRecords(targetGovIds, "unchecked", "", 0, int.MaxValue, condition).Count;

            return Ok(response);
        }

        public ArrayList GetPropertyTypeList(HighSearchModel highSearch) {      
            var propertyTypeList = new ArrayList();
      
            if(highSearch.House==true) propertyTypeList.Add(0);
            if(highSearch.Land==true) propertyTypeList.Add(1);       
            return propertyTypeList;

        }
        public IList<int> GetRegionList(HighSearchModel highSearch) {
            var regionList = new List<int>();
            if (highSearch.TMZ == true) regionList.Add(0);
            if (highSearch.ZSZ == true) regionList.Add(1);
            if (highSearch.HBZ == true) regionList.Add(2);
            if (highSearch.SBZ == true) regionList.Add(3);
            return regionList;
        }
        public ArrayList GetAreaList(HighSearchModel highSearch)
        {
            var areaList = new ArrayList();
            if (highSearch.One == true) areaList.Add(49);
            if (highSearch.Two == true) {
                areaList.Add(50);
                areaList.Add(200);
            }
            if (highSearch.Three == true) {
                areaList.Add(200);
                areaList.Add(500);
            }
            if (highSearch.Four == true) {
                areaList.Add(500);
                areaList.Add(1000);
            }
            if (highSearch.Five == true) areaList.Add(1001);
            return areaList;
        }
        public IList<int> GetCurrentTypeList(HighSearchModel highSearch)
        {
            var currentTypeList = new List<int>();
            if (highSearch.ZY == true) currentTypeList.Add(0);
            if (highSearch.CC == true) currentTypeList.Add(2);
            if (highSearch.XZ == true) currentTypeList.Add(5);
            if (highSearch.SYDP == true) currentTypeList.Add(4);
            return currentTypeList;
        }
        public ArrayList GetRightList(HighSearchModel highSearch)
        {
            var rightList = new ArrayList();
            if (highSearch.All == true) rightList.Add(3);
            if (highSearch.isHouse == true) rightList.Add(2);
            if (highSearch.isLand == true) rightList.Add(1);
            if (highSearch.None == true) rightList.Add(0);
            return rightList;
        }



        [HttpPost]
        [Route("highSearch")]
        public IHttpActionResult GetHighSearchProperties(HighSearchModel highSearch) {

            var properyTypeList = GetPropertyTypeList(highSearch);
            var regionList = GetRegionList(highSearch);
            var areaList = GetAreaList(highSearch);
            var currentList = GetCurrentTypeList(highSearch);
            var rightList = GetRightList(highSearch);

            var properties = _propertyService.GetHighSearchProperties(properyTypeList, regionList,areaList, currentList, rightList);



            var response =   properties.Select(p =>
            {
                var geoModel = p.ToGeoModel();
                geoModel.X = Convert.ToDouble(geoModel.Location.Split(' ')[2].Substring(0, geoModel.Location.Split(' ')[2].Length - 1));
                geoModel.Y = Convert.ToDouble(geoModel.Location.Split(' ')[1].Substring(1, geoModel.Location.Split(' ')[1].Length - 1));

                return geoModel;
            });


            return Ok(response);

        }

        [HttpGet]
        [Route("ProcessFilter")]
        public IHttpActionResult GetAllProcess(string query = "", string sort = "",bool showHidden = false, long time = 0,
          bool manage = false, bool isGovernment = false, bool isInstitution = false, bool isCompany = false, int selectedId = 0,
          bool construct = false, bool land = false, bool constructOnLand = false,//资产类型
          bool old = false, bool west = false, bool jjq = false, bool kc = false, bool qj = false, bool other = false, //区域
          bool certi_both = false, bool certi_land = false, bool certi_construct = false, bool certi_none = false, //证书情况
          bool current_self = false, bool current_rent = false, bool current_lend = false, bool currnet_idle = false, //使用现状
          bool auction = false, bool ct = false, bool jt = false, bool jk = false, bool self = false, bool storeUp = false, bool adjust = false, bool greenland = false, bool house = false//下步使用
       //   string constructAreaRange = "", string landAreaRange = "", string priceRange = "", string getDateRange=""  //范围参数
          )
        {
            var currentUser = _workContext.CurrentAccountUser;
            

            //初始化排序条件
            var sortConditions = PropertySortCondition.Instance(sort);

            //特殊字段排序调整
            if (sort.ToLower().StartsWith("governmentname")) sortConditions[0].PropertyName = "Government";

            //高级搜索参数设置
            PropertyAdvanceConditionModel advance = new PropertyAdvanceConditionModel
            {
                Government = new GovernmentFilterModel { Manage = manage, IsCompany = isCompany, IsGovernment = isGovernment, IsInstitution = isInstitution, SelectedId = selectedId },
                PropertyType = new PropertyTypeModel { Construct = construct, Land = land, ConstructOnLand = constructOnLand },
                Region = new RegionModel { West = west, Jjq = jjq, Kc = kc, Old = old, Other = other, Qj = qj },
                Certificate = new Certificate { Both = certi_both, Construct = certi_construct, Land = certi_land, None = certi_land },
                Current = new CurrentModel { Idle = currnet_idle, Lend = current_lend, Rent = current_rent, Self = current_self },
                NextStep = new NextStepModel { Adjust = adjust, Auction = auction, Ct = ct, Jt = jt, Jk = jk, Greenland = greenland, House = house, Self = self, StoreUp = storeUp },
                Time = time
            };
  
            //高级搜索参数设置
            PropertyAdvanceConditionRequest request = PrepareAdvanceCondition(advance);

            var governmentIds = _governmentService.GetGovernmentIdsByCurrentUser(true);  //不包含已有账号的下级单位
            var properties = _propertyService.GetAllProcessProperties(governmentIds, query, 
                request, sortConditions).Select(sp=>
                {
                    
                    return new
                    {
                        Id = sp.Id,
                        Name = sp.Name,
                        Region = sp.Region.ToDescription(),
                        Address = sp.Address,
                        G = sp.Government.Name,
                        ConstructArea = sp.ConstructArea,
                        LandArea = sp.LandArea,
                        PropertyType = sp.PropertyType

                    };
                });

            //activity log
            _accountUserActivityService.InsertActivity("GetpropertyList", "获取资产列表信息");

            return Ok(properties);
        }

        #endregion

        #region 资产导入导出   

        [HttpPost]
        [Route("Export")]
        public HttpResponseMessage ExportProperties(ExportModel exportModel)
        {
            string ids = "";
            //var exportModel = advance.Fields;
            var browser = String.Empty;
            //    string path = @"~/Content/资产导出" + DateTime.Now.ToString("yyyyMMddhhmmss");
            string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/资产导出/" + DateTime.Now.ToString("yyyyMMddhhmmss"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, "资产导出.xls");
          

            #region 获取导出的资产集合
            IList<Property> properties = new List<Property>();

            properties = _propertyService.GetAllProperties();
            //if (ids != "all")
            //{
            //    var pids = ids.Split(';');
            //    foreach (var id in pids)
            //    {
            //        var property = _propertyService.GetPropertyById(Convert.ToInt32(id));
            //        properties.Add(property);
            //    }
            //}
          //  else
          //  {
                //var currentUser = _workContext.CurrentAccountUser;

                //var showHidden = currentUser.IsRegistered() && currentUser.AccountUserRoles.Count == 1;  //只是注册单位可以获取未发布的

                ////初始化排序条件
                //var sortConditions = PropertySortCondition.Instance(advance.Sort);

                ////特殊字段排序调整
                //if (advance.Sort.ToLower().StartsWith("governmentname")) sortConditions[0].PropertyName = "Government";

                ////高级搜索参数设置
                //PropertyAdvanceConditionRequest request = PrepareAdvanceCondition(advance);
                //var governmentIds = _governmentService.GetGovernmentIdsByCurrentUser();  //获取当前账户的可查询的资产
                //properties = _propertyService.GetAllProperties(governmentIds, advance.Query, 0, int.MaxValue, showHidden, request, sortConditions);

            //}
            #endregion

            using (FileStream stream = System.IO.File.Create(filePath))
            {
                Type t = exportModel.GetType();
                PropertyInfo[] PropertyList = t.GetProperties();
                IList<string> headers = new List<string>();

                foreach (var item in PropertyList)
                {
                    if (item.PropertyType.Name == "Boolean")
                    {
                        var data = item.Name.Substring(2);
                        if (Convert.ToBoolean(item.GetValue(exportModel)) == true) headers.Add(data);
                    }
                }
               _exportManager.ExportPropertyToXlsx(stream, properties, headers);

                //activity log
                _accountUserActivityService.InsertActivity("ExportProperties", "批量导出资产");
            }

            try {
                    FileStream outstream = new FileStream(filePath, FileMode.Open);               
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    httpResponseMessage.Content = new StreamContent(outstream);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "资产导出.xls"                 
                    };
                

                return httpResponseMessage;
            }
            catch {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

        }


        [HttpPost]
        [Route("ExportRents")]
        public HttpResponseMessage ExportRents(List<string> timeList) {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/资产导出/" + DateTime.Now.ToString("yyyyMMddhhmmss"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, "出租表.xls");
            var rents = _propertyRentService.GetRentsByTime(timeList);

            using (FileStream stream = System.IO.File.Create(filePath)) {
                _exportManager.ExportRentsToExl(stream, rents);
            }

                try
            {
                FileStream outstream = new FileStream(filePath, FileMode.Open);
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(outstream);
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "出租表.xls"
                };


                return httpResponseMessage;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }


        [HttpPost]
        [Route("Import/excel")]
        public IHttpActionResult ImportProperties()
        {
            bool sucess = false;
            var currentUser = _workContext.CurrentAccountUser;
            if (currentUser.IsAdmin() || currentUser.IsGovAuditor() || currentUser.IsStateOwnerAuditor() ||
                currentUser.IsDataReviewer()) throw new Exception("当前用户没有权限导入数据");

            ImportResponse reponse = new ImportResponse();
            var httpRequest = HttpContext.Current.Request;
            
            var id = currentUser.Government.Id;
          
            string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/资产导入/" + DateTime.Now.ToString("yyyyMMddhhmmss"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }  

            foreach (string file in httpRequest.Files)
            {              
                var postedFile = httpRequest.Files[file];
                var filePath = Path.Combine(path, postedFile.FileName);
                var  stream = postedFile.InputStream;

                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始   
                stream.Seek(0, SeekOrigin.Begin);

                // 把 byte[] 写入文件   
                FileStream fs = new FileStream(filePath, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();

                if (UnRAR(path, path, postedFile.FileName))
                {
                    string excelPath = path + "\\"+postedFile.FileName.Substring(0, postedFile.FileName.Length-4) + "\\资产导入表.xlsx";
                    string picPath = path + "\\" + postedFile.FileName.Substring(0, postedFile.FileName.Length - 4);
                    if (!Directory.Exists(excelPath)) {
                        excelPath = GetXml(path);
                        picPath = excelPath.Substring(0,excelPath.Length-11);
                    }
                    using (FileStream excelStream = new FileStream(excelPath, FileMode.Open))
                    {

                        if (excelStream == null) throw new Exception("没有找到名称为资产导入表的excel文件");
                     
                        reponse = _importManager.ImportProductsFromXlsx(excelStream, picPath);
                    }
                }
            }

            //activity log
            _accountUserActivityService.InsertActivity("ImportProperties", "批量导入资产");
            return Ok(reponse);
          

        }


        #endregion

        [HttpPost]
        [Route("exportMonthTotal/{month}")]
        #region 每月导出
        public HttpResponseMessage ExportMonthTotal(string month) {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/资产导出/" + DateTime.Now.ToString("yyyyMMddhhmmss"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, "每月统计导出.xls");

            var properties = new List<Property>();

                var currentUser = _workContext.CurrentAccountUser;
                var g = currentUser.Government;
                properties = _propertyService.GetExportMonthTotalProperties(g.Id);

         

            using (FileStream stream = System.IO.File.Create(filePath))
            {
                _exportManager.ExportMonthTotal(stream,properties,g.Id,month);
                //activity log
                _accountUserActivityService.InsertActivity("ExportProperties", "每月统计表导出");
            }

            try
            {

                   
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    FileStream outstream = new FileStream(filePath, FileMode.Open);
                    httpResponseMessage.Content = new StreamContent(outstream);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "每月统计导出.xls"
                    };

                    return httpResponseMessage;
                
                       
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

        }

        #endregion

    }
}