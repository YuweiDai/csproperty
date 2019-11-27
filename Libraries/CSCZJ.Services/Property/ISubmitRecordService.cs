using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Property
{
   public  interface ISubmitRecordService
    {

        void DeleteSubmitRecord(SubmitRecord m);

        void InsertSubmitRecord(SubmitRecord m);

        void UpdateSubmitRecord(SubmitRecord m);

        SubmitRecord GetSubmitRecordByGId(int id,string recordDate);



    }
}
