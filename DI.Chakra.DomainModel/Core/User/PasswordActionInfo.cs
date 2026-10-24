using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;

namespace DI.Chakra.DomainModel
{
    public class PasswordActionInfo : DomainModelBase<PasswordActionInfo, EntityModel.PasswordActionInfo>
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

        public static PasswordActionInfo GetLastActionByUserId(long userId, PasswordActionType actionType)
        {
            try
            {
                using (var transaction = new DataAccessTransaction())
                {
                    return ORMap(new PasswordActionRepository(transaction).GetLastActionByUserId(userId, actionType));
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
