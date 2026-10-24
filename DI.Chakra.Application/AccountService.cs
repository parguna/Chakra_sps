

using DI.Chakra.Infrastructure;
//using DI.Chakra.Infrastructure.;
using DI.Chakra.Facade;
using System.Collections;
using System;
using DI.Chakra.Model;

namespace Application
{
    // testing svn
    public class AccountService : IAccountService
    {
        public UserFacade UserFacade { get; set; }

        public AccountService()
        {

            UserFacade = new UserFacade();
        }

        #region IAccountService Members

        #region Users

        public User GetUserByName(string name,string tenant)
        {

            try
            {
                return UserFacade.GetUserByName(name);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

       

        public void UpdateLastLogonInformation(User User)
        {
            UserFacade.UpdateLastLogonInformation(User);
        }
        
        #endregion


        #endregion

    }
}
