using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPObjectInfo:DtoBase
    {
        public string SapObjectCode { get; set; }

        public string FieldCode1 { get; set; }

        public string FieldCode2 { get; set; }

        public string FieldCode3 { get; set; }

        public string FieldCode4 { get; set; }

        public string FieldCode5 { get; set; }

        public string FieldCode6 { get; set; }

        public string FieldCode7 { get; set; }

        public string FieldCode8 { get; set; }

        public string FieldCode9 { get; set; }

        public string FieldCode0 { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public int? Version { get; set; }

    }
}
