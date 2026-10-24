using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class Report: DtoBase
    {
        public ReportType Type { get; set; }
        public string Format { get; set; }
        public Process Process { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
        public List<ReportAttributes> ReportAttributes { get; set; }
        public string ReportUrl { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string MailTo { get; set; }
        public string MailCC { get; set; }
    }
}
