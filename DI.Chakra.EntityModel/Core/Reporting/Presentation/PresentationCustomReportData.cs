using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationCustomReportData : EntityBase
    {
        public long Count { get; set; }
        public DataTable ReportData { get; set; }
        public List<DataColumn> ReportColumns { get; set; }
    }
}
