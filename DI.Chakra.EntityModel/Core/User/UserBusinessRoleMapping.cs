using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Serializable]
    [Table("DI_M_UserBusinessRoleMapping")]
    public class UserBusinessRoleMapping : EntityBase, ICreatable, IUpdatable
    {
        public long UserId { get; set; }
        public long EndpointUserId { get; set; }
        public long EndpointSystemId { get; set; }
        public string BusinessRole { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
