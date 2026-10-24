using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_SODFunction")]
    public class SodFunction
    {
        [Key]
        public long Id { get; set; }
        public string SodFunctionCode { get; set; }
        public string SodFunctionDescription { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public virtual List<SodFunctionItem> SodFunctionItems { get; set; }

        public virtual List<SODConflictFunction> ConflictFunctions { get; set; }
        //public virtual List<SODException> Exceptions { get; set; }


        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public SodFunction()
        {

        }

    }


    [Serializable]
    [Table("DI_M_SODFunctionItem")]
    public class SodFunctionItem
    {
        [Key]
        public long Id { get; set; }

        public Nullable<long> SodFunctionId { get; set; }
        public string SodFunctionType { get; set; }
        public string SodRoleId { get; set; }
        public string SodRoleDesc { get; set; }
        public string ObjectCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }
        public Nullable<long> Endpointsystemid { get; set; }
        [ForeignKey("EndpointSystemTypeId")]
        public virtual EndPointSystemType EndpointType { get; set; }
        [ForeignKey("Endpointsystemid")]
        public virtual EndPoint endpoint { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }


        public SodFunctionItem()
        {

        }

    }
}
