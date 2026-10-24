using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace DI.Chakra.Connector
{
    public class ADAccountHelper
    {

        public static DateTime? AccountExpiresOn(DirectoryEntry oDE)
        {
            try
            {
                //int flags = (int)oDE.Properties[ADProperties.userAccountControl].Value;
                //if (Convert.ToBoolean(flags & ADAccountValues.ADS_UF_DONT_EXPIRE_PASSWD))
                //{
                //    return null;
                //}

                return ADHelper.GetCOMProperty_DateTime(oDE, ADProperties.accountExpires);

            }
            catch (Exception)
            {

                return null;
            }
           
        }
    }
}
