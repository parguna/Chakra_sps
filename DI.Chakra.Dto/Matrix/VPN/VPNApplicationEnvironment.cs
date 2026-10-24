using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.Dto
{
    public class VPNApplicationEnvironment : DtoBase
    {
        public string Name { get; set; }
        public long MatrixId { get; set; }
        public VPNMatrixType MatrixType { get; set; }
        public VPNMatrixEnviornmentType EnviornmentType { get; set; }
        public string ADGroups { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        
    }
}
