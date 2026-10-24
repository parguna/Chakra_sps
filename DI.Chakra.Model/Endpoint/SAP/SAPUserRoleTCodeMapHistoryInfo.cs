using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Infrastructure;

namespace DeepIdentity.Model.SAP
{
    public class SAPUserRoleTCodeMapHistoryInfo : IAggregateRoot
    {
        public string TenantId { get; set; }
        public int HistoryId { get; set; }
        public int EndPointSystemId { get; set; }
        public string Role_Code { get; set; }
        public string TCode { get; set; }
        public string TCode_Desc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
