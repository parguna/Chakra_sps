using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPRoleToAuthMapRepository : RepositoryBase<SAPRoleToAuthMap>
    {
        public SAPRoleToAuthMapRepository(DataAccessTransaction dataAccessTransaction) : base(dataAccessTransaction) { }

        public List<string> GetListSAPTCode(string roleCode, string objectCode, string fieldCode)
        {
            return CurrentContext.SapRoleToAuthMap.Where(role => role.RoleName == roleCode
                                                     && role.ObjectCode == objectCode && role.FieldCode == fieldCode)
                                                     .Select(tcode => tcode.FromValue).ToList();
        }

        public List<string> LstRoleToAuth(string roleCode, string objectCode)
        {
            return CurrentContext.SapRoleToAuthMap.Where(role => role.RoleName == roleCode
                                                        && role.ObjectCode != objectCode)
                                                        .Select(auth => auth.AuthCode).Distinct().ToList();
        }


        public IList<PresentationSAPUsersToRolesReport> GetSAPUserToRolesReportData(int userId, long endpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    where sapUser.Id == userId && sapUser.EndPointSystemId == endpointSystemId
                    from sapRole in sapUser.SAPRoleInfo
                    where sapRole.EndPointSystemId == endpointSystemId
                    select new PresentationSAPUsersToRolesReport
                    {
                        UserId = sapUser.Id,
                        UserName = sapUser.UserLogonName,
                        DisplayName = sapUser.Firstname + sapUser.Lastname,
                        RoleCode = sapRole.ROLE_CODE,
                        RoleDescription = sapRole.ROLE_DESC,
                        IsActive = sapUser.IsActive,
                        Department = sapUser.Department


                    }).ToList();
        }



        public IList<PresentationSAPUsersToRolesReport> GetSAPUserToRolesReportData(long endpointSystemId)
        {
            try
            {

                string sqlQuery = string.Format("select 0, sapuser.UserLogonName as username, sapuser.firstname + sapuser.lastname as DisplayName, sapuser.Department, sapuser.IsActive, saprole.ROLE_CODE as RoleCode, saprole.ROLE_DESC as RoleDescription, userrole.Role_FromDate, userrole.Role_ToDate  from SAPRoleInfoSAPUserinfo userrole inner join di_sap_userinfo sapuser on userrole.SAPUserinfo_Id = sapuser.userid and sapuser.EndPointSystemId = {0} inner join di_sap_roleinfo saprole on userrole.SAPRoleInfo_Id = saprole.Id and saprole.EndPointSystemId = {0}", endpointSystemId);
                var userstoroles = CurrentContext.Database.SqlQuery<PresentationSAPUsersToRolesReport>
                                (sqlQuery, 0);


                return userstoroles.ToList();


                    //return (from sapUser in _context.SAPUserinfos
                    //        where sapUser.EndPointSystemId == EndpointSystemId
                    //        from sapRole in sapUser.SAPRoleInfo
                    //        where sapRole.EndPointSystemId == EndpointSystemId
                    //        select new SAPUsersToRolesReportData
                    //        {
                    //            UserId = sapUser.UserId,
                    //            UserName = sapUser.UserLogonName,
                    //            DisplayName = sapUser.Firstname + sapUser.Lastname,
                    //            RoleCode = sapRole.ROLE_CODE,
                    //            RoleDescription = sapRole.ROLE_DESC,
                    //            IsActive = sapUser.IsActive ? "YES" : "NO",
                    //            Department = sapUser.Department


                    //        }
                    //        )
                    //         .ToList();
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<PresentationSAPUsersToRolesReport> GetSAPUserToRolesReportDataIncludeUserId(long EndpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    from sapRole in sapUser.SAPRoleInfo
                    where (!string.IsNullOrEmpty(sapRole.ROLE_CODE) && sapUser.EndPointSystemId == EndpointSystemId)
                    select new PresentationSAPUsersToRolesReport
                    {
                        UserId = sapUser.Id,
                        UserName = sapUser.UserLogonName,
                        DisplayName = sapUser.Firstname + " " + sapUser.Lastname,
                        Department = sapUser.Department,

                        RoleCode = sapRole.ROLE_CODE,
                        RoleDescription = sapRole.ROLE_DESC
                    }
                         ).ToList();
        }
        public IList<SAPRoleToAuthMap> GetTCodesBySAPRole(string roleCode, long endpointSystemId)
        {
            return (from RoleToAuth in CurrentContext.SapRoleToAuthMap
                    where (RoleToAuth.RoleName == roleCode && RoleToAuth.ObjectCode == "S_TCODE"
                        && RoleToAuth.FieldCode == "TCD" && RoleToAuth.EndPointSystemId == endpointSystemId && RoleToAuth.ProfileGeneratorVariant != "X")
                    select RoleToAuth
                 ).ToList();
        }



        public Dictionary<long, string> GetAllAssignedTCodes(long endpointSystemId)
        {

            return (from authObject in CurrentContext.SapRoleToAuthMap
                    where authObject.EndPointSystemId == endpointSystemId && authObject.ObjectCode == "S_TCODE" && authObject.FieldCode == "TCD"
                    join tCodes in CurrentContext.SAPTCodeInfo on authObject.FromValue equals tCodes.TCODE_Code
                    select tCodes)
                    .Select(tCode => new { key = tCode.Id, value = tCode.TCODE_Code })
                    .Take(1000)
                    .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);

        }



        public List<PresentationSAPRoleToAuthorizationReport> GetSapRolesToAuthorizationData(string RoleCode, long EndpointSystemId)
        {
            string query = "select distinct roleauth.Id, roleauth.RoleName, roleauth.AuthCode,  roleauth.ObjectCode, roleauth.FieldCode, case when fieldvalue.FromValue is not null then fieldvalue.FromValue else roleauth.fromvalue end as FromValue, roleauth.ToValue from DI_SAP_RoleToAuthMap roleauth  left outer join DI_SAP_FIELDVALUEMAPPING fieldvalue on roleauth.FromValue = fieldvalue.FieldCode and fieldvalue.endpointsystemid = @EndpointSystemId and roleauth.rolename = fieldvalue.RoleCode and left(roleauth.fromvalue, 1) = '$'	where roleauth.endpointsystemid = @EndpointSystemId and roleauth.RoleName = @RoleCode and roleauth.ObjectCode != 'S_TCODE' and roleauth.ProfileGeneratorVariant != 'X'";
            var queryParameters = new[]
            {
                new System.Data.SqlClient.SqlParameter("@EndpointSystemId", EndpointSystemId),
                new System.Data.SqlClient.SqlParameter("@RoleCode", (object)RoleCode ?? DBNull.Value)
            };
            var RoleAuth = CurrentContext.Database.SqlQuery<PresentationSAPRoleToAuthorizationReport>(query, queryParameters);
            return RoleAuth.ToList();
        }



        public List<string> GetTCodesByPattern(string pattern, long endpointSystemId)
        {
            pattern = pattern.Replace("*", "%");
            //SELECT * FROM DI_SAP_TCODEINFO WHERE EndPointSystemId = 16 AND TCODE_Code LIKE 'pattern'
            string query = string.Format("SELECT * FROM [dbo].[DI_SAP_TCODEINFO] WHERE EndPointSystemId = {0} AND TCODE_Code LIKE '{1}'", endpointSystemId, pattern);
            return CurrentContext.Database.SqlQuery<SAPTCodeInfo>(query).Select(item => item.TCODE_Code).ToList();
        }

        public List<string> GetTCodesByRange(string fromValue, string toValue, long endpointSystemId)
        {
            //SELECT * FROM DI_SAP_TCODEINFO WHERE EndPointSystemId = 16 AND TCODE_Code BETWEEN 'SU51' AND 'SU56' ORDER BY TCODE_Code
            string query = string.Format("SELECT * FROM [dbo].[DI_SAP_TCODEINFO] WHERE EndPointSystemId = {0} AND TCODE_Code BETWEEN '{1}' AND '{2}'", endpointSystemId, fromValue, toValue);
            return CurrentContext.Database.SqlQuery<SAPTCodeInfo>(query).Select(item => item.TCODE_Code).ToList();
        }
    }
}
