using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.Model;
using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.DataAccess
{
    public class VPNMasterDA
    {

        public List<ADSGroupInfo> GetGroups()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSGroupInfos.Select(grp => grp).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SaveVPNDetails(VPNMaster vpnMaster)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (vpnMaster.Id == 0)
                    {
                        _context.VPNMasters.Add(vpnMaster);
                        _context.SaveChanges();
                    }
                    else
                    {
                        VPNMaster oCurrentItem = _context.VPNMasters.Find(vpnMaster.Id);
                        if (oCurrentItem != null)
                        {
                            vpnMaster.Id = oCurrentItem.Id;
                            _context.Entry(oCurrentItem).CurrentValues.SetValues(vpnMaster);
                            _context.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<VPNMaster> GetVPNMasterList()
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                try
                {
                    return _context.VPNMasters.Where(Items => Items.IsActive).ToList();
                }
                catch (Exception)
                { throw; }
            }
        }
        public VPNMaster GetVPNMasterById(int Id)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                try
                {
                    return _context.VPNMasters.Where(Items => Items.Id == Id).FirstOrDefault();
                }
                catch (Exception)
                { throw; }
            }
        }
        public VPNMaster GetVPNItem(int Id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    // return (from Items in _context.VPNMasters where Items.Id == Id select Items).FirstOrDefault();
                    return _context.VPNMasters.Find(Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public void VPNUpdate(VPNMaster vpnMaster, int Id)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            VPNMaster oCurrentItem = _context.VPNMasters.Find(Id);
        //            if (oCurrentItem != null)
        //            {
        //                vpnMaster.Id = oCurrentItem.Id;
        //                _context.Entry(oCurrentItem).CurrentValues.SetValues(vpnMaster);
        //                _context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public void ActivateVPN(int Id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    VPNMaster oCurrentItem = _context.VPNMasters.Find(Id);
                    if (oCurrentItem != null)
                    {
                        oCurrentItem.IsActive = !oCurrentItem.IsActive;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<int, string> GetApplications(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.VPNMasters
                        .Where(c => c.ApplicationName.Contains(searchString)).OrderBy(C=>C.ApplicationName)
                        .Select(c => c).ToDictionary(c => c.Id, c => c.ApplicationName);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetVPNADGroupsByApplication(string applicationName, EnvironmentType environmentType)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (environmentType == EnvironmentType.Development)
                {
                    return _context.VPNMasters.FirstOrDefault(c => c.ApplicationName == applicationName).DevADGroups;
                }
                if (environmentType == EnvironmentType.QA)
                {
                    return _context.VPNMasters.FirstOrDefault(c => c.ApplicationName == applicationName).QAADGroups;
                }
                return _context.VPNMasters.FirstOrDefault(c => c.ApplicationName == applicationName).ProductionADGroups;
            }
        }

    }
}
