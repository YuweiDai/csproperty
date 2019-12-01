using CSCZJ.Core;
using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Property
{
    public interface IPropertyRentService
    {
        IPagedList<CSCZJ.Core.Domain.Properties.PropertyRent> GetAllRentRecords(IList<int> governmentIds, string checkState = "unchecked", string search = "", int pageIndex = 0,
            int pageSize = int.MaxValue, params PropertySortCondition[] sortConditions);

        IPagedList<CSCZJ.Core.Domain.Properties.PropertyRent> GetRentListRecords(int page = 0, int results = int.MaxValue, string sortField = "", string sortOrder = "", string tabKey = "即将过期", params PropertySortCondition[] sortConditions);
             
        void DeletePropertyRent(PropertyRent p);

        void InsertPropertyRent(PropertyRent p);
        void UpdatePropertyRent(PropertyRent p);

        PropertyRent GetPropertyRentById(int id);

        IList<PropertyRent> GetAllRents();

        List<PropertyRent> GetRentsByTime(List<string> timeList);

        IList<PropertyRent> GetRentsByPropertyId(int id);

        #region Property pictures

        /// <summary>
        /// Deletes a propertyRent picture
        /// </summary>
        /// <param name="propertyRentPicture">Property picture</param>
        void DeletePropertyRentPicture(PropertyRentPicture propertyRentPicture);

        /// <summary>
        /// Gets a propertyRent pictures by propertyRent identifier
        /// </summary>
        /// <param name="propertyRentId">The propertyRent identifier</param>
        /// <returns>Property pictures</returns>
        IList<PropertyRentPicture> GetPropertyRentPicturesByPropertyId(int propertyRentId);

        /// <summary>
        /// Gets a propertyRent picture
        /// </summary>
        /// <param name="propertyRentPictureId">Property picture identifier</param>
        /// <returns>Property picture</returns>
        PropertyRentPicture GetPropertyRentPictureById(int propertyRentPictureId);

        /// <summary>
        /// Inserts a propertyRent picture
        /// </summary>
        /// <param name="propertyRentPicture">Property picture</param>
        void InsertPropertyRentPicture(PropertyRentPicture propertyRentPicture);

        /// <summary>
        /// Updates a propertyRent picture
        /// </summary>
        /// <param name="propertyRentPicture">Property picture</param>
        void UpdatePropertyRentPicture(PropertyRentPicture propertyRentPicture);

        #endregion

        #region Property FILES

        /// <summary>
        /// Deletes a propertyRent file
        /// </summary>
        /// <param name="propertyRentFile">Property picture</param>
        void DeletePropertyRentFile(PropertyRentFile propertyRentFile);

        /// <summary>
        /// Gets a propertyRent files by propertyRent identifier
        /// </summary>
        /// <param name="propertyRentId">The propertyRent identifier</param>
        /// <returns>Property files</returns>
        IList<PropertyRentFile> GetPropertyFilesByPropertyRentId(int propertyRentId);

        /// <summary>
        /// Gets a propertyRent file
        /// </summary>
        /// <param name="propertyFileId">Property file identifier</param>
        /// <returns>Property file</returns>
        PropertyRentFile GetPropertyFileById(int propertyFileId);

        /// <summary>
        /// Inserts a propertyRent file
        /// </summary>
        /// <param name="propertyRentFile">Property file</param>
        void InsertPropertyFile(PropertyRentFile propertyRentFile);

        /// <summary>
        /// Updates a propertyRent file
        /// </summary>
        /// <param name="propertyRentFile">Property file</param>
        void UpdatePropertyFile(PropertyRentFile propertyRentFile);

        #endregion
    }
}
