using System;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using System.DirectoryServices;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class SharedFolderOrchestrator
    {
         

        public SharedFolderOrchestrator()
        {

        } 
        public long EndpointSystemId { get; set; }
         
         public int HistoryId { get; set; }
         public ADSSharedFolderInfo PopulateSharedFolder(DirectoryEntry directoryEntry)
         {
             if (null == directoryEntry)
                 return null;



             ADSSharedFolderInfo obj = new ADSSharedFolderInfo();

             try
             {
                 obj.EndPointSystemId = EndpointSystemId;
                 obj.cn = ADHelper.GetProperty(directoryEntry, ADProperties.cn);
                 obj.description = ADHelper.GetProperty(directoryEntry, ADProperties.description);
                 obj.distinguishedName = ADHelper.GetProperty(directoryEntry, ADProperties.distinguishedName);
                 obj.instanceType = ADHelper.GetProperty(directoryEntry, ADProperties.instanceType);
                 obj.whenCreated = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenCreated);
                 obj.whenChanged = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenChanged);
                 obj.uSNCreated = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNCreated);
                 obj.uSNChanged = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNChanged);

                 obj.name = ADHelper.GetProperty(directoryEntry, ADProperties.name);
                 obj.objectGUID = ADHelper.GetProperty_GUID(directoryEntry, ADProperties.objectGUID);
                 obj.uNCName = ADHelper.GetProperty(directoryEntry, ADProperties.uNCName);
                 obj.managedBy = ADHelper.GetProperty(directoryEntry, ADProperties.managedBy);
                 obj.objectCategory = ADHelper.GetProperty(directoryEntry, ADProperties.objectCategory);
                 //obj.nTSecurityDescriptor = ADHelper.GetProperty(directoryEntry, ADProperties.nTSecurityDescriptor);
                 obj.Version = Convert.ToInt16(VersionNo.Available);
                 

             }
             catch (Exception ex)
             {
                 //NOTE: Dont throw the exception. Keep running all objects
                 //DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id,  LogLevelType.Error,
                 //                              string.Format("Error occured in accessing the path {0}",
                 //                              directoryEntry.Path), ex);

                 return null;
             }

             return obj;

         }
    }
}
