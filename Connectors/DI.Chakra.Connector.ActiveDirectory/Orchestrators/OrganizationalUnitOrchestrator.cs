using System;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using System.DirectoryServices;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class OrganizationalUnitOrchestrator
    {
         

         public OrganizationalUnitOrchestrator()
        {

        }
         public long EndpointSystemId { get; set; }
         
         public int HistoryId { get; set; }
         public ADSOuInfo PopulateOrganizationalUnit(DirectoryEntry directoryEntry, DirectorySearcher _dirSearcher)
         {
             if (null == directoryEntry)
                 return null;



             ADSOuInfo adsOuInfo = new ADSOuInfo();

             try
             {
                 adsOuInfo.EndPointSystemId = EndpointSystemId;
                 adsOuInfo.ou = ADHelper.GetProperty(directoryEntry, ADProperties.ou);
                 adsOuInfo.description = ADHelper.GetProperty(directoryEntry, ADProperties.description);
                 adsOuInfo.distinguishedName = ADHelper.GetProperty(directoryEntry, ADProperties.distinguishedName);
                 adsOuInfo.instanceType = ADHelper.GetProperty(directoryEntry, ADProperties.instanceType);
                 adsOuInfo.whenCreated = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenCreated);
                 adsOuInfo.whenChanged = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenChanged);
                 adsOuInfo.uSNCreated = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNCreated);
                 adsOuInfo.uSNChanged = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNChanged);
                 adsOuInfo.showInAdvancedViewOnly = ADHelper.GetProperty(directoryEntry, ADProperties.showInAdvancedViewOnly);
                 adsOuInfo.name = ADHelper.GetProperty(directoryEntry, ADProperties.name);
                 adsOuInfo.objectGUID = ADHelper.GetProperty_GUID(directoryEntry, ADProperties.objectGUID);
                 adsOuInfo.systemFlags = ADHelper.GetProperty(directoryEntry, ADProperties.systemFlags);
                 adsOuInfo.objectCategory = ADHelper.GetProperty(directoryEntry, ADProperties.objectCategory);
                 adsOuInfo.isCriticalSystemObject = ADHelper.GetProperty(directoryEntry, ADProperties.isCriticalSystemObject);
                 adsOuInfo.gPLink = ADHelper.GetProperty(directoryEntry, ADProperties.gPLink);
                 //adsOuInfo.nTSecurityDescriptor = ADHelper.GetProperty(directoryEntry, ADProperties.nTSecurityDescriptor);
                 adsOuInfo.Version = Convert.ToInt16(VersionNo.Available);
                 

             }
             catch (Exception ex)
             {
                 //NOTE: Dont throw the exception. Keep running all objects
                 //DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id,  LogLevelType.Error,
                 //                 string.Format("Error occured in accessing the path {0}",
                 //                 directoryEntry.Path), ex);

                 return null;

             }

             return adsOuInfo;

         }
    }
}
