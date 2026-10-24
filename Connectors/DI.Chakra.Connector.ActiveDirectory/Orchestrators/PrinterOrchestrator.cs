using System;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using System.DirectoryServices;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class PrinterOrchestrator
    {
      
        

        public PrinterOrchestrator()
        {
            
        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public ADSPrinterInfo PopulatePrinter(DirectoryEntry directoryEntry)
        {
            if (null == directoryEntry)
                return null;



            ADSPrinterInfo obj = new ADSPrinterInfo();

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
                obj.versionNumber = ADHelper.GetProperty(directoryEntry, ADProperties.versionNumber);
                obj.location = ADHelper.GetProperty(directoryEntry, ADProperties.location);
                obj.serverName = ADHelper.GetProperty(directoryEntry, ADProperties.serverName);
                obj.portName = ADHelper.GetProperty(directoryEntry, ADProperties.portName);
                obj.driverName = ADHelper.GetProperty(directoryEntry, ADProperties.driverName);
                obj.priority = ADHelper.GetProperty_Int(directoryEntry, ADProperties.priority);
                obj.printStartTime = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.printStartTime);
                obj.printEndTime = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.printEndTime);
                obj.printBinNames = ADHelper.GetProperty(directoryEntry, ADProperties.printBinNames);
                obj.printMaxResolutionSupported = ADHelper.GetProperty(directoryEntry, ADProperties.printMaxResolutionSupported);
                obj.printOrientationsSupported = ADHelper.GetProperty(directoryEntry, ADProperties.printOrientationsSupported);

                obj.printCollate = ADHelper.GetProperty(directoryEntry, ADProperties.printCollate);
                obj.printColor = ADHelper.GetProperty(directoryEntry, ADProperties.printColor);
                obj.printShareName = ADHelper.GetProperty(directoryEntry, ADProperties.printShareName);
                obj.printSpooling = ADHelper.GetProperty(directoryEntry, ADProperties.printSpooling);
                obj.printKeepPrintedJobs = ADHelper.GetProperty(directoryEntry, ADProperties.printKeepPrintedJobs);
                obj.driverVersion = ADHelper.GetProperty(directoryEntry, ADProperties.driverVersion);
                obj.printMaxXExtent = ADHelper.GetProperty(directoryEntry, ADProperties.printMaxXExtent);
                obj.printMaxYExtent = ADHelper.GetProperty(directoryEntry, ADProperties.printMaxYExtent);
                obj.printMinXExtent = ADHelper.GetProperty(directoryEntry, ADProperties.printMinXExtent);
                obj.printMinYExtent = ADHelper.GetProperty(directoryEntry, ADProperties.printMinYExtent);
                obj.printStaplingSupported = ADHelper.GetProperty(directoryEntry, ADProperties.printStaplingSupported);
                obj.printMemory = ADHelper.GetProperty(directoryEntry, ADProperties.printMemory);
                obj.printRate = ADHelper.GetProperty(directoryEntry, ADProperties.printRate);
                obj.printerName = ADHelper.GetProperty(directoryEntry, ADProperties.printerName);
                obj.managedBy = ADHelper.GetProperty(directoryEntry, ADProperties.managedBy);
                obj.url = ADHelper.GetProperty(directoryEntry, ADProperties.url);
                obj.objectCategory = ADHelper.GetProperty(directoryEntry, ADProperties.objectCategory);
                obj.shortServerName = ADHelper.GetProperty(directoryEntry, ADProperties.shortServerName);
                obj.printDuplexSupported = ADHelper.GetProperty(directoryEntry, ADProperties.printDuplexSupported);
                //obj.nTSecurityDescriptor = ADHelper.GetProperty(directoryEntry, ADProperties.nTSecurityDescriptor);
                obj.Version = Convert.ToInt16(VersionNo.Available);
               
            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                //DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id,  LogLevelType.Error,
                //               string.Format("Error occured in accessing the path {0}",
                //               directoryEntry.Path), ex);


                return null;

            }

            return obj;

        }
    }
}
