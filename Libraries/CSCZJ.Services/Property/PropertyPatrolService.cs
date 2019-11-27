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
    public class PropertyPatrolService:IPropertyPatrolService
    {

        private readonly IRepository<PropertyPatrol> _propertyPatrolRepository;
        private readonly IRepository<PropertyPatrolPicture> _propertyPictureRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;

        public PropertyPatrolService(IRepository<PropertyPatrol> propertyPatrolRepository, IEventPublisher eventPublisher,
                         IRepository<PropertyPatrolPicture> propertyPictureRepository,
                         IWorkContext workContext)
        {
            _propertyPatrolRepository = propertyPatrolRepository;
            _eventPublisher = eventPublisher;
            _workContext = workContext;
            _propertyPictureRepository = propertyPictureRepository;
        }


        public Core.PagedList<Core.Domain.Properties.PropertyPatrol> GetPatrolListRecords(int page = 0, int results = int.MaxValue, string sortField = "", string sortOrder = "", string tabKey = "即将过期", params Core.PropertySortCondition[] sortConditions)
        {
            var query = _propertyPatrolRepository.Table.AsNoTracking();

            Expression<Func<CSCZJ.Core.Domain.Properties.PropertyPatrol, bool>> expression = p => !p.Deleted;
            var now = DateTime.Now;
            var patrols = new List<PropertyPatrol>();
            query = query.Where(expression);
            switch (tabKey)
            {
                case "今年巡查":
                    query = query.Where(p => p.PatrolDate.Year==now.Year);
                    break;
                case "往年巡查":
                    query = query.Where(p => p.PatrolDate.Year<now.Year);
                    break;
                case "全部":
                    break;
            }


            var defaultSort = new PropertySortCondition("Id", System.ComponentModel.ListSortDirection.Ascending);
            if (sortField == "" || sortField == null || sortField == "null")
            {

                sortField = "PatrolDate";
                defaultSort = new PropertySortCondition(sortField, System.ComponentModel.ListSortDirection.Ascending);
            }
            else
            {
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

            var reds = new PagedList<CSCZJ.Core.Domain.Properties.PropertyPatrol>();
            reds.TotalCount = query.Count();
            reds.TotalPages = query.Count() / results;
            if (query.Count() % results > 0) reds.TotalPages++;
            reds.PageSize = results;
            reds.PageIndex = page;
            reds.AddRange(query.ToList());

            return reds;
        }

        public void DeletePropertyPatrol(Core.Domain.Properties.PropertyPatrol p)
        {
            if (p == null)
                throw new ArgumentNullException("property is null");

            p.Deleted = true;
            UpdatePropertyPatrol(p);
        }

        public void InsertPropertyPatrol(Core.Domain.Properties.PropertyPatrol p)
        {
            if (p == null)
                throw new ArgumentNullException("property is null");

            _propertyPatrolRepository.Insert(p);
            _eventPublisher.EntityInserted(p);
        }

        public void UpdatePropertyPatrol(Core.Domain.Properties.PropertyPatrol p)
        {
            if (p == null)
                throw new ArgumentNullException("property is null");

            _propertyPatrolRepository.Update(p);
            _eventPublisher.EntityUpdated(p);
        }

        public Core.Domain.Properties.PropertyPatrol GetPropertyPatrolById(int id)
        {
            var query = from c in _propertyPatrolRepository.Table
                        where c.Id == id
                        select c;
            var p = query.FirstOrDefault();
            return p;
        }

        public void DeletePropertyPatrolPicture(Core.Domain.Properties.PropertyPatrolPicture propertyPatrolPicture)
        {
            if (propertyPatrolPicture == null)
                throw new ArgumentNullException("propertyRentPicture");

            _propertyPictureRepository.Delete(propertyPatrolPicture);

            //event notification
            _eventPublisher.EntityDeleted(propertyPatrolPicture);
        }

        public IList<Core.Domain.Properties.PropertyPatrolPicture> GetPropertyPatrolPicturesByPropertyId(int propertyPatrolId)
        {
            var query = from sp in _propertyPictureRepository.Table
                        where sp.PropertyPatrolId == propertyPatrolId
                        orderby sp.DisplayOrder
                        select sp;

            var propertyPictures = query.ToList();
            return propertyPictures;
        }

        public Core.Domain.Properties.PropertyPatrolPicture GetPropertyPatrolPictureById(int propertyPatrolPictureId)
        {
            if (propertyPatrolPictureId == 0)
                return null;

            return _propertyPictureRepository.GetById(propertyPatrolPictureId);
        }

        public void InsertPropertyPatrolPicture(Core.Domain.Properties.PropertyPatrolPicture propertyPatrolPicture)
        {
            if (propertyPatrolPicture == null)
                throw new ArgumentNullException("propertyPatrolPicture");

            _propertyPictureRepository.Insert(propertyPatrolPicture);

            //event notification
            _eventPublisher.EntityInserted(propertyPatrolPicture);
        }

        public void UpdatePropertyPatrolPicture(Core.Domain.Properties.PropertyPatrolPicture propertyPatrolPicture)
        {
            if (propertyPatrolPicture == null)
                throw new ArgumentNullException("propertyPatrolPicture");

            _propertyPictureRepository.Update(propertyPatrolPicture);

            //event notification
            _eventPublisher.EntityUpdated(propertyPatrolPicture);
        }
    }
}
