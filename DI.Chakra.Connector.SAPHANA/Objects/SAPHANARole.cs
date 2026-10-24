using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANARole
    {
        public SAPHANAProcessorConfig ProcessorConfig { get; set; }
        private const string LangFilter = "LANGU EQ 'E'";
        string ServiceUser;
        public SAPHANARole(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }


        public bool IsFirstTimeDiscovery { get; set; }

        public void StartProcess(bool hasDoneUserDiscovery)
        {
            if (!hasDoneUserDiscovery)
            {
                ProcessRoles();

                // delete roles
                var deletedRoles = SAPHANARoleInfo.GetDeletedByModifiedDate(ProcessorConfig.EndpointId, ProcessorConfig.Process.ProcessStartAt);
                SAPHANARoleInfo.DeleteList(deletedRoles);
            }
            try
            {
                SAPHANACompositeRoleInfo.DeleteSAPCompositeRoleInfoByEndpoint(ProcessorConfig.EndpointId, ServiceUser);

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"DeleteSAPHANACompositeRoleInfoByEndpoint: {ex.Message}", ex);
            }

            ProcessCompositeRoles();

            try
            {
                SAPHANASingleRoleInfo.DeleteSAPSingleRoleInfoByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"DeleteSAPHANASingleRoleInfoByEndpoint: {ex.Message}", ex);
            }
            ProcessChildRoles();

            UpdateSAPRoleType();

            ProcessCompositeChildRoleMapping();
            try
            {
                // delete existing role tcode mapping
                SAPHANARoleTCodeMapping.DeleteSapRoleTCodeMappingForEndpoint(ProcessorConfig.EndpointId);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"DeleteSAPHANARoleTCodeMappingForEndpoint & ProcessChildRoles: {ex.Message}", ex);
            }

            //  insert new entries
            ProcessRoleTCodeMapping();
            try
            {
                SAPHANARoleToTCodes.UpdateRoleToTcodeAfterDiscovery(ProcessorConfig.EndpointId);
                SAPHANARoleToTCodes.DeleteSapRoleTCodeMappingForEndpoint(ProcessorConfig.EndpointId, ProcessorConfig.Process.ProcessStartAt);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Error While Generating Role To TCodes.", DiscoveryObjectType.Role, ex);
            }

            // delete existing role Auth mapping
            SAPHANARoleToAuthMap.DeleteSAPRoleToAuthMapForEndpoint(ProcessorConfig.EndpointId);
            //  insert new entries
            ProcessRoleToAuthMap();

        }
        public IList<SAPHANARoleInfo> ProcessRoles()
        {
            Dictionary<string, string> dictRoleDescs = null;
            Dictionary<string, SAPHANARoleInfo> dictExistingRoles = null;
            IList<SAPHANARoleInfo> lstRoleInfo = new List<SAPHANARoleInfo>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoles.", DiscoveryObjectType.Role);
            DataTable dtRoles = null;

            try
            {
                // get role descriptions from role text table
                dictRoleDescs = GetRoleDescriptions();


                string deltaDate = ProcessorConfig.DeltaDate.ToString("yyyyMMdd");
                ProcessorConfig.sapUtil.ResetPaging();

                string filter;
                if (ProcessorConfig.IsDelta)
                {
                    filter = "CHANGE_DAT >= '" + deltaDate + "' ";
                    filter += "OR CREATE_DAT >= '" + deltaDate + "' ";
                }
                else
                {
                    filter = string.Empty;
                }

                // create a dictionary of existing roles to avoid calls to db to check for existing roles
                dictExistingRoles = new Dictionary<string, SAPHANARoleInfo>();
                var listExistingRoles = SAPHANARoleInfo.GetByEndpointSystemId(ProcessorConfig.EndpointId);
                // creating dictionary items from list. Could use LINQ to the same as well
                foreach (var rol in listExistingRoles)
                {
                    if (!dictExistingRoles.ContainsKey(rol.ROLE_CODE))
                    {
                        dictExistingRoles.Add(rol.ROLE_CODE, rol);
                    }
                }
                listExistingRoles.Clear();

                var sapRoleInfo = new SAPHANARoleInfo();
                do
                {

                    dtRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_DEFINE", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtRoles.Rows)
                    {
                        sapRoleInfo = new SAPHANARoleInfo();
                        sapRoleInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                        sapRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                        sapRoleInfo.TargetSysId = null;
                        sapRoleInfo.ROLE_CODE = row["AGR_NAME"].ToString().Trim();

                        // get the description from dictionary
                        if (dictRoleDescs.ContainsKey(sapRoleInfo.ROLE_CODE))
                        {
                            sapRoleInfo.ROLE_DESC = dictRoleDescs[sapRoleInfo.ROLE_CODE];
                        }

                        // check whether this role code is already added
                        if (!lstRoleInfo.Any(r => r.ROLE_CODE == sapRoleInfo.ROLE_CODE))
                        {
                            lstRoleInfo.Add(sapRoleInfo);
                        }

                        if (lstRoleInfo.Count > 0 && lstRoleInfo.Count % ProcessorConfig.insertThreshold == 0)
                        {
                            SAPHANARoleInfo.SaveRoles(lstRoleInfo, ServiceUser, dictExistingRoles);
                            lstRoleInfo.Clear();
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtRoles.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                // add any remaining roles missed abvoe
                if (lstRoleInfo.Count > 0)
                {
                    SAPHANARoleInfo.SaveRoles(lstRoleInfo, ServiceUser, dictExistingRoles);
                    lstRoleInfo.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessRoles.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                throw ex;
            }
            finally
            {
                if (dictRoleDescs != null)
                {
                    dictRoleDescs.Clear();
                    dictRoleDescs = null;
                }
                if (dictExistingRoles != null)
                {
                    dictExistingRoles.Clear();
                    dictExistingRoles = null;
                }
                dtRoles?.Dispose();
                dtRoles = null;
            }
            return lstRoleInfo;
        }

        /// <summary>
        /// Gets a dictionary of role code to role description to update while adding roles
        /// </summary>
        private Dictionary<string, string> GetRoleDescriptions()
        {
            Dictionary<string, string> dictRoleDesc = new Dictionary<string, string>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetRoleDescriptions.", DiscoveryObjectType.Role);
            ProcessorConfig.sapUtil.ResetPaging();
            List<string> fields = new List<string>();
            fields.Add("AGR_NAME");
            fields.Add("TEXT");
            try
            {

                string filter = "SPRAS EQ 'E' AND LINE EQ '00000'";
                DataTable dtRoleDescs = null;
                string name, desc;
                do
                {
                    dtRoleDescs = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_TEXTS", null, fields, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in dtRoleDescs.Rows)
                    {
                        name = row["AGR_NAME"].ToString().Trim();
                        desc = row["TEXT"].ToString().Trim();

                        if (!dictRoleDesc.ContainsKey(name))
                        {
                            dictRoleDesc.Add(name, desc);
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtRoleDescs.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed GetRoleDescriptions.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            fields.Clear();
            fields = null;
            return dictRoleDesc;
        }

        private IList<DomainModel.SAPHANACompositeRoleInfo> ProcessCompositeRoles()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessCompositeRoles.", DiscoveryObjectType.Role);
            var listSAPCompositeRoleInfo = new List<DomainModel.SAPHANACompositeRoleInfo>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable m_CompositeRoles = null;

                // get a hashset of all composite role codes
                HashSet<string> hsExistingitems = new HashSet<string>();
                var sapCompositeRoleInfo = new DomainModel.SAPHANACompositeRoleInfo();
                do
                {
                    m_CompositeRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_AGRS", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in m_CompositeRoles.Rows)
                    {
                        sapCompositeRoleInfo = new DomainModel.SAPHANACompositeRoleInfo();
                        sapCompositeRoleInfo.CompositeRole_Code = row["AGR_NAME"].ToString().Trim();
                        sapCompositeRoleInfo.CompositeRole_Desc = String.Empty;
                        sapCompositeRoleInfo.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        sapCompositeRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                        sapCompositeRoleInfo.CreatedDate=DateTime.Now.Date;
                        sapCompositeRoleInfo.CreatedBy = ServiceUser;
                        if (!hsExistingitems.Contains(sapCompositeRoleInfo.CompositeRole_Code))
                        {
                            hsExistingitems.Add(sapCompositeRoleInfo.CompositeRole_Code);
                            listSAPCompositeRoleInfo.Add(sapCompositeRoleInfo);
                        }

                        if (listSAPCompositeRoleInfo.Count % ProcessorConfig.insertThreshold == 0 && listSAPCompositeRoleInfo.Count > 0)
                        {
                            try
                            {
                                SAPHANACompositeRoleInfo.SaveSAPCompositeRoles(listSAPCompositeRoleInfo, ServiceUser);
                            }
                            catch (Exception ex)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"ProcessCompositeRoles Exception:{ex}.", DiscoveryObjectType.Role);
                            }
                            
                            listSAPCompositeRoleInfo.Clear();
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (m_CompositeRoles.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                // save any remaining Composite roles
                if (listSAPCompositeRoleInfo.Count > 0)
                {
                    try
                    {
                        SAPHANACompositeRoleInfo.SaveSAPCompositeRoles(listSAPCompositeRoleInfo, ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Final List ProcessCompositeRoles Exception:{ex}.", DiscoveryObjectType.Role);
                    }
                    listSAPCompositeRoleInfo.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessCompositeRoles.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            return listSAPCompositeRoleInfo;
        }

        private IList<SAPHANASingleRoleInfo> ProcessChildRoles()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessChildRoles.", DiscoveryObjectType.Role);
            var listSAPSingleRoleInfo = new List<SAPHANASingleRoleInfo>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable m_CompositeRoles = null;

                // get a hashset of all composite role codes
                HashSet<string> hsExistingitems = new HashSet<string>();
                SAPHANASingleRoleInfo SAPSingleRoleInfo = new SAPHANASingleRoleInfo();
                do
                {
                    m_CompositeRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_AGRS", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in m_CompositeRoles.Rows)
                    {
                        SAPSingleRoleInfo = new SAPHANASingleRoleInfo();

                        SAPSingleRoleInfo.SingleRole_Code = row["CHILD_AGR"].ToString().Trim();
                        SAPSingleRoleInfo.SingleRole_Desc = String.Empty;
                        SAPSingleRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                        SAPSingleRoleInfo.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SAPSingleRoleInfo.CreatedDate = DateTime.Now.Date;
                        SAPSingleRoleInfo.CreatedBy = ServiceUser;
                        if (!hsExistingitems.Contains(SAPSingleRoleInfo.SingleRole_Code))
                        {
                            hsExistingitems.Add(SAPSingleRoleInfo.SingleRole_Code);
                            listSAPSingleRoleInfo.Add(SAPSingleRoleInfo);
                        }

                        if (listSAPSingleRoleInfo.Count % ProcessorConfig.insertThreshold == 0 && listSAPSingleRoleInfo.Count > 0)
                        {
                            try
                            {
                                DI.Chakra.DomainModel.SAPHANASingleRoleInfo.SaveSingleRoles(listSAPSingleRoleInfo, ServiceUser);
                            }
                            catch (Exception ex)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"ProcessChildRoles Exception:{ex}", ex);
                            }
                            listSAPSingleRoleInfo.Clear();
                        }

                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (m_CompositeRoles.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                // save the last remaining items, if any
                if (listSAPSingleRoleInfo.Count > 0)
                {
                    try
                    {
                        DI.Chakra.DomainModel.SAPHANASingleRoleInfo.SaveSingleRoles(listSAPSingleRoleInfo, ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Final List ProcessChildRoles Exception:{ex}", ex);
                    }
                    listSAPSingleRoleInfo.Clear();
                }

                hsExistingitems = null;
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessChildRoles.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            return listSAPSingleRoleInfo;
        }
        private void UpdateSAPRoleType()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started UpdateSAPRoleType.", DiscoveryObjectType.Role);
            var listSAPRoles = new List<SAPHANARoleInfo>();
            var listSAPCompositeRoles = new List<DomainModel.SAPHANACompositeRoleInfo>();

            listSAPRoles = SAPHANARoleInfo.GetByEndpointSystemId(ProcessorConfig.EndpointId);
            listSAPCompositeRoles = DomainModel.SAPHANACompositeRoleInfo.GetByEndPointSystemId(ProcessorConfig.EndpointId);
            var composite = (from l in listSAPRoles
                             from m in listSAPCompositeRoles
                             where l.ROLE_CODE == m.CompositeRole_Code
                             select l);

            if (composite.Count() > 0)
            {
                foreach (SAPHANARoleInfo role in composite)
                {
                    role.RoleType = Constants.COMPOSITE;
                }
            }
            // saving Composite roleType data
            SAPHANARoleInfo.SaveRoles(composite.ToList<SAPHANARoleInfo>(), ProcessorConfig.EndpointId, ServiceUser);
            listSAPCompositeRoles = null;
            // Updating remaining roles to Single Role type
            SAPHANARoleInfo.UpdateRolesAsSingleIfNull(ProcessorConfig.EndpointId, ServiceUser);
            listSAPRoles = null;
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed UpdateSAPHANARoleType.", DiscoveryObjectType.Role);
        }
        public IList<SAPHANARoleTCodeMapping> ProcessRoleTCodeMapping()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleTCodeMapping.", DiscoveryObjectType.Role);
            var lstRoleTCodeMapping = new List<SAPHANARoleTCodeMapping>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                var objSapSingleRoleTcodeMap = new SAPHANARoleTCodeMapping();
                do
                {
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_TCODES", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        objSapSingleRoleTcodeMap = new SAPHANARoleTCodeMapping();
                        objSapSingleRoleTcodeMap.TCode = row["TCODE"].ToString().Trim();
                        objSapSingleRoleTcodeMap.RoleCode = row["AGR_NAME"].ToString().Trim();
                        objSapSingleRoleTcodeMap.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        objSapSingleRoleTcodeMap.Version = Convert.ToInt16(VersionNo.Available);
                        objSapSingleRoleTcodeMap.CreatedDate = DateTime.Now.Date;
                        objSapSingleRoleTcodeMap.CreatedBy = ServiceUser;
                        lstRoleTCodeMapping.Add(objSapSingleRoleTcodeMap);

                        if (lstRoleTCodeMapping.Count % ProcessorConfig.insertThreshold == 0 && lstRoleTCodeMapping.Count > 0)
                        {
                            try
                            {
                                // since we are deleting all mapping entries before saving, no need to check for existing
                                SAPHANARoleTCodeMapping.SaveSapRoleTCodeMapping(lstRoleTCodeMapping, ServiceUser);
                            }
                            catch (Exception ex)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"ProcessRoleTCodeMapping exception:{ex}", ex);
                            }
                            lstRoleTCodeMapping.Clear();
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                      ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                try
                {
                    // save any items not saved above
                    SAPHANARoleTCodeMapping.SaveSapRoleTCodeMapping(lstRoleTCodeMapping, ServiceUser);
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Final List ProcessRoleTCodeMapping exception:{ex}", ex);
                }
               
                lstRoleTCodeMapping.Clear();
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessRoleTCodeMapping.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            return lstRoleTCodeMapping;
        }

        /// <summary>
        /// Role to Profile mapping. 
        /// To be tested.
        /// </summary>
        /// <returns></returns>
        public IList<SAPHANARoleProfileMapping> ProcessRoleProfileMapping()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleProfileMapping.", DiscoveryObjectType.Role);
            var lstRoleProfileMapping = new List<SAPHANARoleProfileMapping>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                string filter = LangFilter;
                long count = 0;
                var objSapSingleRoleProfileMap = new SAPHANARoleProfileMapping();
                do
                {
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_PROF", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        objSapSingleRoleProfileMap = new SAPHANARoleProfileMapping();
                        objSapSingleRoleProfileMap.RoleName = row["AGR_NAME"].ToString().Trim();
                        objSapSingleRoleProfileMap.ProfileName = row["PROFILE"].ToString().Trim();
                        objSapSingleRoleProfileMap.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        objSapSingleRoleProfileMap.Version = Convert.ToInt16(VersionNo.Available);
                        lstRoleProfileMapping.Add(objSapSingleRoleProfileMap);
                        count++;
                        if (lstRoleProfileMapping.Count % ProcessorConfig.insertThreshold == 0 && lstRoleProfileMapping.Count > 0)
                        {
                            // since we are deleting all mapping entries before saving, no need to check for existing
                            SAPHANARoleProfileMapping.SaveSapRoleProfileMapping(lstRoleProfileMapping, ServiceUser);
                            lstRoleProfileMapping.Clear();
                        }

                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                         ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (lstRoleProfileMapping.Count > 0)
                {
                    // save any items not saved above
                    SAPHANARoleProfileMapping.SaveSapRoleProfileMapping(lstRoleProfileMapping, ServiceUser);
                    lstRoleProfileMapping.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessRoleProfileMapping. Count: " + count, DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            return lstRoleProfileMapping;
        }
        private IList<SAPHANARoleToAuthMap> ProcessRoleToAuthMap()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleToAuthMap.", DiscoveryObjectType.Role);
            IList<SAPHANARoleToAuthMap> lstSapRoleToAuthMap = new List<SAPHANARoleToAuthMap>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                long count = 0;
                SAPHANARoleToAuthMap sapRoleToAuthMap = null;
                var filter = $"DELETED NE 'X'";
                do
                {

                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_1251", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtParameters.Rows)
                    {

                        sapRoleToAuthMap = new SAPHANARoleToAuthMap();
                        sapRoleToAuthMap.RoleName = row["AGR_NAME"].ToString().Trim();
                        sapRoleToAuthMap.BiwMenuCounter = row["COUNTER"].ToString().Trim();
                        sapRoleToAuthMap.ObjectCode = row["OBJECT"].ToString().Trim();
                        sapRoleToAuthMap.AuthCode = row["AUTH"].ToString().Trim();
                        sapRoleToAuthMap.ProfileGeneratorVariant = row["VARIANT"].ToString().Trim();
                        sapRoleToAuthMap.FieldCode = row["FIELD"].ToString().Trim();
                        sapRoleToAuthMap.FromValue = row["LOW"].ToString().Trim();
                        sapRoleToAuthMap.ToValue = row["HIGH"].ToString().Trim();
                        sapRoleToAuthMap.Version = Convert.ToInt16(VersionNo.Available);
                        sapRoleToAuthMap.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        sapRoleToAuthMap.CreatedBy = this.ServiceUser;
                        sapRoleToAuthMap.CreatedDate = DateTime.Now.Date;
                        count++;
                        lstSapRoleToAuthMap.Add(sapRoleToAuthMap);

                        if (lstSapRoleToAuthMap.Count % ProcessorConfig.insertThreshold == 0 && lstSapRoleToAuthMap.Count > 0)
                        {
                            try
                            {
                                SAPHANARoleToAuthMap.SaveSapRoleAuthMap(lstSapRoleToAuthMap, ServiceUser);
                            }
                            catch (Exception ex)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"SaveSapRoleAuthMap AGR_1251 Running count:{count} failed:{ex}", ex);
                            }
                            lstSapRoleToAuthMap.Clear();
                        }

                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (lstSapRoleToAuthMap.Count > 0)
                {
                    try
                    {
                        SAPHANARoleToAuthMap.SaveSapRoleAuthMap(lstSapRoleToAuthMap, (int)ProcessorConfig.EndpointId, ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"SaveSapRoleAuthMap AGR_1251 count:{count} failed:{ex}", ex);
                    }
                    lstSapRoleToAuthMap.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessRoleToAuthMap. Count: " + count, DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"{ex}", ex);

            }
            return lstSapRoleToAuthMap;
        }
        /// <summary>
        /// FOr testing purpose
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public IList<SAPHANARoleToAuthMap> ProcessRoleToAuthMap(string rolename, bool showDeleted = false)
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleToAuthMap.", DiscoveryObjectType.Role);
            var lstSapRoleToAuthMap = new List<SAPHANARoleToAuthMap>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                long count = 0;
                SAPHANARoleToAuthMap sapRoleToAuthMap = null;

                var filter = string.Empty;
                if (showDeleted)
                {
                    filter = $"DELETED NE 'X'";
                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    if (!string.IsNullOrWhiteSpace(rolename))
                        filter = filter + $"AND OBJECT EQ 'S_TCODE' AND AGR_NAME EQ '{rolename}'";

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(rolename))
                        filter = $"OBJECT EQ 'S_TCODE' AND AGR_NAME EQ '{rolename}'";
                }
                do
                {

                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_1251", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        sapRoleToAuthMap = new SAPHANARoleToAuthMap();
                        sapRoleToAuthMap.RoleName = row["AGR_NAME"].ToString().Trim();
                        sapRoleToAuthMap.BiwMenuCounter = row["COUNTER"].ToString().Trim();
                        sapRoleToAuthMap.ObjectCode = row["OBJECT"].ToString().Trim();
                        sapRoleToAuthMap.AuthCode = row["AUTH"].ToString().Trim();
                        sapRoleToAuthMap.ProfileGeneratorVariant = row["VARIANT"].ToString().Trim();
                        sapRoleToAuthMap.FieldCode = row["FIELD"].ToString().Trim();
                        sapRoleToAuthMap.FromValue = row["LOW"].ToString().Trim();
                        sapRoleToAuthMap.ToValue = row["HIGH"].ToString().Trim();
                        sapRoleToAuthMap.Version = Convert.ToInt16(VersionNo.Available);
                        sapRoleToAuthMap.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        count++;
                        lstSapRoleToAuthMap.Add(sapRoleToAuthMap);
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessRoleToAuthMap. Count: " + count, DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            return lstSapRoleToAuthMap;
        }
        public void ProcessCompositeChildRoleMapping()
        {
            var listCompositeChildRoleMap = new List<SAPHANAUserinfo.SAPHANACompositeChildRoleMap>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessCompositeChildRoleMapping.", DiscoveryObjectType.Role);
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable m_CompositeRoles = null;
                var SAPCompositeChildRoleMap = new SAPHANAUserinfo.SAPHANACompositeChildRoleMap();
                var objSAPCompositeRoleInfo = new DomainModel.SAPHANACompositeRoleInfo();
                var objSAPSingleRoleInfo = new SAPHANASingleRoleInfo();
                do
                {
                    m_CompositeRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_AGRS", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in m_CompositeRoles.Rows)
                    {

                        SAPCompositeChildRoleMap = new SAPHANAUserinfo.SAPHANACompositeChildRoleMap();
                        objSAPCompositeRoleInfo = new DomainModel.SAPHANACompositeRoleInfo();
                        objSAPSingleRoleInfo = new SAPHANASingleRoleInfo();
                        objSAPCompositeRoleInfo = SAPHANACompositeRoleInfo.GetByRoleCodeIncludingSAPSingleRoleInfos(row["AGR_NAME"].ToString().Trim(), ProcessorConfig.EndpointId);

                        if (objSAPCompositeRoleInfo != null)
                        {
                            SAPCompositeChildRoleMap.SAPHANACompositeRoleInfo_Id = objSAPCompositeRoleInfo.Id;
                            objSAPSingleRoleInfo = SAPHANASingleRoleInfo.GetByRoleCode(ProcessorConfig.EndpointId, row["CHILD_AGR"].ToString().Trim());
                            if (objSAPSingleRoleInfo != null)
                            {
                                SAPCompositeChildRoleMap.SAPHANASingleRoleInfo_Id = objSAPSingleRoleInfo.Id;
                                SAPCompositeChildRoleMap.EndpointSystemId = ProcessorConfig.EndpointId;
                                SAPCompositeChildRoleMap.Version = (int)VersionNo.Available;

                                if (listCompositeChildRoleMap.FirstOrDefault(CompChildRole =>
                                                                        CompChildRole.SAPHANACompositeRoleInfo_Id == objSAPCompositeRoleInfo.Id
                                                                        && CompChildRole.SAPHANASingleRoleInfo_Id == objSAPSingleRoleInfo.Id
                                                                        && CompChildRole.EndpointSystemId == ProcessorConfig.EndpointId) == null)
                                    listCompositeChildRoleMap.Add(SAPCompositeChildRoleMap);
                            }
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (m_CompositeRoles.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                SAPHANAUserinfo.SaveCompositeChildRoleMap(listCompositeChildRoleMap, ProcessorConfig.EndpointId);

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessCompositeChildRoleMapping.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
        }

        #region ForTool
        public List<SAPHANARoleInfo> GetRoles(string search = null)
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetRoles.", DiscoveryObjectType.Role);

            var lstRoleInfo = new List<SAPHANARoleInfo>();
            var sapRoleInfo = new SAPHANARoleInfo();

            DataTable dtRoles = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                try
                {
                    dtRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_DEFINE", null, null, search, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    if (dtRoles == null)
                        return lstRoleInfo;

                    foreach (DataRow row in dtRoles.Rows)
                    {
                        sapRoleInfo = new SAPHANARoleInfo();
                        sapRoleInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                        sapRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                        sapRoleInfo.TargetSysId = null;
                        sapRoleInfo.ROLE_CODE = row["AGR_NAME"].ToString().Trim();
                        lstRoleInfo.Add(sapRoleInfo);
                    }
                    return lstRoleInfo;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                Dictionary<string, string> dictRoleDescs = null;

                try
                {
                    // get role descriptions from role text table
                    dictRoleDescs = GetRoleDescriptions();


                    string deltaDate = ProcessorConfig.DeltaDate.ToString("yyyyMMdd");
                    ProcessorConfig.sapUtil.ResetPaging();

                    string filter;
                    if (ProcessorConfig.IsDelta)
                    {
                        filter = "CHANGE_DAT >= '" + deltaDate + "' ";
                        filter += "OR CREATE_DAT >= '" + deltaDate + "' ";
                    }
                    else
                    {
                        filter = string.Empty;
                    }

                    do
                    {

                        dtRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_DEFINE", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);

                        foreach (DataRow row in dtRoles.Rows)
                        {
                            sapRoleInfo = new SAPHANARoleInfo();
                            sapRoleInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                            sapRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                            sapRoleInfo.TargetSysId = null;
                            sapRoleInfo.ROLE_CODE = row["AGR_NAME"].ToString().Trim();

                            // get the description from dictionary
                            if (dictRoleDescs.ContainsKey(sapRoleInfo.ROLE_CODE))
                            {
                                sapRoleInfo.ROLE_DESC = dictRoleDescs[sapRoleInfo.ROLE_CODE];
                            }

                            // check whether this role code is already added
                            if (!lstRoleInfo.Any(r => r.ROLE_CODE == sapRoleInfo.ROLE_CODE))
                            {
                                lstRoleInfo.Add(sapRoleInfo);
                            }
                        }

                        if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                             ProcessorConfig.sapUtil.NextPage();

                    } while (dtRoles.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed GetRoles.", DiscoveryObjectType.Role);
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                    throw ex;
                }
                finally
                {
                    if (dictRoleDescs != null)
                    {
                        dictRoleDescs.Clear();
                        dictRoleDescs = null;
                    }
                }
                return lstRoleInfo;
            }
        }
        #endregion
    }
}
