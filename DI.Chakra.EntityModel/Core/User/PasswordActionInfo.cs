using System;
using System.ComponentModel.DataAnnotations.Schema;

using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_PasswordActionInfo")]
    public class PasswordActionInfo : EntityBase,ICreatable,IUpdatable
    {
        public long UserId { get; set; }
        public long EndpointUserId { get; set; }
        public long EndpointSystemId { get; set; }
        public PasswordActionType PasswordActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsSuccess { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
