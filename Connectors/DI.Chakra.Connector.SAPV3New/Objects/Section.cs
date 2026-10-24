
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector.SAPV3
{
    public class Section
    {
        readonly ProcessorConfig _processorConfig;
        readonly IdmsLogger _idmsLogger;
        readonly string _tableName;
        readonly string _masterUser;
        //public ProcessorConfig ProcessorConfig { get; set; }

        public Section(ProcessorConfig ProcessorConfig, IdmsLogger logger)
        {
            _processorConfig = ProcessorConfig;
            _tableName = "ZFPSTB_SECNAME";
            _masterUser = MasterConfigHelper.ServiceUser;
            _idmsLogger = logger;
        }

        public void StartProcess()
        {
            ProcessSections();
        }

        public void ProcessSections()
        {
            if (!SAPSectionMappingInfo.TruncateTable()) return;
            _processorConfig.sapUtil.ResetPaging();
            List<SAPSectionMappingInfo> lstSAPSectionMappingInfo = new List<SAPSectionMappingInfo>();
            DataTable dtSectionMapping = null;

            try
            {


                DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, "Before Reading SectionMapping", DiscoveryObjectType.Section);

                do
                {

                    dtSectionMapping = _processorConfig.sapUtil.ReadRFCTblAsFunction("ZFPSTB_SECNAME", null, null, null);

                    if (dtSectionMapping != null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Reading SectionMapping Rows Count - {dtSectionMapping.Rows.Count}");

                        foreach (DataRow row in dtSectionMapping.Rows)
                        {
                            
                            if (row["OBJID"] == DBNull.Value) continue;
                            var HRMS_Section = Convert.ToString(row["OBJID"]);
                            var sectionMappingInfo = new SAPSectionMappingInfo() { IsActive = true, HRMSSectionCode = HRMS_Section };

                            if (row["ZZSECNAME"] != DBNull.Value)
                                sectionMappingInfo.SAPSectionCode = Convert.ToString(row["ZZSECNAME"]);
                            if (row["VBUKR"] != DBNull.Value)
                                sectionMappingInfo.VBUKR = Convert.ToString(row["VBUKR"]); 

                            lstSAPSectionMappingInfo.Add(sectionMappingInfo);
                        }

                        DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, $"Before saving SectionMapping objects Count - {lstSAPSectionMappingInfo.Count}", DiscoveryObjectType.Section);
                        using (var transaction = new Transaction.DataAccessTransaction(_masterUser))
                        {
                            SAPSectionMappingInfo.SaveList(transaction, lstSAPSectionMappingInfo);
                            transaction.Commit();
                        }
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, $"After saving SectionMapping objects Count - {lstSAPSectionMappingInfo.Count}", DiscoveryObjectType.Section);
                        lstSAPSectionMappingInfo.Clear();
                    }

                    _processorConfig.sapUtil.NextPage();

                    DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, $"After next page.");

                } while (dtSectionMapping != null && dtSectionMapping.Rows.Count > 0);


            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, $"Error in Reading SectionMapping: {ex.GetOriginalException()}", ex);
                //throw ex;
            }
            finally
            {
                dtSectionMapping?.Dispose();
                dtSectionMapping = null;

                lstSAPSectionMappingInfo?.Clear();
                lstSAPSectionMappingInfo = null;
            }
        }

        public List<SAPSectionMappingInfo> GetSections(string tableName = "")
        {
            if (string.IsNullOrWhiteSpace(tableName))
                tableName = _tableName;

            List<SAPSectionMappingInfo> lstSAPSectionMappingInfo = new List<SAPSectionMappingInfo>();
            try
            {
                _processorConfig.sapUtil.ResetPaging();
                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Trying to read {tableName}");
                var dtSectionMapping = _processorConfig.sapUtil.ReadRFCTblAsFunction(tableName, null, null, null);
                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Success reading {tableName}");
                foreach (DataRow row in dtSectionMapping.Rows)
                {
                    SAPSectionMappingInfo sectionMappingInfo;
                    if (row["OBJID"] == DBNull.Value) continue;
                    var HRMS_Section = Convert.ToString(row["OBJID"]);
                    sectionMappingInfo = new SAPSectionMappingInfo() { IsActive = true };
                    if (row["ZZSECNAME"] != DBNull.Value)
                        sectionMappingInfo.SAPSectionCode = Convert.ToString(row["ZZSECNAME"]);
                    if (row["VBUKR"] != DBNull.Value)
                        sectionMappingInfo.SAPSectionCode = Convert.ToString(row["VBUKR"]);
                    lstSAPSectionMappingInfo.Add(sectionMappingInfo);
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error in reading section from test harness", DiscoveryObjectType.Section,ex);
            }
            return lstSAPSectionMappingInfo;
        }

    }
}
