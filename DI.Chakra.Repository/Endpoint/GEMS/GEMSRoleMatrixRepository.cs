using DI.Chakra.DBClient;
using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class GEMSRoleMatrixRepository : RepositoryBase<GEMSRoleMatrix>
    {
        public GEMSRoleMatrixRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<string> GetRolesForGEMS(string gemsDetail)
        {
            try
            {
                return CurrentContext.GEMSRoleInfo
                    .Where(Role => Role.Display_Name == gemsDetail)
                    .Select(role => role.Name)
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetGEMSTechnicalRolesFromBusinessRole(string roleName)
        {
            try
            {
                List<string> techRoles = new List<string>();
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return techRoles;
                }

                GEMSRoleMatrix gemsRoleMatrix = CurrentContext.GEMSRoleMatrix.FirstOrDefault(item => item.BusinessRoleName == roleName && item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year);

                if (gemsRoleMatrix != null && (!string.IsNullOrWhiteSpace(gemsRoleMatrix.TechnicalRole)))
                {
                    List<string> RoleList = gemsRoleMatrix.TechnicalRole.Split(',').ToList();

                    foreach (string role in RoleList)
                    {
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            GEMSRoleInfo _gemsRoleInfo = CurrentContext.GEMSRoleInfo.FirstOrDefault(Role => Role.Display_Name == role.Trim());
                            if (_gemsRoleInfo != null)
                                techRoles.Add(_gemsRoleInfo.Name.Trim());
                        }
                    }
                }

                return techRoles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CheckBusinessRolesUsedInOtherBusinessRoles(string businessRoles, ref List<string> tobeRemovedtechRoles, ref List<string> tobeRemoveadGroups)
        {
            try
            {
                tobeRemovedtechRoles = new List<string>();
                tobeRemoveadGroups = new List<string>();

                var tempbusinessroles = businessRoles.Split(',');

                foreach (var brole in tempbusinessroles)
                {
                    GEMSRoleMatrix gemsRoleMatrix = CurrentContext.GEMSRoleMatrix.FirstOrDefault(item => item.BusinessRoleName == brole && item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year);

                    if (gemsRoleMatrix != null && (!string.IsNullOrWhiteSpace(gemsRoleMatrix.TechnicalRole)))
                    {
                        List<string> RoleList = gemsRoleMatrix.TechnicalRole.Split(',').ToList();
                        List<string> grpList = gemsRoleMatrix.ADGroups.Split(',').ToList();

                        //Check for Other BusinessRoles
                        foreach (var techrole in RoleList)
                        {
                            var byTechroles = CurrentContext.GEMSRoleMatrix.Where(item => item.TechnicalRole.Contains(techrole) && item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();

                            foreach (var byTech in byTechroles)
                            {
                                if (string.Compare(byTech.BusinessRoleName,gemsRoleMatrix.BusinessRoleName) !=0 &&
                                       !string.IsNullOrWhiteSpace(byTech.ADGroups))
                                {
                                    var adgrps = byTech.ADGroups.Split(',').ToList();
                                    foreach (var grpItem in adgrps)
                                    {
                                        if (!string.IsNullOrWhiteSpace(grpItem) && grpList.Contains(grpItem))
                                        {
                                            tobeRemovedtechRoles.Add(techrole.Trim());
                                            tobeRemoveadGroups.Add(grpItem.Trim());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

        public List<string> GetGEMSADGroupsFromBusinessRole(string roleName)
        {
            try
            {
                List<string> adGroups = new List<string>();

                GEMSRoleMatrix gemsRoleMatrix = CurrentContext.GEMSRoleMatrix.FirstOrDefault(item => item.BusinessRoleName == roleName && item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year);

                if (gemsRoleMatrix != null && (!string.IsNullOrWhiteSpace(gemsRoleMatrix.ADGroups)))
                {
                    List<string> grpList = gemsRoleMatrix.ADGroups.Split(',').ToList();

                    foreach (string grp in grpList)
                    {
                        if (!string.IsNullOrWhiteSpace(grp))
                        {
                            ADSGroupInfo _adsGroupInfo = CurrentContext.ADSGroupInfos.FirstOrDefault(Role => Role.cn == grp.Trim()); // Role.ValidFrom <= DateTime.Today && Role.ValidTo >= DateTime.Today && 
                            if (_adsGroupInfo != null)
                                adGroups.Add(_adsGroupInfo.cn);
                        }
                    }
                }


                return adGroups;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<string> GetGEMSADGroupsByBusinessRole(string businessRole)
        {
            try
            {
                List<string> adGroups = new List<string>();
                if (!string.IsNullOrWhiteSpace(businessRole))
                {
                    GEMSRoleMatrix _GEMSRoleMatrix = CurrentContext.GEMSRoleMatrix.FirstOrDefault(item => item.BusinessRoleName == businessRole && item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year);

                    if (_GEMSRoleMatrix != null && (!string.IsNullOrWhiteSpace(_GEMSRoleMatrix.ADGroups)))
                    {
                        List<string> grpList = _GEMSRoleMatrix.ADGroups.Split(',').ToList();

                        foreach (string grp in grpList)
                        {
                            ADSGroupInfo _adsGroupInfo = CurrentContext.ADSGroupInfos.FirstOrDefault(Role => Role.cn == grp.Trim()); // Role.ValidFrom <= DateTime.Today && Role.ValidTo >= DateTime.Today && 
                            if (_adsGroupInfo != null)
                                adGroups.Add(_adsGroupInfo.cn);
                        }
                    }

                }
                return adGroups;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<string> GetGEMSGroupsFromADGroups(long adsEndpointUserId)
        {
            try
            {
                List<string> adGoups = CurrentContext.ADSUserInfos.Include("ADSGroups").FirstOrDefault(item => item.Id == adsEndpointUserId).
                                     ADSGroups.Select(item => item.name).ToList();

                List<string> gemsADGroups = new List<string>();

                var gemsGroups = CurrentContext.GEMSRoleMatrix.Select(item => item.ADGroups).ToList();

                foreach (string item in gemsGroups)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var groups = item.Split(',');
                        foreach (string group in groups)
                        {
                            if (!string.IsNullOrWhiteSpace(group) && !gemsADGroups.Contains(group))
                            {
                                gemsADGroups.Add(group.Trim());
                            }
                        }
                    }
                }

                return (from gemsGroup in gemsADGroups
                        from adGroup in adGoups
                        where gemsGroup == adGroup
                        select adGroup).Distinct().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetRoleMatrixFromRoleInfo(string technicalRole)
        {
            List<string> adGroups = new List<string>();
            var gemsRoleMatrix = (from matrix in CurrentContext.GEMSRoleMatrix
                     join role in CurrentContext.GEMSRoleInfo on matrix.TechnicalRole equals role.Display_Name
                     where role.Name == technicalRole
                     select matrix).FirstOrDefault();
            if (gemsRoleMatrix != null && (!string.IsNullOrWhiteSpace(gemsRoleMatrix.ADGroups)))
            {
                List<string> grpList = gemsRoleMatrix.ADGroups.Split(',').ToList();

                foreach (string grp in grpList)
                {
                    if (!string.IsNullOrWhiteSpace(grp))
                    {
                        ADSGroupInfo _adsGroupInfo = CurrentContext.ADSGroupInfos.FirstOrDefault(Role => Role.cn == grp.Trim()); 
                        if(_adsGroupInfo != null)
                        adGroups.Add(_adsGroupInfo.cn);
                    }
                }
            }
            return adGroups;
        }
    }
}
