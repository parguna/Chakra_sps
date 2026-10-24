using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_ComplianceType")]
    public class ComplianceType
    {
        [Key]
        public long Id { get; set; }

        public string PolicyTypeName { get; set; }
    }
}
