using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationOrgChart :DtoBase
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public byte[] Image { get; set; }
        public Nullable<long> ParentId { get; set; }
        public OrgChartNodeType NodeType { get; set; }
        public long OrgId { get; set; }
        public string OrgHeadCaption { get; set; }
    }
}
