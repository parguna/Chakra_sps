using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
namespace DI.Chakra.Dto.AdHocRequest.Presentation
{
    [Serializable]
    public class PresentationInterimAdhocRequest : DtoBase
    {
        public InterimStoreItemType InterimStoreItemType { get; set; }

        public long? TaskId { get; set; }

        public long? UserId { get; set; }
        public AdHocAccessRequest AdhocAccessRequest { get; set; }

        public bool IsSaved { get; set; }

        public bool IsActive { get; set; }

        public bool HasAttachment { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string AttachmentFileName { get; set; }
        public string AdditionalInfoFirst { get; set; }
        public string AccessItemType { get; set; }

        public string RequestedBy { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string AdhocActionTypeDesc
        {
            get
            {
                var desc = string.Empty;
                if (AdhocAccessRequest != null)
                {
                    desc = AdhocAccessRequest.AdhocActionType.GetDisplayAs();
                }
                return desc;
            }
        }
    }
}
