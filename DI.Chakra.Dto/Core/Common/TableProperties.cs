using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class TableProperties 
    {
        public string ColumnName { get; set; } 

        public string DataType { get; set; }

    }
    public class IDMSReportParam
    {
        public List<IDMSReportDataColumn> ReportDataColumns { get; set; }
    }
    public class IDMSReportDataColumn
    {
        public IDMSReportDataColumn(string header,string dataFieldName, string format)
        {
            this.Header = header;
            this.DataFieldName = dataFieldName;
            this.Format = format;
        }
        public string Header { get; set; }
        public string DataFieldName { get; set; }
        public string Format { get; set; }
    }
}
