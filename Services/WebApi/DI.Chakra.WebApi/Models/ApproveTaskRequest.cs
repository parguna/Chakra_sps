using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChakraService.Models
{
    public class ApproveTaskRequest
    {
        public int TaskId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
    }
}