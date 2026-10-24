using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChakraService.Models
{
    public class PasswordExpireResponse
    {
        public string DisplayName { get; set; }
        public string MobileNumber { get; set; }
        public AccountInfo Accounts { get; set; }
    }


    public class AccountInfo
    {
        public string AccountName { get; set; }
        public string EndpointSystemType { get; set; }
        public string Endpoint { get; set; }
    }
}