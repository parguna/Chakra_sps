using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model
{
    public class RequestGridData
    {
        public long AccessRequestId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public AccessRequestSource RequestSource { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRequestedToRemove { get; set; }
        public bool IsNewUser { get; set; }

        public string RequestType
        {
            get
            {
                if (this.IsRequestedToRemove)
                {
                    return "Remove";
                }
                else
                {
                    return "Create";
                }
            }
        }

        public string Status
        {
            get
            {
                if (this.RequestStatus == AccessRequestStatus.Requested)
                {
                    return "Pending";
                }
                else if (this.RequestStatus == AccessRequestStatus.Rejected)
                {
                    return "Rejected";
                }
                else
                {
                    return "Approved";
                }
            }
        }

    }
}
