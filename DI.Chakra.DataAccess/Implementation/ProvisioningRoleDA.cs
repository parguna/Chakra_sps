using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DataAccess
{
    public class ProvisioningRoleDA
    {

        #region Private Members
        // private ProvisioningRole ProvisioningRole;
        // private IACMDBContext oIACMDBContext = new IACMDBContext();
        #endregion


        //public void AddProvisioningRole(ProvisioningRole _provisioningRole)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            _context.ProvisioningRoles.Add(_provisioningRole);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Adding Provisioning Role", ex);
        //        throw ex;
        //    }

        //}
        //public void UpdateProvisioningRole(ProvisioningRole _provisoningRole)
        //{
        //    try
        //    {

        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            ProvisioningRole provisioningRole = _context.ProvisioningRoles.Include("ProvisioningRoleDetails").Where(prv => prv.Id == _provisoningRole.Id).FirstOrDefault();

        //            provisioningRole.Name = _provisoningRole.Name;
        //            provisioningRole.Description = _provisoningRole.Description;
        //            provisioningRole.LastModifiedDate = DateTime.Now;
        //            provisioningRole.LastModifiedBy = "Admin";

        //            // To Delete Old ProvRoleDetails For Selected Prov Role ID
        //            _context.ProvisioningRoleDetails.RemoveRange(provisioningRole.ProvisioningRoleDetails);
        //            _context.SaveChanges();

        //            //To Add New ProvRoleDetails For Selected Prov Role ID
        //            provisioningRole.ProvisioningRoleDetails = _provisoningRole.ProvisioningRoleDetails;

        //            _context.ProvisioningRoles.Attach(provisioningRole);
        //            _context.Entry(provisioningRole).State = EntityState.Modified;
        //            //_context.Entry(provisioningRole.ProvisioningRoleDetails).State = EntityState.Modified;
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating Provisioning Role", ex);
        //        throw ex;
        //    }

        //}
        //public void UpdateProvisioningRoleByAddingProvisiongDetail(ProvisioningRole _provsioningRole, ProvisioningRoleDetail _provisioningRoleDetail)
        //{
        //    try
        //    {

        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            ProvisioningRole provisioningRole = _context.ProvisioningRoles.Include("ProvisioningRoleDetails").Where(prv => prv.Id == _provsioningRole.Id).FirstOrDefault();
        //            provisioningRole.Name = _provsioningRole.Name;
        //            provisioningRole.Description = _provsioningRole.Description;
        //            provisioningRole.LastModifiedDate = DateTime.Now;
        //            provisioningRole.LastModifiedBy = "Admin";

        //            provisioningRole.ProvisioningRoleDetails.Add(_provisioningRoleDetail);


        //            _context.ProvisioningRoles.Attach(provisioningRole);
        //            _context.Entry(provisioningRole).State = EntityState.Modified;
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating Provisioning Role", ex);
        //        throw ex;
        //    }

        //}

        //public void AddProvisioningRole(ProvisioningRole _provisioningRole, List<ProvisioningRoleDetail> _provisioningDetail)
        //{
        //    try
        //    {
        //        _provisioningRole.ProvisioningRoleDetails = _provisioningDetail;

        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            _context.ProvisioningRoles.Add(_provisioningRole);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Adding Provisioning Role", ex);
        //        throw ex;
        //    }

        //}

        //public void AddProvisioningRoleDetail_Test()
        //{
        //    using (IACMDBContext _context = new IACMDBContext())
        //    {
        //        IList<ProvisioningRoleDetail> pd = new List<ProvisioningRoleDetail>() {
        //            new ProvisioningRoleDetail() { EndpointSystemId = 1, Entitlements = "New Entitlement1" },
        //            new ProvisioningRoleDetail() { EndpointSystemId = 2, Entitlements = "New Entitlement2" },
        //            new ProvisioningRoleDetail() { EndpointSystemId = 3, Entitlements = "New Entitlement3" }
        //        };
        //        //ProvisioningRoleDetail pd1 = new ProvisioningRoleDetail() { EndpointSystemId = 2, Entitlements = "New Entitlement 2" };

        //        ProvisioningRole p = new ProvisioningRole() { Name = "ww2", Description = "desc2", CreatedBy = "Mari", CreatedDate = DateTime.Now, LastModifiedBy = "Mari", LastModifiedDate = DateTime.Now, ProvisioningRoleDetails = pd };
        //        _context.ProvisioningRoles.Add(p);
        //        _context.SaveChanges();
        //    }
        //}


        //public List<ProvisioningRole> GetAllProvisioningRolesWithDetails()
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return (from provRole in oIACMDBContext.ProvisioningRoles

        //                    select provRole
        //                    ).ToList();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        //public void DeleteProvisioningRole(int ProvRoleId)
        //{
        //    ProvisioningRole provisioningRole = new ProvisioningRole();

        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            provisioningRole = (from prvrole in _context.ProvisioningRoles.Include("ProvisioningRoleDetails")
        //                                where prvrole.Id == ProvRoleId
        //                                select prvrole).FirstOrDefault();
        //            if (provisioningRole != null)
        //            {
        //                _context.ProvisioningRoles.Remove(provisioningRole);
        //                _context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public ProvisioningRole GetProvisioningRole(int ProvRoleId)
        //{
        //    ProvisioningRole provisioningRole = null;

        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {                  
                    
        //            provisioningRole = (from prvrole in _context.ProvisioningRoles.Include("ProvisioningRoleDetails")
        //                                where prvrole.Id == ProvRoleId
        //                                select prvrole).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return provisioningRole;
        //}

        public List<AccessRequestItem> GetAllEntitlementForProvisioningRole(string searchString, List<EndpointType> EndpointSystem)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    List<AccessRequestItem> ReturnList = new List<AccessRequestItem>();
                    if (EndpointSystem == null || EndpointSystem.Count == 0)
                    {
                        var EntitlementAD = new ActiveDirectoryDA().GetAccessRequestItem(searchString, false);
                        ReturnList.AddRange(EntitlementAD);

                        var EntitlementLNS = new LotusNotesDA().GetAccessRequestItem(searchString, false);
                        ReturnList.AddRange(EntitlementLNS);

                        var EntitlementSAP = new SAPConnectorDA().GetAccessRequestItem(searchString, false);
                        ReturnList.AddRange(EntitlementSAP);

                        var EntitlementLINUX = new LinuxDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementLINUX);

                        var EntitlementOEBS = new OracleEBSDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementOEBS);

                        var EntitlementORCLUM = new ORCLUMDA().GetAccessRequestItem(searchString);
                        ReturnList.AddRange(EntitlementORCLUM);
                    }
                    else
                    {
                        foreach (EndpointType Endpoint in EndpointSystem)
                        {
                            if (Endpoint == EndpointType.ActiveDirectory)
                            {
                                var Entitlement = new ActiveDirectoryDA().GetAccessRequestItem(searchString, false);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.LotusNotes)
                            {
                                var Entitlement = new LotusNotesDA().GetAccessRequestItem(searchString, false);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.SAP)
                            {
                                var Entitlement = new SAPConnectorDA().GetAccessRequestItem(searchString, false);
                                ReturnList.AddRange(Entitlement);
                            }

                            else if (Endpoint == EndpointType.Linux)
                            {
                                var Entitlement = new LinuxDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.OracleEBS)
                            {
                                var Entitlement = new OracleEBSDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                            else if (Endpoint == EndpointType.ORCLUM)
                            {
                                var Entitlement = new ORCLUMDA().GetAccessRequestItem(searchString);
                                ReturnList.AddRange(Entitlement);
                            }
                        }
                    }
                    return ReturnList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
