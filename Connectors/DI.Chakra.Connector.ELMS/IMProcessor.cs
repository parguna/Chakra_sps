using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CMessage = DI.Chakra.Connector.ELMS.Message;
namespace DI.Chakra.Connector.ELMS
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        public EndpointConfiguration _elmsConfiguration { get; set; }
        public object CommandType { get; private set; }
        public EndPoint _endpoint;
        private string Configuration(EndpointKey Key)
        {
            var config = _elmsConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            try
            {

                ELMServiceInstance.RWS_IDS_OFF_USER_RQST_CONTType userReq = new ELMServiceInstance.RWS_IDS_OFF_USER_RQST_CONTType();
                userReq.RWS_IDS_OFF_USER_RQST = new ELMServiceInstance.RWS_IDS_OFF_USER_RQST_TypeShape();
                userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR = new ELMServiceInstance.RWS_ID_MSG1_HDRMsgDataRecord_TypeShape();

                User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["ADID"].Value));
                if (IMuser != null)
                {
                    userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.EMAILID = Convert.ToString(IMuser.EmailId);
                    userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.OPRID = Convert.ToString(fields["ADID"].Value);
                    userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.EMPLID = Convert.ToString(IMuser.EmployeeId);
                    userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.RWS_SYSTEMID = "102";
                    userReq.RWS_IDS_OFF_USER_RQST.RWS_ID_MSG1_HDR.RWS_TRANSACTION_ID = Convert.ToString(fields["IDAS_Tnx_ID"].Value);


                    ELMServiceInstance.RS_DeleteUser_IMS_ELM_ReqRespClient requClient = new ELMServiceInstance.RS_DeleteUser_IMS_ELM_ReqRespClient();
                    requClient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServiceUserName)));
                    requClient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServicePassword)));
                    var resp = requClient.DeleteUser(userReq);

                    return ReadResponse(resp.RWS_IDS_OFF_USER_RESP.RWS_ID_MSG1_HDR);
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["ADID"].Value), Convert.ToString(fields["IDAS_Tnx_ID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
        }
        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            try
            { }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };

        }
        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            try
            {
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            try
            {
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        private ProvisioningResult ReadResponse(ELMServiceInstance.RWS_ID_MSG1_HDRMsgDataRecord_TypeShape1 resp)
        {
            string faultCodeId = Convert.ToString(resp.RWS_TRANS_STATUS);
            string faultCodeDesc = Convert.ToString(resp.RWS_ERRORMSG);
            string cmTxId = Convert.ToString(resp.RWS_TRANSACTION_ID);
            string systemId = Convert.ToString(resp.RWS_SYSTEMID);
            string @class = Convert.ToString(resp.@class);

            ProvisioningResult result = new ProvisioningResult();
            result.Result_TX_ID = cmTxId;
            result.Result = string.Compare(faultCodeId, "0", true) == 0 ? true : false;

            result.Message = string.Format("FaultCode:{0}, FaultCodeMessage:{1},TX_ID:{2},SystemId:{3},Class:{4}", faultCodeId, faultCodeDesc, cmTxId, systemId, @class);

            return result;
        }
    }
}
