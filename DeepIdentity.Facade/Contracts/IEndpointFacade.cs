using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.Model;

namespace DeepIdentity.Facade
{
    public interface IEndpointFacade
    {
        IEndpointUserInfo GeEndpointUserInfo(int EndpointUserId);
    }
}
