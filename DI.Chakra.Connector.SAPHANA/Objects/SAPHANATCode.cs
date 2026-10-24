using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANATCode
    {
        string ServiceUser;
        public SAPHANATCode(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }

        public SAPHANAProcessorConfig ProcessorConfig { get; set; }

        public void StartProcess()
        {
            // delete all tcodes
            SAPHANATCodeInfo.DeleteTCodesByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            // retrieve tcodes
            ProcessTCodes();

            // delete all existing mapping
            SAPHANATCodeObjectMap.DeleteSAPTCodeObjectMap(ProcessorConfig.EndpointId);
            // insert current mapping
            ProcessTCodeObjectMapping();
        }

        /// <summary>
        /// Gets a dictionary of TCode to TCode description to update while adding TCode
        /// </summary>
        private Dictionary<string, string> GetTCodeDescriptions()
        {
            Dictionary<string, string> dictTCodeDesc = new Dictionary<string, string>();

            ProcessorConfig.sapUtil.ResetPaging();

            // storing current row count
            var currentRowCount = ProcessorConfig.sapUtil.RowCount;
            // increasing row count to 30000, if set to lesser
            if (ProcessorConfig.sapUtil.RowCount < 30000)
                ProcessorConfig.sapUtil.RowCount = 30000;
            try
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetTCodeDescriptions.",DiscoveryObjectType.TCode);
                string filter = "SPRSL EQ 'E'";
                DataTable dtTCodeDescs = null;
                string name, desc;
                do
                {
                    dtTCodeDescs = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TSTCT", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in dtTCodeDescs.Rows)
                    {
                        name = row["TCODE"].ToString().Trim();
                        desc = row["TTEXT"].ToString().Trim();

                        if (!dictTCodeDesc.ContainsKey(name))
                        {
                            dictTCodeDesc.Add(name, desc);
                        }
                    }
                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();


                } while (dtTCodeDescs.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            finally
            {
                ProcessorConfig.sapUtil.RowCount = currentRowCount;
            }
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed GetTCodeDescriptions.", DiscoveryObjectType.TCode);
            return dictTCodeDesc;
        }

        private IList<SAPHANATCodeInfo> ProcessTCodes()
        {
            var listTcodes = new List<SAPHANATCodeInfo>();

            // get tcode description as dictionary
            var dictTCodeDesc = GetTCodeDescriptions();

            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessTCodes.", DiscoveryObjectType.TCode);
            ProcessorConfig.sapUtil.ResetPaging();
            HashSet<string> existingTCodes = new HashSet<string>();
            long count = 0;
            DataTable dtCodes = null;

            try
            {
                listTcodes = new List<SAPHANATCodeInfo>();

                do
                {
                    dtCodes = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TSTC", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in dtCodes.Rows)
                    {
                        /** not getting Tcode description table here, got it as dictionary **/
                        //string filtera = string.Format("SPRSL EQ 'EN' AND TCODE EQ '{0}'", row["TCODE"].ToString());
                        //var dtText = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TSTCT", null, null, filtera);
                        var sapTCodeInfo = new SAPHANATCodeInfo();
                        sapTCodeInfo.TCODE_Code = row["TCODE"].ToString().Trim();

                        if (existingTCodes.Contains(sapTCodeInfo.TCODE_Code))
                        {
                            // already added TCode, ignore
                            continue;
                        }

                        // check if we have TCode description
                        if (dictTCodeDesc.ContainsKey(sapTCodeInfo.TCODE_Code))
                        {
                            sapTCodeInfo.TCODE_Desc = dictTCodeDesc[sapTCodeInfo.TCODE_Code];
                        }
                        sapTCodeInfo.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        sapTCodeInfo.Version = Convert.ToInt16(VersionNo.Available);
                        listTcodes.Add(sapTCodeInfo);
                        count++;

                        if (listTcodes.Count % ProcessorConfig.insertThreshold == 0 && listTcodes.Count > 0)
                        {
                            SAPHANATCodeInfo.SaveTcodes(listTcodes, ServiceUser);
                            listTcodes.Clear();
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtCodes.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (listTcodes.Count > 0)
                {
                    SAPHANATCodeInfo.SaveTcodes(listTcodes, ServiceUser);
                    listTcodes.Clear();
                }

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed ProcessTCodes. Count:{0} TCodes", count), DiscoveryObjectType.TCode);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            finally
            {
                dtCodes?.Dispose();
                dtCodes = null;

                if (existingTCodes != null)
                {
                    existingTCodes.Clear();
                    existingTCodes = null;
                }

                if (dictTCodeDesc != null)
                {
                    dictTCodeDesc.Clear();
                    dictTCodeDesc = null;
                }
                
            }
            return listTcodes;
        }

        private IList<SAPHANATCodeObjectMap> ProcessTCodeObjectMapping()
        {
            var lstSapTCodeObjects = new List<SAPHANATCodeObjectMap>();
            long count = 0;
            ProcessorConfig.sapUtil.ResetPaging();
            string format = "{0}__{1}__{2}__{3}";
            HashSet<string> hsTCodeObjMap = new HashSet<string>();
            DataTable dtParameters = null;

            try
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessTCodeObjectMapping.", DiscoveryObjectType.TCode);

                do
                {
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USOBT_C", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    if(dtParameters!=null)
                    foreach (DataRow row in dtParameters.Rows)
                    {
                        var TCodeObjectMap = new SAPHANATCodeObjectMap();
                        // TODO: Verify whether the following columns are being mapped properly
                        TCodeObjectMap.ObjectType = row["NAME"].ToString().Trim();
                        TCodeObjectMap.ObjectCode = row["OBJECT"].ToString().Trim();
                        TCodeObjectMap.FieldCode = row["FIELD"].ToString().Trim();
                        TCodeObjectMap.FieldValue = row["LOW"].ToString().Trim();
                        TCodeObjectMap.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        TCodeObjectMap.Version = Convert.ToInt16(VersionNo.Available);
                        TCodeObjectMap.CreatedDate = DateTime.Now.Date;
                        TCodeObjectMap.CreatedBy = ServiceUser;
                        // adding to list only if this has not already been added
                        // NOTE: ideally, being a mapping table, there should not be any duplicates
                        if (!hsTCodeObjMap.Contains(String.Format(format, TCodeObjectMap.ObjectType, TCodeObjectMap.ObjectCode,
                            TCodeObjectMap.FieldCode, TCodeObjectMap.FieldValue)))
                        {
                            lstSapTCodeObjects.Add(TCodeObjectMap);
                            hsTCodeObjMap.Add(String.Format(format, TCodeObjectMap.ObjectType, TCodeObjectMap.ObjectCode,
                                TCodeObjectMap.FieldCode, TCodeObjectMap.FieldValue));
                        }

                        count++;
                        if (lstSapTCodeObjects.Count % ProcessorConfig.insertThreshold == 0 && lstSapTCodeObjects.Count > 0)
                        {
                                try
                                {
                                    SAPHANATCodeObjectMap.SaveSAPTCodeObjectMap(lstSapTCodeObjects, ServiceUser);
                                }
                                catch (Exception ex)
                                {
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info,$"ProcessTCodeObjectMapping Exception {ex}" , DiscoveryObjectType.TCode);
                                }
                            lstSapTCodeObjects.Clear();
                        }
                    }
                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (lstSapTCodeObjects!=null && lstSapTCodeObjects.Count > 0)
                {
                    try
                    {
                        SAPHANATCodeObjectMap.SaveSAPTCodeObjectMap(lstSapTCodeObjects, ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Final List ProcessTCodeObjectMapping Exception {ex}", DiscoveryObjectType.TCode);
                    }
                    lstSapTCodeObjects.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed ProcessTCodeObjectMapping. Processed Count:{0} TCodeObjectMapping", count), DiscoveryObjectType.TCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtParameters?.Dispose();
                dtParameters = null;
            }
            return lstSapTCodeObjects;
        }

        public List<SAPHANATCodeInfo> GetTCodes()
        {
            var listTcodes = new List<SAPHANATCodeInfo>();

            // get tcode description as dictionary
            var dictTCodeDesc = GetTCodeDescriptions();

            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetTCodes.");
            try
            {
                listTcodes = new List<SAPHANATCodeInfo>();

                DataTable dtCodes = null;
                dtCodes = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TSTC", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);
                foreach (DataRow row in dtCodes.Rows)
                {
                    /** not getting Tcode description table here, got it as dictionary **/
                    //string filtera = string.Format("SPRSL EQ 'EN' AND TCODE EQ '{0}'", row["TCODE"].ToString());
                    //var dtText = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TSTCT", null, null, filtera);
                    var sapTCodeInfo = new SAPHANATCodeInfo();
                    sapTCodeInfo.TCODE_Code = row["TCODE"].ToString().Trim();

                    if (dictTCodeDesc.ContainsKey(sapTCodeInfo.TCODE_Code))
                    {
                        sapTCodeInfo.TCODE_Desc = dictTCodeDesc[sapTCodeInfo.TCODE_Code];
                    }
                    sapTCodeInfo.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                    sapTCodeInfo.Version = Convert.ToInt16(VersionNo.Available);
                    listTcodes.Add(sapTCodeInfo);
                }

                ProcessorConfig.sapUtil.NextPage();

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessTCodes", DiscoveryObjectType.TCode);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            finally
            {
                if (dictTCodeDesc != null)
                {
                    dictTCodeDesc.Clear();
                    dictTCodeDesc = null;
                }

            }
            return listTcodes;
        }
    }
}
