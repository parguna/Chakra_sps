using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Serialization;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class UserBusinessRoleMapping : DomainModelBase<UserBusinessRoleMapping, EntityModel.UserBusinessRoleMapping>
    {
        public long UserId { get; set; }
        public long EndpointUserId { get; set; }
        public long EndpointSystemId { get; set; }
        public string BusinessRole { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }


        public static List<string> GetAllBussRoleMappingRecordsForUser(long endpointuserId, long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return (new UserBusinessRoleMappingRepository(transaction).GetAllBussRoleMappingRecordsForUser(endpointuserId, endpointSystemId));
            }
        }

        public static UserBusinessRoleMapping GetBussRoleMappingbyRoleName(long endpointuserId, long endpointSystemId, string businessRole)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.UserBusinessRoleMapping>(transaction)
                    .Get(ueb => ueb.EndpointSystemId == endpointSystemId
                    && ueb.EndpointUserId == endpointuserId && ueb.BusinessRole.Trim().Equals(businessRole.Trim(),StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault());

            }
        }

        public static void RemoveUserBussRoleMappingRecord(UserBusinessRoleMapping roleMapping)
        {
            using (var transaction = new DataAccessTransaction())
            {
                new UserBusinessRoleMappingRepository(transaction).RemoveUserBussRoleMappingRecord(roleMapping.UserId, roleMapping.EndpointUserId, roleMapping.BusinessRole, roleMapping.EndpointSystemId);
                transaction.Commit();
            }
        }

        public static void RemoveAllUserBussRoleMappingRecord(long endpointUserId, long endPointId, long userId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                new UserBusinessRoleMappingRepository(transaction).RemoveAllUserBussRoleMappingRecord(userId, endpointUserId, endPointId);
                transaction.Commit();
            }
        }

        public static List<UserBusinessRoleMapping> GetBusinessRoleForUser(long accountReconId, long endpointSystemId, string accountName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new UserBusinessRoleMappingRepository(transaction).GetBusinessRoleByUserName(endpointSystemId, accountReconId, accountName));
            }
        }
    }
}


