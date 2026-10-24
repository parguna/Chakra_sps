using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ChakraService.Models;

namespace ChakraService.Controllers
{
#if DEBUG
    // Test/mock double for IAccountService: several actions accept a client-supplied "Fail" sentinel
    // that controls the success/failure response instead of performing real validation. This must
    // never be reachable in production, so the whole type is compiled out of Release builds
    // (production is always built Release - see DefineConstants in DI.Chakra.WebApi.csproj) via
    // this #if DEBUG block, not merely gated by a runtime flag. The runtime "EnableTestAccountController"
    // app-setting check below is kept as defense-in-depth for non-production Debug deployments
    // (e.g. a shared QA environment) where this type IS compiled in.
    public class AccountTestController : ApiController, IAccountService
    {
        AccountControllerHelper helper = new AccountControllerHelper();

        private static bool IsEnabled =>
            string.Equals(System.Configuration.ConfigurationManager.AppSettings["EnableTestAccountController"], "true", StringComparison.OrdinalIgnoreCase);

        private HttpResponseException Disabled()
        {
            return new HttpResponseException(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [HttpPost]
        public ProcessResponse TestConnection([FromBody]TestConnectionRequest request)
        {
            if (!IsEnabled) throw Disabled();
            if (request != null)
            {
                if (request.IMEI == "Fail")
                    return helper.getFailureResponse("Connection Failed");

                return helper.getSuccessResponse();
            }
            else
                return helper.getFailureResponse("Request with  no data");
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse Authenticate([FromBody]UserInfo userInfo)
        {
            if (!IsEnabled) throw Disabled();
            if (userInfo != null)
            {
                string Authmessage = "";
                bool isResponsed = false;
                isResponsed = helper.IsAuthenticated(userInfo.UserName, userInfo.Password, out Authmessage);
                if (isResponsed)
                {
                    return helper.getSuccessResponse();
                }
                else
                {
                    return helper.getFailureResponse(Authmessage);
                }
            }
            else
            {
                return helper.getFailureResponse("Request with no data");
            }
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse ValidateMobileNumber(string userName, string mobileNumber)
        {
            if (!IsEnabled) throw Disabled();
            string ResponseMessage;
            bool isResponded = false;
            isResponded = helper.ValidateNumber(out ResponseMessage, userName, mobileNumber);
            if (isResponded)
            {
                return helper.getSuccessResponse(ResponseMessage);
            }
            else
            {
                return helper.getFailureResponse(ResponseMessage);
            }
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse ValidateOTPAndPwdReset(ValidateOTPRequest request)
        {
            if (!IsEnabled) throw Disabled();
            if (request != null)
            {
                if (request.OTP == "Fail")
                    return helper.getFailureResponse("OTP Expired");
                helper.ValidateOTPPasswordChange(request);
                return helper.getSuccessResponse();
            }
            else
            {
                return helper.getFailureResponse("Request with no data");
            }
        }
        [HttpGet]
        [HttpPost]
        public IEnumerable<EndpointDTO> GetEndpointList(string userName)
        {
            if (!IsEnabled) throw Disabled();
            if (userName == "Fail")
                return null;
            return helper.getEndpointList(userName);
        }

        [HttpGet]
        [HttpPost]
        public PasswordPolicyDTO GetPasswordPolicy(int endpointId)
        {
            if (!IsEnabled) throw Disabled();
            if (endpointId == -1)
                return null;
            return helper.getPasswordPolicy(endpointId);
           
        }
        [HttpGet]
        [HttpPost]
        public List<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
        {
            if (!IsEnabled) throw Disabled();
            if (request != null)
            {
                if (request.UserName == "Fail")
                    return helper.getFailureChangePasswordResponse(null);
                    helper.ChangePasword(request);
                return helper.getSuccessChangePasswordResponse();
            }
            else
            {
                return helper.getFailureChangePasswordResponse("Request with no data");
            }
        }
        [HttpGet]
        [HttpPost]
        public UserDTO GetUserDetail(string userName)
        {
            if (!IsEnabled) throw Disabled();
            if (userName == "Fail")
                return null;
            return helper.getUserInfo(userName);
           
        }
        [HttpGet]
        [HttpPost]
        public List<UserConfigResponse> GetUserConfiguration()
        {
            if (!IsEnabled) throw Disabled();
            return helper.getUserConfigResponse();
           
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse UpdateUser(UserDTO userDTO)
        {
            if (!IsEnabled) throw Disabled();
            if (userDTO != null)
            {
                if (userDTO.Name == "Fail")
                    return helper.getFailureResponse("Update failed");
                helper.UpdateUser(userDTO);
                return helper.getSuccessResponse("User details updated");
            }
            else
            {
                return helper.getFailureResponse("Request with no data");
            }

        }
        [HttpGet]
        [HttpPost]
        public IEnumerable<EntitlementResponse> GetEntitlement(int endpointUserId)
        {
            if (!IsEnabled) throw Disabled();
            if (endpointUserId == -1)
                return null;

            return helper.GetEntitlementResponse(endpointUserId);
        }
    }
#endif
}