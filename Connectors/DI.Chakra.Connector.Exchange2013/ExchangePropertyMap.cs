using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using DI.Chakra.Model;
using DI.Chakra.DomainModel;
using System.DirectoryServices;
using System.Collections;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Connector
{
    public class ExOrchestrator
    {
        public ExOrchestrator()
        {

        }

        public ExUserInfo GetExUserInfo(DirectoryEntry de)
        {
            ExUserInfo user = null;
            try
            {
                user = new ExUserInfo();

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
