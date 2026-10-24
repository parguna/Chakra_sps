using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;


namespace DI.Chakra.Connector.SAPV3
{
    public class Field
    {
        string ServiceUser;
        public Field(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public ProcessorConfig ProcessorConfig { get; set; }

        public void StartProcess()
        {
            SAPFieldValueMapping.DeleteSAPFieldValueMapByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            ProcessFieldValueMap();
        }

        private IList<SAPFieldValueMapping> ProcessFieldValueMap()
        {
            IList<SAPFieldValueMapping> lstSapFieldValueMapping = new List<SAPFieldValueMapping>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessFieldValueMap.", DiscoveryObjectType.Field);
                DataTable dtParameters = null;
                long count = 0;
                do
                {
                    // freeing previous value
                    dtParameters = null;
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_1252", null, null, null);

                    foreach (DataRow Row in dtParameters.Rows)
                    {
                        SAPFieldValueMapping SapFieldValueMapping = new SAPFieldValueMapping();
                        SapFieldValueMapping.RoleCode = Row["AGR_NAME"].ToString().Trim();
                        SapFieldValueMapping.FieldCode = Row["COUNTER"].ToString().Trim();
                        SapFieldValueMapping.FromValue = Row["LOW"].ToString().Trim();
                        SapFieldValueMapping.ToValue = Row["HIGH"].ToString().Trim();
                        SapFieldValueMapping.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SapFieldValueMapping.Version = Convert.ToInt16(VersionNo.Available);
                        lstSapFieldValueMapping.Add(SapFieldValueMapping);
                        count++;
                        if (lstSapFieldValueMapping.Count % ProcessorConfig.insertThreshold == 0 && lstSapFieldValueMapping.Count > 0)
                        {
                            SAPFieldValueMapping.SaveSAPFieldValueMap(lstSapFieldValueMapping, ServiceUser);
                            lstSapFieldValueMapping.Clear();
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0);

                if (lstSapFieldValueMapping.Count > 0)
                {
                    SAPFieldValueMapping.SaveSAPFieldValueMap(lstSapFieldValueMapping, ServiceUser);
                    lstSapFieldValueMapping.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed ProcessFieldValueMap. Count:{0} Fields", count), DiscoveryObjectType.Field);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstSapFieldValueMapping;
        }
    }
}
