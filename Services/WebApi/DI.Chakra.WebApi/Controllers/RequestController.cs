using System.Web.Http;
using ChakraService.Models;
using System.Collections.Generic;

namespace ChakraService.Controllers
{
    public class RequestController : ApiController
    {
        #region Members
        private RequestControllerHelper Helper = new RequestControllerHelper();
        #endregion

        #region Methods
        [HttpGet]
        [HttpPost]
        public IEnumerable<RequestResponse> GetRequestsCount(string UserName)
        {
            if (UserName == "Fail")
                return null;
            return Helper.GetRequestsCount(UserName);
        }

        [HttpGet]
        [HttpPost]
        public IEnumerable<RequestDTO> GetRequests(string UserName)
        {
            if (UserName == "Fail")
                return null;
            return Helper.GetRequests(UserName);
        }
        #endregion
    }
}
