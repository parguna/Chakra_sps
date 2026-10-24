using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class CustomReportParameter : DtoBase
    {
        public CustomReportParameter()
        {
            IsDeleted = false;
        }
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
        public object Value { get; set; }
    }
}
