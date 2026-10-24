using System.Web.Http;
using ChakraService.Models;
using System.Collections.Generic;

namespace ChakraService.Controllers
{
    public class AccountController : ApiController, IAccountService
    {
        #region Members
        private AccountControllerHelper Helper = new AccountControllerHelper();
        #endregion

        #region Methods
        [HttpGet]
        [HttpPost]
        public ProcessResponse TestConnection([FromBody]TestConnectionRequest request)
        {
            if (request.IMEI == "Fail")
                return Helper.getFailureResponse("Connection Failed");

            return Helper.getSuccessResponse();
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse Authenticate(string userName, string password)
        {
            if (userName == "Fail")
                return Helper.getFailureResponse("Username not found");

            if (password == "Fail")
                return Helper.getFailureResponse("Password is wrong");

            return Helper.getSuccessResponse();
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse ValidateMobileNumber(string userName, string mobileNumber)
        {
            
            if (mobileNumber == "Fail")
                return Helper.getFailureResponse("mobileNumber not found");

            return Helper.getSuccessResponse();
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse ValidateOTPAndPwdReset(ValidateOTPRequest request)
        {
            if (request.OTP == "Fail")
                return Helper.getFailureResponse("OTP Expired");

            return Helper.getSuccessResponse();
        }
        [HttpGet]
        [HttpPost]
        public IEnumerable<EndpointDTO> GetEndpointList(string userName)
        {
            if (userName == "Fail")
                return null;

            return Helper.getEndpointList(userName);
        }
        [HttpGet]
        [HttpPost]
        public List<PasswordPolicyDTO> GetPasswordPolicy(int endpointId)
        {
            if (endpointId == -1)
                return null;

            return new List<PasswordPolicyDTO>()
            {
                new PasswordPolicyDTO() { Name="Minimum Characters" , Value = "5" },
                new PasswordPolicyDTO() { Name="Maximum Characters" , Value = "15" },
                new PasswordPolicyDTO() { Name="Minimum Number" , Value = "1" }
            };
        }
        [HttpGet]
        [HttpPost]
        public List<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
        {
            if (request.UserName == "Fail")
                return Helper.getFailureChangePasswordResponse(null);

            return Helper.getSuccessChangePasswordResponse();
        }
        [HttpGet]
        [HttpPost]
        public UserDTO GetUserDetail(string userName)
        {
            if (userName == "Fail")
                return null;

            return new UserDTO()
            {
                Id = 1,
                Name = "Scotts",
                DisplayName = "Scott Miller",
                MobileNumber = "23223223"
            };
        }
        [HttpGet]
        public List<UserConfigResponse> GetUserConfiguration()
        {
            
            return new List<UserConfigResponse>()
            {
                new UserConfigResponse() {FieldName="FirstName", SelfUpdate=false},
                new UserConfigResponse() {FieldName="LastName", SelfUpdate=false},
                new UserConfigResponse() {FieldName="DisplayName", SelfUpdate=false},
                new UserConfigResponse() {FieldName="MobileNumber", SelfUpdate=true},
            };
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse UpdateUser(UserDTO userDTO)
        {
            if (userDTO.Name == "Fail")
                return Helper.getFailureResponse("Update failed");

            return Helper.getSuccessResponse();
        }
        [HttpGet]
        [HttpPost]
        public IEnumerable<EntitlementResponse> GetEntitlement(int UserId)
        {
            if (UserId == -1)
                return null;

            return Helper.GetEntitlementResponse(UserId);
        }
        [HttpGet]
        [HttpPost]
        public IEnumerable<PasswordExpireResponse> GetPwdExpiryUsers()
        {
            return Helper.GetPwdExpiryUsers();
        }
        #endregion
    }
}
