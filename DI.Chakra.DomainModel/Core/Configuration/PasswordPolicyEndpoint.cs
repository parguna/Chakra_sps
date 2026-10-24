using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class PasswordPolicyEndpoint : DomainModelBase<PasswordPolicyEndpoint, EntityModel.PasswordPolicyEndpoint>
    {
        public long PolicyId { get; set; }

        public long EndpointSystemId { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public EndPoint Endpoint { get; set; }

        public PasswordPolicy PasswordPolicy { get; set; }

        //Remove during end to end implementation if not required
        public static List<PasswordPolicyEndpoint> GetByPolicyId(long policyId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.PasswordPolicyEndpoint>(dataAccessTransaction).
                    Get(ppEndpoint => ppEndpoint.PolicyId == policyId, ppEndpoint => ppEndpoint.Endpoint).ToList());
            }
        }
        public static PasswordPolicyEndpoint GetEndpointPasswordPolicy(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.PasswordPolicyEndpoint>(dataAccessTransaction).
                    Get(ppEndpoint => ppEndpoint.EndpointSystemId == endpointSystemId, ppEndpoint => ppEndpoint.PasswordPolicy).FirstOrDefault());
            }
        }

        public static string GetIACMLastPasswordChangeByEndpointUser(long endPointSystemId, string userName)
        {
            string pwdLastModifiedDate = string.Empty; ;
            using (var transaction = new DataAccessTransaction())
            {
                long endPointSystemTypeId = EndPoint.GetById(endPointSystemId).EndpointSystemTypeId.Value;
                EndpointType endPointName = EndPointSystemType.GetById(endPointSystemTypeId).EndpointSystemTypeName;

                DateTime? iacmLastPasswordChange = null; 
                if (endPointName == EndpointType.ActiveDirectory)
                {
                    ADSUserInfo userInfo = null;
                    userInfo = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(userName, endPointSystemId);
                    if (userInfo != null)
                        iacmLastPasswordChange = userInfo.IACMLastPasswordChange;
                }
                else if (endPointName == EndpointType.SAP)
                {
                    SAPUserinfo userInfo = null;
                    userInfo = SAPUserinfo.GetByUserLogonName(userName, endPointSystemId);
                    if (userInfo != null)
                        iacmLastPasswordChange = userInfo.IACMLastPasswordChange;
                }
                
                else if (endPointName == EndpointType.SAPEnterprisePortal)
                {
                    SAPEPUserInfo userInfo = null;
                    userInfo = SAPEPUserInfo.GetBysAMAccountNameEndPointSystemId(userName, endPointSystemId);
                    if (userInfo != null)
                        iacmLastPasswordChange = userInfo.IACMLastPasswordChange;
                }
                if (iacmLastPasswordChange == null)
                    pwdLastModifiedDate = null;
                else
                    pwdLastModifiedDate = iacmLastPasswordChange.Value.ToString();
            }
            return pwdLastModifiedDate;
        }
        public new static void SaveList(List<PasswordPolicyEndpoint> passwordPolicyEndpointList, string currentUser)
        {
            using (var transaction = new DataAccessTransaction(currentUser))
            {
                long policyId = passwordPolicyEndpointList.FirstOrDefault().PolicyId;
                List<PasswordPolicyEndpoint> dbPasswordPolicyEndpointList = PasswordPolicyEndpoint.GetByPolicyId(policyId);

                if (dbPasswordPolicyEndpointList.Count > 0)
                {
                    PasswordPolicyEndpoint.DeleteList(transaction, dbPasswordPolicyEndpointList);
                }                
                PasswordPolicyEndpoint.SaveList(transaction, passwordPolicyEndpointList);
                transaction.Commit();
            }
        }

    }
}
