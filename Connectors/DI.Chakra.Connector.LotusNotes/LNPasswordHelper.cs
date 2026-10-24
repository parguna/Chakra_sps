using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;

using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector.LotusNotes
{
    public class LNPasswordHelper
    {
        public string DBName { get; set; }

        public string ServerName { get; set; }

        public string UserName { get; set; }

        public string TempIdFilePath { get; set; }

        public string AdminIdPath { get; set; }

        public string AdminUserName { get; set; }

        public string AdminPassword { get; set; }

        public string NewPasswordToBeChanged { get; set; }

        public bool ChangePassword_Client(string tempPassword)
        {

            bool bChangePwdSuccess = false;

            try
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Entering - ChangePassword_Client");


                int error = ResetIdVaultPassword.resetIDVaultPassword(DBName, ServerName, UserName, tempPassword, TempIdFilePath, AdminIdPath, AdminUserName, AdminPassword);

                if (error == 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Info, string.Format("Notes Password RESET is done for the user {0}", UserName));
                    bChangePwdSuccess = true;
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Error, string.Format("FAILED: Notes Password RESET is Failed for the user {0}  Error Code: {1}", UserName, error.ToString()));
                    bChangePwdSuccess = false;
                }

                if (error == 0 && bChangePwdSuccess)
                {

                    error = ResetIdVaultPassword.changePassword(ServerName, UserName, DBName, TempIdFilePath, tempPassword, NewPasswordToBeChanged);



                    if (error == 0)
                    {
                        DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Info, string.Format("Notes Password CHANGE is done for the user {0}", UserName));
                        bChangePwdSuccess = true;
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Error, string.Format("FAILED: Notes Password CHANGE is Failed for the user {0} Error Code: {1}", UserName, error.ToString()));
                        bChangePwdSuccess = false;
                    }
                }

                error = ResetIdVaultPassword.switchID(AdminIdPath, AdminUserName, AdminPassword);

                DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Info, string.Format("Switch Id is done for User {0}", UserName));


               ushort s = ProcessAdminPReq.ProcessAdminRequest(ServerName, AdminIdPath, AdminUserName, AdminPassword);


            }
            catch (Exception ex) 
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Error, string.Format("Error in Change Password Method for  user {0}", UserName), ex);

                return false;
            }

            return bChangePwdSuccess;

        }
        
        //public static bool ResetPassword(string userName, string newPassword, string serverName, string adminPassword, out string msg)
        //{
        //    bool isPasswordChanged = false;
        //    msg = string.Empty;
        //    string adminUserName = ConfigurationConstant.LNS_IDVaultAdminName;
        //    string adminIDPath = ConfigurationConstant.LNS_IDVaultAdminIDFileValue;
        //    string dbName = string.Empty;

        //    try
        //    {
        //        int rtnCode = IDVault.ResetIdVaultPass.resetIdVaultPassword(dbName, serverName, userName, newPassword, adminIDPath, adminUserName, adminPassword);

        //        switch (rtnCode)
        //        {
        //            case 100: msg = "Error initializing Notes";
        //                break;
        //            case 101: msg = "Error Opening Database";
        //                break;
        //            case 102: msg = "Password dosen't changed, Failed to login as ID Vault Administrator";
        //                break;
        //            case 103: msg = "PASSWORD CHANGE FAILED, during reset Password";
        //                break;
        //            case 1: msg = "Failed due to some other reason";
        //                break;
        //            case 0: msg = "PASSWORD CHANGED Successfully";
        //                isPasswordChanged = true;
        //                break;
        //            default: msg = "Unrecognized Code: " + rtnCode.ToString();
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        isPasswordChanged = false;
        //        msg = ex.Message;
        //    }

        //    return isPasswordChanged;
        //}

        //public static bool UpdateUserLastPasswordChangeDate(string adminPassword, string lastName, string serverName, string usrInternetAddress, DateTime PasswdChangeDate)
        //{
        //    bool isDateUpdated = false;
        //    try
        //    {
        //        string adminUserName = ConfigurationConstant.LNS_IDVaultAdminName;
        //        isDateUpdated = UpdateLastUpdateTime.PasswordChangeDate.updateLastPasswordChangeDate(adminUserName, adminPassword, lastName, serverName, usrInternetAddress, PasswdChangeDate);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.UIErrorLog.WriteMessage(LogLevel.ERROR, "Exception occur in method UpdateUserLastPasswordChangeDate, ErrorMessage: " + ex.Message, ex);
        //        isDateUpdated = false;
        //    }
        //    return isDateUpdated;
        //}
    }
}
