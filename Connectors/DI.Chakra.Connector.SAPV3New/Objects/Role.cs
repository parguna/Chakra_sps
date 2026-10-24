using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector.SAPV3
{
    public class Role
    {
        public ProcessorConfig ProcessorConfig { get; set; }
        private const string LangFilter = "LANGU EQ 'E'";
        string ServiceUser;
        public Role(string _serviceUser)
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
                var deletedRoles = SAPRoleInfo.GetDeletedByModifiedDate(ProcessorConfig.EndpointId, ProcessorConfig.Process.ProcessStartAt);
                SAPRoleInfo.DeleteList(deletedRoles);
            }
            try
            {
                SAPCompositeRoleInfo.DeleteSAPCompositeRoleInfoByEndpoint(ProcessorConfig.EndpointId, ServiceUser);

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"DeleteSAPCompositeRoleInfoByEndpoint: {ex.Message}", ex);
            }

            ProcessCompositeRoles();

            try
            {
                SAPSingleRoleInfo.DeleteSAPSingleRoleInfoByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"DeleteSAPSingleRoleInfoByEndpoint: {ex.Message}", ex);
            }
            ProcessChildRoles();

            UpdateSAPRoleType();

            ProcessCompositeChildRoleMapping();
            try
            {
                // delete existing role tcode mapping
                SapRoleTCodeMapping.DeleteSapRoleTCodeMappingForEndpoint(ProcessorConfig.EndpointId);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"DeleteSapRoleTCodeMappingForEndpoint & ProcessChildRoles: {ex.Message}", ex);
            }

            //  insert new entries
            ProcessRoleTCodeMapping();
            try
            {
                SAPRoleToTCodes.UpdateRoleToTcodeAfterDiscovery(ProcessorConfig.EndpointId);
                SAPRoleToTCodes.DeleteSapRoleTCodeMappingForEndpoint(ProcessorConfig.EndpointId, ProcessorConfig.Process.ProcessStartAt);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Error While Generating Role To TCodes.", DiscoveryObjectType.Role, ex);
            }

            // delete existing role Auth mapping
            SAPRoleToAuthMap.DeleteSAPRoleToAuthMapForEndpoint(ProcessorConfig.EndpointId);
            //  insert new entries
            ProcessRoleToAuthMap();

            //// delete existing role profile mapping
            //SapRoleProfileMapping.DeleteSapRoleProfileMappingForEndpoint(ProcessorConfig.EndpointId);
            ////  insert new entries
            //ProcessRoleProfileMapping();
        }
        public IList<SAPRoleInfo> ProcessRoles()
        {
            Dictionary<string, string> dictRoleDescs = null;
            Dictionary<string, SAPRoleInfo> dictExistingRoles = null;
            IList<SAPRoleInfo> lstRoleInfo = new List<SAPRoleInfo>();
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
                dictExistingRoles = new Dictionary<string, SAPRoleInfo>();
                var listExistingRoles = SAPRoleInfo.GetByEndpointSystemId(ProcessorConfig.EndpointId);
                // creating dictionary items from list. Could use LINQ to the same as well
                foreach (var rol in listExistingRoles)
                {
                    if (!dictExistingRoles.ContainsKey(rol.ROLE_CODE))
                    {
                        dictExistingRoles.Add(rol.ROLE_CODE, rol);
                    }
                }
                listExistingRoles.Clear();

                SAPRoleInfo sapRoleInfo = new SAPRoleInfo();
                do
                {

                    dtRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_DEFINE", null, null, filter);

                    foreach (DataRow row in dtRoles.Rows)
                    {
                        sapRoleInfo = new SAPRoleInfo();
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
                            SAPRoleInfo.SaveRoles(lstRoleInfo, ServiceUser, dictExistingRoles);
                            lstRoleInfo.Clear();
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (dtRoles.Rows.Count > 0);

                // add any remaining roles missed abvoe
                if (lstRoleInfo.Count > 0)
                {
                    SAPRoleInfo.SaveRoles(lstRoleInfo, ServiceUser, dictExistingRoles);
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
                    dtRoleDescs = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_TEXTS", null, fields, filter);
                    foreach (DataRow row in dtRoleDescs.Rows)
                    {
                        name = row["AGR_NAME"].ToString().Trim();
                        desc = row["TEXT"].ToString().Trim();

                        if (!dictRoleDesc.ContainsKey(name))
                        {
                            dictRoleDesc.Add(name, desc);
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();
                } while (dtRoleDescs.Rows.Count > 0);
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

        private IList<DomainModel.SAPCompositeRoleInfo> ProcessCompositeRoles()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessCompositeRoles.", DiscoveryObjectType.Role);
            var listSAPCompositeRoleInfo = new List<DomainModel.SAPCompositeRoleInfo>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable m_CompositeRoles = null;

                // get a hashset of all composite role codes
                HashSet<string> hsExistingitems = new HashSet<string>();
                DomainModel.SAPCompositeRoleInfo sapCompositeRoleInfo = new DomainModel.SAPCompositeRoleInfo();
                do
                {
                    m_CompositeRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_AGRS", null, null, null);

                    foreach (DataRow row in m_CompositeRoles.Rows)
                    {
                        sapCompositeRoleInfo = new DomainModel.SAPCompositeRoleInfo();
                        sapCompositeRoleInfo.CompositeRole_Code = row["AGR_NAME"].ToString().Trim();
                        sapCompositeRoleInfo.CompositeRole_Desc = String.Empty;
                        sapCompositeRoleInfo.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        sapCompositeRoleInfo.Version = Convert.ToInt16(VersionNo.Available);

                        if (!hsExistingitems.Contains(sapCompositeRoleInfo.CompositeRole_Code))
                        {
                            hsExistingitems.Add(sapCompositeRoleInfo.CompositeRole_Code);
                            listSAPCompositeRoleInfo.Add(sapCompositeRoleInfo);
                        }

                        if (listSAPCompositeRoleInfo.Count % ProcessorConfig.insertThreshold == 0 && listSAPCompositeRoleInfo.Count > 0)
                        {
                            SAPCompositeRoleInfo.SaveSAPCompositeRoles(listSAPCompositeRoleInfo, ServiceUser);
                            listSAPCompositeRoleInfo.Clear();
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (m_CompositeRoles.Rows.Count > 0);

                // save any remaining Composite roles
                if (listSAPCompositeRoleInfo.Count > 0)
                {
                    SAPCompositeRoleInfo.SaveSAPCompositeRoles(listSAPCompositeRoleInfo, ServiceUser);
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

        private IList<SAPSingleRoleInfo> ProcessChildRoles()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessChildRoles.", DiscoveryObjectType.Role);
            var listSAPSingleRoleInfo = new List<SAPSingleRoleInfo>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable m_CompositeRoles = null;

                // get a hashset of all composite role codes
                HashSet<string> hsExistingitems = new HashSet<string>();
                SAPSingleRoleInfo SAPSingleRoleInfo = new SAPSingleRoleInfo();
                do
                {
                    m_CompositeRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_AGRS", null, null, null);
                    foreach (DataRow row in m_CompositeRoles.Rows)
                    {
                        SAPSingleRoleInfo = new SAPSingleRoleInfo();

                        SAPSingleRoleInfo.SingleRole_Code = row["CHILD_AGR"].ToString().Trim();
                        SAPSingleRoleInfo.SingleRole_Desc = String.Empty;
                        SAPSingleRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                        SAPSingleRoleInfo.EndPointSystemId = (int)ProcessorConfig.EndpointId;

                        if (!hsExistingitems.Contains(SAPSingleRoleInfo.SingleRole_Code))
                        {
                            hsExistingitems.Add(SAPSingleRoleInfo.SingleRole_Code);
                            listSAPSingleRoleInfo.Add(SAPSingleRoleInfo);
                        }

                        if (listSAPSingleRoleInfo.Count % ProcessorConfig.insertThreshold == 0 && listSAPSingleRoleInfo.Count > 0)
                        {
                            DI.Chakra.DomainModel.SAPSingleRoleInfo.SaveSingleRoles(listSAPSingleRoleInfo, ServiceUser);
                            listSAPSingleRoleInfo.Clear();
                        }

                    }
                    ProcessorConfig.sapUtil.NextPage();

                } while (m_CompositeRoles.Rows.Count > 0);

                // save the last remaining items, if any
                if (listSAPSingleRoleInfo.Count > 0)
                {
                    DI.Chakra.DomainModel.SAPSingleRoleInfo.SaveSingleRoles(listSAPSingleRoleInfo, ServiceUser);
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
            IList<SAPRoleInfo> listSAPRoles = new List<SAPRoleInfo>();
            IList<DomainModel.SAPCompositeRoleInfo> listSAPCompositeRoles = new List<DomainModel.SAPCompositeRoleInfo>();

            listSAPRoles = SAPRoleInfo.GetByEndpointSystemId(ProcessorConfig.EndpointId);
            listSAPCompositeRoles = DomainModel.SAPCompositeRoleInfo.GetByEndPointSystemId(ProcessorConfig.EndpointId);
            var composite = (from l in listSAPRoles
                             from m in listSAPCompositeRoles
                             where l.ROLE_CODE == m.CompositeRole_Code
                             select l);

            if (composite.Count() > 0)
            {
                foreach (SAPRoleInfo role in composite)
                {
                    role.RoleType = Constants.COMPOSITE;
                }
            }
            // saving Composite roleType data
            SAPRoleInfo.SaveRoles(composite.ToList<SAPRoleInfo>(), ProcessorConfig.EndpointId, ServiceUser);
            listSAPCompositeRoles = null;


            //var single = (listSAPRoles.Where(r => string.IsNullOrEmpty(r.RoleType)).ToList());


            //if (single.Count() > 0)
            //{
            //    foreach (SAPRoleInfo role in single)
            //    {
            //        role.RoleType = Constants.SINGLE;
            //    }
            //}
            // Updating remaining roles to Single Role type
            SAPRoleInfo.UpdateRolesAsSingleIfNull(ProcessorConfig.EndpointId, ServiceUser);
            listSAPRoles = null;
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed UpdateSAPRoleType.", DiscoveryObjectType.Role);
        }
        public IList<SapRoleTCodeMapping> ProcessRoleTCodeMapping()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleTCodeMapping.", DiscoveryObjectType.Role);
            var lstRoleTCodeMapping = new List<SapRoleTCodeMapping>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                SapRoleTCodeMapping objSapSingleRoleTcodeMap = new SapRoleTCodeMapping();
                do
                {
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_TCODES", null, null, null);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        objSapSingleRoleTcodeMap = new SapRoleTCodeMapping();
                        objSapSingleRoleTcodeMap.TCode = row["TCODE"].ToString().Trim();
                        objSapSingleRoleTcodeMap.RoleCode = row["AGR_NAME"].ToString().Trim();
                        objSapSingleRoleTcodeMap.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        objSapSingleRoleTcodeMap.Version = Convert.ToInt16(VersionNo.Available);
                        lstRoleTCodeMapping.Add(objSapSingleRoleTcodeMap);

                        if (lstRoleTCodeMapping.Count % ProcessorConfig.insertThreshold == 0 && lstRoleTCodeMapping.Count > 0)
                        {
                            // since we are deleting all mapping entries before saving, no need to check for existing
                            SapRoleTCodeMapping.SaveSapRoleTCodeMapping(lstRoleTCodeMapping, ServiceUser);
                            lstRoleTCodeMapping.Clear();
                        }

                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0);

                // save any items not saved above
                SapRoleTCodeMapping.SaveSapRoleTCodeMapping(lstRoleTCodeMapping, ServiceUser);
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
        public IList<SapRoleProfileMapping> ProcessRoleProfileMapping()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleProfileMapping.", DiscoveryObjectType.Role);
            var lstRoleProfileMapping = new List<SapRoleProfileMapping>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                string filter = LangFilter;
                long count = 0;
                SapRoleProfileMapping objSapSingleRoleProfileMap = new SapRoleProfileMapping();
                do
                {
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_PROF", null, null, filter);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        objSapSingleRoleProfileMap = new SapRoleProfileMapping();
                        objSapSingleRoleProfileMap.RoleName = row["AGR_NAME"].ToString().Trim();
                        objSapSingleRoleProfileMap.ProfileName = row["PROFILE"].ToString().Trim();
                        objSapSingleRoleProfileMap.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        objSapSingleRoleProfileMap.Version = Convert.ToInt16(VersionNo.Available);
                        lstRoleProfileMapping.Add(objSapSingleRoleProfileMap);
                        count++;
                        if (lstRoleProfileMapping.Count % ProcessorConfig.insertThreshold == 0 && lstRoleProfileMapping.Count > 0)
                        {
                            // since we are deleting all mapping entries before saving, no need to check for existing
                            SapRoleProfileMapping.SaveSapRoleProfileMapping(lstRoleProfileMapping, ServiceUser);
                            lstRoleProfileMapping.Clear();
                        }

                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0);

                if (lstRoleProfileMapping.Count > 0)
                {
                    // save any items not saved above
                    SapRoleProfileMapping.SaveSapRoleProfileMapping(lstRoleProfileMapping, ServiceUser);
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
        private IList<SAPRoleToAuthMap> ProcessRoleToAuthMap()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleToAuthMap.", DiscoveryObjectType.Role);
            IList<SAPRoleToAuthMap> lstSapRoleToAuthMap = new List<SAPRoleToAuthMap>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                long count = 0;
                SAPRoleToAuthMap sapRoleToAuthMap = null;
                var filter = $"DELETED NE 'X'";
                do
                {

                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_1251", null, null, filter);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        sapRoleToAuthMap = new SAPRoleToAuthMap();
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

                        if (lstSapRoleToAuthMap.Count % ProcessorConfig.insertThreshold == 0 && lstSapRoleToAuthMap.Count > 0)
                        {
                            //SAPRoleToAuthMap.SaveSapRoleAuthMap(lstSapRoleToAuthMap, (int)ProcessorConfig.EndpointId, ServiceUser);
                            SAPRoleToAuthMap.SaveSapRoleAuthMap(lstSapRoleToAuthMap, ServiceUser);
                            lstSapRoleToAuthMap.Clear();
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0);

                if (lstSapRoleToAuthMap.Count > 0)
                {
                    SAPRoleToAuthMap.SaveSapRoleAuthMap(lstSapRoleToAuthMap, (int)ProcessorConfig.EndpointId, ServiceUser);
                    lstSapRoleToAuthMap.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessRoleToAuthMap. Count: " + count, DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
            return lstSapRoleToAuthMap;
        }
        /// <summary>
        /// FOr testing purpose
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public IList<SAPRoleToAuthMap> ProcessRoleToAuthMap(string rolename, bool showDeleted = false)
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessRoleToAuthMap.", DiscoveryObjectType.Role);
            IList<SAPRoleToAuthMap> lstSapRoleToAuthMap = new List<SAPRoleToAuthMap>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable dtParameters = null;
                long count = 0;
                SAPRoleToAuthMap sapRoleToAuthMap = null;

                var filter = string.Empty;
                if (showDeleted)
                {
                    filter = $"DELETED NE 'X'";
                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                     if(!string.IsNullOrWhiteSpace(rolename))
                        filter = filter + $"AND OBJECT EQ 'S_TCODE' AND AGR_NAME EQ '{rolename}'";

                }else
                {
                    if (!string.IsNullOrWhiteSpace(rolename))
                        filter = $"OBJECT EQ 'S_TCODE' AND AGR_NAME EQ '{rolename}'";
                }
                do
                {

                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_1251", null, null, filter);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        sapRoleToAuthMap = new SAPRoleToAuthMap();
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

                    ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0);
                
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
            IList<SAPUserinfo.SAPCompositeChildRoleMap> listCompositeChildRoleMap = new List<SAPUserinfo.SAPCompositeChildRoleMap>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessCompositeChildRoleMapping.", DiscoveryObjectType.Role);
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DataTable m_CompositeRoles = null;
                SAPUserinfo.SAPCompositeChildRoleMap SAPCompositeChildRoleMap = new SAPUserinfo.SAPCompositeChildRoleMap();
                DomainModel.SAPCompositeRoleInfo objSAPCompositeRoleInfo = new DomainModel.SAPCompositeRoleInfo();
                SAPSingleRoleInfo objSAPSingleRoleInfo = new SAPSingleRoleInfo();
                do
                {
                    m_CompositeRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_AGRS", null, null, null);
                    foreach (DataRow row in m_CompositeRoles.Rows)
                    {

                        SAPCompositeChildRoleMap = new SAPUserinfo.SAPCompositeChildRoleMap();
                        objSAPCompositeRoleInfo = new DomainModel.SAPCompositeRoleInfo();
                        objSAPSingleRoleInfo = new SAPSingleRoleInfo();
                        objSAPCompositeRoleInfo = SAPCompositeRoleInfo.GetByRoleCodeIncludingSAPSingleRoleInfos(row["AGR_NAME"].ToString().Trim(), ProcessorConfig.EndpointId);

                        if (objSAPCompositeRoleInfo != null)
                        {
                            SAPCompositeChildRoleMap.SAPCompositeRoleInfo_Id = objSAPCompositeRoleInfo.Id;
                            objSAPSingleRoleInfo = SAPSingleRoleInfo.GetByRoleCode(ProcessorConfig.EndpointId, row["CHILD_AGR"].ToString().Trim());
                            if (objSAPSingleRoleInfo != null)
                            {
                                SAPCompositeChildRoleMap.SAPSingleRoleInfo_Id = objSAPSingleRoleInfo.Id;
                                SAPCompositeChildRoleMap.EndpointSystemId = ProcessorConfig.EndpointId;
                                SAPCompositeChildRoleMap.Version = (int)VersionNo.Available;

                                if (listCompositeChildRoleMap.FirstOrDefault(CompChildRole =>
                                                                        CompChildRole.SAPCompositeRoleInfo_Id == objSAPCompositeRoleInfo.Id
                                                                        && CompChildRole.SAPSingleRoleInfo_Id == objSAPSingleRoleInfo.Id
                                                                        && CompChildRole.EndpointSystemId == ProcessorConfig.EndpointId) == null)
                                    listCompositeChildRoleMap.Add(SAPCompositeChildRoleMap);
                            }
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (m_CompositeRoles.Rows.Count > 0);

                SAPUserinfo.SaveCompositeChildRoleMap(listCompositeChildRoleMap, ProcessorConfig.EndpointId);

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessCompositeChildRoleMapping.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            }
        }

        #region ForTool
        public List<SAPRoleInfo> GetRoles(string search = null)
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetRoles.", DiscoveryObjectType.Role);

            List<SAPRoleInfo> lstRoleInfo = new List<SAPRoleInfo>();
            SAPRoleInfo sapRoleInfo = new SAPRoleInfo();

            DataTable dtRoles = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                try
                {
                    dtRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_DEFINE", null, null, search);
                    if (dtRoles == null)
                        return lstRoleInfo;

                    foreach (DataRow row in dtRoles.Rows)
                    {
                        sapRoleInfo = new SAPRoleInfo();
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

                        dtRoles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("AGR_DEFINE", null, null, filter);

                        foreach (DataRow row in dtRoles.Rows)
                        {
                            sapRoleInfo = new SAPRoleInfo();
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

                        ProcessorConfig.sapUtil.NextPage();

                    } while (dtRoles.Rows.Count > 0);

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
