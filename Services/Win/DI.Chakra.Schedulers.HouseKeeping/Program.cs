using DI.Chakra.Schedulers.HouseKeepingProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Schedulers.HouseKeeping
{
    class Program
    {
        static void Main(string[] args)
        {
            HouseKeepingHelper housekeepingHelper = new HouseKeepingHelper();
            housekeepingHelper.StartProcess();
        }
    }
}
