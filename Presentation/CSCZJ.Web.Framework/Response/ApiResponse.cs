using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Web.Framework
{

    public class SimpleResponse
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string Data { get; set; }
    }

    public class ResponseObject<T>
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

    public class ResponseObjectList<T>
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public IList<T> Data { get; set; }

        public int Total { get; set; }
    }
}
