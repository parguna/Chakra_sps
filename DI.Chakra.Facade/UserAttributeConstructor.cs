using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DI.Chakra.Facade
{
    public class UserAttributeConstructor
    {

        ActiveDirectoryDA ActiveDirectoryDA = null;

        public UserAttributeConstructor()
        {
            ActiveDirectoryDA = new ActiveDirectoryDA();
        }

        public IList<User> Construct(IList<ADSUserInfo> listadsUser)
        {
            IList<User> users = new List<User>();

            object obj = null;


            IList<AttributeMappingColumns> list = new DiConfigurationFacade().GetAttributeMappingColumns(listadsUser.FirstOrDefault().EndPointSystemId);

            Type type = typeof(User);

            foreach (ADSUserInfo adsUser in listadsUser)
            {
                User user = User.CreateUser();

                foreach (PropertyInfo info in type.GetProperties())
                {
                    obj = null;

                    IList<AttributeMappingColumns> attrs = list.Where(att => att.DiColumn == info.Name).ToList();

                    if (attrs.Count > 0)
                    {
                        AttributeMappingColumns at = attrs.First();

                        obj = GetPropValue(at.Tgcolumn, adsUser);
                    }

                    info.SetValue(user, obj, null);
                }
                
                user.CreatedDate = DateTime.Now;
                user.LastModifiedDate = DateTime.Now;

                users.Add(user);
            }
            return users;


        }

        private Object GetPropValue(String name, Object obj)
        {

            Type type = obj.GetType();

            PropertyInfo info = type.GetProperty(name);

            if (info == null)
                return null;


            if (info.ToString().Equals(string.Empty))
                return null;

            obj = info.GetValue(obj, null);

            if (info.PropertyType == typeof(DateTime))
            {
                if (obj == null || obj.ToString() == string.Empty)
                    return DBNull.Value;
            }

            return obj;
        }
    }
}
