using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_XLApproverExceptions")]
    public class ApproverException
    {
        [Key]
        public long Id { get; set; }
        public string Person { get; set; }
        public string Approver { get; set; }
        public int ApprovalLevel { get; set; }
    }
}
