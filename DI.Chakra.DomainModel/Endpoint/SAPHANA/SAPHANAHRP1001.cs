using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAHRP1001 : DomainModelBase<SAPHANAHRP1001, EntityModel.SAPHANAHRP1001>
    {
        public long EndpointSystemId { get; set; }

        public string ActualObjectType { get; set; }
        public string ActualObjectCode { get; set; }

        public string RelatedObjectType { get; set; }

        public string RelatedObjectCode { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static SAPHANAHRP1001 GetByActualObjectCode(SAPHANAHRP1001 hr1001)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(item => item.ActualObjectCode == hr1001.ActualObjectCode &&
                        item.EndpointSystemId == hr1001.EndpointSystemId && item.RelatedObjectCode == hr1001.RelatedObjectCode &&
                        item.ActualObjectType == hr1001.ActualObjectType && item.RelatedObjectType == hr1001.RelatedObjectType).FirstOrDefault());
            }
        }

        public static List<SAPHANAHRP1001> GetByEndpointSystemId(SAPHANAHRP1001 hr1001)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction)
                    .Get(item => item.ActualObjectCode == hr1001.ActualObjectCode &&
                        item.EndpointSystemId == hr1001.EndpointSystemId && item.RelatedObjectCode == hr1001.RelatedObjectCode).ToList());
            }
        }

        public static List<SAPHANAHRP1001> GetByEndpointSystemId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction)
                    .Get(item => item.ActualObjectType == "S" && item.EndpointSystemId == endpointSystemId && item.RelatedObjectType == "US").ToList());
            }
        }

        public static void DeleteByEndPointSystemId(SAPHANAHRP1001 hr1001)
        {
            List<SAPHANAHRP1001> sapHRPList = GetByEndpointSystemId(hr1001);
            if (sapHRPList != null)
                SAPHANAHRP1001.DeleteList(sapHRPList);
        }
        public static void DeleteHRP1001Dump(SAPHANAHRP1001 hr1001)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var itemToRemove = ORMap(new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(item => item.ActualObjectCode == hr1001.ActualObjectCode &&
                       item.EndpointSystemId == hr1001.EndpointSystemId && item.RelatedObjectCode == hr1001.RelatedObjectCode &&
                       item.ActualObjectType == hr1001.ActualObjectType && item.RelatedObjectType == hr1001.RelatedObjectType).FirstOrDefault());
                    if (itemToRemove != null)
                    {
                        itemToRemove.Delete();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> GetByActualObjectCode(string actualObjectCode, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.SAPHRP1001>(dataAccessTransaction)
                    .Get(sap => sap.EndpointSystemId == endpointSystemId && sap.ActualObjectCode == actualObjectCode &&
                    sap.RelatedObjectType == "AG").Select(sap => sap.RelatedObjectCode)).ToList();
            }
        }

        public static List<string> GetByRelatedObjectCode(string relatedObjectCode, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction)
                    .Get(sap => sap.EndpointSystemId == endpointSystemId && sap.RelatedObjectCode == relatedObjectCode &&
                    sap.RelatedObjectType == "US").Select(sap => sap.ActualObjectCode)).ToList();
            }
        }

        public static List<string> GetSAPRolesByPosition(string position, long endpointSystemId)
        {
            try
            {
                bool isParentExists = true;

                List<string> orgRoles = new List<string>();
                List<string> roles = GetRolesByPosition(position, endpointSystemId);

                var org = GetOrgByPosition(position, endpointSystemId);

                if (org != null)
                {
                    orgRoles = GetRolesByOrg(org.RelatedObjectCode, endpointSystemId);

                    if (orgRoles != null && orgRoles.Count > 0)
                    {
                        roles.AddRange(orgRoles);
                    }
                }

                org = GetParentOrgByOrg(org?.RelatedObjectCode, endpointSystemId);

                int i = 0;
                while (isParentExists)
                {
                    i++;
                    if (org != null && i <= 5)
                    {
                        orgRoles = GetRolesByOrg(org.ActualObjectCode, endpointSystemId);
                        if (orgRoles != null && orgRoles.Count > 0)
                        {
                            roles.AddRange(orgRoles);
                        }
                        org = GetParentOrgByOrg(org.ActualObjectCode, endpointSystemId);
                    }
                    else
                    {
                        isParentExists = false;
                    }
                }

                return roles != null ? roles.Distinct().ToList() : roles;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> GetRolesByPosition(string position, long endpointSystemId)
        {
            List<string> roles = new List<string>();
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    roles = new SAPHANAUserInfoRepository(dataAccessTransaction).GetRoleFromPosition(endpointSystemId, position);
                }
            }
            catch(Exception ex)
            {
            }
            return roles;
        }

        public static List<string> GetRolesByOrg(string orgUnit, long endpointSystemId)
        {
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(item => item.ActualObjectCode == orgUnit &&
                            item.EndpointSystemId == endpointSystemId && item.RelatedObjectType == "AG" &&
                            item.ActualObjectType == "O").Select(role => role.RelatedObjectCode).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public static SAPHANAHRP1001 GetOrgByPosition(string position, long endpointSystemId)
        {
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(item =>
                    item.EndpointSystemId == endpointSystemId && item.ActualObjectCode == position &&
                                    item.RelatedObjectType == "O" && item.ActualObjectType == "S").FirstOrDefault());
                }
            }
            catch
            {
                throw;
            }
        }

        public static SAPHANAHRP1001 GetParentOrgByOrg(string orgUnit, long endpointSystemId)
        {
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(item =>
                    item.EndpointSystemId == endpointSystemId && item.ActualObjectCode == orgUnit &&
                                    item.RelatedObjectType == "O" && item.ActualObjectType == "O").FirstOrDefault());
                }
            }
            catch
            {
                throw;
            }
        }

        public static List<string> GetPositionsByUser(string logonName, long endpointSystemId)
        {
            try
            {

                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(item => item.RelatedObjectCode == logonName &&
                            item.EndpointSystemId == endpointSystemId && item.RelatedObjectType == "US" &&
                            item.ActualObjectType == "S").Select(role => role.ActualObjectCode).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public static IList<SAPHANAHRP1001> GetByModifiedDate(DateTime? modifiedDate, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(saphrp1001 =>
                // if last modified date is null, check created date
                ((saphrp1001.LastModifiedDate == null && saphrp1001.CreatedDate < modifiedDate) || saphrp1001.LastModifiedDate < modifiedDate) && saphrp1001.EndpointSystemId == endpointSystemId).ToList());
            }
        }
        
        public static List<string> GetRolesByPosition(List<string> positions, long endpointSystemId)
        {
            List<string> roles = new List<string>();
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    roles = new RepositoryBase<EntityModel.SAPHANAHRP1001>(dataAccessTransaction).Get(item => positions.Contains(item.ActualObjectCode) &&
                            item.EndpointSystemId == endpointSystemId && item.RelatedObjectType == "AG" &&
                            item.ActualObjectType == "S").Select(role => role.RelatedObjectCode).ToList();
                }
            }
            catch
            {
            }
            return roles;
        }
    }
}
