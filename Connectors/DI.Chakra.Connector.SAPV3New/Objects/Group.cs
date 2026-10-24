using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector.SAPV3
{
    public class Group
    {
        public ProcessorConfig ProcessorConfig { get; set; }
        string ServiceUser;
        public Group(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public void StartProcess()
        {
            ProcessGroups();
            var deletedgroups = SAPGroupInfo.GetByModifiedDate(ProcessorConfig.EndpointId, ProcessorConfig.Process.ProcessStartAt);
            SAPGroupInfo.DeleteList(deletedgroups.ToList());
            deletedgroups = null;
        }

        /// <summary>
        /// Gets a dictionary of Group to Group description to update while adding Group
        /// </summary>
        private Dictionary<string, string> GetGroupDescriptions()
        {
            Dictionary<string, string> dictGroupDesc = new Dictionary<string, string>();

            ProcessorConfig.sapUtil.ResetPaging();

            // storing current row count
            var currentRowCount = ProcessorConfig.sapUtil.RowCount;
            // increasing row count to 30000, if set to lesser
            if (ProcessorConfig.sapUtil.RowCount < 30000)
                ProcessorConfig.sapUtil.RowCount = 30000;
            try
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetGroupDescriptions.", DiscoveryObjectType.Group);
                string filter = "SPRSL EQ 'E'";
                DataTable dtGroupDescs = null;
                string name, desc;
                do
                {
                    dtGroupDescs = null;
                    dtGroupDescs = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USGRPT", null, null, filter);
                    foreach (DataRow row in dtGroupDescs.Rows)
                    {
                        name = row["USERGROUP"].ToString().Trim();
                        desc = row["TEXT"].ToString().Trim();

                        if (!dictGroupDesc.ContainsKey(name))
                        {
                            dictGroupDesc.Add(name, desc);
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();


                } while (dtGroupDescs.Rows.Count > 0);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            finally
            {
                ProcessorConfig.sapUtil.RowCount = currentRowCount;
            }
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed GetGroupDescriptions.", DiscoveryObjectType.Group);
            return dictGroupDesc;
        }


        public IList<SAPGroupInfo> ProcessGroups()
        {
            IList<SAPGroupInfo> lstGroupInfo = new List<SAPGroupInfo>();
            Dictionary<string, string> dictGroupDesc = new Dictionary<string, string>();
            var dictExistingGroups = new Dictionary<string, SAPGroupInfo>();
            DataTable dtGroups = null;

            try
            {
                // get group descriptions
                dictGroupDesc = GetGroupDescriptions();

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessGroups.", DiscoveryObjectType.Group);
                ProcessorConfig.sapUtil.ResetPaging();

                long count = 0;
                var listExistingSAPgroups = SAPGroupInfo.GetByEndPointSystemId(ProcessorConfig.EndpointId);

                // creating dictionary items from list. Could use LINQ to the same as well
                foreach (var grp in listExistingSAPgroups)
                {
                    if (!dictExistingGroups.ContainsKey(grp.GROUP_CODE))
                    {
                        dictExistingGroups.Add(grp.GROUP_CODE, grp);
                    }
                }

                listExistingSAPgroups.Clear();
                SAPGroupInfo sapGroupInfo = new SAPGroupInfo();
                do
                {
                    dtGroups = null;
                    dtGroups = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USGRP", null, null, null);

                    foreach (DataRow row in dtGroups.Rows)
                    {
                        //string filter = string.Format("SPRSL EQ 'E' AND USERGROUP EQ '{0}'", row["USERGROUP"].ToString().Trim());
                        //var dtText = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USGRPT", null, null, filter);
                        sapGroupInfo = new SAPGroupInfo();
                        sapGroupInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                        sapGroupInfo.Version = Convert.ToInt16(VersionNo.Available);
                        sapGroupInfo.TargetSysId = null;
                        sapGroupInfo.GROUP_CODE = row["USERGROUP"].ToString().Trim();
                        if (dictGroupDesc.ContainsKey(sapGroupInfo.GROUP_CODE))
                        {
                            sapGroupInfo.GROUP_DESC = dictGroupDesc[sapGroupInfo.GROUP_CODE].Trim();
                        }
                        lstGroupInfo.Add(sapGroupInfo);
                        count++;
                    }

                    if (lstGroupInfo.Count % ProcessorConfig.insertThreshold == 0 && lstGroupInfo.Count > 0)
                    {
                        SAPGroupInfo.SaveGroups(lstGroupInfo, ServiceUser, dictExistingGroups);
                        lstGroupInfo.Clear();
                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (dtGroups.Rows.Count > 0);

                if (lstGroupInfo.Count > 0)
                {
                    SAPGroupInfo.SaveGroups(lstGroupInfo, ServiceUser, dictExistingGroups);
                    lstGroupInfo.Clear();
                }

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed ProcessGroups. Count: {0}", count), DiscoveryObjectType.Group);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dictExistingGroups?.Clear();
                dictExistingGroups = null;

                dtGroups?.Dispose();
                dtGroups = null;
            }
            return lstGroupInfo;
        }

        //For Testing 
        public IList<SAPGroupInfo> GetGroups()
        {
            IList<SAPGroupInfo> lstGroupInfo = new List<SAPGroupInfo>();
            Dictionary<string, string> dictGroupDesc = new Dictionary<string, string>();
            DataTable dtGroups = null;

            try
            {
                // get group descriptions
                dictGroupDesc = GetGroupDescriptions();

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetGroups.", DiscoveryObjectType.Group);
                ProcessorConfig.sapUtil.ResetPaging();

                long count = 0;

                SAPGroupInfo sapGroupInfo = new SAPGroupInfo();
                dtGroups = null;
                dtGroups = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USGRP", null, null, null);

                foreach (DataRow row in dtGroups.Rows)
                {
                    //string filter = string.Format("SPRSL EQ 'E' AND USERGROUP EQ '{0}'", row["USERGROUP"].ToString().Trim());
                    //var dtText = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USGRPT", null, null, filter);
                    sapGroupInfo = new SAPGroupInfo();
                    sapGroupInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                    sapGroupInfo.Version = Convert.ToInt16(VersionNo.Available);
                    sapGroupInfo.TargetSysId = null;
                    sapGroupInfo.GROUP_CODE = row["USERGROUP"].ToString().Trim();
                    if (dictGroupDesc.ContainsKey(sapGroupInfo.GROUP_CODE))
                    {
                        sapGroupInfo.GROUP_DESC = dictGroupDesc[sapGroupInfo.GROUP_CODE].Trim();
                    }
                    lstGroupInfo.Add(sapGroupInfo);
                    count++;
                }

                ProcessorConfig.sapUtil.NextPage();
                
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed GetGroups. Count: {0}", count), DiscoveryObjectType.Group);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dictGroupDesc?.Clear();
                dictGroupDesc = null;

                dtGroups?.Dispose();
                dtGroups = null;
            }
            return lstGroupInfo;
        }
    }
}
