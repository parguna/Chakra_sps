using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPSRMMainRoleMatrixRepository : RepositoryBase<SAPSRMMainRoleMatrix>
    {
        public SAPSRMMainRoleMatrixRepository(DataAccessTransaction transaction) : base(transaction) { }
        public Dictionary<string, string> GetSRMAllRoles(string searchString)
        {
            
            return this.CurrentContext.SAPSRMMainRoleMatrices.Where(item =>
                            !string.IsNullOrEmpty(item.BusinessRole) && (string.IsNullOrEmpty(searchString) ? true : item.BusinessRole.Contains(searchString)))
                            .Select(i=>i.BusinessRole).Distinct().ToDictionary(key=> key, value=>value);
                            
        }
        public Dictionary<string, string> GetSAPSRMRoles(AdHocActionType adhocActionType, long Userid, string searchString, AccountType accountType, 
            string srmUserName, long EndpointId)
        {
            #region old logic
            /*     using (IACMDBContext _context = new IACMDBContext())
            {
                Dictionary<string, string> roles;
                if (adhocActionType == AdHocActionType.NewAccount)
                {
                    roles = _context.SAPSRMMainRoleMatrices.ToDictionary(key => key.BusinessRole, value => value.BusinessRole);
                }
                else if (adhocActionType == AdHocActionType.AmendmentAdd)
                {
                    var nonEligibleList = new List<string>();

                    var ExistingBusinessRoles = _context.SAPSRMBusinessRoleMappings.Where(item => item.UserId == Userid).Select(item => item.BusinessRole).Distinct().ToList();
                    if (ExistingBusinessRoles.Count > 0)
                    {

                        foreach (var businessRole in ExistingBusinessRoles)
                        {
                            var enablerRule = _context.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == businessRole).Select(item => item.SRMEnablerRule).FirstOrDefault();
                           List<string> rules = enablerRule.Split(',').ToList();
                           Dictionary<string,int> dicrule = new Dictionary<string,int>();
                           if(rules.Count>1)
                           {
                            rules.ForEach(rule=>
                            {
                           int count = _context.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == rule).Distinct().Select(item=>item.POrg).Count();
                            dicrule.Add(rule,count);
                            });
                            var value = dicrule.OrderByDescending(pair=>pair.Value).Take(1);
                            enablerRule =   value.Select(key=>key.Key).First();
                            }

                            var porgCount = _context.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == enablerRule).Select(item => item.POrg).Distinct().Count();
                            var porgcountinBusinessRoleMapping = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == businessRole).Select(item => item.POrg).Distinct().Count();
                            if (porgCount != porgcountinBusinessRoleMapping)
                            {
                                continue;
                            }
                            else
                            {

                                var porgs = _context.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == enablerRule).Select(item => item.POrg).Distinct().ToList();
                                var nonEligiblePorgs = new List<string>();
                                foreach (string porg in porgs)
                                {
                                    var pgrpsBusinssRoleMapCount = 0;
                                    var pgrpsmap = _context.SAPSRMBusinessRoleMappings.Where(item => item.POrg == porg && item.BusinessRole == businessRole).Select(item => item.PGrp).Distinct().FirstOrDefault();
                                    if (pgrpsmap != null && pgrpsmap != string.Empty)
                                    {
                                        pgrpsBusinssRoleMapCount = pgrpsmap.Split(',').ToList().Count;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    var pgrpsCount = 0;
                                    var pgrps = _context.SAPSRMEnablerRoleMatrices.Where(item => item.POrg == porg && item.SRMEnablerRule == enablerRule).Select(item => item.PGrp).Distinct().ToList();
                                    if (pgrps != null)
                                    {
                                        pgrpsCount = pgrps.Count();
                                        if (pgrpsCount == 0)
                                        {
                                            continue;
                                        }
                                    }

                                    if ( pgrpsCount != pgrpsBusinssRoleMapCount)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        nonEligiblePorgs.Add(porg);
                                    }
                                }
                                if (nonEligiblePorgs.Count == porgCount)
                                {
                                    nonEligibleList.Add(businessRole);
                                }
                            }
                        }
                    }
                    var eligibleRoles = _context.SAPSRMMainRoleMatrices.Where(item => !nonEligibleList.Contains(item.BusinessRole)).ToList();
                    roles = eligibleRoles.ToDictionary(key => key.BusinessRole, value => value.BusinessRole);
                }

                else if (adhocActionType == AdHocActionType.AmendmentRemove)
                {
                    roles = _context.SAPSRMBusinessRoleMappings.Where(item => item.UserId == Userid).DistinctBy(item => item.BusinessRole).ToDictionary(key => key.BusinessRole, Value => Value.BusinessRole);
                }
                else
                {
                    roles = new Dictionary<string, string>();
                }
                return roles;
            }

        }
        * */
            #endregion

            var depCode = string.Empty;
            depCode = this.CurrentContext.User.FirstOrDefault(item => item.Id == Userid).DepartmentCode;
            Dictionary<string, string> roles = new Dictionary<string, string>();
            if (adhocActionType == AdHocActionType.NewAccount || adhocActionType == AdHocActionType.AmendmentAdd)
            {
                var query = this.CurrentContext.SAPSRMMainRoleMatrices.Where(item =>
                            !string.IsNullOrEmpty(item.BusinessRole) && (string.IsNullOrEmpty(searchString) ? true : item.BusinessRole.Contains(searchString)))
                            .Select(i => new { i.BusinessRole, i.DepartmentCode });
                if (depCode != string.Empty)
                {
                    query = query.Where(item => item.DepartmentCode == depCode || string.IsNullOrEmpty(item.DepartmentCode));
                }
                roles = query.Select(i=>i.BusinessRole).Distinct().ToDictionary(key => key, Value => Value);
            }
            else if(adhocActionType == AdHocActionType.AmendmentRemove)
            {
                var mappings = this.CurrentContext.SAPSRMBusinessRoleMappings
                    .Where(item => item.IsActive && item.UserName == srmUserName && item.AccountType == accountType && !string.IsNullOrEmpty(item.BusinessRole)).Select(i => i.BusinessRole);

                roles = mappings.Where(item => (string.IsNullOrEmpty(searchString) ? true : item.Contains(searchString))).Distinct().ToDictionary(key => key, Value => Value);
            }
            return roles;
        }

        public Dictionary<string, string> GetSAPSRMPgrps(string role, string porg)
        {
            var businessrole = CurrentContext.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == role).FirstOrDefault();
            if (businessrole != null && !string.IsNullOrWhiteSpace(businessrole.SRMEnablerRule))
            {
                var str = businessrole.SRMEnablerRule?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                str.ForEach(i =>
                {
                    i = i.Trim();
                });
                var enablerroles = CurrentContext.SAPSRMEnablerRoleMatrices.Where(item => str.Contains(item.SRMEnablerRule) && item.POrg == porg);
                if (enablerroles != null)
                {
                    return enablerroles.Select(item => item.PGrp).Distinct().ToDictionary(key => key, value => value);
                }
            }

            return new Dictionary<string, string>();
        }
        public Dictionary<string, string> GetSAPSRMPgrp(string BuisnessRole, AdHocActionType adhocActionType, AccountType accType, long userId, string POrg, string username, long endpointId)
        {

            #region Old logic
            /* using (IACMDBContext _context = new IACMDBContext())
            {
                var sapsrmMainRole = _context.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == BuisnessRole).FirstOrDefault();
                var sapSrmEnablerMatrix = _context.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == sapsrmMainRole.SRMEnablerRule && item.POrg == POrg).ToList();

                if (adhocActionType == AdHocActionType.NewAccount)
                {
                    return sapSrmEnablerMatrix.DistinctBy(item => item.PGrp).ToDictionary(key => key.PGrp, value => value.PGrp);
                }
                else if (adhocActionType == AdHocActionType.AmendmentAdd)
                {
                    List<string> ExistingPgrps = new List<string>();
                    sapSrmEnablerMatrix = sapSrmEnablerMatrix.DistinctBy(item => item.PGrp).ToList();
<<<<<<< .mine
                    var existingPgrp = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.UserId == userId && item.POrg == POrg && item.AccountType == accType).Select(item=>item.PGrp).FirstOrDefault();
                    if (existingPgrp != null)
                    {
                        ExistingPgrps = existingPgrp.Split(',').Distinct().ToList();
                    }

                    var result = sapSrmEnablerMatrix.Where(item => !ExistingPgrps.Contains(item.PGrp)).ToDictionary(key => key.PGrp, value => value.PGrp);
||||||| .r26775
                    var existingPgrp = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.UserId==userId && item.POrg == POrg && item.AccountType == accType).Select(item => item.PGrp).Distinct();
                    var result = sapSrmEnablerMatrix.Where(item => !existingPgrp.Contains(item.PGrp)).ToDictionary(key => key.PGrp, value => value.PGrp);
=======
                    var existingPgrp = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.UserId == userId && item.POrg == POrg && item.AccountType == accType).Select(item => item.PGrp).Distinct();
                    var result = sapSrmEnablerMatrix.Where(item => !existingPgrp.Contains(item.PGrp)).ToDictionary(key => key.PGrp, value => value.PGrp);
>>>>>>> .r26812
                    return result;
                }
                else if (adhocActionType == AdHocActionType.AmendmentRemove)
                {

                    List<string> ExistingPgrps = new List<string>();
                    var result = new Dictionary<string, string>();
                    sapSrmEnablerMatrix = sapSrmEnablerMatrix.DistinctBy(item => item.PGrp).ToList();
<<<<<<< .mine
                    var existingPgrp = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.UserId == userId && item.POrg == POrg && item.AccountType == accType).Select(item => item.PGrp).FirstOrDefault();
                    if (existingPgrp!=null)
||||||| .r26775
                    var existingPgrp = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.UserId==userId && item.POrg==POrg && item.AccountType == accType).Select(item => item.PGrp).Distinct().ToList();
                    if (existingPgrp.Count > 0)
=======
                    var existingPgrp = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.UserId == userId && item.POrg == POrg && item.AccountType == accType).Select(item => item.PGrp).Distinct().ToList();
                    if (existingPgrp.Count > 0)
>>>>>>> .r26812
                    {
                        ExistingPgrps = existingPgrp.Split(',').Distinct().ToList();
                        result = ExistingPgrps.ToDictionary(key => key, value => value);
                    }
                    return result;
                }
                return null;
            }*/
            #endregion
            var pGrp = new Dictionary<string, string>();
            var sapsrmMainRole = CurrentContext.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == BuisnessRole).FirstOrDefault();
            var enablerRule = CurrentContext.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == BuisnessRole).Select(item => item.SRMEnablerRule).FirstOrDefault();
            List<string> rules = enablerRule?.Split(',').Distinct().ToList();
            Dictionary<string, int> dicrule = new Dictionary<string, int>();
            if (rules != null && rules.Count > 1)
            {
                rules.ForEach(rule =>
                {
                    int count = CurrentContext.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == rule).Distinct().Select(item => item.POrg).Count();
                    dicrule.Add(rule, count);
                });
                var value = dicrule.OrderByDescending(pair => pair.Value).Take(1);
                enablerRule = value.Select(key => key.Key).First();
            }
            if (adhocActionType == AdHocActionType.NewAccount || adhocActionType == AdHocActionType.AmendmentAdd)
            {
                List<SAPSRMBusinessRoleMapping> existingDataQuery = null;
                if(adhocActionType == AdHocActionType.AmendmentAdd)
                {
                    existingDataQuery = CurrentContext.SAPSRMBusinessRoleMappings.Where(i => i.IsActive && i.BusinessRole == BuisnessRole && i.UserName == username 
                    && i.POrg == POrg && i.AccountType == accType).ToList();
                }
                var resultQuery = CurrentContext.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == enablerRule && item.POrg == POrg);
                if (existingDataQuery != null && existingDataQuery.Count> 0)
                {
                    string[] delimiter = {","};
                    var result = resultQuery.ToList();
                    foreach (var item in resultQuery)
                    {
                        if(existingDataQuery.Exists(i=> i.POrg.ToUpper() == item.POrg.ToUpper() 
                        && !string.IsNullOrWhiteSpace(i.PGrp)  
                        && !i.PGrp.Split(delimiter,StringSplitOptions.RemoveEmptyEntries).ToList().Contains(item.PGrp,StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if(!pGrp.ContainsKey(item.PGrp))
                                pGrp.Add(item.PGrp, item.PGrp);
                        }
                    }
                }
                else
                {
                    pGrp = resultQuery.Select(i => i.PGrp).Distinct().ToDictionary(key => key, value => value);
                }
            }

            else if (adhocActionType == AdHocActionType.AmendmentRemove)
            {
                List<string> ExistingPgrps = new List<string>();
                var result = new Dictionary<string, string>();
                var existingPgrp = CurrentContext.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.UserName == username
                && item.POrg == POrg && item.AccountType == accType).Select(item => item.PGrp).FirstOrDefault();
                if (existingPgrp != null && existingPgrp != string.Empty)
                {
                    ExistingPgrps = existingPgrp.Split(',').Distinct().ToList();
                    result = ExistingPgrps.ToDictionary(key => key, value => value);
                }
                return result;
            }

            return pGrp;


        }
        public List<SAPSRMBusinessRoleMapping> GetSAPSRMEndpointUsers(long requesteeuserId, AccountType acctype)
        {

            string ouOfRequestee = CurrentContext.User.Where(lq => lq.Id == requesteeuserId).Select(lq => lq.SectionCode).FirstOrDefault();
            List<long> listofUser = CurrentContext.User.Where(lq => lq.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
            var listofitems = CurrentContext.SAPSRMBusinessRoleMappings.Where(item => item.UserId != null && (item.AccountType == acctype));
            return listofitems.Where(lq => listofUser.Contains((int)(lq.UserId))).ToList();
        }

        public List<SAPSRMBusinessRoleMapping> GetSAPSRMPgrp(long requesteeuserId, AccountType acctype)
        {
            var result = CurrentContext.SAPSRMBusinessRoleMappings.Where(item => item.UserId == requesteeuserId && item.AccountType == acctype).ToList();
            return result;
        }

        public Dictionary<string, string> GetSAPSRMPorg(string BuisnessRole, AdHocActionType adhocActionType, AccountType accType, int Userid, string userName)
        {

            #region old logic
            /*   using (IACMDBContext _context = new IACMDBContext())
            {
                var sapsrmMainRole = _context.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == BuisnessRole).FirstOrDefault();
                var sapSrmEnablerMatrix = _context.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == sapsrmMainRole.SRMEnablerRule).ToList();
                var nonEligiblePorgs = new List<string>();
                if (adhocActionType == AdHocActionType.NewAccount)
                {
                    return sapSrmEnablerMatrix.DistinctBy(item => item.POrg).ToDictionary(key => key.POrg, value => value.POrg);
                }
                else if (adhocActionType == AdHocActionType.AmendmentAdd)
                {
                    sapSrmEnablerMatrix = sapSrmEnablerMatrix.DistinctBy(item => item.POrg).ToList();
                    var existingProg = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.AccountType == accType && item.UserId == Userid).Select(item => item.POrg).Distinct().ToList();
                    if (existingProg.Count() > 0)
                    {
                        nonEligiblePorgs = new List<string>();
                        foreach (var porg in existingProg)
                        {
                            var PgrpCount = 0;
                            var PgrpCountinBusniessRoleMapping = 0;

                            var Pgrps = _context.SAPSRMEnablerRoleMatrices.Where(item => item.POrg == porg).Select(item => item.PGrp).Distinct().ToList();
                            if (Pgrps != null)
                            {
                                PgrpCount = Pgrps.Distinct().Count();
                            }
                            var PgrpsinBusniessRoleMapping = _context.SAPSRMBusinessRoleMappings.Where(item => item.POrg == porg).Select(item => item.PGrp).Distinct().ToList();
                            if (PgrpsinBusniessRoleMapping != null)
                            {
                                PgrpCountinBusniessRoleMapping = PgrpsinBusniessRoleMapping.Distinct().Count();
                            }

                            if (PgrpCount != PgrpCountinBusniessRoleMapping)
                            {
                                continue;
                            }
                            else
                            {
                                nonEligiblePorgs.Add(porg);
                            }

                        }
                    }
                    var result = sapSrmEnablerMatrix.Where(item => !nonEligiblePorgs.Contains(item.POrg)).ToDictionary(key => key.POrg, value => value.POrg);
                    return result;
                }
                else if (adhocActionType == AdHocActionType.AmendmentRemove)
                {
                    sapSrmEnablerMatrix = sapSrmEnablerMatrix.DistinctBy(item => item.POrg).ToList();
                    var existingProg = _context.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.AccountType == accType && item.UserId == Userid).Select(item => item.POrg).Distinct();
                    var result = sapSrmEnablerMatrix.Where(item => existingProg.Contains(item.POrg)).DistinctBy(item => item.POrg).ToDictionary(key => key.POrg, value => value.POrg);
                    return result;
                }

                */
            #endregion


            var sapsrmMainRole = CurrentContext.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == BuisnessRole).FirstOrDefault();

            var enablerRule = CurrentContext.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == BuisnessRole).Select(item => item.SRMEnablerRule).FirstOrDefault();
            List<string> rules = enablerRule != null ? enablerRule.Split(',').Distinct().ToList(): new List<string>();
            Dictionary<string, int> dicrule = new Dictionary<string, int>();
            if (rules.Count > 1)
            {
                rules.ForEach(rule =>
                {
                    int count = CurrentContext.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == rule).Distinct().Select(item => item.POrg).Count();
                    dicrule.Add(rule, count);
                });
                var value = dicrule.OrderByDescending(pair => pair.Value).Take(1);
                enablerRule = value.Select(key => key.Key).First();
            }
            if (adhocActionType == AdHocActionType.NewAccount || adhocActionType == AdHocActionType.AmendmentAdd)
            {
                var data = CurrentContext.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == enablerRule && item.POrg != string.Empty && item.PGrp != string.Empty).Select(item => item.POrg).Distinct().ToDictionary(key => key, value => value);
                return data;
            }
            else if (adhocActionType == AdHocActionType.AmendmentRemove)
            {
                var existingProg = CurrentContext.SAPSRMBusinessRoleMappings.Where(item => item.BusinessRole == BuisnessRole && item.AccountType == accType && item.UserName == userName).Select(item => item.POrg).Distinct();
                var result = CurrentContext.SAPSRMEnablerRoleMatrices.Where(item => item.SRMEnablerRule == enablerRule && existingProg.Contains(item.POrg)).Select(item => item.POrg).Distinct().ToDictionary(key => key, value => value);
                return result;
            }

            return null;
        }

        public Dictionary<string, string> GetSAPSRMPorgs(string role)
        {
            var businessrole = CurrentContext.SAPSRMMainRoleMatrices.Where(item => item.BusinessRole == role).FirstOrDefault();
            if (businessrole != null && !string.IsNullOrWhiteSpace(businessrole.SRMEnablerRule))
            {
                var str = businessrole.SRMEnablerRule.Split(new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                str.ForEach(i=>
                {
                    i = i.Trim();
                });
                var enablerrole = CurrentContext.SAPSRMEnablerRoleMatrices.Where(item => str.Contains(item.SRMEnablerRule));
                if (enablerrole != null)
                {
                    return enablerrole.Select(item => item.POrg).Distinct().ToDictionary(key => key, value => value);
                }
            }

            return new Dictionary<string, string>();

        }




        public   bool IsAdGroupsExist(string RoleName, AdHocActionType actionType, AccountType accountType, long DIUserID, string AccountName)
        {

            var result = Get(item => item.BusinessRole == RoleName).FirstOrDefault();

            if (result == null || string.IsNullOrWhiteSpace(result.ADGroups))
            { return true;
            }
            else
            {
                var ep = this.CurrentContext.EndPoint.Where(e => e.ApplicationType == ApplicationType.ADSingaporePowerLocal).FirstOrDefault();
                var returnValue = false;

                var accountReconQuery = CurrentContext.AccountReconcilations.Where(accountRecon =>
                    accountRecon.EndpointSystemId == ep.Id && accountRecon.AccountType == accountType && !accountRecon.IsLocked && !accountRecon.IsDeleted).AsQueryable();

                if (accountType == AccountType.Service || accountType == AccountType.Shared)
                {
                    var UserIds = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                              .Where(bothuser => bothuser.currentUser.Id == DIUserID).Select(bothuser => bothuser.UserList.Id).ToList();

                    accountReconQuery = accountReconQuery.Where(a => a.AccountName.Equals(AccountName, StringComparison.CurrentCultureIgnoreCase) && UserIds.Contains(a.UserId.Value)).Select(a => a);
                }
                else
                {
                    accountReconQuery = accountReconQuery.Where(a => a.UserId.Value == DIUserID).Select(a => a);

                    if (accountType == AccountType.Secondary)
                    {
                        accountReconQuery = accountReconQuery.Where(a => a.AccountName.Equals(AccountName, StringComparison.CurrentCultureIgnoreCase)).Select(a => a);
                    }

                }

                var accountRecons = accountReconQuery.ToList();

                if(accountRecons!=null)
                {
                    returnValue = accountRecons.Count() > 0;
                }

                return returnValue;
            }
        }
    }

}
