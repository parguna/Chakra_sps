using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DataAccess;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Facade
{
    public class VPNMasterFacade
    {
        VPNMasterDA oVPNDA = new VPNMasterDA();

        public List<ADSGroupInfo> GetGroups()
        {
            return oVPNDA.GetGroups();
        }

        public void SaveVPNDetails(VPNMaster vpnMaster)
        {
            oVPNDA.SaveVPNDetails(vpnMaster);
        }

        public List<VPNMaster> GetVPNMasterList()
        {
            return oVPNDA.GetVPNMasterList();
        }
        public VPNMaster GetVPNMasterById(long Id)
        {
            return oVPNDA.GetVPNMasterById(Id);
        }
        public VPNMaster GetVPNItem(long Id)
        {
            return oVPNDA.GetVPNItem(Id);
        }

        //public void VPNUpdate(VPNMaster vpnMaster, int Id)
        //{
        //    oVPNDA.VPNUpdate(vpnMaster, Id);
        //}
        public void ActivateVPN(long Id)
        {
            oVPNDA.ActivateVPN(Id);
        }

        public Dictionary<long, string> GetApplications(string searchString)
        {
            return oVPNDA.GetApplications(searchString);
        }

        public string GetVPNADGroupsByApplication(string applicationName, EnvironmentType environmentType)
        {
            try
            {
                return oVPNDA.GetVPNADGroupsByApplication(applicationName, environmentType);
            }
            catch
            {
                throw;
            }
        }

    }
}
