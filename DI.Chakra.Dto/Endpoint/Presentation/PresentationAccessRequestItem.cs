using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationAccessRequestItem : IAccessRequestItem
    {

        public PresentationAccessRequestItem()
        {
            this.IsApproved = true;
            this.AdditionalInfo = string.Empty;
            this.AccountType = AccountType.User;
            this.ACRCategory = ACRCategory.Default;
        }
        public long EndpointUserId { get; set; }
        public string EndpointAccountName { get; set; }
        public AccessRequestItemType AccessRequestItemType { get; set; }
        public AccessItemType AccessItemType { get; set; }
        public long KeyId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointSystemTypeId { get; set; }
        public string EndpointName { get; set; }
        public string EndpointCode { get; set; }
        public EndpointType EndpointType { get; set; }
        public bool IsMockEndpoint { get; set; }
        public string ItemDisplayName { get; set; }
        public string ItemName { get; set; }
        public string ItemDetail { get; set; }
        public string ItemDescription { get; set; }
        public string ItemDetailDescription { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public bool IsApproved { get; set; }

        public AccountType AccountType { get; set; }
        public string AdditionalInfo { get; set; }
        public ACRCategory ACRCategory { get; set; }
        public long RequestFor { get; set; }
        public bool Enabled { get; set; }

        public bool RemoveAccess { get; set; }
        public dynamic AdditionalInfoObj
        {
            get
            {
                if (AdditionalInfo != null)
                    return JsonConvert.DeserializeObject(AdditionalInfo);

                else
                    return null;
            }
        }
        public long? ParentAccessRequestId { get; set; }
    }
}
