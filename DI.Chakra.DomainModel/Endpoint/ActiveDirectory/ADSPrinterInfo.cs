using DI.Chakra.Logger;
using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ADSPrinterInfo : DomainModelBase<ADSPrinterInfo, EntityModel.ADSPrinterInfo>
    {
        
        public long EndPointSystemId { get; set; }

        public string cn { get; set; }

        public string description { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? whenChanged { get; set; }

        public Int64 uSNCreated { get; set; }

        public Int64 uSNChanged { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string uNCName { get; set; }

        public string versionNumber { get; set; }

        public string location { get; set; }

        public string serverName { get; set; }

        public string portName { get; set; }

        public string driverName { get; set; }

        public int priority { get; set; }

        public DateTime? printStartTime { get; set; }

        public DateTime? printEndTime { get; set; }

        public string printBinNames { get; set; }

        public string printMaxResolutionSupported { get; set; }

        public string printOrientationsSupported { get; set; }

        public string printCollate { get; set; }

        public string printColor { get; set; }

        public string printShareName { get; set; }

        public string printSpooling { get; set; }

        public string printKeepPrintedJobs { get; set; }

        public string driverVersion { get; set; }

        public string printMaxXExtent { get; set; }

        public string printMaxYExtent { get; set; }

        public string printMinXExtent { get; set; }

        public string printMinYExtent { get; set; }

        public string printStaplingSupported { get; set; }

        public string printMemory { get; set; }

        public string printRate { get; set; }

        public string printerName { get; set; }

        public string managedBy { get; set; }

        public string url { get; set; }

        public string objectCategory { get; set; }

        public string shortServerName { get; set; }

        public string printDuplexSupported { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public static  void AddADSPrinterInfoBulk(IList<ADSPrinterInfo> list)
        {
            ADSUserInfo.DeleteAll<ADSPrinterInfo>(list.First().EndPointSystemId);
            SQLBulkUploader<ADSPrinterInfo> sql = null;
           
                string tableName = "DI_ADS_PrinterInfo";
                sql = new SQLBulkUploader<ADSPrinterInfo>(tableName);
                sql.Flush(list);
                sql.Close();
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} AD Printer objects", list.Count.ToString()));
         }

    }
}
