using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
   public class PresentationNotification : Notification
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public long? RequesterID { get; set; }
    }
}
