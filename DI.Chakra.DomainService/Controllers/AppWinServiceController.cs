using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Web.Http;

namespace DI.Chakra.DomainService.Controllers
{
    [RoutePrefix("AppWinService")]
    public class AppWinServiceController : ApiController
    {
        [HttpGet]
        [Route("StartStopWinService")]
        public string StartStopWinService(string toStartStop, string serviceName, int timeoutMilliseconds)
        {
            if (toStartStop == "Start")
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                catch(Exception ex)
                {
                    return ex.ToString();
                }
            }
            else if (toStartStop == "Stop")
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }

            else if (toStartStop == "Restart")
            {
                ServiceController service = new ServiceController(serviceName);
                try
                {
                    int millisec1 = Environment.TickCount;
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                    int millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                catch(Exception ex)
                {
                    return ex.ToString();
                }

            }
                return "Completed";
        }

        [HttpGet]
        [Route("GetServicesStatus")]
        public ServiceControllerStatus GetServicesStatus(string servicename)
        {
            try
            {
                ServiceController service = ServiceController.GetServices().Where(t => t.ServiceName == servicename).FirstOrDefault();

                ServiceControllerStatus scStatus = service.Status;

                return scStatus;

            }
            catch (Exception ex)
            {
            }

            return ServiceControllerStatus.ContinuePending;
        }
    }
}
