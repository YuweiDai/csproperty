using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCZJ.Core.Domain.Properties;
using CSCZJ.Core.Data;
using CSCZJ.Services.Events;
using CSCZJ.Core;
using System.Linq.Expressions;
using System.Data.Entity;
using CSCZJ.Data;
using CSCZJ.Services.AccountUsers;

namespace CSCZJ.Services.Property
{
    public class PropertyRentService : IPropertyRentService
    {
        private readonly IRepository<PropertyRent> _propertyRentRepository;
        private readonly IRepository<PropertyRentPicture> _propertyPictureRepository;
        private readonly IRepository<PropertyRentFile> _propertyFileRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;

        public PropertyRentService(IRepository<PropertyRent> propertyRentRepository, IEventPublisher eventPublisher,
                         IRepository<PropertyRentPicture> propertyPictureRepository, IRepository<PropertyRentFile> propertyFileRepository, 
                         IWorkContext workContext)
        {
            _propertyRentRepository = propertyRentRepository;
            _eventPublisher = eventPublisher;
            _workContext = workContext;

            _propertyPictureRepository = propertyPictureRepository;
            _propertyFileRepository = propertyFileRepository;
        }

        public virtual IPagedList<CSCZJ.Core.Domain.Properties.PropertyRent> GetAllRentRecords(IList<int> governmentIds, string checkState = "unchecked", string search = "", int pageIndex = 0, int pageSize = int.MaxValue, params PropertySortCondition[] sortConditions)
        {
            var query = _propertyRentRepository.Table.AsNoTracking();
            var currentUser = _workContext.CurrentAccountUser;

            //query = GetAllProperties(governmentId, includeChildren);

            Expression<Func<CSCZJ.Core.Domain.Properties.PropertyRent, bool>> expression = p => !p.Deleted;

            if (governmentIds != null && governmentIds.Count > 0)
            {
                expression = expression.And(p => governmentIds.Contains(p.SuggestGovernmentId));
            }

            //字符串查询
            if (!string.IsNullOrEmpty(search))
            {
                expression = expression.And(p => p.Title.Contains(search) || p.ASuggestion.Contains(search) || p.DSuggestion.Contains(search));
            }
            switch (checkState)
            {
                case "unchecked":
                    if (currentUser.IsGovAuditor() || currentUser.IsStateOwnerAuditor())
                    {
                        expression = expression.And(p => p.State == PropertyApproveState.AdminApprove);
                    }
                    else if (currentUser.Government.ParentGovernmentId == 0)
                    {
                        expression = expression.And(p => p.State == PropertyApproveState.DepartmentApprove || (p.State == PropertyApproveState.Start & p.SuggestGovernmentId == currentUser.Government.Id));
                    }
                    else
                    {
                        expression = expression.And(p => p.State == PropertyApproveState.Start);
                    }
                    break;
                case "checked":
                    if (currentUser.IsGovAuditor() || currentUser.IsStateOwnerAuditor())
                    {
                        expression = expression.And(p => p.State == PropertyApproveState.Finish);
                    }
                    else if (currentUser.Government.ParentGovernmentId == 0)
                    {
                        expression = expression.And(p => p.State == PropertyApproveState.Finish || p.State == PropertyApproveState.AdminApprove);
                    }
                    else
                    {
                        expression = expression.And(p => p.State != PropertyApproveState.Start);
                    }
                    break;
                case "all":
                    //expression = expression.And(p => p.State != PropertyApproveState.Start);
                    break;
            }
            query = query.Where(expression);

            if (sortConditions != null && sortConditions.Length != 0)
            {
                query = query.Sort(sortConditions);
            }
            else
            {
                query = query.Sort(new PropertySortCondition[1] {
                    new PropertySortCondition("DisplayOrder", System.ComponentModel.ListSortDirection.Ascending)
                });
            }

            var propertiesRentRecords = new PagedList<CSCZJ.Core.Domain.Properties.PropertyRent>(query, pageIndex, pageSize);
            return propertiesRentRecords;
        }

        public IPagedList<PropertyRent> GetRentListRecords(int page = 0, int results = int.MaxValue, string sortField = "", string sortOrder = "", string tabKey = "即将过期", params PropertySortCondition[] sortConditions)
        {
            var query = _propertyRentRepository.Table.AsNoTracking();

            Expression<Func<CSCZJ.Core.Domain.Properties.PropertyRent, bool>> expression = p => !p.Deleted;
            var now = DateTime.Now;
            var rents = new List<PropertyRent>();
            query = query.Where(expression);
            switch (tabKey)
            {
                case "即将过期":
                    query = query.Where(p => System.Data.Entity.DbFunctions.DiffDays(now,p.BackTime) >=0 && System.Data.Entity.DbFunctions.DiffDays(now, p.BackTime) < 30);
                    break;
                case "已经过期":
                    query = query.Where(p=> System.Data.Entity.DbFunctions.DiffDays(now, p.BackTime) < 0);
                    break;
                case "全部信息":
                    break;
            }


            var defaultSort = new PropertySortCondition("Id", System.ComponentModel.ListSortDirection.Ascending);
            if (sortField == "" || sortField == null ||sortField=="null")
            {

                sortField = "BackTime";
               defaultSort = new PropertySortCondition(sortField, System.ComponentModel.ListSortDirection.Ascending);
            }
            else {
                sortField = sortField.Substring(0, 1).ToUpper() + sortField.Substring(1);
                if (sortOrder == "ascend") defaultSort = new PropertySortCondition(sortField, System.ComponentModel.ListSortDirection.Ascending);
                else
                {
                    defaultSort = new PropertySortCondition(sortField, System.ComponentModel.ListSortDirection.Descending);
                }
            }

            if (sortConditions != null && sortConditions.Length != 0)
            {
                query = query.Sort(sortConditions[0]);
            }
            else
            {
                query = query.Sort(defaultSort);
            }

            var reds = new PagedList<CSCZJ.Core.Domain.Properties.PropertyRent>();
            reds.TotalCount = query.Count();
            reds.TotalPages = query.Count() / results;
            if (query.Count() % results > 0) reds.TotalPages++;
            reds.PageSize = results;
            reds.PageIndex = page;
            reds.AddRange(query.ToList());

                return reds;
        }


        public IList<PropertyRent> GetAllRents() {

            var query = from p in _propertyRentRepository.TableNoTracking
                        where !p.Deleted
                        select p;

            return query.ToList();

        }


        public void DeletePropertyRent(PropertyRent p)
        {
            if (p == null)
                throw new ArgumentNullException("property is null");

            p.Deleted = true;
            UpdatePropertyRent(p);
        }

        public PropertyRent GetPropertyRentById(int id)
        {
            var query = from c in _propertyRentRepository.Table
                        where c.Id == id
                        select c;
            var p = query.FirstOrDefault();
            return p;
        }

        public void InsertPropertyRent(PropertyRent p)
        {
            if (p == null)
                throw new ArgumentNullException("property is null");

            _propertyRentRepository.Insert(p);
            _eventPublisher.EntityInserted(p);
        }

        public void UpdatePropertyRent(PropertyRent p)
        {
            if (p == null)
                throw new ArgumentNullException("property is null");

            _propertyRentRepository.Update(p);
            _eventPublisher.EntityUpdated(p);
        }

        public IList<PropertyRent> GetRentsByPropertyId(int id)
        {
            var query = from c in _propertyRentRepository.Table
                        where c.Property.Id == id
                        select c;
            var rents = query.ToList();
            return rents;

        }

        #region 图片
        public void DeletePropertyRentPicture(PropertyRentPicture propertyRentPicture)
        {
            if (propertyRentPicture == null)
                throw new ArgumentNullException("propertyRentPicture");

            _propertyPictureRepository.Delete(propertyRentPicture);

            //event notification
            _eventPublisher.EntityDeleted(propertyRentPicture);
        }

        public IList<PropertyRentPicture> GetPropertyRentPicturesByPropertyId(int propertyRentId)
        {
            var query = from sp in _propertyPictureRepository.Table
                        where sp.PropertyRentId == propertyRentId
                        orderby sp.DisplayOrder
                        select sp;

            var propertyPictures = query.ToList();
            return propertyPictures;
        }

        public PropertyRentPicture GetPropertyRentPictureById(int propertyPictureId)
        {
            if (propertyPictureId == 0)
                return null;

            return _propertyPictureRepository.GetById(propertyPictureId);
        }

        public void InsertPropertyRentPicture(PropertyRentPicture propertyRentPicture)
        {
            if (propertyRentPicture == null)
                throw new ArgumentNullException("propertyRentPicture");

            _propertyPictureRepository.Insert(propertyRentPicture);

            //event notification
            _eventPublisher.EntityInserted(propertyRentPicture);
        }

        public void UpdatePropertyRentPicture(PropertyRentPicture propertyRentPicture)
        {
            if (propertyRentPicture == null)
                throw new ArgumentNullException("propertyRentPicture");

            _propertyPictureRepository.Update(propertyRentPicture);

            //event notification
            _eventPublisher.EntityUpdated(propertyRentPicture);
        }
        #endregion

        #region 文件
        public void DeletePropertyRentFile(PropertyRentFile propertyRentFile)
        {
            if (propertyRentFile == null)
                throw new ArgumentNullException("propertyRentFile");

            _propertyFileRepository.Delete(propertyRentFile);

            //event notification
            _eventPublisher.EntityDeleted(propertyRentFile);
        }

        public IList<PropertyRentFile> GetPropertyFilesByPropertyRentId(int propertyRentId)
        {
            var query = from sp in _propertyFileRepository.Table
                        where sp.PropertyRentId == propertyRentId
                        orderby sp.DisplayOrder
                        select sp;

            var propertyFiles = query.ToList();
            return propertyFiles;
        }

        public PropertyRentFile GetPropertyFileById(int propertyFileId)
        {
            if (propertyFileId == 0)
                return null;

            return _propertyFileRepository.GetById(propertyFileId);
        }

        public void InsertPropertyFile(PropertyRentFile propertyRentFile)
        {
            if (propertyRentFile == null)
                throw new ArgumentNullException("propertyRentFile");

            _propertyFileRepository.Insert(propertyRentFile);

            //event notification
            _eventPublisher.EntityInserted(propertyRentFile);
        }

        public void UpdatePropertyFile(PropertyRentFile propertyRentFile)
        {
            if (propertyRentFile == null)
                throw new ArgumentNullException("propertyRentFile");

            _propertyFileRepository.Update(propertyRentFile);

            //event notification
            _eventPublisher.EntityUpdated(propertyRentFile);
        }

      
        #endregion




        public List<PropertyRent> GetRentsByTime(List<string> timeList)
        {

            var query = from c in _propertyRentRepository.Table
                        where c.RentTime> Convert.ToDateTime(timeList[0]) && c.RentTime<Convert.ToDateTime(timeList[1])
                        select c;
            var rents = query.ToList();
            return rents;


            throw new NotImplementedException();
        }
    }
}
