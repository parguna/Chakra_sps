using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SODFunctionItem : DtoBase
    {
        public Nullable<long> SodFunctionId { get; set; }

        public string SodFunctionType { get; set; }

        public string SodRoleId { get; set; }

        public string SodRoleDesc { get; set; }

        public string ObjectCode { get; set; }

        public string FieldCode { get; set; }

        public string FromValue { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }

        public Nullable<long> Endpointsystemid { get; set; }

        public EndPointSystemType EndpointType { get; set; }

        public EndPoint endpoint { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
