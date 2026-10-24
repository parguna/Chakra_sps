using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.DataAccess;
using DeepIdentity.Model;
using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.Facade
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
        public VPNMaster GetVPNMasterById(int Id)
        {
            return oVPNDA.GetVPNMasterById(Id);
        }
        public VPNMaster GetVPNItem(int Id)
        {
            return oVPNDA.GetVPNItem(Id);
        }

        //public void VPNUpdate(VPNMaster vpnMaster, int Id)
        //{
        //    oVPNDA.VPNUpdate(vpnMaster, Id);
        //}
        public void ActivateVPN(int Id)
        {
            oVPNDA.ActivateVPN(Id);
        }

        public Dictionary<int, string> GetApplications(string searchString)
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
