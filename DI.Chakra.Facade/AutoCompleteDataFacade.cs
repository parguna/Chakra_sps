using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Model;
using DI.Chakra.DataAccess;
using DI.Chakra.Model.SAP;
using DI.Chakra.DataAccess.Implementation;

namespace DI.Chakra.Facade
{
    public class AutoCompleteDataFacade
    {
        EndPointFacade epFacade = new EndPointFacade();
        private UserDA UserDA = new UserDA();
        ACLDA aclda = new ACLDA();

        public Dictionary<long, string> GetSingleEntitlements(string searchString, long? EndpointUserId, bool existingEntitlements)
        {
            IList<Dictionary<long, string>> listSingleEntitlements = new List<Dictionary<long, string>>();

            Dictionary<long, string> retSingleEntitlements = new Dictionary<long, string>();

            try
            {
                List<EndPoint> epList = epFacade.GetAllEndpoint();

                foreach (var item in epList)
                {
                    retSingleEntitlements = GetSingleEntitlements(searchString, item.Id, EndpointUserId, existingEntitlements);

                    listSingleEntitlements.Add(retSingleEntitlements);
                }
            }
            catch
            {
                throw;
            }

            return null;
        }
        public Dictionary<long, string> GetSingleEntitlements(string searchString, long EndpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            try
            {
                EndPoint ep = epFacade.GetEndPointSystemByKey(EndpointSystemId);

                if (ep.EndPointSystemType.EndpointSystemTypeName == Infrastructure.Enums.EndpointType.ActiveDirectory)
                {
                    ActiveDirectoryDA adDA = new ActiveDirectoryDA();
                    IList<ADSGroupInfo> list = null;

                    if (existingEntitlements)
                        list = adDA.GetADGroupsByUserId(EndpointUserId.Value, EndpointSystemId);
                    else
                        list = adDA.GetAllADSGroupsByEndpointSystem(EndpointSystemId);

                    return list.Where(item => item.sAMAccountName.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.name })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
                if (ep.EndPointSystemType.EndpointSystemTypeName == Infrastructure.Enums.EndpointType.LotusNotes)
                {
                    LotusNotesDA lnDA = new LotusNotesDA();
                    IList<LotusGroupInfo> list = null;

                    if (existingEntitlements)
                        list = lnDA.GetLNSGroupsByUserId(EndpointUserId.Value, EndpointSystemId);
                    else
                        list = lnDA.GetAllLNSGroupInfo();

                    return list.Where(item => item.Group.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.Group })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
                if (ep.EndPointSystemType.EndpointSystemTypeName == Infrastructure.Enums.EndpointType.SAP)
                {
                    //RoleMatrixDA roleMatrixDA = new RoleMatrixDA();
                    //AdHocRequestDA adhocDA = new AdHocRequestDA();

                    //if (existingEntitlements)
                    //    return roleMatrixDA.GetSAPRoleCatalog(EndpointUserId.Value);
                    //else
                    //    return adhocDA.GetBusinessRoleFromRoleCatalog(searchString, string.Empty, string.Empty);


                }


                if (ep.EndPointSystemType.EndpointSystemTypeName == Infrastructure.Enums.EndpointType.Linux)
                {
                    EndPointDA roleMatrixDA = new EndPointDA();
                    object obj = null;

                    bool Result = roleMatrixDA.getDatabyFunction<LinuxGroupInfo>(new Func<LinuxGroupInfo, bool>(c => c.EndPointSystemId == EndpointSystemId &&
                            c.GroupName.Contains(searchString)), out obj, true, "LinuxUserInfo");

                    if (Result)
                    {
                        var ReadObjects = (List<LinuxGroupInfo>)obj;

                        if (ReadObjects != null)
                        {
                            if (existingEntitlements)
                                return ReadObjects.Where(c => c.LinuxUserInfo.Count(d => d.Id == EndpointUserId) > 0).ToDictionary(c => c.Id, c => c.GroupName);
                            else
                                return ReadObjects.Where(c => c.LinuxUserInfo.Count(d => d.Id == EndpointUserId) == 0).ToDictionary(c => c.Id, c => c.GroupName);
                        }
                    }
                    else
                        return new Dictionary<long, string>();

                }

                //if (ep.EndPointSystemType.EndpointSystemTypeName == Infrastructure.Enums.EndpointType.OracleEBS)
                //{
                //    OracleEBSDA roleMatrixDA = new OracleEBSDA();
                //    AdHocRequestDA adhocDA = new AdHocRequestDA();
                //    if (existingEntitlements)
                //        return roleMatrixDA.Get(EndpointUserId.Value);
                //    else
                //        return adhocDA.GetBusinessRoleFromRoleCatalog(searchString, string.Empty, string.Empty);
                //}
            }
            catch
            {
                throw;
            }

            return null;
        }
        public Dictionary<string, string> GetReferenceUser(string prefix)
        {
            try
            {
                return UserDA.GetReferenceUser(prefix);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<string, string> GetCompanyName(string prefix)
        {
            try
            {
                return UserDA.GetCompanyName(prefix);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<string, string> GetDepartmentName(string prefix)
        {
            try
            {
                return UserDA.GetDepartmentName(prefix);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<string, string> GetDivisionName(string searchString, int departmentId)
        {
            try
            {
                return UserDA.GetDivisionName(searchString, departmentId);

            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetReportingManager(string prefix)
        {
            try
            {
                return UserDA.GetReportingManager(prefix);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<string, string> GetPosition(string prefix)
        {
            try
            {
                return UserDA.GetPosition(prefix);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, string> GetOrganisations(string searchString)
        {
            try
            {
                return UserDA.GetOrganisations(searchString);

            }
            catch
            {
                throw;
            }
        }
        public Dictionary<string, string> GetOrganisationsWithPrefix(string searchString)
        {
            try
            {
                return UserDA.GetOrganisationsWithPrefix(searchString);

            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetCostCenters(string searchString, int companyId)
        {
            try
            {
                return UserDA.GetCostCenters(searchString, companyId);

            }
            catch
            {
                throw;
            }
        }
        //public Dictionary<string, string> GetAllTenantList()
        //{
        //    return UserDA.GetAllTenantList();
        //}
        public Dictionary<string, string> GetAllDepartmentsList(string tenantID)
        {
            //return UserDA.GetAllDepartmentsList(tenantID);
            return null;
        }
        public Dictionary<long, string> GetUsersByGroup(string searchString, string groupName)
        {
            try
            {
                return UserDA.GetUsersByGroup(searchString, groupName);

            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetUsersBySearchString(string searchString)
        {
            return UserDA.GetUsersBySearchString(searchString);
        }
        public Dictionary<long, string> GetActiveAndNonTerminatedUsers(string searchString)
        {
            return UserDA.GetActiveAndNonTerminatedUsers(searchString);
        }
        public Dictionary<long, string> GetNonReportingUsersOfThisManagerID(long ManagerID, string SearchingString)
        {
            return UserDA.GetNonReportingUsersOfThisManagerID(ManagerID, SearchingString);
        }

        public Dictionary<long, string> GetReportingUsersOfThisManagerIDandAdmin(long ManagerID, string searchString)
        {
            return UserDA.GetReportingUsersOfThisManagerIDandAdmin(ManagerID, searchString);
        }

        public Dictionary<long, string> GetReportingofSubordinate(long UserID, string searchString, List<long> CompanyId, List<long> DepartmentId, List<long> DivisionId, List<long> OrganisationId, List<string> SectionId)
        {
            return UserDA.GetReportingofSubordinate(UserID, searchString, CompanyId, DepartmentId, DivisionId, OrganisationId, SectionId);
        }
        public Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            return UserDA.GetDisticntSectionFromUsers(searchString);
        }

        #region "ACL"
        
        public Dictionary<long, string> GetUrlsBySearchString(string searchString)
        {
            return aclda.GetUrlsBySearchString(searchString);
        }
        
        public Dictionary<long, string> GetPagesBySearchString(string searchString)
        {
            return aclda.GetPagesBySearchString(searchString);
        }
       
        public Dictionary<long, string> GetMenusBySearchString(string searchString)
        {
            return aclda.GetMenusBySearchString(searchString);
        }
        
        #endregion
    }
}
