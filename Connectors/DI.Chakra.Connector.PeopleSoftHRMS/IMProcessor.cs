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
using CMessage = DI.Chakra.Connector.PeopleSoftHRMS.Message;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector.PeopleSoftHRMS
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        public EndpointConfiguration _peoplesoftHRMSConfiguration { get; set; }
        public EndPoint EndPoint { get; set; }      
        public string _connectionString = string.Empty;
        private string GetConfiguration(EndpointKey Key)
        {
            var config = _peoplesoftHRMSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }
        public bool Connect(string serviceType)
        {
            bool isConnected = true;
            try
            {
                switch (serviceType)
                {
                    case "CreateUser":
                        using (HCMCreateUser.RS_CreateUser_IMS_HRM_ReqRespClient createuserclient = new HCMCreateUser.RS_CreateUser_IMS_HRM_ReqRespClient())
                        {
                            createuserclient.Open();
                            System.Threading.Thread.Sleep(5000);
                            if (createuserclient.State == System.ServiceModel.CommunicationState.Opened)
                            {
                                createuserclient.Close();
                                isConnected = true;
                            }
                        }
                        break;
                    case "DeleteUser":
                        using (HCMOffBoard.RS_DeleteUser_IMS_HRM_ReqRespClient deleteuserclient = new HCMOffBoard.RS_DeleteUser_IMS_HRM_ReqRespClient())
                        {
                            deleteuserclient.Open();
                            System.Threading.Thread.Sleep(5000);
                            if (deleteuserclient.State == System.ServiceModel.CommunicationState.Opened)
                            {
                                deleteuserclient.Close();
                                isConnected = true;
                            }
                        }
                        break;
                    case "ELMDeleteUser":
                        using (ELMServiceInstance.RS_DeleteUser_IMS_ELM_ReqRespClient requClient = new ELMServiceInstance.RS_DeleteUser_IMS_ELM_ReqRespClient())
                        {
                            requClient.Open();
                            System.Threading.Thread.Sleep(5000);
                            if (requClient.State == System.ServiceModel.CommunicationState.Opened)
                            {
                                requClient.Close();
                                isConnected = true;
                            }
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
            try
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("HCM provisiong started for user {0},{1}", Convert.ToString(fields["OPRID"].Value), Convert.ToString(fields["EMPLID"].Value)));
                if (this.Connect("CreateUser"))
                {
                    using (HCMCreateUser.RS_CreateUser_IMS_HRM_ReqRespClient hcmclient = new HCMCreateUser.RS_CreateUser_IMS_HRM_ReqRespClient())
                    {
                        hcmclient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServiceUserName)));
                        hcmclient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServicePassword)));

                        HCMCreateUser.RWS_IDS_ADD_USER_RQST_CONTType req = new HCMCreateUser.RWS_IDS_ADD_USER_RQST_CONTType();
                        req.RWS_IDS_ADD_USER_RQST = new HCMCreateUser.RWS_IDS_ADD_USER_RQST_TypeShape();
                        req.RWS_IDS_ADD_USER_RQST.RWS_ID_MSG1_HDR = new HCMCreateUser.RWS_ID_MSG1_HDRMsgDataRecord_TypeShape();

                        User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["OPRID"].Value));
                        if (IMuser != null)
                        {
                            req.RWS_IDS_ADD_USER_RQST.RWS_ID_MSG1_HDR.EMAILID = Convert.ToString(fields["EMAILID"].Value);
                            req.RWS_IDS_ADD_USER_RQST.RWS_ID_MSG1_HDR.OPRID = Convert.ToString(fields["OPRID"].Value);
                            req.RWS_IDS_ADD_USER_RQST.RWS_ID_MSG1_HDR.EMPLID = Convert.ToString(fields["EMPLID"].Value);
                            req.RWS_IDS_ADD_USER_RQST.RWS_ID_MSG1_HDR.RWS_SYSTEMID = "102";
                            req.RWS_IDS_ADD_USER_RQST.RWS_ID_MSG1_HDR.RWS_TRANSACTION_ID = Convert.ToString(fields["IDASTnxID"].Value);

                            var resp = hcmclient.CreateUser(req);

                            return CreateUserReadResponse(resp.RWS_IDS_ADD_USER_RESP.RWS_ID_MSG1_HDR);
                        }
                        else
                        {
                            return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["OPRID"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
                        }
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
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("HCM provisiong completed for user {0}", Convert.ToString(fields["OPRID"].Value)));
        }
        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            ProvisioningResult result = new ProvisioningResult();
            string elmMsg = string.Empty;

            #region ELMOffBoard
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("ELM Offboard started for user {0},{1}", Convert.ToString(fields["OPRID"].Value), Convert.ToString(fields["EMPLID"].Value)));

            try
            {
                if (this.Connect("ELMDeleteUser"))
                {
                    using (ELMServiceInstance.RS_DeleteUser_IMS_ELM_ReqRespClient requClient = new ELMServiceInstance.RS_DeleteUser_IMS_ELM_ReqRespClient())
                    {
                        requClient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServiceUserName)));
                        requClient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServicePassword)));

                        User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["OPRID"].Value));

                        ELMServiceInstance.RWS_IDS_OFF_USER_RQST_CONTType userReq = new ELMServiceInstance.RWS_IDS_OFF_USER_RQST_CONTType();
                        userReq.RWS_IDS_OFF_USER_RQST = new ELMServiceInstance.RWS_IDS_OFF_USER_RQST_TypeShape();
                        userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR = new ELMServiceInstance.RWS_ID_MSG1_HDRMsgDataRecord_TypeShape();
                        userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.EMAILID = Convert.ToString(fields["EMAILID"]);
                        userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.OPRID = Convert.ToString(fields["OPRID"].Value);
                        userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.EMPLID = Convert.ToString(fields["EMPLID"].Value);
                        userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.RWS_SYSTEMID = "102";
                        userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.RWS_TRANSACTION_ID = Convert.ToString(fields["IDASTnxID"].Value);


                        var resp = requClient.DeleteUser(userReq);
                        elmMsg = ELMDeleteUserReadResponse(resp.RWS_IDS_OFF_USER_RESP);
                    }
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("ELM Offboard completed for user {0}", Convert.ToString(fields["OPRID"].Value)));
                }
                else
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("ELM End Point BizTalk Service not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)));
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("ELM Offboard completed for user {0}", Convert.ToString(fields["OPRID"].Value)), ex);
            }

            #endregion

            #region HCMOffboard

            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("HCM Offboard started for user {0},{1}", Convert.ToString(fields["OPRID"].Value), Convert.ToString(fields["EMPLID"].Value)));
            try
            {
                if (this.Connect("DeleteUser"))
                {
                    using (HCMOffBoard.RS_DeleteUser_IMS_HRM_ReqRespClient hcmclient = new HCMOffBoard.RS_DeleteUser_IMS_HRM_ReqRespClient())
                    {
                        hcmclient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServiceUserName)));
                        hcmclient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(GetConfiguration(EndpointKey.BizServicePassword)));

                        HCMOffBoard.RWS_IDS_OFF_USER_RQST_CONTType req = new HCMOffBoard.RWS_IDS_OFF_USER_RQST_CONTType();
                        req.RWS_IDS_OFF_USER_RQST = new HCMOffBoard.RWS_IDS_OFF_USER_RQST_TypeShape();
                        req.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR = new HCMOffBoard.RWS_ID_MSG1_HDRMsgDataRecord_TypeShape();

                        User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["OPRID"].Value));
                        if (IMuser != null)
                        {
                            req.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.EMAILID = Convert.ToString(fields["EMAILID"]);
                            req.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.OPRID = Convert.ToString(fields["OPRID"].Value);
                            req.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.EMPLID = Convert.ToString(fields["EMPLID"].Value);
                            req.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.RWS_SYSTEMID = "102";
                            req.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.RWS_TRANSACTION_ID = Convert.ToString(fields["IDASTnxID"].Value);

                            var resp = hcmclient.DeleteUser(req);

                            result = DeleteUserReadResponse(resp.RWS_IDS_OFF_USER_RESP.RWS_ID_MSG1_HDR);
                        }
                        else
                        {
                            result = new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["OPRID"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
                        }
                    }
                }
                else
                {
                    result = new ProvisioningResult() { Result = false, Message = string.Format("End Point BizTalk Service not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                result = new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("HCM Offboard completed for user {0}", Convert.ToString(fields["OPRID"].Value)));

            #endregion

            result.Message = result.Message + elmMsg;

            return result;
        }
        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }

        private ProvisioningResult CreateUserReadResponse(HCMCreateUser.RWS_ID_MSG1_HDRMsgDataRecord_TypeShape1 resp)
        {
            string faultCodeId = Convert.ToString(resp.RWS_TRANS_STATUS);
            string faultCodeDesc = Convert.ToString(resp.ERRORMSG);
            string cmTxId = Convert.ToString(resp.RWS_TRANSACTION_ID);
            string systemId = Convert.ToString(resp.RWS_SYSTEMID);
            string @class = Convert.ToString(resp.@class);

            ProvisioningResult result = new ProvisioningResult();
            result.Result_TX_ID = cmTxId;
            result.Result = string.Compare(faultCodeId, "0", true) == 0 ? true : false;

            result.Message = string.Format("FaultCode:{0}, FaultCodeMessage:{1},TX_ID:{2},SystemId:{3},Class:{4}", faultCodeId, faultCodeDesc, cmTxId, systemId, @class);

            return result;
        }
        private ProvisioningResult DeleteUserReadResponse(HCMOffBoard.RWS_ID_MSG1_HDRMsgDataRecord_TypeShape1 resp)
        {
            string faultCodeId = Convert.ToString(resp.RWS_TRANS_STATUS);
            string faultCodeDesc = Convert.ToString(resp.ERRORMSG);
            string cmTxId = Convert.ToString(resp.RWS_TRANSACTION_ID);
            string systemId = Convert.ToString(resp.RWS_SYSTEMID);
            string @class = Convert.ToString(resp.@class);

            ProvisioningResult result = new ProvisioningResult();
            result.Result_TX_ID = cmTxId;
            result.Result = string.Compare(faultCodeId, "0", true) == 0 ? true : false;

            result.Message = string.Format("FaultCode:{0}, FaultCodeMessage:{1},TX_ID:{2},SystemId:{3},Class:{4}", faultCodeId, faultCodeDesc, cmTxId, systemId, @class);

            return result;
        }

        private string ELMDeleteUserReadResponse(ELMServiceInstance.RWS_IDS_OFF_USER_RESP_TypeShape resp)
        {
            string faultCodeId = Convert.ToString(resp.RWS_ID_MSG1_HDR.RWS_TRANS_STATUS);
            string faultCodeDesc = Convert.ToString(resp.RWS_ID_MSG1_HDR.RWS_ERRORMSG);
            string cmTxId = Convert.ToString(resp.RWS_ID_MSG1_HDR.RWS_TRANSACTION_ID);
            string systemId = Convert.ToString(resp.RWS_ID_MSG1_HDR.RWS_SYSTEMID);
            string @class = Convert.ToString(resp.RWS_ID_MSG1_HDR.@class);

            ProvisioningResult result = new ProvisioningResult();
            result.Result_TX_ID = cmTxId;
            result.Result = string.Compare(faultCodeId, "0", true) == 0 ? true : false;

            result.Message = string.Format("ELM Offboard status : FaultCode:{0}, FaultCodeMessage:{1},TX_ID:{2},SystemId:{3},Class:{4}", faultCodeId, faultCodeDesc, cmTxId, systemId, @class);

            return result.Message;
        }
    }
}

