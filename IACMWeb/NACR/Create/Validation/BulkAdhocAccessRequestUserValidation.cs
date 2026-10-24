using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.NACR.Create.BulkRequest;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.Web.NACR.Create.Validation
{
    public class BulkAdhocAccessRequestUserValidation
    {

        public static ValidationUser ValidateUsers(BulkRequestParameter param, List<User> RequestedUser)
        {

            if (param == null || RequestedUser == null || RequestedUser.Count == 0) return new ValidationUser() { DisplayName = string.Empty, ErrorType = 13 };

            param.UserIds = RequestedUser.Select(usr => usr.Id).ToList();

            switch (param.ActionType)
            {
                case Infrastructure.Enums.ActionType.NewAccount:

                    var validationUser = getValidationUser(param, RequestedUser, 2, true);
                    if (validationUser != null) return validationUser;

                    var validationRequestExist = checkDuplicateRequest(param);
                    if (validationRequestExist != null) return validationRequestExist;

                    if (param.EndpointSystemTypeId == EndpointType.GEMS || param.EndpointSystemTypeId == EndpointType.SRM)
                    {
                        param.EndpointSystemId = 0;
                        param.ApplicationType = ApplicationType.ADSingaporePowerLocal;
                        var validationUser_GEMS = getValidationUser(param, RequestedUser, 3, false);
                        if (validationUser_GEMS != null) return validationUser_GEMS;
                        if(param.EndpointSystemTypeId == EndpointType.SRM)
                        {
                            param.ApplicationType = ApplicationType.HRMS_FLINS;
                            validationUser_GEMS = getValidationUser(param, RequestedUser, 12, false);
                            if (validationUser_GEMS != null) return validationUser_GEMS;
                        }
                    }
                    break;

                case Infrastructure.Enums.ActionType.DeleteAccount:
                    {
                        param.keyValuePairs.Remove("considerLock");
                        param.keyValuePairs.Add("considerLock", bool.FalseString);
                        var validationUser_Exist = getValidationUser(param, RequestedUser, 6, false);
                        if (validationUser_Exist != null) return validationUser_Exist;
                        param.Locked = true;
                        param.keyValuePairs.Remove("considerLock");
                        var validationUser_IsLocked = getValidationUser(param, RequestedUser, 5, true);
                        if (validationUser_IsLocked != null) return validationUser_IsLocked;
                        break;
                    }

                case Infrastructure.Enums.ActionType.AmendmentAdd:
                case Infrastructure.Enums.ActionType.AmendmentRemove:
                    {
                        param.keyValuePairs.Remove("considerLock");
                        param.keyValuePairs.Add("considerLock", bool.FalseString);
                        var validationUser_Exist = getValidationUser(param, RequestedUser, 9, false);
                        if (validationUser_Exist != null) return validationUser_Exist;
                        param.Locked = true;
                        param.keyValuePairs.Remove("considerLock");
                        var validationUser_IsLocked = getValidationUser(param, RequestedUser, 8, true);
                        if (validationUser_IsLocked != null) return validationUser_IsLocked;

                    }
                    break;
            }
            return null;
        }

        public static string GetErrormessage(int ErrorType)
        {
            switch (ErrorType)
            {
                case 1:
                    return "valid to cannot be empty for Non-Employee(s)";
                case 2:
                    return "user account already exists";
                case 3:
                    return "does not hold account in AD. Hence GEMS/SRM creation request cannot be raised";
                case 4:
                    return "does not hold account in AD. Hence delete request cannot be raised";
                case 5:
                    return "account is deactivated. Hence delete request cannot be raised";
                case 6:
                    return "does not hold account. Hence delete request cannot be raised";
                case 7:
                    return "does not hold account in VPN. Hence amendment request cannot be raised";
                case 8:
                    return "account is deactivated. Hence amendment request cannot be raised";
                case 9:
                    return "does not hold account. Hence amendment request cannot be raised";
                case 10:
                    return "does not hold account. Hence deletion request cannot be raised";
                case 11:
                    return "does not hold account in AD. Hence amendment request cannot be raised";// + (misc == MiscSystemType.ExternalEmailAccess ? " lotus notes" : " AD");
                case 12:
                    return "does not hold account in SAP FLINS. Hence new account request cannot be raised";
                case 13:
                    return "please select the user";
                case 14:
                    return "does not hold account in AD. Hence VPN creation request cannot be raised";
                case 15:
                    return "duplicate request. Same request already exists";
                default: return "";
            }

        }

        static ValidationUser checkDuplicateRequest(BulkRequestParameter param)
        {
            string details = DomainApiClient<Dto.AccountReconcilation>
               .GetByAction<string>(DomainApiAction.Provisioning.AccountReconcilation.CheckDuplicateRequestExist, null, param);

            if (!string.IsNullOrWhiteSpace(details))
            {
                return new ValidationUser() { DisplayName = details, ErrorType = 15 };
            }
            return null;
        }
        static ValidationUser getValidationUser(BulkRequestParameter param, List<User> RequestedUser, int ErrorType, bool shouldCheckIfExist)
        {

            Dictionary<long, string> details = DomainApiClient<Dto.AccountReconcilation>
                .GetByAction<Dictionary<long, string>>(DomainApiAction.Provisioning.AccountReconcilation.GetByUserIdsDoesNotExist, null, param);

            if (details == null)
            {
                return new ValidationUser() { DisplayName = string.Empty, ErrorType = 4 };
            }

            string displayName = string.Join(",", RequestedUser.Where(user =>
            {
                if (shouldCheckIfExist)
                    return details.Keys.Contains(user.Id);
                else
                    return !details.Keys.Contains(user.Id);
            }).Select(user => user.DisplayName).ToList());

            if (!string.IsNullOrEmpty(displayName))
            {
                return new ValidationUser() { DisplayName = displayName, ErrorType = ErrorType };
            }

            return null;
        }



    }
}