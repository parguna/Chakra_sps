using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_ADS_OUInfo")]
    public class ADSOuInfo : EntityBase ,ICreatable,IUpdatable
    {
       public long EndPointSystemId { get; set; }

        public string ou { get; set; }

        public string description { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? whenChanged { get; set; }

        public Int64 uSNCreated { get; set; }

        public Int64 uSNChanged { get; set; }

        public string showInAdvancedViewOnly { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string systemFlags { get; set; }

        public string objectCategory { get; set; }

        public string isCriticalSystemObject { get; set; }

        public string gPLink { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? Version { get; set; }
    }
}
