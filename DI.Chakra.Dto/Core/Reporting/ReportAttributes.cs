using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class ReportAttributes: DtoBase
    {
        public long ReportId { get; set; }
        public Report ReportSchedule { get; set; }
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
