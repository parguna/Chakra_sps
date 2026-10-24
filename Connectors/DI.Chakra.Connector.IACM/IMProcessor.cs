using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Data.SqlClient;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;
using DI.Chakra.Logger; 
using DI.Chakra.Connector;


namespace DI.Chakra.Connector.IACM
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        SqlConnection sqlConnect = null;
        public EndpointConfiguration IacmConfiguration { get; set; }
        public EndPoint EndPoint
        {
            get;
            set;
        }

        private string Configuration(EndpointKey Key)
        {
            var config = IacmConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }


        public bool Connect()
        {
            try
            {
                //var users = User.GetActiveEmployees();
                //if (users == null || users.Count == 0)
                //{
                //    return false;
                //}

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {

            }
            return true;

        }

        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                this.Connect();
                //var user = User.GetByUserNameFromIMProcessor(fields["AccountName"].Value);
                //if (user != null && user.Id > 0)
                //{
                //    user.IsActive = false;
                //    user.Save(MasterConfigHelper.ServiceUser);
                //    ReturnValue = true;
                //}
                //else
                //{
                //    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                //}


            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }



        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                //var user = User.GetByUserNameFromIMProcessor(fields["AccountName"].Value);
                //if (user != null && user.Id > 0)
                //{
                //    user.IsActive = false;
                //    user.Save(MasterConfigHelper.ServiceUser);
                //}
                //else
                //{
                //    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                //}


            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                var user = User.GetByName(fields["AccountName"].Value.ToString());
                if (user != null && user.Id > 0)
                {
                    if (!string.IsNullOrEmpty(fields["Roles"].Value))
                    {
                        foreach (var role in fields["Roles"].Value.Split(','))
                        {
                            var idasRole = Role.GetByRoleName(role);
                            if (idasRole != null)
                            {
                                string insertRole = string.Format("INSERT INTO UserRole VALUES({0},{1})", idasRole.Id, user.Id);
                                Role.ExecuteQuery(insertRole);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(fields["Groups"].Value))
                    {
                        foreach (var role in fields["Groups"].Value.Split(','))
                        {
                            var idasGroup = Group.GetByGroupName(role);
                            if (idasGroup != null)
                            {
                                string insertGroup = string.Format("INSERT INTO UserGroup VALUES({0},{1})", idasGroup.Id, user.Id);
                                Group.ExecuteQuery(insertGroup);
                            }
                        }
                    }
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                }
            }


            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }



        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                var user = User.GetByName(fields["AccountName"].Value.ToString());
                if (user != null && user.Id > 0)
                {
                    if (!string.IsNullOrEmpty(fields["Roles"].Value))
                    {
                        foreach (var role in fields["Roles"].Value.Split(','))
                        {
                            var idasRole = Role.GetByRoleName(role);
                            if (idasRole != null)
                            {
                                string removetRole = string.Format("DELETE FROM UserRole WHERE User_UserId = {0} AND Role_RoleId = {1}", user.Id, idasRole.Id);
                                Role.ExecuteQuery(removetRole);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(fields["Groups"].Value))
                    {
                        foreach (var role in fields["Groups"].Value.Split(','))
                        {
                            var idasGroup = Group.GetByGroupNameWithoutUsers(role);
                            if (idasGroup != null)
                            {
                                string removeGroup = string.Format("DELETE FROM UserGroup WHERE User_UserId = {0} AND Group_GroupId = {1}", user.Id, idasGroup.Id);
                                Group.ExecuteQuery(removeGroup);
                            }
                        }
                    }
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                }
            }


            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {

            try
            {
                this.Connect();


                var user = User.GetByName(fields["AccountName"].Value.ToString());
                if (user != null && user.Id > 0)
                {
                    user.Password = RSAHelper.EncryptData(fields["Password"].Value.ToString());
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                }

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
                this.Connect();
                var user = User.GetByUserNameFromIMProcessor(fields["AccountName"].Value);
                if (user != null && user.Id > 0)
                {
                    user.IsActive = true;
                    user.Save(MasterConfigHelper.ServiceUser);
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                }


            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {

            try
            {
                this.Connect();


                var user = User.GetByUserNameFromIMProcessor(fields["AccountName"].Value);
                if (user != null && user.Id > 0)
                {
                    user.IsActive = true;
                    user.Save(MasterConfigHelper.ServiceUser);
                    var selectedSubsytemsFromTable = user.Reserved10;
                    AccessRequest parentAccessRequest = null;
                    if (!string.IsNullOrEmpty(user.Reserved9))
                    {
                        parentAccessRequest = AccessRequest.GetById(Convert.ToInt64(user.Reserved9));
                        var childAccessRequestId = AccessRequest.GetByParentRequestId(parentAccessRequest.Id);

                        if (childAccessRequestId.Count() > 0)
                        {
                            return new ProvisioningResult() { Result = true, Message = "AD & Exchange AccessRequests already created for this user." };
                        }
                    }

                    if (!string.IsNullOrEmpty(selectedSubsytemsFromTable))
                    {
                        var selectedSubsytems = selectedSubsytemsFromTable.Split(',');
                        // this needs to be moved to endpoint as a dependent system 
                        selectedSubsytems = selectedSubsytems.OrderBy(a => a).ToArray();
                        long adRequestId = 0;
                        DateTime ADExecutionDatetime = DateTime.Now;
                        foreach (string endpointType in selectedSubsytems)
                        {
                            var endpoints = EndPoint.GetByEndpointSystemTypeId(Convert.ToByte(Enum.Parse(typeof(EndpointType), endpointType.Trim())));
                            var transactionId = new Guid();
                            foreach (var ep in endpoints)
                            {
                                var newRequest = new AccessRequest();
                                newRequest.UserId = user.Id;
                                newRequest.EndpointSystemId = ep.Id;
                                newRequest.RequestStatus = AccessRequestStatus.Granted;
                                newRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                newRequest.RequesterId = parentAccessRequest.RequesterId;
                                newRequest.ActivationDate = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                                newRequest.Description = "This request is created automatically for manual On-Boarding via IDAS.";
                                newRequest.DeactivationDate = user.TerminationDate;
                                newRequest.TransactionId = transactionId;
                                newRequest.ExecutionTime = DateTime.Now;
                                if (ep.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
                                {
                                    ADExecutionDatetime = newRequest.ExecutionTime.Value;
                                }

                                newRequest.Remarks = string.Empty;
                                newRequest.AccountType = AccountType.User;
                                newRequest.ApplicationType = ApplicationType.IDMS;
                                newRequest.RequestSource = AccessRequestSource.EmployeeCreation;

                                if (!string.IsNullOrEmpty(user.Reserved9))
                                {
                                    newRequest.IsParentRequest = false;
                                    newRequest.ParentRequestId = Convert.ToInt64(user.Reserved9);
                                }

                                if (ep.EndPointSystemType.EndpointSystemTypeName != EndpointType.ActiveDirectory && adRequestId > 0)
                                {
                                    newRequest.IsDependentRequest = true;
                                    newRequest.ParentRequestId = adRequestId;
                                }

                                AdHocAccessRequest adhocrequst = new AdHocAccessRequest();
                                adhocrequst.RequesteeUserId = user.Id;
                                adhocrequst.RequesterUserId = parentAccessRequest.RequesterId.Value;
                                adhocrequst.EndpointSystemType = (DI.Chakra.Infrastructure.Enums.EndpointType)ep.EndPointSystemType.EndpointSystemTypeName;
                                adhocrequst.EndpointSystemId = ep.Id;
                                adhocrequst.AdhocActionType = AdHocActionType.NewAccount;
                                adhocrequst.AccountType = AccountType.User;
                                adhocrequst.ValidFrom = user.JoinDate.HasValue ? user.JoinDate.Value : DateTime.Now;
                                adhocrequst.ValidTo = user.TerminationDate;
                                adhocrequst.Comments = string.Format("{0} new account Request", ep.DisplayName);

                                adhocrequst.Save(MasterConfigHelper.ServiceUser);

                                newRequest.AdHocAccessRequestId = adhocrequst.Id;
                                newRequest.Save(MasterConfigHelper.ServiceUser);

                                if (ep.EndPointSystemType.EndpointSystemTypeName == EndpointType.ActiveDirectory)
                                    adRequestId = newRequest.Id;
                            }
                        }
                    }
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }



        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                var user = User.GetByUserNameFromIMProcessor(fields["AccountName"].Value);
                if (user != null && user.Id > 0)
                {
                    user.IsActive = true;
                    user.Save(MasterConfigHelper.ServiceUser);
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                }


            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                //var user = User.GetByUserNameFromIMProcessor(fields["AccountName"].Value);
                //if (user != null && user.Id > 0)
                //{
                //    user.IsActive = false;
                //    user.Save(MasterConfigHelper.ServiceUser);
                //}
                //else
                //{
                //    return new ProvisioningResult() { Result = false, Message = string.Format("User{0} not found.", fields["AccountName"].Value) };
                //}


            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

    }
}