using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum AttestationReportType : byte
    {

        [Description("PDF")]
        PDF = 0,

        [Description("CSV")]
        CSV = 1,

        [Description("Excel-Flat")]
        ExcelFlat = 2,

        [Description("Excel-Hierarchical")]
        ExcelHierarchical = 3,
    }
}