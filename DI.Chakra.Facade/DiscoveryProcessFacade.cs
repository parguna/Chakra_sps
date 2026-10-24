using DI.Chakra.DataAccess;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Facade
{
    public class DiscoveryProcessFacade
    {
        DiscoveryProcessDA discoveryProcessDA = null;

        public DiscoveryProcessFacade()
        {
            discoveryProcessDA = new DiscoveryProcessDA();
        }
        public long SaveDiscoveryProcess(DiscoveryProcess process)
        {
            try
            {
                return discoveryProcessDA.SaveDiscoveryProcess(process);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DiscoveryProcess GetDiscovery(int _processId)
        {
            try
            {
                return discoveryProcessDA.GetDiscovery(_processId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool IsDicoveryProcessNameExists(string Name)
        {
            try
            {
                return discoveryProcessDA.IsDicoveryProcessNameExists(Name);
            }
            catch (Exception)
            {

                throw;
            }
        }
      
        //public void UpdateDiscovery(DiscoveryProcess _discoveryProcess)
        //{
        //    try
        //    {
        //         discoveryProcessDA.UpdateDiscovery(_discoveryProcess);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        public DiscoveryProcess GetDefaultDiscoveryProcess(int EndpointSystemId)
        {
            try
            {
                return discoveryProcessDA.GetDefaultDiscoveryProcess(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
