using System;
using System.Collections.Generic;
using System.Text;

namespace DI.Chakra.Services.EmailingProcessor
{
    public enum EmailStatus
    {
        PENDING =0,

        INPROGRESS,

        SENT,

        FAILED
    }
}
