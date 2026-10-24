using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
//using DI.Chakra.Infrastructure.;
using System.Collections;
using DI.Chakra.Dto;

namespace Application
{
    public interface IAccountService : IApplicationService
    {

        #region User
        User GetUserByName(string name,string tenant);
        #endregion

        void UpdateLastLogonInformation(User User);

    }
}
