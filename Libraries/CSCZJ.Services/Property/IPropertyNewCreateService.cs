﻿using CSCZJ.Core;
using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Property
{
   public interface IPropertyNewCreateService
    {
        IPagedList<CSCZJ.Core.Domain.Properties.PropertyNewCreate> GetAllNewCreateRecords(IList<int> governmentIds, string checkState = "unchecked", string search = "", int pageIndex = 0, 
            int pageSize = int.MaxValue, params PropertySortCondition[] sortConditions);

        void DeletePropertyNewCreate(PropertyNewCreate p);

        void InsertPropertyNewCreate(PropertyNewCreate p);

        void UpdatePropertyNewCreate(PropertyNewCreate p);

        PropertyNewCreate GetPropertyNewCreateById(int id);

        PropertyNewCreate GetPropertyNewCreateByPropertyId(int property_Id);
    }
}
