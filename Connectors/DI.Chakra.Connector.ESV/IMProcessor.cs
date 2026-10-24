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
using System.Xml;
using System.Xml.Serialization;
using CMessage = DI.Chakra.Connector.ESV.Message;

namespace DI.Chakra.Connector.ESV
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        public EndpointConfiguration _esvConfiguration { get; set; }
        public object CommandType { get; private set; }
        public EndPoint _endpoint;

        private string Configuration(EndpointKey Key)
        {
            var config = _esvConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
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
                        using (ESVCreateUser.RS_CreateUser_IMS_ESV_ReqRespClient createuserclient = new ESVCreateUser.RS_CreateUser_IMS_ESV_ReqRespClient())
                        {
                            createuserclient.Open();
                            if (createuserclient.State == System.ServiceModel.CommunicationState.Opened)
                            {
                                createuserclient.Close();
                                isConnected = true;
                            }
                        }
                        break;
                    case "DeleteUser":
                        using (ESVDeleteUser.RS_DeleteUser_IMS_ESV_ReqRespClient deleteuserclient = new ESVDeleteUser.RS_DeleteUser_IMS_ESV_ReqRespClient())
                        {
                            deleteuserclient.Open();
                            if (deleteuserclient.State == System.ServiceModel.CommunicationState.Opened)
                            {
                                deleteuserclient.Close();
                                isConnected = true;
                            }
                        }
                        break;
                    case "UpdateUser":
                        using (ESVUpdateRole.RS_UpdateRole_IMS_ESV_ReqRespClient updateuserclient = new ESVUpdateRole.RS_UpdateRole_IMS_ESV_ReqRespClient())
                        {
                            updateuserclient.Open();
                            if (updateuserclient.State == System.ServiceModel.CommunicationState.Opened)
                            {
                                updateuserclient.Close();
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
                throw ex;
            }
        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            string remarks = string.Empty;
            try
            {

                if (this.Connect("CreateUser"))
                {
                    using (ESVCreateUser.RS_CreateUser_IMS_ESV_ReqRespClient createuserclient = new ESVCreateUser.RS_CreateUser_IMS_ESV_ReqRespClient())
                    {
                        createuserclient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServiceUserName)));
                        createuserclient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServicePassword)));

                        ESVCreateUser.CreateUser userRequest = new ESVCreateUser.CreateUser();
                        userRequest.sds = new ESVCreateUser.UserIntegrationDS();

                        ESVCreateUser.UserIntegrationDS.createUserVALParamDTRow createUserRow = (ESVCreateUser.UserIntegrationDS.createUserVALParamDTRow)userRequest.sds
                                                                                   .createUserVALParamDT.NewRow();

                        User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["ADID"].Value));
                        if (IMuser != null)
                        {

                            string domain = ConfigurationMaster.GetConfigKey("DefaultDomain", Product.IACM);


                            createUserRow.AD_ID = string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value));
                            createUserRow.Email = Convert.ToString(IMuser.EmailId);
                            createUserRow.EmployeeID = Convert.ToString(IMuser.EmployeeId);
                            createUserRow.FirstName = Convert.ToString(IMuser.FirstName);
                            createUserRow.LastName = Convert.ToString(IMuser.LastName);
                            createUserRow.IDAS_Tnx_ID = Convert.ToInt64(fields["IDASTnxID"].Value);


                            userRequest.sds.createUserVALParamDT.Rows.Add(createUserRow);


                            ESVCreateUser.UserIntegrationDS.InterfaceRequestHeaderDTRow headerRow = (ESVCreateUser.UserIntegrationDS.InterfaceRequestHeaderDTRow)userRequest.sds.InterfaceRequestHeaderDT.NewRow();
                            headerRow.SystemID = 102;
                            userRequest.sds.InterfaceRequestHeaderDT.Rows.Add(headerRow);


                            var response = createuserclient.CreateUser(userRequest);

                            ProvisioningResult responseMsg = CreateUserReadResponseObj(response.CreateUserResult.InterfaceResponseHeaderDT, response.CreateUserResult.ResultInfoDT);
                            responseMsg.Message += responseMsg.Message + (string.Format("User Account Created: {0}", string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value))));
                            return responseMsg;
                        }

                        else
                        {
                            return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["ADID"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
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
                return new ProvisioningResult() { Result = false, Message = ex.Message + remarks + (ex.InnerException != null ? ex.InnerException.ToString() : "") };
            }

        }
        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect("DeleteUser"))
                {
                    using (ESVDeleteUser.RS_DeleteUser_IMS_ESV_ReqRespClient userclient = new ESVDeleteUser.RS_DeleteUser_IMS_ESV_ReqRespClient())
                    {

                        userclient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServiceUserName)));
                        userclient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServicePassword)));

                        ESVDeleteUser.DeleteUser userRequest = new ESVDeleteUser.DeleteUser();
                        userRequest.sds = new ESVDeleteUser.UserIntegrationDS();

                        ESVDeleteUser.UserIntegrationDS.deleteUserVALParamDTRow userRow = (ESVDeleteUser.UserIntegrationDS.deleteUserVALParamDTRow)userRequest.sds
                                                                             .deleteUserVALParamDT.NewRow();
                        User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["ADID"].Value));
                        if (IMuser != null)
                        {
                            string domain = ConfigurationMaster.GetConfigKey("DefaultDomain", Product.IACM);

                            userRow.AD_ID = string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value));
                            userRow.IDAS_Tnx_ID = Convert.ToInt64(fields["IDASTnxID"].Value); ;

                            userRequest.sds.deleteUserVALParamDT.Rows.Add(userRow);

                            ESVDeleteUser.UserIntegrationDS.InterfaceRequestHeaderDTRow headerRow = (ESVDeleteUser.UserIntegrationDS.InterfaceRequestHeaderDTRow)userRequest.sds.InterfaceRequestHeaderDT.NewRow();
                            headerRow.SystemID = 102;
                            userRequest.sds.InterfaceRequestHeaderDT.Rows.Add(headerRow);

                            var response = userclient.DeleteUser(userRequest);

                            ProvisioningResult responseMsg = DeleteUserReadResponseObj(response.DeleteUserResult.InterfaceResponseHeaderDT, response.DeleteUserResult.ResultInfoDT);
                            responseMsg.Message += responseMsg.Message + (string.Format("User Account Deleted: {0}", string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value))));
                            return responseMsg;
                        }
                        else
                        {
                            return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["ADID"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
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

        }
        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            try
            { return new ProvisioningResult() { Result = true, Message = string.Empty }; }
            catch (Exception ex)
            { return new ProvisioningResult() { Result = false, Message = ex.Message }; }
        }

        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            ProvisioningResult respMsg;
            try
            {
                if (this.Connect("UpdateUser"))
                {
                    using (ESVUpdateRole.RS_UpdateRole_IMS_ESV_ReqRespClient userclient = new ESVUpdateRole.RS_UpdateRole_IMS_ESV_ReqRespClient())
                    {
                        userclient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServiceUserName)));
                        userclient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServicePassword)));

                        ESVUpdateRole.UpdateRole userRequest = new ESVUpdateRole.UpdateRole();
                        userRequest.sds = new ESVUpdateRole.UserIntegrationDS();

                        ESVUpdateRole.UserIntegrationDS.updateRoleVALParamDTRow userRow = (ESVUpdateRole.UserIntegrationDS.updateRoleVALParamDTRow)userRequest.sds
                        .updateRoleVALParamDT.NewRow();

                        User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["ADID"].Value));
                        if (IMuser != null)
                        {
                            string domain = ConfigurationMaster.GetConfigKey("DefaultDomain", Product.IACM);

                            if (!string.IsNullOrEmpty(fields["Roles"].Value))
                            {
                                userRow.AD_ID = string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value));
                                string roles = SerializeRolestoXml(Convert.ToString(fields["Roles"].Value), "Add");
                                userRow.Roles = roles; // string.Format("<Roles> {0} </Roles>", roles);
                                userRow.IDAS_Tnx_ID = Convert.ToInt64(fields["IDASTnxID"].Value);

                                userRequest.sds.updateRoleVALParamDT.Rows.Add(userRow);

                                ESVUpdateRole.UserIntegrationDS.InterfaceRequestHeaderDTRow headerRow = (ESVUpdateRole.UserIntegrationDS.InterfaceRequestHeaderDTRow)userRequest.sds.InterfaceRequestHeaderDT.NewRow();
                                headerRow.SystemID = 102;
                                userRequest.sds.InterfaceRequestHeaderDT.Rows.Add(headerRow);

                                var response = userclient.UpdateRole(userRequest);

                                respMsg = UpdateUserReadResponseObj(response.UpdateRoleResult.InterfaceResponseHeaderDT, response.UpdateRoleResult.ResultInfoDT);
                                respMsg.Message += respMsg.Message + (string.Format("User Account: {0}, Roles Added: {1} ", string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value)), Convert.ToString(fields["Roles"].Value)));

                            }
                            else
                            {
                                respMsg = new ProvisioningResult() { Result = true, Message = "fields[Roles] empty." };
                            }

                            return respMsg;
                        }
                        else
                        {
                            return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["ADID"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
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

        }

        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            ProvisioningResult respMsg;
            try
            {
                if (this.Connect("UpdateUser"))
                {
                    using (ESVUpdateRole.RS_UpdateRole_IMS_ESV_ReqRespClient userclient = new ESVUpdateRole.RS_UpdateRole_IMS_ESV_ReqRespClient())
                    {
                        userclient.ClientCredentials.Windows.ClientCredential.UserName = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServiceUserName)));
                        userclient.ClientCredentials.Windows.ClientCredential.Password = RSAHelper.DecryptData(Convert.ToString(Configuration(EndpointKey.BizServicePassword)));

                        ESVUpdateRole.UpdateRole userRequest = new ESVUpdateRole.UpdateRole();
                        userRequest.sds = new ESVUpdateRole.UserIntegrationDS();

                        ESVUpdateRole.UserIntegrationDS.updateRoleVALParamDTRow userRow = (ESVUpdateRole.UserIntegrationDS.updateRoleVALParamDTRow)userRequest.sds
                        .updateRoleVALParamDT.NewRow();

                        User IMuser = User.GetByUserNameFromIMProcessor(Convert.ToString(fields["ADID"].Value));
                        if (IMuser != null)
                        {
                            string domain = ConfigurationMaster.GetConfigKey("DefaultDomain", Product.IACM);

                            if (!string.IsNullOrEmpty(fields["Roles"].Value))
                            {
                                userRow.AD_ID = string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value));
                                string roles = SerializeRolestoXml(Convert.ToString(fields["Roles"].Value), "Remove");
                                userRow.Roles = roles; // string.Format("<Roles> {0} </Roles>", roles);
                                userRow.IDAS_Tnx_ID = Convert.ToInt64(fields["IDASTnxID"].Value);

                                userRequest.sds.updateRoleVALParamDT.Rows.Add(userRow);

                                ESVUpdateRole.UserIntegrationDS.InterfaceRequestHeaderDTRow headerRow = (ESVUpdateRole.UserIntegrationDS.InterfaceRequestHeaderDTRow)userRequest.sds.InterfaceRequestHeaderDT.NewRow();
                                headerRow.SystemID = 102;
                                userRequest.sds.InterfaceRequestHeaderDT.Rows.Add(headerRow);

                                var response = userclient.UpdateRole(userRequest);

                                respMsg = UpdateUserReadResponseObj(response.UpdateRoleResult.InterfaceResponseHeaderDT, response.UpdateRoleResult.ResultInfoDT);
                                respMsg.Message += respMsg.Message + (string.Format("User Account: {0}, Roles Reomved: {1} ", string.Format("{0}\\{1}", domain, Convert.ToString(fields["ADID"].Value)), Convert.ToString(fields["Roles"].Value)));
                            }
                            else
                            {
                                respMsg = new ProvisioningResult() { Result = true, Message = "fields[Roles] empty." };
                            }
                            return respMsg;
                        }
                        else
                        {
                            return new ProvisioningResult() { Result = false, Message = string.Format("Employee Account ADID:{0} not exist in IDAS. Reference TX-Id:{1}", Convert.ToString(fields["ADID"].Value), Convert.ToString(fields["IDASTnxID"].Value)) };
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
        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            try
            { return new ProvisioningResult() { Result = true, Message = string.Empty }; }
            catch (Exception ex)
            { return new ProvisioningResult() { Result = false, Message = ex.Message }; }
        }
        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            try
            { return new ProvisioningResult() { Result = true, Message = string.Empty }; }
            catch (Exception ex)
            { return new ProvisioningResult() { Result = false, Message = ex.Message }; }
        }
        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            try
            { return new ProvisioningResult() { Result = true, Message = string.Empty }; }
            catch (Exception ex)
            { return new ProvisioningResult() { Result = false, Message = ex.Message }; }
        }

        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            try
            { return new ProvisioningResult() { Result = true, Message = string.Empty }; }
            catch (Exception ex)
            { return new ProvisioningResult() { Result = false, Message = ex.Message }; }
        }

        private ProvisioningResult CreateUserReadResponseObj(ESVCreateUser.UserIntegrationDS.InterfaceResponseHeaderDTDataTable responseHeader, ESVCreateUser.UserIntegrationDS.ResultInfoDTDataTable result)
        {
            ProvisioningResult resultresponse = new ProvisioningResult();
            try
            {
                string responseMessage = "Fault Code Id:{0},Fault Code Description:{1}, ESV_TX_ID:{2}.";
                long faultCodeId = 0; string faultCodeDesc = string.Empty;
                string cmTxId = string.Empty;

                if (responseHeader.Rows.Count > 0)
                {
                    faultCodeId = Convert.ToInt64(Convert.ToString(responseHeader.Rows[0][responseHeader.FaultCodeIDColumn]));
                    faultCodeDesc = Convert.ToString(responseHeader.Rows[0][responseHeader.FaultCodeDescriptionColumn]);
                }
                else
                {
                    StringWriter streamwriter = new StringWriter();
                    responseHeader.WriteXml(streamwriter);

                    faultCodeDesc = string.Format("No repsonse message recieved from ESV. Response xml: {0}", streamwriter.ToString());
                }
                if (result.Rows.Count > 0)
                {
                    cmTxId = Convert.ToString(result.Rows[0]["ESV_Tnx_ID"]);
                }
                else
                {
                    StringWriter streamwriter = new StringWriter();
                    result.WriteXml(streamwriter);

                    // remarks += streamwriter.ToString();
                    faultCodeDesc = faultCodeDesc + string.Format("No TXID message recieved from ESV. TXID xml: {0}", streamwriter.ToString());
                }

                if (faultCodeId == 0)
                {
                    resultresponse = new ProvisioningResult() { Result = true, Message = string.Format(responseMessage, faultCodeId, faultCodeDesc, cmTxId), Result_TX_ID = Convert.ToString(cmTxId) };
                }
                else
                {
                    resultresponse = new ProvisioningResult() { Result = false, Message = string.Format(responseMessage, faultCodeId, faultCodeDesc, cmTxId), Result_TX_ID = Convert.ToString(cmTxId) };
                }
            }
            catch (Exception ex)
            {
                resultresponse = new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return resultresponse;
        }

        private ProvisioningResult DeleteUserReadResponseObj(ESVDeleteUser.UserIntegrationDS.InterfaceResponseHeaderDTDataTable responseHeader, ESVDeleteUser.UserIntegrationDS.ResultInfoDTDataTable result)
        {
            ProvisioningResult resultresponse = new ProvisioningResult();
            try
            {
                string responseMessage = "Fault Code Id:{0},Fault Code Description:{1}, ESV_TX_ID:{2}.";
                long faultCodeId = 0; string faultCodeDesc = string.Empty;
                string cmTxId = string.Empty;

                if (responseHeader.Rows.Count > 0)
                {
                    faultCodeId = Convert.ToInt64(Convert.ToString(responseHeader.Rows[0][responseHeader.FaultCodeIDColumn]));
                    faultCodeDesc = Convert.ToString(responseHeader.Rows[0][responseHeader.FaultCodeDescriptionColumn]);
                }
                else
                {
                    StringWriter streamwriter = new StringWriter();
                    responseHeader.WriteXml(streamwriter);

                    faultCodeDesc = string.Format("No repsonse message recieved from ESV. Response xml: {0}", streamwriter.ToString());
                }
                if (result.Rows.Count > 0)
                {
                    cmTxId = Convert.ToString(result.Rows[0]["ESV_Tnx_ID"]);
                }
                else
                {
                    StringWriter streamwriter = new StringWriter();
                    result.WriteXml(streamwriter);

                    // remarks += streamwriter.ToString();
                    faultCodeDesc = faultCodeDesc + string.Format("No TXID message recieved from ESV. TXID xml: {0}", streamwriter.ToString());
                }

                if (faultCodeId == 0)
                {
                    resultresponse = new ProvisioningResult() { Result = true, Message = string.Format(responseMessage, faultCodeId, faultCodeDesc, cmTxId), Result_TX_ID = Convert.ToString(cmTxId) };
                }
                else
                {
                    resultresponse = new ProvisioningResult() { Result = false, Message = string.Format(responseMessage, faultCodeId, faultCodeDesc, cmTxId), Result_TX_ID = Convert.ToString(cmTxId) };
                }
            }
            catch (Exception ex)
            {
                resultresponse = new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return resultresponse;
        }

        private ProvisioningResult UpdateUserReadResponseObj(ESVUpdateRole.UserIntegrationDS.InterfaceResponseHeaderDTDataTable responseHeader, ESVUpdateRole.UserIntegrationDS.ResultInfoDTDataTable result)
        {

            ProvisioningResult resultresponse = new ProvisioningResult();
            try
            {
                string responseMessage = "Fault Code Id:{0},Fault Code Description:{1}, ESV_TX_ID:{2}.";
                long faultCodeId = 0; string faultCodeDesc = string.Empty;
                string cmTxId = string.Empty;

                if (responseHeader.Rows.Count > 0)
                {
                    faultCodeId = Convert.ToInt64(Convert.ToString(responseHeader.Rows[0][responseHeader.FaultCodeIDColumn]));
                    faultCodeDesc = Convert.ToString(responseHeader.Rows[0][responseHeader.FaultCodeDescriptionColumn]);
                }
                else
                {
                    StringWriter streamwriter = new StringWriter();
                    responseHeader.WriteXml(streamwriter);

                    faultCodeDesc = string.Format("No repsonse message recieved from ESV. Response xml: {0}", streamwriter.ToString());
                }
                if (result.Rows.Count > 0)
                {
                    cmTxId = Convert.ToString(result.Rows[0]["ESV_Tnx_ID"]);
                }
                else
                {
                    StringWriter streamwriter = new StringWriter();
                    result.WriteXml(streamwriter);

                    // remarks += streamwriter.ToString();
                    faultCodeDesc = faultCodeDesc + string.Format("No TXID message recieved from ESV. TXID xml: {0}", streamwriter.ToString());
                }

                if (faultCodeId == 0)
                {
                    resultresponse = new ProvisioningResult() { Result = true, Message = string.Format(responseMessage, faultCodeId, faultCodeDesc, cmTxId), Result_TX_ID = Convert.ToString(cmTxId) };
                }
                else
                {
                    resultresponse = new ProvisioningResult() { Result = false, Message = string.Format(responseMessage, faultCodeId, faultCodeDesc, cmTxId), Result_TX_ID = Convert.ToString(cmTxId) };
                }
            }
            catch (Exception ex)
            {
                resultresponse = new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return resultresponse;
        }

        private string SerializeRolestoXml(string roleCode, string actionType)
        {
            string rolesXml = string.Empty;
            try
            {
                Root root = new Root();
                root.Roles = new List<Role>();

                string[] roles = roleCode.Split(',');

                foreach (string role in roles)
                {
                    root.Roles.Add(new Role()
                    {
                        Actiontype = actionType,
                        RoleCode = new List<string>() { Convert.ToString(role).Trim() }
                    });
                }

                XmlSerializer serializer = new XmlSerializer(root.GetType());
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                using (MemoryStream xmlStream = new MemoryStream())
                {
                    serializer.Serialize(xmlStream, root, ns);
                    xmlStream.Position = 0;

                    xmlDoc.Load(xmlStream);
                    rolesXml = xmlDoc.InnerXml;
                }
            }
            catch (Exception ex)
            {

            }

            return rolesXml;
        }
    }
}
