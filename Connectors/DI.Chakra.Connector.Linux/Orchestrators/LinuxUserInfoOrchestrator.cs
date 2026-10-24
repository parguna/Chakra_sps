using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector.Linux
{
    public class LinuxUserInfoOrchestrator
    {


        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public LinuxUserInfo getUser(string strUser, EndPoint _endpoint)
        {

            LinuxUserInfo linuxUserInfo = null;

            try
            {
                
                char[] newdelimiters = new char[] { ':' };

                IList<string> newparts = strUser.Split(newdelimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (newparts.Count != 7)
                    return null;

                linuxUserInfo = new LinuxUserInfo();

              
                linuxUserInfo.EndPointSystemId = _endpoint.Id;
                linuxUserInfo.UserName = newparts[0];
                linuxUserInfo.Password = newparts[1];
                linuxUserInfo.LinuxUserId = newparts[2];
                linuxUserInfo.GroupId = newparts[3];
                linuxUserInfo.UserIdInfo = newparts[4];
                linuxUserInfo.HomeDirectory = newparts[5];
                linuxUserInfo.CommandShell = newparts[6];
            
            }
            catch (Exception ex)
            {

                DILog.GeneralLogger.WriteLog(LogLevelType.Error, string.Format("Exception generated reading linux userinfo"), ex);

                return null;

            }


            return linuxUserInfo;
        }

        public List<LinuxUserGroupMap> getUserGroups(string strUser, EndPoint _endpoint)
        {
            LinuxUserGroupMap userGroup = null;
          
            List<LinuxUserGroupMap> listUserGroup = new List<LinuxUserGroupMap>();
            try
            {
                
                char[] newdelimiters = new char[] { ':' };

                IList<string> newparts = strUser.Split(newdelimiters, StringSplitOptions.RemoveEmptyEntries).ToList();



                string GroupNumber = newparts[1].Trim().Replace(" ", ",");
                string UserName = newparts[0];

                foreach (string strGroups in GroupNumber.Split(','))
                {
                    userGroup = new LinuxUserGroupMap();
                    var currentUser = LinuxUserInfo.GetByUserNameEndpointSystemId(UserName, _endpoint.Id);
                    var currentGroup = LinuxGroupInfo.GetByGroupAndEndpoint(UserName, _endpoint.Id);
                    if (currentGroup != null && currentUser != null)
                    {
                        userGroup.LinuxUserInfo_Id = currentUser.Id;
                        userGroup.LinuxGroupInfo_Id = currentGroup.Id;
                        listUserGroup.Add(userGroup);
                    }

                }
            }
            catch (Exception ex)
            {

                DILog.GeneralLogger.WriteLog(LogLevelType.Error, string.Format("Exception generated reading linux userinfo"), ex);

                return null;

            }


            return listUserGroup;
        }
    }
}
