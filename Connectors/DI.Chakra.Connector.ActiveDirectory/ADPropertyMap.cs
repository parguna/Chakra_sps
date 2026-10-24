using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Collections;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Connector
{
    public class ADOrchestrator
    {
        public ADOrchestrator()
        {

        }

        public ADSUserInfo GetADSUserInfo(DirectoryEntry de)
        {
            ADSUserInfo user = null;
            try
            {
                user = new ADSUserInfo();

                IDictionaryEnumerator ide = de.Properties.GetEnumerator();
                ide.Reset();
                while (ide.MoveNext())
                    
                {
                    foreach (object listIterProp in de.Properties[ide.Entry.Key.ToString()])
                    {
                        if (de.Properties[ide.Entry.Key.ToString()].Count > 1)
                        {
                            Console.WriteLine(string.Format("Name:{0} value:{1}", ide.Entry.Key.ToString(), listIterProp.ToString()));
                        }
                        else
                        {
                            PropertyValueCollection pvc = ide.Entry.Value as PropertyValueCollection;
                            Console.WriteLine(string.Format("name:{0} value:{1}", ide.Entry.Key.ToString(), pvc.Value));

                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }



            return user;
        }
    }
}
