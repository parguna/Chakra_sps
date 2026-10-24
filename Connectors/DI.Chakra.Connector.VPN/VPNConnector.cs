using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector
{
    public class VPNConnector : ConnectorBase
    {
        #region private
        private string IPAddress = string.Empty;
        bool IsUserSelected = false;
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private List<VPNApplicationEnvironment> vpnApplicationEnvironment = new List<VPNApplicationEnvironment>();
        private List<VPNApplicationEnvironment> vpnApplicationEnv = new List<VPNApplicationEnvironment>();
        private List<VPNApplicationEnvironment> vpnProjectEnv = new List<VPNApplicationEnvironment>();

        private List<VPNMatrix> vpnMatrix = new List<VPNMatrix>();
        private List<VPNMatrix> projectMatrix = new List<VPNMatrix>();
        List<EntitlementReconcilation> existingEntilements = new List<EntitlementReconcilation>();
        private List<KeyValuePair<long, long>> vpnUserOneKeyToken = new List<KeyValuePair<long, long>>();
        private string currentUser;
        #endregion
        public List<VPNUserInfo> VPNUserList { get; set; } = new List<VPNUserInfo>();
        public VPNConnector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.IPAddress = Convert.ToString(Configuration(EndpointKey.IPAddress));
            this.currentUser = MasterConfigHelper.ServiceUser;
        }
        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        public override bool Connect()
        {
            //We are connecting to DI_M_User for VPN discovery.Hence by default returning true. 
            return true;
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            return true;
        }

        public override void Disconnect()
        {
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override void GetData()
        {
            var discoveryProcessId = base.DiscoveryProcess.Id;
            List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(discoveryProcessId);
            IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_VPN_User) != null;
            try
            {
                DiscoverUser();
                //this.AddUpdateEntitlementRecon();
                CreateUserApplicationEnviornment();
                VPNUserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, discoveryProcessId);
                VPNUserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, discoveryProcessId);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Discovery failed for VPN system", ex);
                throw ex;
            }
            finally
            {
                EndpointObjects?.Clear();
                EndpointObjects = null;
                this.vpnApplicationEnvironment?.Clear();
                this.vpnApplicationEnvironment = null;
            }
        }
        private void CreateUserApplicationEnviornment()
        {
            if (ConstructVPNApplicationEnviornment())
            {
                this.vpnApplicationEnvironment = VPNApplicationEnvironment.GetAll();
                if (this.vpnApplicationEnvironment.Any())
                {
                    this.vpnApplicationEnv = this.vpnApplicationEnvironment.Where(i=>i.MatrixType == VPNMatrixType.Application).ToList();
                    this.vpnProjectEnv = this.vpnApplicationEnvironment.Where(i => i.MatrixType == VPNMatrixType.Project).ToList();
                    var vpnUsers = VPNUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                    var adEndpoint = DI.Chakra.DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);

                    foreach (var vpnUser in vpnUsers)
                    {
                        var adAccountRecon = AccountReconcilation.GetByDIUserIdByAccountType(vpnUser.UserId, adEndpoint.Id);
                        if (adAccountRecon == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AD AccountRecon does not exists");
                            continue;
                        }
                        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AD AccountRecon exists for VPN User{vpnUser.UserName}");
                        var adEntitlementRecons = EntitlementReconcilation.GetAllByActiveAccountReconcilationId(adAccountRecon.Id);
                        if (adEntitlementRecons.Any())
                        {
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AD Entitlement exists for VPN User{vpnUser.OneKeyTokenId}");
                            var vpnAccountRecon = AccountReconcilation.GetByAccountName(vpnUser.OneKeyTokenId, this.Endpoint.Id);
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN vpnAccountRecon is Null:{vpnAccountRecon == null}");
                            if (vpnAccountRecon != null)
                            {
                                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AddUpdateVPNUserEnviornment Table for VPN User{vpnUser.UserName}");
                                var isAll = (vpnUser.EmployeeType == (int)EmployeeType.Permanent || vpnUser.EmployeeType == (int)EmployeeType.TempStaff);
                                AddUpdateVPNUserEnviornment(vpnAccountRecon, adEntitlementRecons, isAll);
                            }
                        }
                    }
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN ProcessDeletedEntitlements started");
                    VPNUserApplicationEnviornment.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, currentUser);
                    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN ProcessDeletedEntitlements completed");
                    try
                    {
                        EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, currentUser);
                    }
                    catch (Exception ex)
                    {
                        this.WriteDiscoveryLog(LogLevelType.Exception, "Error while ProcessDelete Entitlements Recon.", ex);
                    }
                }
                
            }

        }

        private void AddUpdateVPNUserEnviornment(AccountReconcilation vpnAccountRecon, List<EntitlementReconcilation> adEntitlementRecons,bool isPermanent)
        {
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AddUpdateVPNUserEnviornment started");
            var delimiter = new char[] {','};
            var userApplnEnviornments = new List<VPNUserApplicationEnviornment>();
            List<VPNApplicationEnvironment> dataExists = new List<VPNApplicationEnvironment>();
            if (isPermanent)
            {
                //dataExists = vpnApplicationEnv.Where(i => adEntitlementRecons.Exists(adEnt => !string.IsNullOrWhiteSpace(i.ADGroups) && i.ADGroups.Split(delimiter).ToList().Contains(adEnt.Entitlement))).ToList();
                foreach (var item in this.vpnApplicationEnvironment)
                {
                    if (!string.IsNullOrWhiteSpace(item.ADGroups))
                    {
                        var allGroups = item.ADGroups.Split(delimiter).ToList();
                        foreach (var group in allGroups)
                        {
                            if (adEntitlementRecons.Exists(i => i.Entitlement.Equals(group, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (dataExists.Exists(i => i.Name.ToUpper() == item.Name.ToUpper())) continue;
                                    dataExists.Add(item);
                            }
                        }
                    }
                }
                foreach (var item in vpnProjectEnv)
                {
                    if (!string.IsNullOrWhiteSpace(item.ADGroups))
                    {
                        var allGroups = item.ADGroups.Split(delimiter).ToList();
                        foreach (var group in allGroups)
                        {
                            if (adEntitlementRecons.Exists(i => i.Entitlement.Equals(group, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (dataExists.Exists(i => i.Name.ToUpper() == item.Name.ToUpper())) continue;
                                dataExists.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                //Employee Type  Vendor and Contractor
                //dataExists = vpnApplicationEnv.Where(i => adEntitlementRecons.Exists(adEnt => i.MatrixType == VPNMatrixType.Project && !string.IsNullOrWhiteSpace(i.ADGroups) && i.ADGroups.Split(delimiter).ToList().Contains(adEnt.Entitlement))).ToList();
                foreach (var item in this.vpnProjectEnv)
                {
                    if (!string.IsNullOrWhiteSpace(item.ADGroups))
                    {
                        var allGroups = item.ADGroups.Split(delimiter).ToList();
                        foreach (var group in allGroups)
                        {
                            if (adEntitlementRecons.Exists(i => i.Entitlement.Equals(group, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (dataExists.Exists(i => i.Name.ToUpper() == item.Name.ToUpper())) continue;
                                    dataExists.Add(item);
                            }
                        }
                    }
                }
            }
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AddUpdateVPNUserEnviornment VPNApplicationEnviornment count:{dataExists.Count}");
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AddUpdateVPNUserEnviornment started");
            foreach (var vpnApplication in dataExists)
            {
                var appln = new VPNUserApplicationEnviornment()
                {
                    MatrixId = vpnApplication.MatrixId,
                    EndpointUserId = vpnAccountRecon.EndpointUserId,
                    ADGroups = vpnApplication.ADGroups,
                    MatrixType = vpnApplication.MatrixType,
                    EnviornmentType = vpnApplication.EnviornmentType,
                    Name = vpnApplication.Name,
                };
                userApplnEnviornments.Add(appln);
            }
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN VPNUserEnviornment Count:{userApplnEnviornments.Count}");
            foreach (var userApplnEnviornment in userApplnEnviornments)
            {
                var data = VPNUserApplicationEnviornment.GetByNameAndType(userApplnEnviornment.EndpointUserId, userApplnEnviornment.Name, userApplnEnviornment.MatrixType, userApplnEnviornment.EnviornmentType, userApplnEnviornment.MatrixId);
                if (data == null)
                {
                    data = userApplnEnviornment;
                    data.CreatedBy = currentUser;
                    data.CreatedDate = DateTime.Now;
                    data.Save(currentUser);
                }
                else
                {
                    data.ADGroups = userApplnEnviornment.ADGroups;
                    data.EndpointUserId = userApplnEnviornment.EndpointUserId;
                    data.MatrixId = userApplnEnviornment.MatrixId;
                    data.LastModifiedBy = currentUser;
                    data.LastModifiedDate = DateTime.Now;
                    data.Save(currentUser);
                }
                var entitlementType = data.MatrixType == VPNMatrixType.Application ? EntitlementType.Application : EntitlementType.Project;
                var vpnEntitlement = this.CreateEntitlementReconcilationShell(vpnAccountRecon, entitlementType, data.Name);
                vpnEntitlement.Entitlement = data.Name;
                vpnEntitlement.EntitlementDescription = data.ADGroups;
                vpnEntitlement.EntitlementId = data.Id;
                vpnEntitlement.Functions = data.EnviornmentType.ToString();
                var entRecon = EntitlementReconcilation.GetByEntitlement(vpnAccountRecon.EndpointSystemId, vpnAccountRecon.Id, data.Name, (int)entitlementType);
                if(entRecon == null)
                {
                    entRecon = vpnEntitlement;
                    EntitlementReconcilation.UpdateException(vpnEntitlement, true);
                    entRecon.CreatedDate = DateTime.Now;
                    entRecon.CreatedBy = "System";
                    entRecon.AccountName = vpnAccountRecon.AccountName;
                    entRecon.ValidToDate = vpnAccountRecon.ValidToDate;
                    entRecon.Save(MasterConfigHelper.ServiceUser);
                }
                else
                {
                    if (entRecon.IsDeleted)
                    {
                        EntitlementReconcilation.UpdateException(entRecon, true);
                    }
                    entRecon.ValidToDate = vpnAccountRecon.ValidToDate;
                    entRecon.AccountName = vpnAccountRecon.AccountName;
                    entRecon.Functions = vpnEntitlement.Functions;
                    entRecon.LastModifiedDate = DateTime.Now;
                    entRecon.LastModifiedBy = "System";
                    entRecon.Entitlement = vpnEntitlement.Entitlement;
                    entRecon.EntitlementId = vpnEntitlement.EntitlementId;
                    entRecon.EntitlementDescription = vpnEntitlement.EntitlementDescription;
                    
                    entRecon.Save(MasterConfigHelper.ServiceUser);
                }
            }
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"VPN AddUpdateVPNUserEnviornment Completed");
        }
        private void AddUpdateVPNOtherUserEnviornment(AccountReconcilation vpnAccountRecon, List<EntitlementReconcilation> adEntitlementRecons)
        {
            var userApplnEnviornments = new List<VPNUserApplicationEnviornment>();
            var dataExists = vpnProjectEnv.Where(i => adEntitlementRecons.Exists(adEnt => i.ADGroups.Contains(adEnt.Entitlement, StringComparison.InvariantCultureIgnoreCase)));
            foreach (var vpnApplication in dataExists)
            {
                userApplnEnviornments.Add(new VPNUserApplicationEnviornment()
                {
                    MatrixId = vpnApplication.MatrixId,
                    EndpointUserId = vpnAccountRecon.EndpointUserId,
                    ADGroups = vpnApplication.ADGroups,
                    MatrixType = vpnApplication.MatrixType,
                    EnviornmentType = vpnApplication.EnviornmentType,
                    Name = string.Format("{0}", vpnApplication.MatrixType == VPNMatrixType.Project ? vpnApplication.Name : string.Format("{0}-{1}", vpnApplication.Name, vpnApplication.EnviornmentType.GetDescription()))
                });
            }
            foreach (var userApplnEnviornment in userApplnEnviornments)
            {
                var data = VPNUserApplicationEnviornment.GetByNameAndType(userApplnEnviornment.EndpointUserId, userApplnEnviornment.Name, userApplnEnviornment.MatrixType, userApplnEnviornment.EnviornmentType, userApplnEnviornment.MatrixId);
                if (data == null)
                {
                    data.CreatedBy = currentUser;
                    data.CreatedDate = DateTime.Now;
                    data.Save(currentUser);
                }
                else
                {
                    data.LastModifiedBy = currentUser;
                    data.LastModifiedDate = DateTime.Now;
                    data.Save(currentUser);
                }
            }
        }
        private bool ConstructVPNApplicationEnviornment()
        {
            bool isSuccess = true;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, $"VPN: TruncateApplicationEnviornment");
                VPNApplicationEnvironment.TruncateApplicationEnviornment();
                this.vpnMatrix = VPNMatrix.GetActiveValidApplication();
                WriteDiscoveryLog(LogLevelType.Info, $"VPN: ActiveValidApplication count:{this.vpnMatrix.Count}");
                var vpnApplnEnvs = new List<VPNApplicationEnvironment>();
                WriteDiscoveryLog(LogLevelType.Info, $"VPN: ActiveValidApplication count:{vpnApplnEnvs.Count}");
                foreach (var vpnMatrixData in vpnMatrix)
                {
                    var tempResult = GetApplicationEnvironment(vpnMatrixData);
                    foreach (var item in tempResult)
                    {
                        var vpnEnv = VPNApplicationEnvironment.GetByMatrixId(item.MatrixId);
                        if (vpnEnv != null && vpnEnv.Count > 0)
                        {
                            var matched = vpnEnv.FirstOrDefault(i => i.MatrixType == item.MatrixType && i.EnviornmentType == item.EnviornmentType);
                            if (matched != null)
                            {
                                item.Id = matched.Id;
                            }
                        }
                        item.Save(currentUser);
                    }
                }
                WriteDiscoveryLog(LogLevelType.Info, $"VPN: GetApplicationEnvironment Save completed");
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "ConstructVPNApplicationEnviornment failed", ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        private static List<VPNApplicationEnvironment> GetApplicationEnvironment( VPNMatrix vpnMatrixData)
        {
            List<VPNApplicationEnvironment> vpnApplnEnvs = new List<VPNApplicationEnvironment>();
            if(vpnMatrixData.VPNMatrixType == VPNMatrixType.Application)
            {
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.ProductionADgroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = VPNMatrixType.Application,
                        EnviornmentType = VPNMatrixEnviornmentType.Production,
                        Name = string.Format("{0}-{1}", vpnMatrixData.Name, VPNMatrixEnviornmentType.Production.GetDescription()),
                        ADGroups = vpnMatrixData.ProductionADgroups,
                    });

                }
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.QAADGroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = VPNMatrixType.Application,
                        EnviornmentType = VPNMatrixEnviornmentType.QA,
                        Name = string.Format("{0}-{1}", vpnMatrixData.Name, VPNMatrixEnviornmentType.QA.GetDescription()),
                        ADGroups = vpnMatrixData.QAADGroups,
                    });
                }
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.DevelpomentADGroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = vpnMatrixData.VPNMatrixType,
                        EnviornmentType = VPNMatrixEnviornmentType.Development,
                        Name = string.Format("{0}-{1}", vpnMatrixData.Name, VPNMatrixEnviornmentType.Development.GetDescription()),
                        ADGroups = vpnMatrixData.DevelpomentADGroups,
                    });
                }
            }
            else if(vpnMatrixData.VPNMatrixType == VPNMatrixType.Project)
            {
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.ADGroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = vpnMatrixData.VPNMatrixType,
                        EnviornmentType = VPNMatrixEnviornmentType.ADGroup,
                        Name = vpnMatrixData.Name,
                        ADGroups = vpnMatrixData.ADGroups,
                    });
                }
            }
            return vpnApplnEnvs;
        }

        private void DiscoverUser()
        {
            WriteDiscoveryLog(LogLevelType.Info, "User Discovery started",DiscoveryObjectType.User);
            CreateUserAndOneKeyToken();
            if (this.VPNUserList.Count > 0)
            {
                VPNUserInfo.AddUpdateAccountRecon(this.VPNUserList, this.Endpoint.Id, MasterConfigHelper.ServiceUser);
                var userList = VPNUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                VPNUserInfo.RemoveUser(userList.ToList());
                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            WriteDiscoveryLog(LogLevelType.Info, "User Discovery completed", DiscoveryObjectType.User);
        }
        #region UserDiscovery
        private void CreateUserAndOneKeyToken()
        {
            WriteDiscoveryLog(  LogLevelType.Info, "Fetch User with Token Number started", DiscoveryObjectType.User);
            var users = User.GetOneKeyTokenEmployees();
            WriteDiscoveryLog(LogLevelType.Info, $"Total No.Of VPNAccount {users.Count}", DiscoveryObjectType.User);
            if (users != null && users.Any())
            {
                foreach (var user in users)
                {
                    if (!string.IsNullOrWhiteSpace(user.OneKeyTokenId))
                    {
                        var vpnUserInfo = CreateUpdateVPNuserInfo(user);
                        this.VPNUserList.Add(vpnUserInfo);
                    }
                }
            }
            WriteDiscoveryLog(LogLevelType.Info, "Fetch User with Token Number completed", DiscoveryObjectType.User);
        }

        private VPNUserInfo CreateUpdateVPNuserInfo(User user)
        {
            var vpnUsr = DomainModel.VPNUserInfo.GetByUserId(user.Id);
            if (vpnUsr == null)
            {
                vpnUsr = new VPNUserInfo()
                {
                    OneKeyTokenId = user.OneKeyTokenId,
                    EndPointSystemId = this.Endpoint.Id,
                    UserId = user.Id,
                    UserName = user.EmployeeId,
                    DisplayName = user.DisplayName,
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.Now
                };
            }
            else
            {
                vpnUsr.DisplayName = user.DisplayName;
                vpnUsr.OneKeyTokenId = user.OneKeyTokenId;
                vpnUsr.LastModifiedDate = DateTime.Now;
                vpnUsr.LastModifiedBy = currentUser;
            }
            vpnUsr.Save(currentUser);
            return vpnUsr;
        }

        #endregion
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }
    }
}
