using CSCZJ.Core;
using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Property
{
    public interface IPropertyPatrolService
    {

        PagedList<CSCZJ.Core.Domain.Properties.PropertyPatrol> GetPatrolListRecords(int page = 0, int results = int.MaxValue, string sortField = "", string sortOrder = "", string tabKey = "即将过期", params PropertySortCondition[] sortConditions);

        void DeletePropertyPatrol(PropertyPatrol p);

        void InsertPropertyPatrol(PropertyPatrol p);
        void UpdatePropertyPatrol(PropertyPatrol p);

        PropertyPatrol GetPropertyPatrolById(int id);

        #region PropertyPatrol pictures

        /// <summary>
        /// Deletes a propertyRent picture
        /// </summary>
        /// <param name="propertyPatrolPicture">Property picture</param>
        void DeletePropertyPatrolPicture(PropertyPatrolPicture propertyPatrolPicture);

        /// <summary>
        /// Gets a propertyRent pictures by propertyRent identifier
        /// </summary>
        /// <param name="propertyPatrolId">The propertyRent identifier</param>
        /// <returns>Property pictures</returns>
        IList<PropertyPatrolPicture> GetPropertyPatrolPicturesByPropertyId(int propertyPatrolId);

        /// <summary>
        /// Gets a propertyRent picture
        /// </summary>
        /// <param name="propertyPatrolPictureId">Property picture identifier</param>
        /// <returns>Property picture</returns>
        PropertyPatrolPicture GetPropertyPatrolPictureById(int propertyPatrolPictureId);

        /// <summary>
        /// Inserts a propertyRent picture
        /// </summary>
        /// <param name="propertyPatrolPicture">Property picture</param>
        void InsertPropertyPatrolPicture(PropertyPatrolPicture propertyPatrolPicture);

        /// <summary>
        /// Updates a propertyRent picture
        /// </summary>
        /// <param name="propertyPatrolPicture">Property picture</param>
        void UpdatePropertyPatrolPicture(PropertyPatrolPicture propertyPatrolPicture);

        #endregion


    }
}
