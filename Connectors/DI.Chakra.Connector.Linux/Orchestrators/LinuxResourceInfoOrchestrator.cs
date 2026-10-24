using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector.Linux
{
    public class LinuxResourceInfoOrchestrator
    {
        private static string parentfolder;
        public static LinuxResourceInfo getMounts(string strResource, EndPoint _endpoint)
        {

            LinuxResourceInfo linuxResourceInfo = null;

            try
            {

               
                char[] newdelimiters = new char[] { ' ' };

             
                string[] newparts = strResource.Split(newdelimiters, StringSplitOptions.RemoveEmptyEntries);
                if (newparts.Length == 1)
                {
                    char[] sdelimiters = new char[] { ':' };
                    string[] sparts = newparts[0].Split(sdelimiters, StringSplitOptions.RemoveEmptyEntries);
                    parentfolder = sparts[0];

                }
                if (newparts.Length > 2)
                {
                    string str = newparts[6] + " " + newparts[5] + " " + newparts[7];
                    string ResourceType = string.Empty;
                    if (newparts[0][0].ToString() == "d")
                    {
                        ResourceType = "Folder";
                    }
                    else
                    {
                        ResourceType = "File";
                    }
                    if (newparts[0][0].ToString() != "c" && newparts[0][0].ToString() != "b")
                    {
                        linuxResourceInfo = new LinuxResourceInfo();

                        linuxResourceInfo.Id = 1;
                        linuxResourceInfo.EndPointSystemId = _endpoint.Id;
                        linuxResourceInfo.Resource_Name = newparts[8];
                        linuxResourceInfo.Resource_Type = ResourceType;
                        linuxResourceInfo.Resource_User = newparts[2];
                        linuxResourceInfo.Resource_Group = newparts[3];
                        linuxResourceInfo.Resource_Parent = parentfolder;
                        linuxResourceInfo.Resource_Level = Convert.ToInt32(newparts[1]);
                        linuxResourceInfo.Resouce_AccessRights = newparts[0];
                        linuxResourceInfo.Resouce_Size = newparts[4];
                        linuxResourceInfo.Resouce_LastModified = str;
                        
                    }
                }
            }
            catch (Exception ex)
            {

               DILog.GeneralLogger.WriteLog(LogLevelType.Error, string.Format("Exception generated reading linux shareinfo"), ex);

                return null;

            }


            return linuxResourceInfo;
        }
    }
}
