using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.UserHelper
{
    public class IacmUserHelper
    {
        static string roleInsert = "INSERT INTO IACMRoleInfoIACMUserInfo VALUES({0},{1})";
        static string groupInsert = "INSERT INTO IACMUserInfoIACMGroupInfo VALUES({0},{1})";
        static string deleteUserGroup = "Delete from IACMUserInfoIACMGroupInfo where IACMUserInfo_Id ={0}";
        static string deleteUserRole = "Delete from IACMRoleInfoIACMUserInfo where IACMUserInfo_Id ={0}";

        public static IACMUserInfo CreateAndSaveIacmUserInfo(User diUserItem, IList<IACMUserInfo> listExistingADusers, EndPoint endPoint, IdmsLogger idmsLogger)
        {
            IACMUserInfo iacmUserInfo = null;
            bool fetchFromDB = false;
            if (listExistingADusers == null)
            {
                fetchFromDB = true;
            }

            if (idmsLogger == null)
                idmsLogger = DILog.ProvisioningLogger;

            string executeQuery = "";

            if (endPoint == null)
                endPoint = EndPoint.GetByApplicationType(Infrastructure.Enums.ApplicationType.IDMS);


            List<IACMRoleInfo> rolesToAdd = new List<IACMRoleInfo>();
            List<IACMGroupInfo> grpsToAdd = new List<IACMGroupInfo>();
            User userGroup = null;
            User userRoles = null;

            bool isExist = true;
            try
            {

                if (!string.IsNullOrEmpty(diUserItem.AccountName))
                {
                    if (fetchFromDB)
                        iacmUserInfo = IACMUserInfo.GetByUserName(diUserItem.AccountName, false);
                    else
                        iacmUserInfo = listExistingADusers.Where(e => e.AccountName == diUserItem.AccountName).FirstOrDefault();
                }

                if (iacmUserInfo == null  && !string.IsNullOrEmpty(diUserItem.EmployeeId))
                {
                    if (fetchFromDB)
                        iacmUserInfo = IACMUserInfo.GetByEmployeeId(diUserItem.EmployeeId);
                    else
                        iacmUserInfo = listExistingADusers.Where(e => (e.EmployeeId == diUserItem.EmployeeId)).FirstOrDefault();
                }

                if ( iacmUserInfo == null && !string.IsNullOrEmpty(diUserItem.IdentificationNo))
                {
                    if (fetchFromDB)
                        iacmUserInfo = IACMUserInfo.GetByIdentificationNo(diUserItem.IdentificationNo);
                    else
                        iacmUserInfo = listExistingADusers.Where(e => (e.NRIC == diUserItem.IdentificationNo)).FirstOrDefault();
                }

                if (iacmUserInfo != null && string.IsNullOrEmpty(diUserItem.AccountName) && string.IsNullOrEmpty(diUserItem.EmployeeId) && string.IsNullOrEmpty(diUserItem.IdentificationNo))
                    return null;

                if (iacmUserInfo == null)
                {
                    isExist = false;
                    iacmUserInfo = new IACMUserInfo();
                    iacmUserInfo.AccountCreatedDate = iacmUserInfo.CreatedDate = DateTime.Now;
                }
                if(iacmUserInfo.Id > 0)
                {
                    iacmUserInfo.AccountCreatedDate = iacmUserInfo.CreatedDate;
                }
                
                iacmUserInfo.EndPointSystemId = endPoint.Id;
                iacmUserInfo.NRIC = diUserItem.IdentificationNo;
                iacmUserInfo.EmployeeId = diUserItem.EmployeeId;
                iacmUserInfo.Email = diUserItem.EmailId;
                iacmUserInfo.UserName = diUserItem.UserName;
                iacmUserInfo.DisplayName = diUserItem.DisplayName;
                iacmUserInfo.FirstName = diUserItem.FirstName;
                iacmUserInfo.LastName = diUserItem.LastName;
                iacmUserInfo.ADID = diUserItem.UserName;
                iacmUserInfo.LastLoginDate = diUserItem.LastLogonDate;
                iacmUserInfo.DeletedDate = diUserItem.TerminationDate;
                iacmUserInfo.IsDeleted = iacmUserInfo.IsDisabled = !diUserItem.IsActive;
                iacmUserInfo.Save("System");

                var query = string.Format(deleteUserGroup, iacmUserInfo.Id);
                IACMRoleInfo.ExecuteQuery(query);
                grpsToAdd = new List<IACMGroupInfo>();
                userGroup = User.GetByIdIncludeGroup(diUserItem.Id);

                if (userGroup != null && userGroup.Groups != null && userGroup.Groups.Count() > 0)
                {
                    foreach (var grp in userGroup.Groups)
                    {
                        var executeGroupQuery = "";
                        var iacmgroup = IACMGroupInfo.GetByGroupName(grp.GroupName);
                        if (iacmgroup != null)
                        {
                            grpsToAdd.Add(iacmgroup);
                            executeGroupQuery = string.Format(groupInsert, iacmUserInfo.Id, iacmgroup.Id);
                            IACMRoleInfo.ExecuteQuery(executeGroupQuery);
                        }
                    }
                }

                iacmUserInfo.Groups = grpsToAdd;


                var query1 = string.Format(deleteUserRole, iacmUserInfo.Id);
                IACMRoleInfo.ExecuteQuery(query1);
                rolesToAdd = new List<IACMRoleInfo>();
                userRoles = User.GetByIdIncludeRoles(diUserItem.Id);

                if (userRoles != null && userRoles.Roles != null && userRoles.Roles.Count() > 0)
                {
                    foreach (var role in userRoles.Roles)
                    {
                        var executeRoleQuery = "";
                        var iacmrole = IACMRoleInfo.GetByRoleName(role.RoleName);
                        if (iacmrole != null)
                        {
                            executeRoleQuery = string.Format(roleInsert, iacmrole.Id, iacmUserInfo.Id);
                            IACMRoleInfo.ExecuteQuery(executeRoleQuery);
                            rolesToAdd.Add(iacmrole);
                        }
                    }
                }
                iacmUserInfo.Roles = rolesToAdd;


            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "Exception at IACM Discovery", ex);
            }

            return iacmUserInfo;

        }

        public static void MapUserIacmUser(EndPoint endPoint, bool mapAll, long discoveryProcessId, string commaSeparatedUserIds = null)
        {
            if (endPoint == null)
                endPoint = EndPoint.GetByApplicationType(Infrastructure.Enums.ApplicationType.IDMS);

            IACMUserInfo.MapWithMasterUserForReconTable(endPoint, mapAll, discoveryProcessId, commaSeparatedUserIds);
            IACMUserInfo.MapWithMasterUserForEntitlementReconTable(endPoint, mapAll, discoveryProcessId, commaSeparatedUserIds);
        }
    }
}
