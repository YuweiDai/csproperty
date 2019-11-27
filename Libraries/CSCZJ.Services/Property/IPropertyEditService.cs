﻿using CSCZJ.Core;
using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Property
{
   public  interface IPropertyEditService
    {

        IPagedList<CSCZJ.Core.Domain.Properties.PropertyEdit> GetAllEditRecords(IList<int> governmentIds, string checkState = "unchecked", string search = "", int pageIndex = 0,
          int pageSize = int.MaxValue, params PropertySortCondition[] sortConditions);
        void DeletePropertyEdit(PropertyEdit p);

        void InsertPropertyEdit(PropertyEdit p);

        void UpdatePropertyEdit(PropertyEdit p);

        PropertyEdit GetPropertyEditById(int id);

       IList< PropertyEdit> GetPropertyEditByPropertyId(int property_Id);



    }
}
