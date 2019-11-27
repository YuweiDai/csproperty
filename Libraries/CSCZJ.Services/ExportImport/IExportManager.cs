using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.ExportImport
{
    public interface IExportManager
    {
        /// <summary>
        /// Export products to XLSX
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="properties">Properties</param>
        void ExportPropertyToXlsx(Stream stream, IList<CSCZJ.Core.Domain.Properties.Property> properties, IList<string> headers);

        void ExportMonthTotal(Stream stream, List<CSCZJ.Core.Domain.Properties.Property> properties,int id,string month);

        void ExportRentsToExl(Stream stream,IList<PropertyRent> rents);

    }
}