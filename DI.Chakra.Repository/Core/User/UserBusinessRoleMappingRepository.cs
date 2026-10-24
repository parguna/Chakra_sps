using DI.Chakra.DBClient;
using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class UserBusinessRoleMappingRepository : RepositoryBase<UserBusinessRoleMapping>
    {
        public UserBusinessRoleMappingRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<string> GetAllBussRoleMappingRecordsForUser(long endpointUserId, long endpointSystemId)
        {
            try
            {
                return CurrentContext.UserBusinessRoleMapping.Where(item => item.EndpointUserId == endpointUserId &&
                      item.EndpointSystemId == endpointSystemId).Select(item => item.BusinessRole).ToList();

            }
            catch
            {
                throw;
            }
        }
        public void RemoveUserBussRoleMappingRecord(long userId, long endpointUserId, string businessRole, long endpointsystemid)
        {
            try
            {

                UserBusinessRoleMapping existingRoleMapping = CurrentContext.UserBusinessRoleMapping.FirstOrDefault(item => item.BusinessRole == businessRole &&
                    item.UserId == userId && item.EndpointUserId == endpointUserId && item.EndpointSystemId == endpointsystemid);
                if (existingRoleMapping != null)
                {
                    CurrentContext.UserBusinessRoleMapping.Remove(existingRoleMapping);
                    CurrentContext.SaveChanges();
                }

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void RemoveAllUserBussRoleMappingRecord(long userId, long endpointUserId, long endpointsystemid)
        {
            try
            {

                var existingRoleMapping = CurrentContext.UserBusinessRoleMapping.Where(item =>
                    item.UserId == userId && item.EndpointUserId == endpointUserId && item.EndpointSystemId == endpointsystemid).ToList();

                foreach (var item in existingRoleMapping)
                {
                    CurrentContext.UserBusinessRoleMapping.Remove(item);
                    CurrentContext.SaveChanges();
                }

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public List<UserBusinessRoleMapping> GetBusinessRoleByUserName(long endpointSystemId,long accountReconId, string accountName)
        {
            return (from actRecon in CurrentContext.AccountReconcilations
                        join usrRoleMap in CurrentContext.UserBusinessRoleMapping on actRecon.EndpointUserId equals usrRoleMap.EndpointUserId
                        where actRecon.Id == accountReconId && actRecon.EndpointSystemId == endpointSystemId && actRecon.AccountName == accountName
                    select usrRoleMap).ToList();
        }
    }
}
