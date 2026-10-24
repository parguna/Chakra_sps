using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
//using DI.Chakra.Facade;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector.Linux
{
    public class LinuxGroupInfoOrchestrator
    {
        public static LinuxGroupInfo getGroup(string strGroup, EndPoint _endpoint, ref IList<LinuxUserGroupMap> _listUserGroups)
        {
            LinuxGroupInfo linuxGroupInfo = null;
            
            try
            {
              
                char[] newdelimiters = new char[] { ':' };

                IList<string> newparts = strGroup.Split(newdelimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (newparts.Count != 4)
                    return null;

                linuxGroupInfo = new LinuxGroupInfo();

               
                linuxGroupInfo.EndPointSystemId = (int)_endpoint.Id;
                
                linuxGroupInfo.GroupNumber = newparts[2];
                linuxGroupInfo.GroupPassword = newparts[1];
                linuxGroupInfo.GroupName = newparts[0];
                
                IList<string> users = newparts[3].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (users.Count > 0)
                {
                    LinuxUserGroupMap userGroup = null;

                    foreach (string str in users)
                    {
                        string GroupsList = newparts[2].Replace(" ", ",");
                        foreach (string Groups in GroupsList.Split(',').ToList())
                        {

                            userGroup = new LinuxUserGroupMap();
                            userGroup.EndpointSystemId = _endpoint.Id;
                           
                            
                            userGroup.Version = (int)DI.Chakra.Infrastructure.Enums.VersionNo.Available;

                            if (null != userGroup)
                                _listUserGroups.Add(userGroup);
                        }
                    }

                }

                return linuxGroupInfo;

            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error, string.Format("Exception generated reading linux userinfo"), ex);

                return null;
            }
        }
    }
}
