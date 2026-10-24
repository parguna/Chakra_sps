using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.Model;
using DeepIdentity.DataAccess;
using DeepIdentity.Model.SAP;
using DeepIdentity.DataAccess.Implementation;

namespace DeepIdentity.Facade
{
    public class AutoCompleteDataFacade
    {
        EndPointFacade epFacade = new EndPointFacade();
        private UserDA UserDA = new UserDA();
        ACLDA aclda = new ACLDA();

        public Dictionary<int, string> GetSingleEntitlements(string searchString, int? EndpointUserId, bool existingEntitlements)
        {
            IList<Dictionary<int, string>> listSingleEntitlements = new List<Dictionary<int, string>>();

            Dictionary<int, string> retSingleEntitlements = new Dictionary<int, string>();

            try
            {
                List<EndPoint> epList = epFacade.GetAllEndpoint();

                foreach (var item in epList)
                {
                    retSingleEntitlements = GetSingleEntitlements(searchString, item.EndpointSystemId, EndpointUserId, existingEntitlements);

                    listSingleEntitlements.Add(retSingleEntitlements);
                }
            }
            catch
            {
                throw;
            }

            return null;
        }
        public Dictionary<int, string> GetSingleEntitlements(string searchString, int EndpointSystemId, int? EndpointUserId, bool existingEntitlements)
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
                        .Select(x => new { Key = x.GroupId, Value = x.name })
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
                    RoleMatrixDA roleMatrixDA = new RoleMatrixDA();
                    AdHocRequestDA adhocDA = new AdHocRequestDA();

                    if (existingEntitlements)
                        return roleMatrixDA.GetSAPRoleCatalog(EndpointUserId.Value);
                    else
                        return adhocDA.GetBusinessRoleFromRoleCatalog(searchString, string.Empty, string.Empty);


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
                                return ReadObjects.Where(c => c.LinuxUserInfo.Count(d => d.UserId == EndpointUserId) > 0).ToDictionary(c => c.Id, c => c.GroupName);
                            else
                                return ReadObjects.Where(c => c.LinuxUserInfo.Count(d => d.UserId == EndpointUserId) == 0).ToDictionary(c => c.Id, c => c.GroupName);
                        }
                    }
                    else
                        return new Dictionary<int, string>();

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
        public Dictionary<int, string> GetReportingManager(string prefix)
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
        public Dictionary<int, string> GetOrganisations(string searchString)
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
        public Dictionary<int, string> GetCostCenters(string searchString, int companyId)
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
        public Dictionary<string, string> GetAllTenantList()
        {
            return UserDA.GetAllTenantList();
        }
        public Dictionary<string, string> GetAllDepartmentsList(string tenantID)
        {
            //return UserDA.GetAllDepartmentsList(tenantID);
            return null;
        }
        public Dictionary<int, string> GetUsersByGroup(string searchString, string groupName)
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
        public Dictionary<int, string> GetUsersBySearchString(string searchString)
        {
            return UserDA.GetUsersBySearchString(searchString);
        }
        public Dictionary<int, string> GetActiveAndNonTerminatedUsers(string searchString)
        {
            return UserDA.GetActiveAndNonTerminatedUsers(searchString);
        }
        public Dictionary<int, string> GetNonReportingUsersOfThisManagerID(int ManagerID, string SearchingString)
        {
            return UserDA.GetNonReportingUsersOfThisManagerID(ManagerID, SearchingString);
        }

        public Dictionary<int, string> GetReportingUsersOfThisManagerIDandAdmin(int ManagerID, string searchString)
        {
            return UserDA.GetReportingUsersOfThisManagerIDandAdmin(ManagerID, searchString);
        }

        public Dictionary<int, string> GetReportingofSubordinate(int UserID, string searchString, List<int> CompanyId, List<int> DepartmentId, List<int> DivisionId, List<int> OrganisationId, List<string> SectionId)
        {
            return UserDA.GetReportingofSubordinate(UserID, searchString, CompanyId, DepartmentId, DivisionId, OrganisationId, SectionId);
        }
        public Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            return UserDA.GetDisticntSectionFromUsers(searchString);
        }

        #region "ACL"
        
        public Dictionary<int, string> GetUrlsBySearchString(string searchString)
        {
            return aclda.GetUrlsBySearchString(searchString);
        }
        
        public Dictionary<int, string> GetPagesBySearchString(string searchString)
        {
            return aclda.GetPagesBySearchString(searchString);
        }
       
        public Dictionary<int, string> GetMenusBySearchString(string searchString)
        {
            return aclda.GetMenusBySearchString(searchString);
        }
        
        #endregion
    }
}
