using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using System.DirectoryServices;
using System.Management.Automation;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class GroupInfoOrchestrator
    {

         public GroupInfoOrchestrator()
        {
        }
         public long EndpointSystemId { get; set; }
         
         public int HistoryId { get; set; }
         public ExGroupInfo PopulateGroup(PSObject directoryEntry)
         {
             if (null == directoryEntry)
                 return null;

             ExGroupInfo excgroupInfo = new ExGroupInfo();

             try
             {
                 excgroupInfo.EndPointSystemId = EndpointSystemId;
                 excgroupInfo.cn = ExHelper.GetProperty(directoryEntry, ExProperties.name);
                 excgroupInfo.description = ExHelper.GetProperty(directoryEntry, ExProperties.description);
                 excgroupInfo.member = ExHelper.GetProperty(directoryEntry, ExProperties.member);
                 excgroupInfo.distinguishedName = ExHelper.GetProperty(directoryEntry, ExProperties.distinguishedName);
                 excgroupInfo.instanceType = ExHelper.GetProperty(directoryEntry, ExProperties.instanceType);
                 excgroupInfo.whenCreated = ExHelper.GetProperty_DateTime(directoryEntry, ExProperties.whenCreated);
                 excgroupInfo.whenChanged = ExHelper.GetProperty_DateTime(directoryEntry, ExProperties.whenChanged);
                 //excgroupInfo.uSNCreated = ExHelper.GetCOMProperty_Int64(directoryEntry, ExProperties.uSNCreated);
                 //excgroupInfo.uSNChanged = ExHelper.GetCOMProperty_Int64(directoryEntry, ExProperties.uSNChanged);
                 excgroupInfo.name = ExHelper.GetProperty(directoryEntry, ExProperties.name);
                 excgroupInfo.objectGUID = ExHelper.GetProperty_GUID(directoryEntry, ExProperties.objectGUID);
                // excgroupInfo.objectSid = ExHelper.GetProperty_SID(directoryEntry, ExProperties.objectSid);
                 excgroupInfo.sAMAccountName = ExHelper.GetProperty(directoryEntry, ExProperties.sAMAccountName);
                 excgroupInfo.sAMAccountType = ExHelper.GetProperty(directoryEntry, ExProperties.sAMAccountType);
                 excgroupInfo.groupType = ExHelper.GetProperty(directoryEntry, ExProperties.groupType);
                 excgroupInfo.objectCategory = ExHelper.GetProperty(directoryEntry, ExProperties.objectCategory);
                 // excgroupInfo.nTSecurityDescriptor = ExHelper.GetProperty(directoryEntry, ExProperties.nTSecurityDescriptor);
                 excgroupInfo.Version = Convert.ToInt16(VersionNo.Available);
                 
             }
             catch (Exception ex)
             {
                 //NOTE: Dont throw the exception. Keep running all objects
                 //Log.Discovery.WriteMessage(LogLevel.ERROR,
                 //               string.Format("Error occured in accessing the path {0}",
                 //               directoryEntry.Path), ex);
                 return null;
             }

             return excgroupInfo;

         }

      

    }
}
