using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_SAPHRP1001")]
    public class SAPHRP1001
    {
        [Key]
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public string ActualObjectCode { get; set; }
        public string RelatedObjectType { get; set; }
        public string RelatedObjectCode { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }
}
