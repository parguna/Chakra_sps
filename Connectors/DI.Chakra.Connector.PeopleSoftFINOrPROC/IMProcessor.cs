using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMessage = DI.Chakra.Connector.PeopleSoftFINOrPROC.Message;

namespace DI.Chakra.Connector.PeopleSoftFINOrPROC
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        public EndpointConfiguration _peoplesoftFINOrPROCConfiguration { get; set; }
        public EndPoint _endpoint;        
        public string _connectionString = string.Empty;
        public EndpointConfiguration PeopleSoftFINOrPROCConfiguration
        {
            get { return _peoplesoftFINOrPROCConfiguration; }
            set { this._peoplesoftFINOrPROCConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = _peoplesoftFINOrPROCConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }
        public bool Connect(string serviceType)
        {
            bool isConnected = true;
            try
            {
                switch (serviceType)
                {
                    case "DeleteUser":
                        FSCMOffBoard.RS_DeleteUser_IMS_FIN_ReqRespClient deleteuserclient = new FSCMOffBoard.RS_DeleteUser_IMS_FIN_ReqRespClient();
                        deleteuserclient.Open();
                        if (deleteuserclient.State == System.ServiceModel.CommunicationState.Opened)
                        {
                            deleteuserclient.Close();
                            isConnected = true;
                        }
                        break;
                    case "UpdateUser":
                        FSCMUpdateRole.RS_UpdateRole_IMS_FIN_ReqRespClient updateuserclient = new FSCMUpdateRole.RS_UpdateRole_IMS_FIN_ReqRespClient();
                        updateuserclient.Open();
                        if (updateuserclient.State == System.ServiceModel.CommunicationState.Opened)
                        {
                            updateuserclient.Close();
                            isConnected = true;
                        }
                        break;
                    default:
                        isConnected = false;
                        break;
                }

                return isConnected;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = "" };
        }
        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect("DeleteUser"))
                {
                    FSCMOffBoard.RS_DeleteUser_IMS_FIN_ReqRespClient fscmClient = new FSCMOffBoard.RS_DeleteUser_IMS_FIN_ReqRespClient();
                    fscmClient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServiceUserName)));
                    fscmClient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServicePassword)));


                    FSCMOffBoard.RWS_IDS_OFF_USR_RQST_CONTType req = new FSCMOffBoard.RWS_IDS_OFF_USR_RQST_CONTType();
                    req.RWS_IDS_OFF_USR_RQST = new FSCMOffBoard.RWS_IDS_OFF_USR_RQST_TypeShape();
                    req.RWS_IDS_OFF_USR_RQST.RWS_IDASOFF_REQ = new FSCMOffBoard.RWS_IDASOFF_REQMsgDataRecord_TypeShape();

                    User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["UserName"].Value));
                    if (IMuser != null)
                    {
                        req.RWS_IDS_OFF_USR_RQST.RWS_IDASOFF_REQ.EMAILID = Convert.ToString(IMuser.EmailId);
                        req.RWS_IDS_OFF_USR_RQST.RWS_IDASOFF_REQ.OPRID = Convert.ToString(fields["UserName"].Value);
                        req.RWS_IDS_OFF_USR_RQST.RWS_IDASOFF_REQ.EMPLID = Convert.ToString(IMuser.EmployeeId);
                        req.RWS_IDS_OFF_USR_RQST.RWS_IDASOFF_REQ.RWS_SYSTEMID = "102";
                        req.RWS_IDS_OFF_USR_RQST.RWS_IDASOFF_REQ.RWS_TRANSACTION_ID = Convert.ToString(fields["IDASTnxID"].Value);

                        var resp = fscmClient.DeleteUser(req);
                        return FSCMResponseOffBoard(resp.RWS_IDS_OFF_USR_RESP.RWS_IDASOFF_RES);
                    }
                    else
                    {
                        return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["UserName"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
                    }
                }

                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("End Point BizTalk Service not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };

            }
        }
        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect("UpdateUser"))
                {
                    FSCMUpdateRole.RS_UpdateRole_IMS_FIN_ReqRespClient fscmClient = new FSCMUpdateRole.RS_UpdateRole_IMS_FIN_ReqRespClient();
                    fscmClient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServiceUserName)));
                    fscmClient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServicePassword)));


                    FSCMUpdateRole.RWS_IDS_UPD_USR_RQST_CONTType req = new FSCMUpdateRole.RWS_IDS_UPD_USR_RQST_CONTType();
                    req.RWS_IDS_UPD_USR_RQST = new FSCMUpdateRole.RWS_IDS_UPD_USR_RQST_TypeShape();
                    req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ = new FSCMUpdateRole.RWS_IDS_UPD_REQMsgDataRecord_TypeShape();

                    User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["UserName"].Value));
                    if (IMuser != null)
                    {
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.EMAILID = Convert.ToString(IMuser.EmailId);
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.OPRID = Convert.ToString(fields["UserName"].Value);
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.EMPLID = Convert.ToString(IMuser.EmployeeId);
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_SYSTEMID = "102";
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_TRANSACTION_ID = Convert.ToString(fields["IDASTnxID"].Value);

                        List<FSCMUpdateRole.RWS_IDS_ROL_REQMsgDataRecord_TypeShape> roles = new List<FSCMUpdateRole.RWS_IDS_ROL_REQMsgDataRecord_TypeShape>();

                        if (!string.IsNullOrEmpty(fields["Roles"].Value))
                        {
                            var endpointId = _peoplesoftFINOrPROCConfiguration.EndpointSystemId.Value;
                            var endpoint = EndPoint.GetById(endpointId);
                            if (endpoint.IsChildEpSplitForm.HasValue && endpoint.IsChildEpSplitForm.Value && endpoint.ParentEndpointId.HasValue && endpoint.ParentEndpointId.Value > 0)
                            {
                                var parentEndpoint = EndPoint.GetById(endpoint.ParentEndpointId.Value);
                                if (parentEndpoint != null)
                                    endpointId = parentEndpoint.Id;
                            }

                            string[] temproles = Convert.ToString(fields["Roles"].Value).Split(',');
                            var userRoles = EntitlementReconcilation.GetByUserId(IMuser.Id, endpointId, EntitlementType.Role);
                            var defaultRoles = PeopleSoftFINOrPROCRoleInfo.GetAllDefaultRolesByEndpoint(endpointId);
                            foreach (string role in temproles)
                            {
                                if (!userRoles.Any(a => a.Entitlement == role))
                                {
                                    if (!defaultRoles.Any(d => d.RoleId == role))
                                    {
                                        roles.Add(new FSCMUpdateRole.RWS_IDS_ROL_REQMsgDataRecord_TypeShape()
                                        {
                                            ACTION_CD = "A",
                                            ROLENAME = Convert.ToString(role)
                                        });
                                    }
                                }
                            }
                        }
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_IDS_ROL_REQ = roles.ToArray();

                        List<FSCMUpdateRole.RWS_IDS_BNK_REQMsgDataRecord_TypeShape> banks = new List<FSCMUpdateRole.RWS_IDS_BNK_REQMsgDataRecord_TypeShape>();

                        if (!string.IsNullOrEmpty(fields["BankAccount"].Value))
                        {
                            string[] tempbankaccounts = Convert.ToString(fields["BankAccount"].Value).Split(',');
                            string[] temproles = Convert.ToString(fields["Roles"].Value).Split(',');
                            foreach (string bnkaccount in tempbankaccounts)
                            {

                                banks.Add(new FSCMUpdateRole.RWS_IDS_BNK_REQMsgDataRecord_TypeShape()
                                {
                                    ACTION_CD = "A",
                                    BANK_ACCOUNT_NUM = Convert.ToString(bnkaccount),
                                    ROLENAME = Convert.ToString(temproles[0])
                                });
                            }
                        }
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_IDS_BNK_REQ = banks.ToArray();

                        List<FSCMUpdateRole.RWS_IDS_BU_REQMsgDataRecord_TypeShape> bus = new List<FSCMUpdateRole.RWS_IDS_BU_REQMsgDataRecord_TypeShape>();
                        if (!string.IsNullOrEmpty(fields["BusinessUnit"].Value))
                        {
                            string[] tempbsinessUnits = Convert.ToString(fields["BusinessUnit"].Value).Split(',');

                            foreach (string unit in tempbsinessUnits)
                            {
                                bus.Add(new FSCMUpdateRole.RWS_IDS_BU_REQMsgDataRecord_TypeShape()
                                {
                                    ACTION_CD = "A",
                                    BUSINESS_UNIT = Convert.ToString(unit)
                                });
                            }
                        }
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_IDS_BU_REQ = bus.ToArray();

                        if (roles.ToArray().Count() > 0 || bus.ToArray().Count() > 0 || banks.ToArray().Count() > 0)
                        {
                            var resp = fscmClient.UpdateRole(req);

                            if (resp != null && resp.RWS_IDS_UPD_USR_RESP != null && resp.RWS_IDS_UPD_USR_RESP.RWS_IDS_OFF_RES != null)
                            {
                                return FSCMResponseUpdateRole(resp.RWS_IDS_UPD_USR_RESP.RWS_IDS_OFF_RES);
                            }
                            else
                            {
                                return new ProvisioningResult() { Result = false, Message = string.Format("Response not recieved - {0}, {1}", Convert.ToString(fields["UserName"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
                            }
                        }
                        else
                        {
                            return new ProvisioningResult() { Result = true, Message = string.Format("No valid entitlements available to sent to subsystem  - {0}, Roles - {1}", Convert.ToString(fields["UserName"].Value), Convert.ToString(fields["Roles"].Value)) };
                        }
                    }
                    else
                    {
                        return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["UserName"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
                    }
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("End Point BizTalk Service not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
        }
        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect("UpdateUser"))
                {
                    FSCMUpdateRole.RS_UpdateRole_IMS_FIN_ReqRespClient fscmClient = new FSCMUpdateRole.RS_UpdateRole_IMS_FIN_ReqRespClient();
                    fscmClient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServiceUserName)));
                    fscmClient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServicePassword)));


                    FSCMUpdateRole.RWS_IDS_UPD_USR_RQST_CONTType req = new FSCMUpdateRole.RWS_IDS_UPD_USR_RQST_CONTType();
                    req.RWS_IDS_UPD_USR_RQST = new FSCMUpdateRole.RWS_IDS_UPD_USR_RQST_TypeShape();
                    req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ = new FSCMUpdateRole.RWS_IDS_UPD_REQMsgDataRecord_TypeShape();

                    User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["UserName"].Value));
                    if (IMuser != null)
                    {
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.EMAILID = Convert.ToString(IMuser.EmailId);
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.OPRID = Convert.ToString(fields["UserName"].Value);
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.EMPLID = Convert.ToString(IMuser.EmployeeId);
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_SYSTEMID = "102";
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_TRANSACTION_ID = Convert.ToString(fields["IDASTnxID"].Value);

                        List<FSCMUpdateRole.RWS_IDS_ROL_REQMsgDataRecord_TypeShape> roles = new List<FSCMUpdateRole.RWS_IDS_ROL_REQMsgDataRecord_TypeShape>();

                        if (!string.IsNullOrEmpty(fields["Roles"].Value))
                        {
                            var endpointId = _peoplesoftFINOrPROCConfiguration.EndpointSystemId.Value;
                            var endpoint = EndPoint.GetById(endpointId);
                            if (endpoint.IsChildEpSplitForm.HasValue && endpoint.IsChildEpSplitForm.Value && endpoint.ParentEndpointId.HasValue && endpoint.ParentEndpointId.Value > 0)
                            {
                                var parentEndpoint = EndPoint.GetById(endpoint.ParentEndpointId.Value);
                                if (parentEndpoint != null)
                                    endpointId = parentEndpoint.Id;
                            }

                            string[] temproles = Convert.ToString(fields["Roles"].Value).Split(',');
                            var defaultRoles = PeopleSoftFINOrPROCRoleInfo.GetAllDefaultRolesByEndpoint(endpointId);

                            foreach (string role in temproles)
                            {
                                if (!defaultRoles.Any(d => d.RoleId == role))
                                {
                                    roles.Add(new FSCMUpdateRole.RWS_IDS_ROL_REQMsgDataRecord_TypeShape()
                                    {
                                        ACTION_CD = "R",
                                        ROLENAME = Convert.ToString(role)
                                    });
                                }
                            }
                        }
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_IDS_ROL_REQ = roles.ToArray();

                        List<FSCMUpdateRole.RWS_IDS_BNK_REQMsgDataRecord_TypeShape> banks = new List<FSCMUpdateRole.RWS_IDS_BNK_REQMsgDataRecord_TypeShape>();

                        if (!string.IsNullOrEmpty(fields["BankAccount"].Value))
                        {
                            string[] tempbankaccounts = Convert.ToString(fields["BankAccount"].Value).Split(',');
                            //string[] temproles = Convert.ToString(fields["Roles"].Value).Split(',');
                            foreach (string bnkaccount in tempbankaccounts)
                            {

                                banks.Add(new FSCMUpdateRole.RWS_IDS_BNK_REQMsgDataRecord_TypeShape()
                                {
                                    ACTION_CD = "R",
                                    BANK_ACCOUNT_NUM = Convert.ToString(bnkaccount),
                                    ROLENAME = "" //Passing Empty data to get validation pass @BizTalk
                                });
                            }
                        }
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_IDS_BNK_REQ = banks.ToArray();

                        List<FSCMUpdateRole.RWS_IDS_BU_REQMsgDataRecord_TypeShape> bus = new List<FSCMUpdateRole.RWS_IDS_BU_REQMsgDataRecord_TypeShape>();
                        if (!string.IsNullOrEmpty(fields["BusinessUnit"].Value))
                        {
                            string[] tempbsinessUnits = Convert.ToString(fields["BusinessUnit"].Value).Split(',');

                            foreach (string unit in tempbsinessUnits)
                            {
                                bus.Add(new FSCMUpdateRole.RWS_IDS_BU_REQMsgDataRecord_TypeShape()
                                {
                                    ACTION_CD = "R",
                                    BUSINESS_UNIT = Convert.ToString(unit)
                                });
                            }
                        }
                        req.RWS_IDS_UPD_USR_RQST.RWS_IDS_UPD_REQ.RWS_IDS_BU_REQ = bus.ToArray();

                        if (roles.ToArray().Count() > 0 || bus.ToArray().Count() > 0 || banks.ToArray().Count() > 0)
                        {
                            var resp = fscmClient.UpdateRole(req);
                            if (resp != null && resp.RWS_IDS_UPD_USR_RESP != null)
                            {
                                return FSCMResponseUpdateRole(resp.RWS_IDS_UPD_USR_RESP.RWS_IDS_OFF_RES);
                            }
                            else
                            {
                                return new ProvisioningResult() { Result = false, Message = string.Format("Response not recieved - {0}, {1}", Convert.ToString(fields["UserName"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
                            }
                        }
                        else
                        {
                            return new ProvisioningResult() { Result = true, Message = string.Format("No valid entitlements available to sent to subsystem  - {0}, Roles - {1}", Convert.ToString(fields["UserName"].Value), Convert.ToString(fields["Roles"].Value)) };
                        }
                    }
                    else
                    {
                        return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["UserName"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
                    }
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("End Point BizTalk Service not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        private ProvisioningResult FSCMResponseOffBoard(FSCMOffBoard.RWS_IDASOFF_RESMsgDataRecord_TypeShape resp)
        {
            ProvisioningResult result = new ProvisioningResult();
            try
            {
                string faultCodeId = Convert.ToString(resp.RWS_IDAS_IB_RES);
                string faultCodeDesc = Convert.ToString(resp.RWS_IDAS_RES_DESCR);
                string cmTxId = Convert.ToString(resp.RWS_IDAS_PSPRO_ID);
                string systemId = Convert.ToString(resp.RWS_SYSTEMID);
                string @class = Convert.ToString(resp.@class);


                result.Result_TX_ID = cmTxId;
                result.Result = string.Compare(faultCodeId, "0", true) == 0 ? true : false;

                result.Message = string.Format("FaultCode:{0}, FaultCodeMessage:{1},TX_ID:{2},SystemId:{3},Class:{4}", faultCodeId, faultCodeDesc, cmTxId, systemId, @class);
            }
            catch (Exception ex)
            {
                result.Message = string.Format("Exception @Response Obj Construction {0}", ex.Message);
            }

            return result;
        }
        private ProvisioningResult FSCMResponseUpdateRole(FSCMUpdateRole.RWS_IDS_OFF_RESMsgDataRecord_TypeShape resp)
        {
            ProvisioningResult result = new ProvisioningResult();
            try
            {
                string faultCodeId = Convert.ToString(resp.RWS_IDAS_IB_RES);
                string faultCodeDesc = Convert.ToString(resp.RWS_IDAS_RES_DESCR);
                string cmTxId = Convert.ToString(resp.RWS_IDAS_PSPRO_ID);
                string systemId = Convert.ToString(resp.RWS_SYSTEMID);
                string @class = Convert.ToString(resp.@class);


                result.Result_TX_ID = cmTxId;
                result.Result = string.Compare(faultCodeId, "0", true) == 0 ? true : false;

                result.Message = string.Format("FaultCode:{0}, FaultCodeMessage:{1},TX_ID:{2},SystemId:{3},Class:{4}", faultCodeId, faultCodeDesc, cmTxId, systemId, @class);


            }
            catch (Exception ex)
            {
                result.Message = string.Format("Exception @Response Obj Construction {0}", ex.Message);
            }

            return result;
        }
    }
}

