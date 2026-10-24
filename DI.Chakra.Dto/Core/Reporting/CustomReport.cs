using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class PresentationCustomReportData : DtoBase
    {
        public long Count { get; set; }
        public DataTable ReportData { get; set; }
        public List<DataColumn> ReportColumns { get; set; }
    }


    [Serializable]
    public class CustomReport : DtoBase
    {
        public string UniqueName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReportQuery { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> LastRunOn { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Attachment { get; set; }
        public bool IsLinkedToMenu { get; set; }
        public long? LinkedToMenu { get; set; }
        public bool IsGroupByEnabled { get; set; }
        public string GroupByColumn { get; set; }
        public long? PageId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<CustomReportParameter> CustomReportParameters { get; set; }

    }
}
