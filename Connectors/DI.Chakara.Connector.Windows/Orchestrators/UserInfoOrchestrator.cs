using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;

namespace DI.Chakra.Connector
{
    public class UserInfoOrchestrator
    {
        
        private const string PROCESSNAME = "DiscoveryProcess";
        public UserInfoOrchestrator()
        {
            
        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public WindowsUserHistoryInfo PopulateUser(DirectoryEntry directoryEntry, string sid)
        {
            if (null == directoryEntry)
                return null;


            IList<WindowsUserHistoryInfo> list = new List<WindowsUserHistoryInfo>();

            WindowsUserHistoryInfo winUserInfo = new WindowsUserHistoryInfo();

            try
            {
                winUserInfo.UserId = 1;
                winUserInfo.HistoryId = this.HistoryId;
                winUserInfo.EndPointSystemId = EndpointSystemId;
                winUserInfo.UserFlags = WinHelper.GetProperty(directoryEntry, WinProperties.UserFlags);
                winUserInfo.MaxStorage = WinHelper.GetProperty(directoryEntry, WinProperties.MaxStorage);
                winUserInfo.PasswordAge = WinHelper.GetProperty(directoryEntry, WinProperties.PasswordAge);
                winUserInfo.PasswordExpired = WinHelper.GetProperty(directoryEntry, WinProperties.PasswordExpired);
                winUserInfo.LoginHours = WinHelper.GetProperty(directoryEntry, WinProperties.LoginHours);
                winUserInfo.FullName = WinHelper.GetProperty(directoryEntry, WinProperties.FullName);
                winUserInfo.Description = WinHelper.GetProperty(directoryEntry, WinProperties.Description);
                winUserInfo.BadPasswordAttempts = WinHelper.GetProperty(directoryEntry, WinProperties.BadPasswordAttempts);
                winUserInfo.LastLogin = WinHelper.GetProperty(directoryEntry, WinProperties.LastLogin);
                winUserInfo.HomeDirectory = WinHelper.GetProperty(directoryEntry, WinProperties.HomeDirectory);
                winUserInfo.LoginScript = WinHelper.GetProperty(directoryEntry, WinProperties.LoginScript);
                winUserInfo.Profile = WinHelper.GetProperty(directoryEntry, WinProperties.Profile);
                winUserInfo.HomeDirDrive = WinHelper.GetProperty(directoryEntry, WinProperties.HomeDirDrive);
                winUserInfo.Parameters = WinHelper.GetProperty(directoryEntry, WinProperties.Parameters);
                winUserInfo.PrimaryGroupID = WinHelper.GetProperty(directoryEntry, WinProperties.PrimaryGroupID);
                winUserInfo.Name = WinHelper.GetProperty(directoryEntry, WinProperties.Name);
                winUserInfo.MinPasswordLength = WinHelper.GetProperty(directoryEntry, WinProperties.MinPasswordLength);
                winUserInfo.MaxPasswordAge = WinHelper.GetProperty(directoryEntry, WinProperties.MaxPasswordAge);
                winUserInfo.MinPasswordAge = WinHelper.GetProperty(directoryEntry, WinProperties.MinPasswordAge);
                winUserInfo.PasswordHistoryLength = WinHelper.GetProperty(directoryEntry, WinProperties.PasswordHistoryLength);
                winUserInfo.AutoUnlockInterval = WinHelper.GetProperty(directoryEntry, WinProperties.AutoUnlockInterval);
                winUserInfo.LockoutObservationInterval = WinHelper.GetProperty(directoryEntry, WinProperties.LockoutObservationInterval);
                winUserInfo.MaxBadPasswordsAllowed = WinHelper.GetProperty(directoryEntry, WinProperties.MaxBadPasswordsAllowed);
                winUserInfo.objectSid = sid;
                winUserInfo.AccountType = "Local";
           
                }                
            
            catch (Exception ex)
            {
                throw ex;
            }
            
            return winUserInfo;

        }


        public WindowsUserInfo PopulateUserInfo(DirectoryEntry directoryEntry, string sid)
        {
            if (null == directoryEntry)
                return null;

            IList<WindowsUserInfo> list = new List<WindowsUserInfo>();
            WindowsUserInfo winUserInfo = new WindowsUserInfo();

            try
            {
                winUserInfo.EndPointSystemId = EndpointSystemId;
                winUserInfo.UserFlags = WinHelper.GetProperty(directoryEntry, WinProperties.UserFlags);
                winUserInfo.MaxStorage = WinHelper.GetProperty(directoryEntry, WinProperties.MaxStorage);
                winUserInfo.PasswordAge = WinHelper.GetProperty(directoryEntry, WinProperties.PasswordAge);
                winUserInfo.PasswordExpired = WinHelper.GetProperty(directoryEntry, WinProperties.PasswordExpired);
                winUserInfo.LoginHours = WinHelper.GetProperty(directoryEntry, WinProperties.LoginHours);
                winUserInfo.FullName = WinHelper.GetProperty(directoryEntry, WinProperties.FullName);
                winUserInfo.Description = WinHelper.GetProperty(directoryEntry, WinProperties.Description);
                winUserInfo.BadPasswordAttempts = WinHelper.GetProperty(directoryEntry, WinProperties.BadPasswordAttempts);
                winUserInfo.LastLogin = WinHelper.GetProperty(directoryEntry, WinProperties.LastLogin);
                winUserInfo.HomeDirectory = WinHelper.GetProperty(directoryEntry, WinProperties.HomeDirectory);
                winUserInfo.LoginScript = WinHelper.GetProperty(directoryEntry, WinProperties.LoginScript);
                winUserInfo.Profile = WinHelper.GetProperty(directoryEntry, WinProperties.Profile);
                winUserInfo.HomeDirDrive = WinHelper.GetProperty(directoryEntry, WinProperties.HomeDirDrive);
                winUserInfo.Parameters = WinHelper.GetProperty(directoryEntry, WinProperties.Parameters);
                winUserInfo.PrimaryGroupID = WinHelper.GetProperty(directoryEntry, WinProperties.PrimaryGroupID);
                winUserInfo.Name = WinHelper.GetProperty(directoryEntry, WinProperties.Name);
                winUserInfo.MinPasswordLength = WinHelper.GetProperty(directoryEntry, WinProperties.MinPasswordLength);
                winUserInfo.MaxPasswordAge = WinHelper.GetProperty(directoryEntry, WinProperties.MaxPasswordAge);
                winUserInfo.MinPasswordAge = WinHelper.GetProperty(directoryEntry, WinProperties.MinPasswordAge);
                winUserInfo.PasswordHistoryLength = WinHelper.GetProperty(directoryEntry, WinProperties.PasswordHistoryLength);
                winUserInfo.AutoUnlockInterval = WinHelper.GetProperty(directoryEntry, WinProperties.AutoUnlockInterval);
                winUserInfo.LockoutObservationInterval = WinHelper.GetProperty(directoryEntry, WinProperties.LockoutObservationInterval);
                winUserInfo.MaxBadPasswordsAllowed = WinHelper.GetProperty(directoryEntry, WinProperties.MaxBadPasswordsAllowed);
                winUserInfo.objectSid = sid;
               // winUserInfo.AccountType = Infrastructure.Enums.AccountType.User;
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return winUserInfo;
        }


    }
}
