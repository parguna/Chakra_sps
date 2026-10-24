using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class UserBusinessRoleMapping : DtoBase
    {
        public int UserId { get; set; }
        public int EndpointUserId { get; set; }
        public int EndpointSystemId { get; set; }
        public string BusinessRole { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
