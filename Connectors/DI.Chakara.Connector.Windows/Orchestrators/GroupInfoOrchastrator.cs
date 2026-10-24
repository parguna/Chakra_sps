using DI.Chakra.Infrastructure;
using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class GroupInfoOrchestrator
    {
        EndpointConfiguration m_winConfig;
        public GroupInfoOrchestrator()
        {

        }
        public EndpointConfiguration WindowsConfiguration
        {
            get
            {
                return m_winConfig;
            }

            set
            {
                this.m_winConfig = value;
            }

        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
       
        private string Configuration(EndpointKey Key)
        {
            var config = WindowsConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public WindowsGroupInfo PopulateGroupInfo(DirectoryEntry directoryEntry, ref ArrayList groupMembers)
        {
            if (null == directoryEntry)
                return null;

            WindowsGroupInfo wingroupInfo = new WindowsGroupInfo();

            try
            {
                //-----Group Info
               
                wingroupInfo.EndPointSystemId = EndpointSystemId;
                wingroupInfo.groupType = WinHelper.GetProperty(directoryEntry, WinProperties.groupType);
                wingroupInfo.Name = WinHelper.GetProperty(directoryEntry, WinProperties.Name);
                wingroupInfo.Description = WinHelper.GetProperty(directoryEntry, WinProperties.Description);
                object obVal = WinHelper.GetProperty_Byte(directoryEntry, WinProperties.objectSid);

                if (null != obVal)
                {
                    wingroupInfo.objectSid = this.ConvertBytesToSidString((Byte[])obVal);// New method added for getting value to insert  Into DB.
                }
                
                //-----------------------------------------------Group Info


                //-----------------------------------------------Group Members Info
                using (DirectoryEntry search = new DirectoryEntry("WinNT://" + Configuration(EndpointKey.IPAddress) + "/" + wingroupInfo.Name + ",group"))
                {
                    foreach (object member in (IEnumerable)search.Invoke("Members"))
                    {
                        DirectoryEntry memberEntry = new DirectoryEntry(member);
                        groupMembers.Add(memberEntry.Name);
                    }
                }
                //-----------------------------------------------Group Members Info
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return wingroupInfo;
        }

        //New Method Added .
        private string ConvertBytesToSidString(Byte[] sidBytes)
        {
            string sid = String.Empty;
            sid = new SecurityIdentifier(sidBytes, 0).ToString();
            return sid;
        }


    }
}
