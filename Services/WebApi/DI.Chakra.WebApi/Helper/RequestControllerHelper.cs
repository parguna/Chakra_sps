using System;
using System.Linq;
using DI.Chakra.Facade;
using ChakraService.Models;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

using System.Collections.Generic;
namespace ChakraService
{
    public class RequestControllerHelper
    {
        #region Members
        private AccessRequestFacade ARFacade = new AccessRequestFacade();
        private UserFacade UserFacade = new UserFacade();
        #endregion

        #region Methods
        public IList<RequestResponse> GetRequestsCount(string UserName)
        {
            long ActionUserId = 0;
            var User = UserFacade.GetUserByName(UserName);
            if (User != null)
                ActionUserId = User.Id;
            var TaskList = ARFacade.GetRequestsByActionUserId(ActionUserId);
            List<RequestResponse> Returnvalue = new List<RequestResponse>();

            foreach (AdHocActionType item in Enum.GetValues(typeof(AdHocActionType)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                if (item != AdHocActionType.None)
                {
                    var Count = TaskList.Where(lq => lq.AdhocActionType == item).Count();
                    Returnvalue.Add(new RequestResponse() { Type = description, Count = Count });
                }
            }
            return Returnvalue;
        }

        public IList<RequestDTO> GetRequests(string UserName)
        {
            long ActionUserId = 0;
            var User = UserFacade.GetUserByName(UserName);
            if (User != null)
                ActionUserId = User.Id;
            var TaskList = ARFacade.GetRequestsByActionUserId(ActionUserId).ToList();
            List<RequestDTO> Returnvalue = new List<RequestDTO>();
            TaskList.ForEach(lq =>
            {
                Returnvalue.Add(new RequestDTO() { Id = lq.AccessRequestID, Name = string.IsNullOrEmpty(lq.RequestType) ? lq.Name : lq.Name + " " + lq.RequestType });
            });
            return Returnvalue;
        }
        #endregion
    }
}