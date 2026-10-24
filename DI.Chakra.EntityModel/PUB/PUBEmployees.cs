using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_PUB_Employees")]
    public  class PUBEmployees : EntityBase, ICreatable, IUpdatable
    {
        public Nullable<long> DIUserId { get; set; }
        public Nullable<long> PUBFileId { get; set; }
        public string EmployeeId { get; set; }
        public string EBSUserId { get; set; }
        public string DisplayName { get; set; }
        public string CompanyName { get; set; }
        [Column(TypeName = "datetime2")]
        public Nullable<DateTime> ActualTerminationDate { get; set; }
        [Column(TypeName = "datetime2")]
        public Nullable<DateTime> NewTerminationDate { get; set; }
        public Nullable<long> EmployeeEventTriggerId { get; set; }
        public string Status { get; set; }
        public string ValidationMsg { get; set; }
        public string Remarks { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
