using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Connector.Linux
{
   public class LinuxUtil
    {
      
            
            public static SshConnectionInfo GetInput()
            {
                SshConnectionInfo info = new SshConnectionInfo();
               
                info.Host = "192.168.16.199";
                info.User = "root";
                info.Pass = "@bcd1234";
                
                return info;
            }
        }

        public struct SshConnectionInfo
        {
            public string Host;
            public string User;
            public string Pass;
         //   public string IdentityFile;
        }

    }

