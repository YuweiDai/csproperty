using CSCZJ.Core;
using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;

namespace CSCZJ.Services.Property
{
    public interface IPropertyService
    {
        /// <summary>
        /// Delete a property
        /// </summary>
        /// <param name="property">Property</param>
        void DeleteProperty(CSCZJ.Core.Domain.Properties.Property property);

        /// <summary>
        /// Gets a property
        /// </summary>
        /// <param name="propertyId">Property identifier</param>
        /// <returns>A property</returns>
        CSCZJ.Core.Domain.Properties.Property GetPropertyById(int propertyId);

        IList<CSCZJ.Core.Domain.Properties.Property> GetAllProperties();

        /// <summary>
        /// 获取资产列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="showHidden"></param>
        /// <param name="sortConditions"></param>
        /// <returns></returns>
        IPagedList<CSCZJ.Core.Domain.Properties.Property> GetAllProperties(IList<int> governmentIds, string search = "",int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, PropertyAdvanceConditionRequest advanceCondition=null, params PropertySortCondition[] sortConditions);

        /// <summary>
        /// 根据当前位置获取周边资产
        /// </summary>
        /// <returns></returns>
        IList<CSCZJ.Core.Domain.Properties.Property> GetAllPropertiesByDistance(double lat, double lng, string search = "", int pageIndex = 0, int pageSize = int.MaxValue);

        IList<CSCZJ.Core.Domain.Properties.Property> GetAllProcessProperties(IList<int> governmentIds, string search = "", PropertyAdvanceConditionRequest advanceCondition = null, params PropertySortCondition[] sortConditions);

        IQueryable<CSCZJ.Core.Domain.Properties.Property> GetAllProperties(IList<int> governmentIds, bool showHidden = true);

        IQueryable<CSCZJ.Core.Domain.Properties.Property> GetPropertiesByGovernmentId(IList<int> governmentIds);

        IList<CSCZJ.Core.Domain.Properties.Property> GetPropertiesByGId(int id);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="property"></param>
        void UpdateProperty(CSCZJ.Core.Domain.Properties.Property property);

        /// <summary>
        /// Insert a property
        /// </summary>
        /// <param name="property">Property</param>
        void InsertProperty(CSCZJ.Core.Domain.Properties.Property property);

        /// <summary>
        /// 名称唯一性检查
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>Picture
        bool NameUniqueCheck(string propertyName);

        IList<CSCZJ.Core.Domain.Properties.Property> GetPropertyProcess(int governmentId);


        IList<CSCZJ.Core.Domain.Properties.Property> GetMonthTotalPropertyProcess(int governmentId);

        /// <summary>
        /// 获取所有可审批的资产
        /// </summary>
        /// <returns></returns>
        IList<CSCZJ.Core.Domain.Properties.Property> GetProcessProperties(string name,IList<int> governmentIds);


        IList<CSCZJ.Core.Domain.Properties.Property> GetCurrentGovermentProperties(string name);

        Region GetPropertyRegion(DbGeography location);

        List<CSCZJ.Core.Domain.Properties.Property> GetExportMonthTotalProperties(int id);

        IList<CSCZJ.Core.Domain.Properties.Property> GetKeyProperties(string search);

        IList<CSCZJ.Core.Domain.Properties.Property> GetHighSearchProperties(ArrayList properyTypeList, IList<int> regionList, ArrayList areaList, IList<int> currentList, ArrayList rightList);


        IList<CSCZJ.Core.Domain.Properties.Property> GetPropertiesBySameNumberId(string numberId, string typeId);

        #region Property pictures

        /// <summary>
        /// Deletes a property picture
        /// </summary>
        /// <param name="propertyPicture">Property picture</param>
        void DeletePropertyPicture(PropertyPicture propertyPicture);

        /// <summary>
        /// Gets a property pictures by property identifier
        /// </summary>
        /// <param name="propertyId">The property identifier</param>
        /// <returns>Property pictures</returns>
        IList<PropertyPicture> GetPropertyPicturesByPropertyId(int propertyId);

        /// <summary>
        /// Gets a property picture
        /// </summary>
        /// <param name="propertyPictureId">Property picture identifier</param>
        /// <returns>Property picture</returns>
        PropertyPicture GetPropertyPictureById(int propertyPictureId);

        /// <summary>
        /// Inserts a property picture
        /// </summary>
        /// <param name="propertyPicture">Property picture</param>
        void InsertPropertyPicture(PropertyPicture propertyPicture);

        /// <summary>
        /// Updates a property picture
        /// </summary>
        /// <param name="propertyPicture">Property picture</param>
        void UpdatePropertyPicture(PropertyPicture propertyPicture);

        #endregion

        #region Property FILES

        /// <summary>
        /// Deletes a property file
        /// </summary>
        /// <param name="propertyFile">Property picture</param>
        void DeletePropertyFile(PropertyFile propertyFile);

        /// <summary>
        /// Gets a property files by property identifier
        /// </summary>
        /// <param name="propertyId">The property identifier</param>
        /// <returns>Property files</returns>
        IList<PropertyFile> GetPropertyFilesByPropertyId(int propertyId);

        /// <summary>
        /// Gets a property file
        /// </summary>
        /// <param name="propertyFileId">Property file identifier</param>
        /// <returns>Property file</returns>
        PropertyFile GetPropertyFileById(int propertyFileId);

        /// <summary>
        /// Inserts a property file
        /// </summary>
        /// <param name="propertyFile">Property file</param>
        void InsertPropertyFile(PropertyFile propertyFile);

        /// <summary>
        /// Updates a property file
        /// </summary>
        /// <param name="propertyFile">Property file</param>
        void UpdatePropertyFile(PropertyFile propertyFile);

        #endregion
         
    }
}
