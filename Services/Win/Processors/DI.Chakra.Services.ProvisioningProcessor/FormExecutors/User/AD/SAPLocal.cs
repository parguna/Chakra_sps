using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    class SAPLocal : Common.ADBase
    {
        public SAPLocal(EndPoint endpoint, AccessRequest currentRequest,IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
        }

        public override bool Propagate(DomainModel.User user, ProvisioningParamCollection fields)
        {

            try
            {
                bool isUpdated = true;

                var adUsr = ADSUserInfo.GetActivePrimaryByDIUserId(user.Id, Endpoint.Id);
                if (adUsr == null)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"{this.Endpoint?.DisplayName}: User:{user.DisplayName} dont have any active account for propagation.", null);
                    return isUpdated;
                }
                adUsr.title = user.PositionName;
                adUsr.sn = user.KnownAs;
                adUsr.displayName = user.KnownAs;

                if (!user.FullTimeEmployee)
                {
                    adUsr.description = user.ProjectDescription;
                }
                else
                {
                    adUsr.description = user.KnownAs;
                }

                adUsr.department = user.OrgDepartment;
                adUsr.division = user.OrgDivision;
                adUsr.company = user.OrgCompany;
                adUsr.businessCategory = user.OrgBranch;
                adUsr.pager = user.OrgSection;

                adUsr.postalAddress = user.PostalAddress;
                adUsr.postalCode = user.PostalCode;
                adUsr.countryCode = user.Country;

                adUsr.telephoneNumber = user.TelephoneNumber;
                adUsr.mobile = user.MobileNumber;
                adUsr.facsimileTelephoneNumber = user.Fax;
                adUsr.mail = user.EmailId;
                adUsr.physicalDeliveryOfficeName = user.EmployeeId;

                adUsr.otherPager = user.OrgUnit;
                adUsr.extensionAttribute6 = user.CostCenter;
                adUsr.extensionAttribute10 = user.OrgSection;

                if (user.FullTimeEmployee)
                {
                    if (user.IsExecutiveEmployee)
                    {
                        adUsr.otherFascimileTelephoneNumber = "Exec";
                    }
                    else
                    {
                        adUsr.otherFascimileTelephoneNumber = "Non-Exec";
                    }
                    adUsr.extensionAttribute13 = ADSUserInfo.GetExtensionAttribute13(user);
                    //if (user.EEGroup == "0")
                    //{
                    //    adUsr.extensionAttribute13 = "VP&Above";
                    //}
                    //else
                    //{
                    //    adUsr.extensionAttribute13 = "Director&Below";
                    //}
                }
                else
                {
                    adUsr.otherFascimileTelephoneNumber = "Non-Employees";
                }

                adUsr.LastModifiedBy = "System";
                adUsr.LastModifiedDate = DateTime.Now;

                //manager update
                adUsr.manager = GetRODistinguishedName(user);

                isUpdated = EndpointConnector.UpdateAccount(adUsr);

                if (isUpdated)
                {
                    adUsr.Save(MasterConfigHelper.ServiceUser);
                    WriteProvisioningLog(LogLevelType.Info, string.Format("User {0} modified  in Active Directory.", adUsr.sAMAccountName), null);
                }

                return isUpdated;
            }

            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while executing the propagation request. {0}", ex.Message), ex);
                return false;
            }
        }
        protected string GetRODistinguishedName(DomainModel.User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.ROEmployeeId))
                {
                    var ro = DomainModel.User.GetByEmployeeId(user.ROEmployeeId);

                    if (ro != null)
                    {
                        var roAD = ADSUserInfo.GetActivePrimaryByDIUserId(ro.Id, Endpoint.Id);

                        if (roAD != null)
                        {
                            return roAD.distinguishedName;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while finding RO AD distinguished name for the User {0}.", user.DisplayName), ex);
                return string.Empty;
            }
        }
    }
}
