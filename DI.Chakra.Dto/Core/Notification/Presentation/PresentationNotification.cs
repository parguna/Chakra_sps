using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{

    public class PresentationNotification : Notification
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public long? RequesterID { get; set; }
    }
}
