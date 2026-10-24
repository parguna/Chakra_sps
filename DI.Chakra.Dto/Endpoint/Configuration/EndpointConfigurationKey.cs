using System;
namespace DI.Chakra.Dto
{
    [Serializable]
    public class EndpointConfigurationKey : DtoBase
    {
        public string KeyName { get; set; }

        public string Caption { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }

}
