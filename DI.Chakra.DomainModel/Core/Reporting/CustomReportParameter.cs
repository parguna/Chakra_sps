using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using DI.Chakra.Logger;

namespace DI.Chakra.DomainModel 
{
   public class CustomReportParameter : DomainModelBase<CustomReportParameter, EntityModel.CustomReportParameter>
    {
        public long CustomReportId { get; set; }
        public CustomReport CustomReport { get; set; }
        public string ParameterName { get; set; }
        public string Description { get; set; }
        public CustomReportParameterVariant ParameterVariant { get; set; }
        public CustomReportParameterTypes ParameterType { get; set; }
        public long? Sequence { get; set; }
        public string DefaultValue { get; set; }
        public string DataFormatString { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
