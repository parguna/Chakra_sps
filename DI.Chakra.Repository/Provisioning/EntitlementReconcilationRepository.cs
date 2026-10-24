using DI.Chakra.EntityModel; 
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class EntitlementReconcilationRepository : RepositoryBase<EntitlementReconcilation>
    {
        public EntitlementReconcilationRepository(DataAccessTransaction transaction) : base(transaction) { }


        public List<PresentationEntitlementReconcilation> GetAll()
        {

            return (from recon in CurrentContext.EntitlementReconcilations
                    join endpoint in CurrentContext.EndPoint on recon.EndpointSystemId equals endpoint.Id
                    where recon.IsResolved
                    select new PresentationEntitlementReconcilation
                    {
                        AccountName = recon.AccountName,
                        Comments = recon.Comments,
                        EndpointSystemName = endpoint.DisplayName,
                        EndpointSystemType = endpoint.EndPointSystemType.DisplayName,
                        EnteredDate = recon.EnteredDate,
                        ResolvedBy = recon.ResolvedBy,
                        ExceptionType = recon.ExceptionType,
                        Entitlement = recon.Entitlement,
                        EntitlementType = (EntitlementType)recon.EntitlementType
                    }).ToList();

        }

        public List<AccountEntitlementRecon> GetAccountEntitlementReconsByUserId(long AccountReconId, long? userId, List<long> endpointIds, AccountType accountType)
        {
            var list1 = this.CurrentContext.AccountReconcilations.Where(acc => acc.Id == AccountReconId).Select(c=>c).ToList();

            var list2 = this.CurrentContext.AccountReconcilations.Where(acc => acc.UserId == userId &&
                                                         acc.AccountType == accountType && !acc.IsDeleted && !acc.IsLocked &&
                                                         endpointIds.Contains(acc.EndpointSystemId)).Select(c => c).ToList();

            
                                          
            List<AccountReconcilation> accounts = list1.Union(list2).Distinct().ToList();

            var ids = accounts.Select(a => a.Id).ToList();

            var accountRecon = this.CurrentContext.EntitlementReconcilations.Where(
                                                  entacc => ids.Contains(entacc.AccountReconcilationId))
                                                 .Select(acc => acc);

            var list = new List<AccountEntitlementRecon>();

            accounts.ForEach(acc =>
            {
                list.Add(new AccountEntitlementRecon() { AccountRecon = acc, Entitlement = accountRecon.Where(ent => ent.AccountReconcilationId == acc.Id).Select(ent => ent).ToList() });
            });

            return list;

        }

        public List<AccountEntitlementRecon> GetAccountEntitlementRecons(long AccountReconId, long userId, List<long> endpointIds, AccountType accountType)
        {
            //var query = this.CurrentContext.AccountReconcilations.Where(acc => (AccountReconId== 0 ||  acc.Id == AccountReconId) && (userId==0 || acc.UserId == userId))
            //                      .Join(this.CurrentContext.AccountReconcilations, first => first.UserId, second => second.UserId, (first, second) => new { First = first, Second = second })
            //                      .Where(firstsecond => (firstsecond.Second.UserId == userId) &&
            //                                             firstsecond.Second.AccountType == accountType && !firstsecond.Second.IsDeleted && !firstsecond.Second.IsLocked &&
            //                                             endpointIds.Contains(firstsecond.Second.EndpointSystemId)
            //                             );


            var accountReconQuery = CurrentContext.AccountReconcilations.Where(ac =>
                     endpointIds.Contains(ac.EndpointSystemId) && ac.AccountType == accountType  ).AsQueryable();

            if (accountType == AccountType.Service || accountType == AccountType.Shared)
            {
                var UserIds = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                          .Where(bothuser => bothuser.currentUser.Id == userId).Select(bothuser => bothuser.UserList.Id).ToList();

                accountReconQuery = accountReconQuery.Where(a => UserIds.Contains(a.UserId.Value)).Select(a => a);
            }
            else
            {
                accountReconQuery = accountReconQuery.Where(a => a.UserId.Value == userId).Select(a => a);
            }


            List<AccountReconcilation> accounts = accountReconQuery.Select(firstSecond => firstSecond).ToList();

            var ids = accounts.Select(a => a.Id).ToList();

            var accountRecon = this.CurrentContext.EntitlementReconcilations.Where(
                                                  entacc => ids.Contains(entacc.AccountReconcilationId))
                                                 .Select(acc => acc);

            var list = new List<AccountEntitlementRecon>();

            accounts.ForEach(acc =>
            {
                list.Add(new AccountEntitlementRecon() { AccountRecon = acc, Entitlement = accountRecon.Where(ent => ent.AccountReconcilationId == acc.Id).Select(ent => ent).ToList() });
            });
             
           return list;

        }

        //public List<EntitlementReconcilation> GetActiveDirectory(long epID)
        //{
        //    return (from adsUser in CurrentContext.ADSUserInfos
        //            from adsGroup in adsUser.ADSGroups
        //            where adsUser.EndPointSystemId == epID
        //            select new
        //            {
        //                EndpointSystemId = epID,
        //                EndpointUserId = adsUser.Id,
        //                EnteredDate = DateTime.Now,
        //                Entitlement = adsGroup.name,
        //                EntitlementType = EntitlementType.Group,
        //                UserId = adsUser.DIUserId,
        //                Version = adsGroup.Version.Value,
        //                AccountName = adsUser.sAMAccountName
        //            }).ToList()
        //                       .Select(Reconcil => new EntitlementReconcilation
        //                       {
        //                           EndpointSystemId = Reconcil.EndpointSystemId,
        //                           EndpointUserId = Reconcil.EndpointUserId,
        //                           EnteredDate = Reconcil.EnteredDate,
        //                           Entitlement = Reconcil.Entitlement,
        //                           EntitlementType = (int)Reconcil.EntitlementType,
        //                           UserId = Reconcil.UserId,
        //                           AccountName = Reconcil.AccountName
        //                       }).ToList();
        //}

        //public List<EntitlementReconcilation> GetLNS(long epID)
        //{
        //    return (from lnsUser in CurrentContext.LotusUserInfos
        //            from lnsGroup in lnsUser.LotusGroupInfos
        //            where lnsUser.EndPointSystemId == epID
        //            select new
        //            {
        //                EndpointSystemId = epID,
        //                EndpointUserId = lnsUser.Id,
        //                EnteredDate = DateTime.Now,
        //                Entitlement = lnsGroup.Group,
        //                EntitlementType = EntitlementType.Group,
        //                UserId = lnsUser.DIUserId,
        //                Version = lnsGroup.Version.Value,
        //                AccountName = lnsUser.InternetAddress
        //            }).ToList()
        //                  .Select(Reconcil => new EntitlementReconcilation
        //                  {
        //                      EndpointSystemId = Reconcil.EndpointSystemId,
        //                      EndpointUserId = Reconcil.EndpointUserId,
        //                      EnteredDate = Reconcil.EnteredDate,
        //                      Entitlement = Reconcil.Entitlement,
        //                      EntitlementType = (int)Reconcil.EntitlementType,
        //                      UserId = Reconcil.UserId,
        //                      AccountName = Reconcil.AccountName
        //                  }).ToList();
        //}

        //public List<EntitlementReconcilation> GetSAP(long epID)
        //{
        //    var RolesList = (from sapUser in CurrentContext.SAPUserinfos
        //                     from sapRole in sapUser.SAPRoleInfo
        //                     where sapUser.EndPointSystemId == epID
        //                     select new
        //                     {
        //                         EndpointSystemId = epID,
        //                         EndpointUserId = sapUser.Id,
        //                         EnteredDate = DateTime.Now,
        //                         Entitlement = sapRole.ROLE_CODE,
        //                         EntitlementType = EntitlementType.Role,
        //                         UserId = sapUser.DIUserId,
        //                         Version = sapRole.Version.Value,
        //                         AccountName = sapUser.UserLogonName
        //                     }).ToList()
        //                                   .Select(Reconcil => new EntitlementReconcilation
        //                                   {
        //                                       EndpointSystemId = Reconcil.EndpointSystemId,
        //                                       EndpointUserId = Reconcil.EndpointUserId,
        //                                       EnteredDate = Reconcil.EnteredDate,
        //                                       Entitlement = Reconcil.Entitlement,
        //                                       EntitlementType = (int)Reconcil.EntitlementType,
        //                                       UserId = Reconcil.UserId,
        //                                       AccountName = Reconcil.AccountName
        //                                   }).ToList();


        //    var positions = (from sapUser in CurrentContext.SAPUserinfos
        //                     from sapPosition in CurrentContext.SAPHRP1001Entries
        //                     where sapUser.EndPointSystemId == epID &&
        //                     sapPosition.EndpointSystemId == epID &&
        //                     sapPosition.RelatedObjectCode == sapUser.UserLogonName &&
        //                     sapPosition.RelatedObjectType == "US"
        //                     select new
        //                     {
        //                         EndpointSystemId = epID,
        //                         EndpointUserId = sapUser.Id,
        //                         EnteredDate = DateTime.Now,
        //                         Entitlement = sapPosition.ActualObjectCode,
        //                         EntitlementType = (int)EntitlementType.Position,
        //                         UserId = sapUser.DIUserId,
        //                         sapUser.Version,
        //                         AccountName = sapUser.UserLogonName
        //                     }).ToList()
        //                    .Select(Reconcil => new EntitlementReconcilation
        //                    {
        //                        EndpointSystemId = Reconcil.EndpointSystemId,
        //                        EndpointUserId = Reconcil.EndpointUserId,
        //                        EnteredDate = Reconcil.EnteredDate,
        //                        Entitlement = Reconcil.Entitlement,
        //                        EntitlementType = Reconcil.EntitlementType,
        //                        UserId = Reconcil.UserId,
        //                        AccountName = Reconcil.AccountName
        //                    }).ToList();

        //    return RolesList.Union(positions).ToList();
        //}

        //public List<EntitlementReconcilation> GetSAPEP(long epID)
        //{
        //    var groups = (from sapepUser in CurrentContext.SAPEPUserInfos
        //                  from sapepGroup in sapepUser.SAPEPGroups
        //                  where sapepUser.EndPointSystemId == epID
        //                  select new
        //                  {
        //                      EndpointSystemId = epID,
        //                      EndpointUserId = sapepUser.Id,
        //                      EnteredDate = DateTime.Now,
        //                      Entitlement = sapepGroup.GroupUniqueName,
        //                      EntitlementType = EntitlementType.Group,
        //                      UserId = sapepUser.DIUserId,
        //                      sapepUser.Version,
        //                      AccountName = sapepUser.UserName
        //                  }).ToList()
        //                      .Select(Reconcil => new EntitlementReconcilation
        //                      {
        //                          EndpointSystemId = Reconcil.EndpointSystemId,
        //                          EndpointUserId = Reconcil.EndpointUserId,
        //                          EnteredDate = Reconcil.EnteredDate,
        //                          Entitlement = Reconcil.Entitlement,
        //                          EntitlementType = (int)Reconcil.EntitlementType,
        //                          UserId = Reconcil.UserId,
        //                          AccountName = Reconcil.AccountName
        //                      }).ToList();

        //    var roles = (from sapepUser in CurrentContext.SAPEPUserInfos
        //                 from sapepRole in sapepUser.SAPEPRoles
        //                 where sapepUser.EndPointSystemId == epID
        //                 select new
        //                 {
        //                     EndpointSystemId = epID,
        //                     EndpointUserId = sapepUser.Id,
        //                     EnteredDate = DateTime.Now,
        //                     Entitlement = sapepRole.UniqueRoleName,
        //                     EntitlementType = EntitlementType.Role,
        //                     UserId = sapepUser.DIUserId,
        //                     sapepUser.Version,
        //                     AccountName = sapepUser.UserName
        //                 }).ToList()
        //        .Select(Reconcil => new EntitlementReconcilation
        //        {
        //            EndpointSystemId = Reconcil.EndpointSystemId,
        //            EndpointUserId = Reconcil.EndpointUserId,
        //            EnteredDate = Reconcil.EnteredDate,
        //            Entitlement = Reconcil.Entitlement,
        //            EntitlementType = (int)Reconcil.EntitlementType,
        //            UserId = Reconcil.UserId,
        //            AccountName = Reconcil.AccountName
        //        }).ToList();

        //    return roles.Union(groups).ToList();
        //}

        //public List<EntitlementReconcilation> GetInHouse(long epID)
        //{
        //    return (from inhouseUser in CurrentContext.InhouseUserInfos
        //            where inhouseUser.EndPointSystemId == epID
        //            select new
        //            {
        //                EndpointSystemId = epID,
        //                EndpointUserId = inhouseUser.Id,
        //                EnteredDate = DateTime.Now,
        //                Entitlement = inhouseUser.Permission,
        //                EntitlementType = EntitlementType.Role,
        //                UserId = inhouseUser.DIUserId,
        //                inhouseUser.Version,
        //                AccountName = inhouseUser.UserName
        //            }).ToList()
        //                      .Select(Reconcil => new EntitlementReconcilation
        //                      {
        //                          EndpointSystemId = Reconcil.EndpointSystemId,
        //                          EndpointUserId = Reconcil.EndpointUserId,
        //                          EnteredDate = Reconcil.EnteredDate,
        //                          Entitlement = Reconcil.Entitlement,
        //                          EntitlementType = (int)Reconcil.EntitlementType,
        //                          UserId = Reconcil.UserId,
        //                          AccountName = Reconcil.AccountName
        //                      }).ToList();
        //}
        //public List<EntitlementReconcilation> GetLinux(long epID)
        //{
        //    return (from linuxUser in CurrentContext.LinuxUserInfos
        //            from linuxGroup in linuxUser.LinuxGroup
        //            where linuxUser.EndPointSystemId == epID
        //            select new
        //            {
        //                EndpointSystemId = epID,
        //                EndpointUserId = linuxUser.Id,
        //                EnteredDate = DateTime.Now,
        //                Entitlement = linuxGroup.GroupName,
        //                EntitlementType = EntitlementType.Group,
        //                UserId = linuxUser.DIUserId,
        //                Version = linuxGroup.Version.Value,
        //                AccountName = linuxUser.UserName
        //            }).ToList()
        //                  .Select(Reconcil => new EntitlementReconcilation
        //                  {
        //                      EndpointSystemId = Reconcil.EndpointSystemId,
        //                      EndpointUserId = Reconcil.EndpointUserId,
        //                      EnteredDate = Reconcil.EnteredDate,
        //                      Entitlement = Reconcil.Entitlement,
        //                      EntitlementType = (int)Reconcil.EntitlementType,
        //                      UserId = Reconcil.UserId,
        //                      AccountName = Reconcil.AccountName
        //                  }).ToList();
        //}


        //public List<EntitlementReconcilation> GetVPN(long epID)
        //{
        //    return null;
        //}

        public List<PresentationEntitlementReconcilationReport> GetAddedExceptionActiveDirectory(long epId)
        {
            return (from entitlementReconcile in CurrentContext.EntitlementReconcilations
                    join usr in CurrentContext.User
                        on entitlementReconcile.UserId equals usr.Id into detail
                    from usr in detail.DefaultIfEmpty()
                    where entitlementReconcile.EndpointSystemId == epId && entitlementReconcile.IsDeleted == false &&
                    CurrentContext.ADSUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                        !item.ADSGroups.Any(item1 => item1.name == entitlementReconcile.Entitlement))
                    select new PresentationEntitlementReconcilationReport
                    {
                        AccountName = entitlementReconcile.AccountName,
                        DisplayName = usr.KnownAs,
                        EndpointSystemId = epId,
                        EndpointUserId = entitlementReconcile.EndpointUserId,
                        CreatedDate = entitlementReconcile.EnteredDate,
                        UserId = entitlementReconcile.UserId,
                        Entitlement = entitlementReconcile.Entitlement,
                        EntitlementType = entitlementReconcile.EntitlementType
                    }).ToList();
        }

        public List<PresentationEntitlementReconcilationReport> GetAddedExceptionSAP(long epId)
        {
            var roles = (from endPointUser in CurrentContext.SAPUserinfos
                         from sapRoles in endPointUser.SAPRoleInfo
                         where endPointUser.EndPointSystemId == epId &&
                               CurrentContext.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapRoles.ROLE_CODE) == null
                         select new PresentationEntitlementReconcilationReport
                         {
                             AccountName = endPointUser.UserLogonName,
                             DisplayName = endPointUser.Firstname + " " + endPointUser.Lastname,
                             EndpointSystemId = epId,
                             EndpointUserId = endPointUser.Id,
                             CreatedDate = endPointUser.CreatedDate,
                             Entitlement = sapRoles.ROLE_CODE,
                             EntitlementType = (int)EntitlementType.Role,
                             ExceptionType = ExceptionTypes.Added
                         }).ToList();

            var positions = (from endPointUser in CurrentContext.SAPUserinfos
                             join sapPositions in CurrentContext.SAPHRP1001Entries
                                 on new { p1 = endPointUser.UserLogonName, p2 = endPointUser.EndPointSystemId } equals
                                 new { p1 = sapPositions.RelatedObjectCode, p2 = sapPositions.EndpointSystemId }
                             where endPointUser.EndPointSystemId == epId && sapPositions.RelatedObjectType == "US" &&
                                   CurrentContext.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id &&
                                                                                                             entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapPositions.ActualObjectCode) == null
                             select new PresentationEntitlementReconcilationReport
                             {
                                 AccountName = endPointUser.UserLogonName,
                                 DisplayName = endPointUser.Firstname + " " + endPointUser.Lastname,
                                 EndpointSystemId = epId,
                                 EndpointUserId = endPointUser.Id,
                                 CreatedDate = endPointUser.CreatedDate,
                                 Entitlement = sapPositions.ActualObjectCode,
                                 EntitlementType = (int)EntitlementType.Position,
                                 ExceptionType = ExceptionTypes.Added
                             }).ToList();

            return roles.Union(positions).ToList();
        }

        public List<PresentationEntitlementReconcilationReport> GetAddedExceptionSAPEP(long epId)
        {
            var groups = (from endPointUser in CurrentContext.SAPEPUserInfos
                          from sapepGroups in endPointUser.SAPEPGroups
                          where endPointUser.EndPointSystemId == epId && sapepGroups.IsXLUploaded &&
                                CurrentContext.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapepGroups.GroupUniqueName) == null
                          select new PresentationEntitlementReconcilationReport
                          {
                              AccountName = endPointUser.UserName,
                              DisplayName = endPointUser.DisplayName,
                              EndpointSystemId = epId,
                              EndpointUserId = endPointUser.Id,
                              CreatedDate = endPointUser.CreatedDate,
                              Entitlement = sapepGroups.GroupUniqueName,
                              EntitlementType = (int)EntitlementType.Group,
                              ExceptionType = ExceptionTypes.Added
                          }).ToList();

            var roles = (from endPointUser in CurrentContext.SAPEPUserInfos
                         from sapepRoles in endPointUser.SAPEPRoles
                         where endPointUser.EndPointSystemId == epId && sapepRoles.IsXLUploaded &&
                               CurrentContext.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapepRoles.UniqueRoleName) == null
                         select new PresentationEntitlementReconcilationReport
                         {
                             AccountName = endPointUser.UserName,
                             DisplayName = endPointUser.DisplayName,
                             EndpointSystemId = epId,
                             EndpointUserId = endPointUser.Id,
                             CreatedDate = endPointUser.CreatedDate,
                             Entitlement = sapepRoles.UniqueRoleName,
                             EntitlementType = (int)EntitlementType.Role,
                             ExceptionType = ExceptionTypes.Added
                         }).ToList();

            return roles.Union(groups).ToList();
        }

        public List<PresentationEntitlementReconcilationReport> GetRemovedExceptionActiveDirectory(long epId)
        {
            return (from entitlementReconcile in CurrentContext.EntitlementReconcilations
                    join usr in CurrentContext.User
                        on entitlementReconcile.UserId equals usr.Id into detail
                    from usr in detail.DefaultIfEmpty()
                    where entitlementReconcile.EndpointSystemId == epId && entitlementReconcile.IsDeleted == false &&
                    CurrentContext.ADSUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                        !item.ADSGroups.Any(item1 => item1.name == entitlementReconcile.Entitlement))
                    select new PresentationEntitlementReconcilationReport
                    {
                        AccountName = entitlementReconcile.AccountName,
                        DisplayName = usr.KnownAs,
                        EndpointSystemId = epId,
                        EndpointUserId = entitlementReconcile.EndpointUserId,
                        CreatedDate = entitlementReconcile.EnteredDate,
                        UserId = entitlementReconcile.UserId,
                        Entitlement = entitlementReconcile.Entitlement,
                        EntitlementType = entitlementReconcile.EntitlementType
                    }).ToList();
        }

        public List<PresentationEntitlementReconcilationReport> GetRemovedExceptionSAP(long epId)
        {
            var roles = (from entitlementReconcile in CurrentContext.EntitlementReconcilations
                         join usr in CurrentContext.User
                             on entitlementReconcile.UserId equals usr.Id into detail
                         from usr in detail.DefaultIfEmpty()
                         where entitlementReconcile.EndpointSystemId == epId && entitlementReconcile.IsDeleted == false &&
                               entitlementReconcile.EntitlementType == (int)EntitlementType.Role &&
                               CurrentContext.SAPUserinfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                                                 !item.SAPRoleInfo.Any(item1 => item1.ROLE_CODE == entitlementReconcile.Entitlement))
                         select new PresentationEntitlementReconcilationReport
                         {
                             AccountName = entitlementReconcile.AccountName,
                             DisplayName = usr.KnownAs,
                             EndpointSystemId = epId,
                             EndpointUserId = entitlementReconcile.EndpointUserId,
                             CreatedDate = entitlementReconcile.EnteredDate,
                             UserId = entitlementReconcile.UserId,
                             Entitlement = entitlementReconcile.Entitlement,
                             EntitlementType = entitlementReconcile.EntitlementType,
                             ExceptionType = ExceptionTypes.Removed
                         }).ToList();

            var positions = (from entitlementReconcile in CurrentContext.EntitlementReconcilations
                             join usr in CurrentContext.User
                                 on entitlementReconcile.UserId equals usr.Id into detail
                             from usr in detail.DefaultIfEmpty()
                             where entitlementReconcile.EndpointSystemId == epId && entitlementReconcile.IsDeleted == false &&
                                   entitlementReconcile.EntitlementType == (int)EntitlementType.Position &&
                                   !CurrentContext.SAPHRP1001Entries.Any(item => item.RelatedObjectCode == entitlementReconcile.AccountName &&
                                                                           item.RelatedObjectType == "US" && item.EndpointSystemId == entitlementReconcile.EndpointSystemId &&
                                                                           item.ActualObjectCode == entitlementReconcile.Entitlement)
                             select new PresentationEntitlementReconcilationReport
                             {
                                 AccountName = entitlementReconcile.AccountName,
                                 DisplayName = usr.KnownAs,
                                 EndpointSystemId = epId,
                                 EndpointUserId = entitlementReconcile.EndpointUserId,
                                 CreatedDate = entitlementReconcile.EnteredDate,
                                 UserId = entitlementReconcile.UserId,
                                 Entitlement = entitlementReconcile.Entitlement,
                                 EntitlementType = entitlementReconcile.EntitlementType,
                                 ExceptionType = ExceptionTypes.Removed
                             }).ToList();

            return roles.Union(positions).ToList();
        }

        public List<PresentationEntitlementReconcilationReport> GetRemovedExceptionSAPEP(long epId)
        {
            var groups = (from entitlementReconcile in CurrentContext.EntitlementReconcilations
                          join usr in CurrentContext.User
                              on entitlementReconcile.UserId equals usr.Id into detail
                          from usr in detail.DefaultIfEmpty()
                          where entitlementReconcile.EndpointSystemId == epId && entitlementReconcile.IsDeleted == false &&
                                entitlementReconcile.EntitlementType == (int)EntitlementType.Group &&
                                CurrentContext.SAPEPUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                                                    !item.SAPEPGroups.Any(item1 => item1.GroupUniqueName == entitlementReconcile.Entitlement))
                          select new PresentationEntitlementReconcilationReport
                          {
                              AccountName = entitlementReconcile.AccountName,
                              DisplayName = usr.KnownAs,
                              EndpointSystemId = epId,
                              EndpointUserId = entitlementReconcile.EndpointUserId,
                              CreatedDate = entitlementReconcile.EnteredDate,
                              UserId = entitlementReconcile.UserId,
                              Entitlement = entitlementReconcile.Entitlement,
                              EntitlementType = entitlementReconcile.EntitlementType,
                              ExceptionType = ExceptionTypes.Removed
                          }).ToList();

            var roles = (from entitlementReconcile in CurrentContext.EntitlementReconcilations
                         join usr in CurrentContext.User
                             on entitlementReconcile.UserId equals usr.Id into detail
                         from usr in detail.DefaultIfEmpty()
                         where entitlementReconcile.EndpointSystemId == epId && entitlementReconcile.IsDeleted == false &&
                               entitlementReconcile.EntitlementType == (int)EntitlementType.Role &&
                               CurrentContext.SAPEPUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                                                   !item.SAPEPRoles.Any(item1 => item1.UniqueRoleName == entitlementReconcile.Entitlement))
                         select new PresentationEntitlementReconcilationReport
                         {
                             AccountName = entitlementReconcile.AccountName,
                             DisplayName = usr.KnownAs,
                             EndpointSystemId = epId,
                             EndpointUserId = entitlementReconcile.EndpointUserId,
                             CreatedDate = entitlementReconcile.EnteredDate,
                             UserId = entitlementReconcile.UserId,
                             Entitlement = entitlementReconcile.Entitlement,
                             EntitlementType = entitlementReconcile.EntitlementType,
                             ExceptionType = ExceptionTypes.Removed
                         }).ToList();

            return roles.Union(groups).ToList();
        }

        public void FlushEntitlementReconcilation(List<DI.Chakra.EntityModel.EntitlementReconcilation> entitlementReconcilations) 
        {
            string tableName = "DI_M_EntitlementReconcilation";
            SQLBulkUploader<EntitlementReconcilation> sqlEntitlmentReconcilation = new SQLBulkUploader<EntitlementReconcilation>(tableName);
            sqlEntitlmentReconcilation.Flush(entitlementReconcilations);

        }

        //public List<EntitlementReconcilation> GetEntitilementReconcilationListForEndpoint(EndPoint _endpoint)
        //{
        //    List<EntitlementReconcilation> result = new List<EntitlementReconcilation>();

        //            if (_endpoint.EndpointSystemTypeId == (int)EndpointType.SAP)
        //            {
        //                var RolesList = (from sapUser in CurrentContext.SAPUserinfos
        //                                 from sapRole in sapUser.SAPRoleInfo
        //                                 where sapUser.EndPointSystemId == _endpoint.Id
        //                                 select new
        //                                 {
        //                                     EndpointSystemId = _endpoint.Id,
        //                                     EndpointUserId = sapUser.Id,
        //                                     EnteredDate = DateTime.Now,
        //                                     Entitlement = sapRole.ROLE_CODE,
        //                                     EntitlementType = EntitlementType.Role,
        //                                     UserId = sapUser.DIUserId,
        //                                     Version = sapRole.Version.Value,
        //                                     AccountName = sapUser.UserLogonName
        //                                 }).ToList()
        //                                .Select(Reconcil => new EntitlementReconcilation
        //                                {
        //                                    EndpointSystemId = Reconcil.EndpointSystemId,
        //                                    EndpointUserId = Reconcil.EndpointUserId,
        //                                    EnteredDate = Reconcil.EnteredDate,
        //                                    Entitlement = Reconcil.Entitlement,
        //                                    EntitlementType = (int)Reconcil.EntitlementType,
        //                                    UserId = Reconcil.UserId,
        //                                    AccountName = Reconcil.AccountName
        //                                }).ToList();


        //                var positions = (from sapUser in CurrentContext.SAPUserinfos
        //                                 from sapPosition in CurrentContext.SAPHRP1001Entries
        //                                 where sapUser.EndPointSystemId == _endpoint.Id &&
        //                                 sapPosition.EndpointSystemId == _endpoint.Id &&
        //                                 sapPosition.RelatedObjectCode == sapUser.UserLogonName &&
        //                                 sapPosition.RelatedObjectType == "US"
        //                                 select new
        //                                 {
        //                                     EndpointSystemId = _endpoint.Id,
        //                                     EndpointUserId = sapUser.Id,
        //                                     EnteredDate = DateTime.Now,
        //                                     Entitlement = sapPosition.ActualObjectCode,
        //                                     EntitlementType = (int)EntitlementType.Position,
        //                                     UserId = sapUser.DIUserId,
        //                                     sapUser.Version,
        //                                     AccountName = sapUser.UserLogonName
        //                                 }).ToList()
        //                                .Select(Reconcil => new EntitlementReconcilation
        //                                {
        //                                    EndpointSystemId = Reconcil.EndpointSystemId,
        //                                    EndpointUserId = Reconcil.EndpointUserId,
        //                                    EnteredDate = Reconcil.EnteredDate,
        //                                    Entitlement = Reconcil.Entitlement,
        //                                    EntitlementType = Reconcil.EntitlementType,
        //                                    UserId = Reconcil.UserId,
        //                                    AccountName = Reconcil.AccountName
        //                                }).ToList();

        //                result= RolesList.Union(positions).ToList();
        //            }
        //            if (_endpoint.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
        //            {
        //               result=(from adsUser in CurrentContext.ADSUserInfos
        //                        from adsGroup in adsUser.ADSGroups
        //                        where adsUser.EndPointSystemId == _endpoint.Id
        //                        select new
        //                        {
        //                            EndpointSystemId = _endpoint.Id,
        //                            EndpointUserId = adsUser.Id,
        //                            EnteredDate = DateTime.Now,
        //                            Entitlement = adsGroup.name,
        //                            EntitlementType = EntitlementType.Group,
        //                            UserId = adsUser.DIUserId,
        //                            Version = adsGroup.Version.Value,
        //                            AccountName = adsUser.sAMAccountName
        //                        }).ToList()
        //                    .Select(Reconcil => new EntitlementReconcilation
        //                    {
        //                        EndpointSystemId = Reconcil.EndpointSystemId,
        //                        EndpointUserId = Reconcil.EndpointUserId,
        //                        EnteredDate = Reconcil.EnteredDate,
        //                        Entitlement = Reconcil.Entitlement,
        //                        EntitlementType = (int)Reconcil.EntitlementType,
        //                        UserId = Reconcil.UserId,
        //                        AccountName = Reconcil.AccountName
        //                    }).ToList();

        //            }
        //            if (_endpoint.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
        //            {
        //             result=(from lnsUser in CurrentContext.LotusUserInfos
        //                        from lnsGroup in lnsUser.LotusGroupInfos
        //                        where lnsUser.EndPointSystemId == _endpoint.Id
        //                        select new
        //                        {
        //                            EndpointSystemId = _endpoint.Id,
        //                            EndpointUserId = lnsUser.Id,
        //                            EnteredDate = DateTime.Now,
        //                            Entitlement = lnsGroup.Group,
        //                            EntitlementType = EntitlementType.Group,
        //                            UserId = lnsUser.DIUserId,
        //                            Version = lnsGroup.Version.Value,
        //                            AccountName = lnsUser.InternetAddress
        //                        }).ToList()
        //                    .Select(Reconcil => new EntitlementReconcilation
        //                    {
        //                        EndpointSystemId = Reconcil.EndpointSystemId,
        //                        EndpointUserId = Reconcil.EndpointUserId,
        //                        EnteredDate = Reconcil.EnteredDate,
        //                        Entitlement = Reconcil.Entitlement,
        //                        EntitlementType = (int)Reconcil.EntitlementType,
        //                        UserId = Reconcil.UserId,
        //                        AccountName = Reconcil.AccountName
        //                    }).ToList();

        //            }
        //            if (_endpoint.EndpointSystemTypeId == (int)EndpointType.InhouseSystems)
        //            {
        //        result=(from inhouseUser in CurrentContext.InhouseUserInfos
        //                        where inhouseUser.EndPointSystemId == _endpoint.Id
        //                        select new
        //                        {
        //                            EndpointSystemId = _endpoint.Id,
        //                            EndpointUserId = inhouseUser.Id,
        //                            EnteredDate = DateTime.Now,
        //                            Entitlement = inhouseUser.Permission,
        //                            EntitlementType = EntitlementType.Role,
        //                            UserId = inhouseUser.DIUserId,
        //                            inhouseUser.Version,
        //                            AccountName = inhouseUser.UserName
        //                        }).ToList()
        //                    .Select(Reconcil => new EntitlementReconcilation
        //                    {
        //                        EndpointSystemId = Reconcil.EndpointSystemId,
        //                        EndpointUserId = Reconcil.EndpointUserId,
        //                        EnteredDate = Reconcil.EnteredDate,
        //                        Entitlement = Reconcil.Entitlement,
        //                        EntitlementType = (int)Reconcil.EntitlementType,
        //                        UserId = Reconcil.UserId,
        //                        AccountName = Reconcil.AccountName
        //                    }).ToList();
        //            }

        //    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.Linux)
        //    {
        //        result = (from linuxUser in CurrentContext.LinuxUserInfos
        //                  where linuxUser.EndPointSystemId == _endpoint.Id
        //                  select new
        //                  {
        //                      EndpointSystemId = _endpoint.Id,
        //                      EndpointUserId = linuxUser.Id,
        //                      EnteredDate = DateTime.Now,
        //                      Entitlement = linuxUser.UserName,
        //                      EntitlementType = EntitlementType.Role,
        //                      UserId = linuxUser.DIUserId,
        //                      linuxUser.Version,
        //                      AccountName = linuxUser.UserName
        //                  }).ToList()
        //                    .Select(Reconcil => new EntitlementReconcilation
        //                    {
        //                        EndpointSystemId = Reconcil.EndpointSystemId,
        //                        EndpointUserId = Reconcil.EndpointUserId,
        //                        EnteredDate = Reconcil.EnteredDate,
        //                        Entitlement = Reconcil.Entitlement,
        //                        EntitlementType = (int)Reconcil.EntitlementType,
        //                        UserId = Reconcil.UserId,
        //                        AccountName = Reconcil.AccountName
        //                    }).ToList();
        //    }

        //    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
        //            {
        //                var groups = (from sapepUser in CurrentContext.SAPEPUserInfos
        //                              from sapepGroup in sapepUser.SAPEPGroups
        //                              where sapepUser.EndPointSystemId == _endpoint.Id
        //                              select new
        //                              {
        //                                  EndpointSystemId = _endpoint.Id,
        //                                  EndpointUserId = sapepUser.Id,
        //                                  EnteredDate = DateTime.Now,
        //                                  Entitlement = sapepGroup.GroupUniqueName,
        //                                  EntitlementType = EntitlementType.Group,
        //                                  UserId = sapepUser.DIUserId,
        //                                  sapepUser.Version,
        //                                  AccountName = sapepUser.UserName
        //                              }).ToList()
        //                    .Select(Reconcil => new EntitlementReconcilation
        //                    {
        //                        EndpointSystemId = Reconcil.EndpointSystemId,
        //                        EndpointUserId = Reconcil.EndpointUserId,
        //                        EnteredDate = Reconcil.EnteredDate,
        //                        Entitlement = Reconcil.Entitlement,
        //                        EntitlementType = (int)Reconcil.EntitlementType,
        //                        UserId = Reconcil.UserId,
        //                        AccountName = Reconcil.AccountName
        //                    }).ToList();

        //                var roles = (from sapepUser in CurrentContext.SAPEPUserInfos
        //                             from sapepRole in sapepUser.SAPEPRoles
        //                             where sapepUser.EndPointSystemId == _endpoint.Id
        //                             select new
        //                             {
        //                                 EndpointSystemId = _endpoint.Id,
        //                                 EndpointUserId = sapepUser.Id,
        //                                 EnteredDate = DateTime.Now,
        //                                 Entitlement = sapepRole.UniqueRoleName,
        //                                 EntitlementType = EntitlementType.Role,
        //                                 UserId = sapepUser.DIUserId,
        //                                 sapepUser.Version,
        //                                 AccountName = sapepUser.UserName
        //                             }).ToList()
        //                    .Select(Reconcil => new EntitlementReconcilation
        //                    {
        //                        EndpointSystemId = Reconcil.EndpointSystemId,
        //                        EndpointUserId = Reconcil.EndpointUserId,
        //                        EnteredDate = Reconcil.EnteredDate,
        //                        Entitlement = Reconcil.Entitlement,
        //                        EntitlementType = (int)Reconcil.EntitlementType,
        //                        UserId = Reconcil.UserId,
        //                        AccountName = Reconcil.AccountName
        //                    }).ToList();

        //        result= roles.Union(groups).ToList();
        //            }
        //            if (_endpoint.EndpointSystemTypeId == (int)EndpointType.VPN)
        //            {
        //                return null;
        //            }
        //    FlushEntitlementReconcilation(result);
        //        return result;      
        //}

        public List<EntitlementReconcilation> GetAllEntitlementReconcilationByEndpointSystemId(List<long> endpointSystemIds, AccountType accountType)
        {
            IQueryable<EntitlementReconcilation> query = this.CurrentContext.EntitlementReconcilations
                .Include("AccountReconcilation")
                .Where(EntitlementRecon => !EntitlementRecon.IsDeleted && 
                !EntitlementRecon.AccountReconcilation.IsDeleted &&

                EntitlementRecon.AccountReconcilation.AccountType == accountType &&
                endpointSystemIds.Contains(EntitlementRecon.EndpointSystemId)
                && EntitlementRecon.AccountReconcilation.UserId != null);


            return query.Select(e => e).ToList();

        }

        public List<EntitlementReconcilation> GetGemsBusinessRoleByAccount(long endpointId, long accountReconId, string accountName)
        {
            return (from ubm in CurrentContext.UserBusinessRoleMapping
                    join matrix in CurrentContext.GEMSRoleMatrix on ubm.BusinessRole equals matrix.BusinessRoleName
                    join role in CurrentContext.GEMSRoleInfo on matrix.TechnicalRole equals role.Display_Name
                    join aR in CurrentContext.AccountReconcilations on ubm.EndpointUserId equals aR.EndpointUserId
                    join ent in CurrentContext.EntitlementReconcilations on role.Name equals ent.Entitlement 
                    where aR.Id == accountReconId && aR.IsDeleted == false && aR.AccountName == accountName && aR.EndpointSystemId == endpointId && ent.EndpointSystemId == endpointId && ent.IsDeleted == false && ent.AccountReconcilationId == accountReconId
                    select new
                    {
                        Id = matrix.Id,
                        AccountName = accountName,
                        EndpointSystemId = endpointId,
                        Entitlement = matrix.BusinessRoleName,
                        AccountReconcilationId = accountReconId,
                        EntitlementDescription = matrix.BusinessRoleName,
                        EntitlementType = (int)EntitlementType.Role,
                        EntitlementId = matrix.Id,
                        EndpointUserId = aR.EndpointUserId,
                        UserId = aR.UserId,
                        ent.LastAttestedOn
                    }).Distinct().ToList().Select(c => new EntitlementReconcilation()
                    {
                        Id = c.Id,
                        AccountName = c.AccountName,
                        EndpointSystemId = c.EndpointSystemId,
                        Entitlement = c.Entitlement,
                        AccountReconcilationId = c.AccountReconcilationId,
                        EntitlementDescription = c.EntitlementDescription,
                        EntitlementType = c.EntitlementType,
                        EntitlementId = c.EntitlementId,
                        EndpointUserId = c.EndpointUserId,
                        UserId = c.UserId,
                        LastAttestedOn = c.LastAttestedOn
                    }).ToList();
        }
        public List<EntitlementReconcilation> GetTechnicalRoleFromBusinessRole(long endpointId, string accountName, string businessRole)
        {
            var delimiter = new char[] { '|' };
            var matrix = CurrentContext.GEMSRoleMatrix.FirstOrDefault(i => i.BusinessRoleName == businessRole);
            var results = (from role in CurrentContext.GEMSRoleInfo
                    join  entitlement in CurrentContext.EntitlementReconcilations on role.Name equals entitlement.Entitlement
                    join ep in CurrentContext.EndPoint on entitlement.EndpointSystemId equals ep.Id
                    where entitlement.AccountName == accountName && entitlement.EndpointSystemId == endpointId && entitlement.IsDeleted == false
                    && role.Display_Name == matrix.TechnicalRole
                           select new
                    {
                        entitlement.Id,
                        entitlement.AccountReconcilationId,
                        AccountName = accountName,
                        EndpointSystemId = endpointId,
                        Entitlement = entitlement.Entitlement,
                        EntitlementDescription = role.Display_Name,
                        EntitlementType = (int)EntitlementType.Role,
                        EntitlementId = role.Id,
                        entitlement.EndpointUserId,
                        Comments = ep.DisplayName,
                        entitlement.UserId,
                        entitlement.IsSensitiveAccess,
                        entitlement.IsSODConflict,
                        entitlement.LastAttestedOn
                    }).Distinct().ToList().Select(c => new EntitlementReconcilation()
                    {
                        Id = c.Id,
                        AccountName = c.AccountName,
                        EndpointSystemId = c.EndpointSystemId,
                        Entitlement = $"{c.Entitlement}",
                        AccountReconcilationId = c.AccountReconcilationId,
                        EntitlementDescription = c.EntitlementDescription,
                        EntitlementType = c.EntitlementType,
                        EntitlementId = c.EntitlementId,
                        EndpointUserId = c.EndpointUserId,
                        Comments = c.Comments,
                        UserId = c.UserId,
                        IsSensitiveAccess = c.IsSensitiveAccess,
                        IsSODConflict = c.IsSODConflict,
                        LastAttestedOn = c.LastAttestedOn
                    }).ToList();

            var oneRecord = results.FirstOrDefault();
            if (matrix != null && oneRecord != null && !string.IsNullOrWhiteSpace(matrix.ADGroups))
            {
                results.Add(new EntitlementReconcilation()
                {
                    Id = matrix.Id,
                    AccountName = oneRecord.AccountName,
                    AccountReconcilationId = oneRecord.AccountReconcilationId,
                    Entitlement = matrix.ADGroups,
                    EntitlementDescription = "AD Groups",
                    LastAttestedOn = oneRecord.LastAttestedOn,
                    UserId = oneRecord.UserId,
                    IsSensitiveAccess = oneRecord.IsSensitiveAccess,
                    IsSODConflict = oneRecord.IsSODConflict,
                    EndpointUserId = oneRecord.EndpointUserId,
                });
            }
            return results;
        }

        public List<EntitlementReconcilation> GetSAPEPBusinessRoleByAccount(long endpointId, long accountReconId, string accountName)
        {
            List<EntitlementReconcilation> entitlementReconcilations = new List<EntitlementReconcilation>();
            entitlementReconcilations = (from entitlement in CurrentContext.EntitlementReconcilations
                                         join accRecon in CurrentContext.AccountReconcilations on entitlement.AccountReconcilationId equals accRecon.Id
                                         join role in CurrentContext.SAPEPRoleInfos on entitlement.Entitlement equals role.UniqueRoleName
                                         join ep in CurrentContext.EndPoint on accRecon.EndpointSystemId equals ep.Id
                                         where accRecon.Id == accountReconId && accRecon.EndpointSystemId == endpointId
                                         && accRecon.IsDeleted == false && entitlement.IsDeleted == false && role.EndPointSystemId == endpointId
                                         && entitlement.EntitlementType == (int)EntitlementType.Role
                                         select new
                                         {
                                             entitlement.AccountName,
                                             entitlement.Entitlement,
                                             entitlement.EntitlementDescription,
                                             entitlement.EntitlementId,
                                             entitlement.EntitlementType,
                                             entitlement.EndpointUserId,
                                             entitlement.Id,
                                             entitlement.UserId,
                                             entitlement.IsSODConflict,
                                             entitlement.IsSensitiveAccess,
                                             ep.DisplayName,
                                             entitlement.ValidToDate
                                         }).Distinct().OrderBy(i => i.Entitlement).ToList().Select(entitlement => new EntitlementReconcilation()
                                         {
                                             AccountName = entitlement.AccountName,
                                             Entitlement = entitlement.Entitlement,
                                             EntitlementDescription = entitlement.EntitlementDescription,
                                             EntitlementId = entitlement.EntitlementId,
                                             EntitlementType = entitlement.EntitlementType,
                                             EndpointUserId = entitlement.EndpointUserId,
                                             Id = entitlement.Id,
                                             UserId = entitlement.UserId,
                                             IsSODConflict = entitlement.IsSODConflict,
                                             IsSensitiveAccess = entitlement.IsSensitiveAccess,
                                             Comments = entitlement.DisplayName,
                                             Functions = entitlement.Entitlement,
                                             ValidToDate = entitlement.ValidToDate
                                         }).ToList();
            return entitlementReconcilations;
        }
        public List<EntitlementReconcilation> GetSAPEPTechnicalRoleFromBusinessRole(long endpointId, string accountName, string businessRole)
        {
            return (from entitlement in CurrentContext.EntitlementReconcilations
                    join role in CurrentContext.SAPEPRoleInfos on entitlement.Entitlement equals role.UniqueRoleName
                    join ep in CurrentContext.EndPoint on entitlement.EndpointSystemId equals ep.Id
                    where entitlement.AccountName == accountName && role.EndPointSystemId == endpointId && entitlement.EndpointSystemId == endpointId && entitlement.IsDeleted == false
                    && role.RoleName == businessRole
                    select new
                    {
                        entitlement.Id,
                        entitlement.AccountReconcilationId,
                        AccountName = accountName,
                        EndpointSystemId = endpointId,
                        Entitlement = role.UniqueRoleName,
                        EntitlementDescription = role.RoleDescription,
                        EntitlementType = (int)EntitlementType.Role,
                        EntitlementId = role.Id,
                        entitlement.EndpointUserId,
                        Comments = ep.DisplayName,
                        entitlement.UserId,
                        entitlement.IsSensitiveAccess,
                        entitlement.IsSODConflict
                    }).Distinct().ToList().Select(c => new EntitlementReconcilation()
                    {
                        Id = c.Id,
                        AccountName = c.AccountName,
                        EndpointSystemId = c.EndpointSystemId,
                        Entitlement = c.Entitlement,
                        AccountReconcilationId = c.AccountReconcilationId,
                        EntitlementDescription = c.EntitlementDescription,
                        EntitlementType = c.EntitlementType,
                        EntitlementId = c.EntitlementId,
                        EndpointUserId = c.EndpointUserId,
                        Comments = c.Comments,
                        UserId = c.UserId,
                        IsSensitiveAccess = c.IsSensitiveAccess,
                        IsSODConflict = c.IsSODConflict
                    }).ToList();
        }

        public List<EntitlementReconcilation> GetOneCardEntitlementWithAdditionalInfo(long reconcilationId, long endpointId)
        {
            return this.CurrentContext.Database.SqlQuery<EntitlementReconcilation>($@"select 
E.[Id], E.[AccountReconcilationId], E.[EndpointType], E.[EndpointSystemId], E.[EndpointUserId], E.[UserId], E.[EntitlementId], E.[EntitlementType], E.[AccountName], E.[Entitlement], ISNULL(M.Description, E.[EntitlementDescription]) as [EntitlementDescription], E.[EnteredDate], E.[IsDeleted], E.[DeletedDate], E.[IsResolved], E.[ResolvedBy], E.[Comments], E.[ExceptionType], E.[ProvisionedThrough], E.[IsSODConflict], E.[IsSensitiveAccess], E.[CreatedBy], E.[CreatedDate], E.[LastModifiedBy], E.[LastModifiedDate], E.[ResolvedStatus], E.[LastAttestedOn], E.[ValidFromDate], E.[ValidToDate], E.[AccessRequestId], E.[ExceptionOccuredDate], E.[ResolvedDate],CASE WHEN ISNULL(M.Id,'')='' THEN NULL ELSE ISNULL(M.Building,'') END as Functions
FROM  (Select * from DI_M_EntitlementReconcilation Where AccountReconcilationId={reconcilationId} AND IsDeleted=0) as  E left join DI_SMS_XLRoleCatalogue M on E.Entitlement = M.RoleName 
where E.EndpointSystemId = {endpointId}
").ToList();
        }
        public List<EntitlementReconcilation> GetSAPPositionWithTextInfo(long reconcilationId, long endpointSystemId, ApplicationType applicationType,EntitlementType entitlementType)
        {
            var entitlements = new List<EntitlementReconcilation>();
            #region MSSL & EBS
            var tableName = (applicationType == ApplicationType.MSSL_CRM || applicationType == ApplicationType.MSSL_ISU) ? "DI_SAP_MSSL_XL_PositionsMatrix" : "DI_SAP_EBS_XL_PositionsMatrix";

            var fieldName = string.Empty;
            var dependentFieldName = string.Empty;
            if (applicationType == ApplicationType.MSSL_CRM || applicationType == ApplicationType.MSSL_ISU)
            {
                fieldName = applicationType == ApplicationType.MSSL_CRM ? "Position_MCP" : "Position_MSP";
                dependentFieldName = applicationType == ApplicationType.MSSL_CRM ? "Position_MSP" : "Position_MCP";
            }
            else
            {
                fieldName = applicationType == ApplicationType.EBS_CRM ? "Position_CRP" : "Position_ISP";
                dependentFieldName = applicationType == ApplicationType.EBS_CRM ? "Position_ISP" : "Position_CRP";
            }
            
                try
                {
                    var query = $@"SELECT 
                                entitlementRecon.EndpointType,
                                entitlementRecon.Id,AccountReconcilationId = entitlementRecon.AccountReconcilationId,
                                EndpointSystemId = entitlementRecon.EndpointSystemId,EndpointUserId = entitlementRecon.EndpointUserId,
                                AccountName = entitlementRecon.AccountName,Entitlement = entitlementRecon.Entitlement,EntitlementDescription = MX.PositionText,LastAttestedOn = entitlementRecon.LastAttestedOn,EntitlementType = entitlementRecon.EntitlementType,IsDeleted = entitlementRecon.IsDeleted,
                                EnteredDate = entitlementRecon.EnteredDate,DeletedDate = entitlementRecon.DeletedDate,ExceptionOccuredDate = entitlementRecon.ExceptionOccuredDate,
                                ExceptionType = entitlementRecon.ExceptionType,Comments = entitlementRecon.Comments,Functions = Mx.{dependentFieldName},IsResolved = entitlementRecon.IsResolved,
                                IsSensitiveAccess = entitlementRecon.IsSensitiveAccess,IsSODConflict  = entitlementRecon.IsSODConflict,
                                ResolvedBy = entitlementRecon.ResolvedBy,ProvisionedThrough = entitlementRecon.ProvisionedThrough,ValidToDate = entitlementRecon.ValidToDate,
                                ValidFromDate = entitlementRecon.ValidFromDate,UserId = entitlementRecon.UserId,ResolvedStatus = entitlementRecon.ResolvedStatus,ResolvedDate = entitlementRecon.ResolvedDate,
                                CreatedDate = entitlementRecon.CreatedDate,LastModifiedDate = entitlementRecon.LastModifiedDate,
                                AccessRequestId = entitlementRecon.AccessRequestId,CreatedBy = entitlementRecon.CreatedBy,LastModifiedBy = entitlementRecon.LastModifiedBy,EntitlementId = entitlementRecon.EntitlementId
                                FROM DI_M_EntitlementReconcilation entitlementRecon INNER JOIN DI_M_AccountReconcilation A ON entitlementRecon.AccountReconcilationId = A.Id
                                INNER JOIN {tableName} MX ON entitlementRecon.Entitlement = MX.{fieldName} WHERE A.ID = {reconcilationId} AND A.EndpointSystemId={endpointSystemId} AND entitlementRecon.EndpointSystemId={endpointSystemId} AND A.IsDeleted =0 AND entitlementRecon.IsDeleted=0
                                AND entitlementRecon.EntitlementType = {(int)EntitlementType.Position}";

                    entitlements = this.CurrentContext.Database.SqlQuery<EntitlementReconcilation>(query).ToList();
                }
                catch(Exception ex)
                {
                    throw new Exception($"GetSAPPositionWithTextInfo error occured",ex);
                }
                #endregion
            return entitlements;
        }
        public List<EntitlementReconcilation> GetSAPHANAPositionWithTextInfo(long reconcilationId, long endpointSystemId, ApplicationType applicationType, EntitlementType entitlementType)
        {
            var entitlements = new List<EntitlementReconcilation>();
            #region MSSL & EBS
            try
            {
                var query = $@"SELECT 
                                entitlementRecon.EndpointType,
                                entitlementRecon.Id,AccountReconcilationId = entitlementRecon.AccountReconcilationId,
                                EndpointSystemId = entitlementRecon.EndpointSystemId,EndpointUserId = entitlementRecon.EndpointUserId,
                                AccountName = entitlementRecon.AccountName,Entitlement = entitlementRecon.Entitlement,EntitlementDescription = MX.PositionText,LastAttestedOn = entitlementRecon.LastAttestedOn,EntitlementType = entitlementRecon.EntitlementType,IsDeleted = entitlementRecon.IsDeleted,
                                EnteredDate = entitlementRecon.EnteredDate,DeletedDate = entitlementRecon.DeletedDate,ExceptionOccuredDate = entitlementRecon.ExceptionOccuredDate,
                                ExceptionType = entitlementRecon.ExceptionType,Comments = entitlementRecon.Comments,Functions = Mx.Position,IsResolved = entitlementRecon.IsResolved,
                                IsSensitiveAccess = entitlementRecon.IsSensitiveAccess,IsSODConflict  = entitlementRecon.IsSODConflict,
                                ResolvedBy = entitlementRecon.ResolvedBy,ProvisionedThrough = entitlementRecon.ProvisionedThrough,ValidToDate = entitlementRecon.ValidToDate,
                                ValidFromDate = entitlementRecon.ValidFromDate,UserId = entitlementRecon.UserId,ResolvedStatus = entitlementRecon.ResolvedStatus,ResolvedDate = entitlementRecon.ResolvedDate,
                                CreatedDate = entitlementRecon.CreatedDate,LastModifiedDate = entitlementRecon.LastModifiedDate,
                                AccessRequestId = entitlementRecon.AccessRequestId,CreatedBy = entitlementRecon.CreatedBy,LastModifiedBy = entitlementRecon.LastModifiedBy,EntitlementId = entitlementRecon.EntitlementId
                                FROM DI_M_EntitlementReconcilation entitlementRecon INNER JOIN DI_M_AccountReconcilation A ON entitlementRecon.AccountReconcilationId = A.Id
                                INNER JOIN DI_SAPHANA_XLPositionsMatrix MX ON entitlementRecon.Entitlement = MX.Position WHERE A.ID = {reconcilationId} AND A.EndpointSystemId={endpointSystemId} AND MX.EndpointSystemId={endpointSystemId} AND entitlementRecon.EndpointSystemId={endpointSystemId} AND A.IsDeleted =0 AND entitlementRecon.IsDeleted=0
                                AND entitlementRecon.EntitlementType = 5";

                entitlements = this.CurrentContext.Database.SqlQuery<EntitlementReconcilation>(query).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"GetSAPPositionWithTextInfo error occured", ex);
            }
            #endregion
            return entitlements;
        }
        public List<EntitlementReconcilation> GetSRMBusinessRoles(long accountReconId)
        {
            return(from ent in CurrentContext.EntitlementReconcilations
                         join act in CurrentContext.AccountReconcilations on ent.AccountReconcilationId equals act.Id
                         join srmRole in CurrentContext.SAPSRMBusinessRoleMappings on ent.Entitlement equals srmRole.BusinessRole
                         where ent.AccountReconcilationId == accountReconId && ent.AccountName == srmRole.UserName && ent.IsDeleted == false
                         select new
                         {
                             ent.Id,
                             ent.Entitlement,
                             ent.EntitlementId,
                             ent.EnteredDate,
                             ent.EndpointUserId,
                             ent.AccountReconcilationId,
                             ent.EndpointSystemId,
                             EntitlementDescription = srmRole.POrg,
                             Functions = srmRole.PGrp,
                             ent.UserId,
                             ent.LastAttestedOn,
                             ent.ResolvedDate,
                             ent.IsSensitiveAccess,
                             ent.IsSODConflict,
                             ent.AccountName,
                             ent.EntitlementType,
                             ent.ValidFromDate,
                             ent.ValidToDate,
                         }).ToList().Select(ent => new EntitlementReconcilation() {
                             Id = ent.Id,
                             Entitlement = ent.Entitlement,
                             EntitlementId = ent.EntitlementId,
                             EnteredDate = ent.EnteredDate,
                             EndpointUserId = ent.EndpointUserId,
                             AccountReconcilationId = ent.AccountReconcilationId,
                             EndpointSystemId = ent.EndpointSystemId,
                             EntitlementDescription = ent.EntitlementDescription,
                             Functions = ent.Functions,
                             UserId = ent.UserId,
                             LastAttestedOn = ent.LastAttestedOn,
                             ResolvedDate = ent.ResolvedDate,
                             IsSensitiveAccess = ent.IsSensitiveAccess,
                             IsSODConflict = ent.IsSODConflict,
                             AccountName = ent.AccountName,
                             EntitlementType = ent.EntitlementType,
                             ValidFromDate = ent.ValidFromDate,
                             ValidToDate = ent.ValidToDate,
                         }).ToList();
        }

        public List<EntitlementReconcilation> GetSRMTechRoleByBusinessRoles(string businessRoleName, string pOrg, string pGroup)
        {
            var isonlyBusinessRole = string.IsNullOrWhiteSpace(pOrg) && string.IsNullOrWhiteSpace(pGroup);
            return (from mainRole in CurrentContext.SAPSRMMainRoleMatrices
                              join enable in CurrentContext.SAPSRMEnablerRoleMatrices on mainRole.SRMEnablerRule equals enable.SRMEnablerRule into srmMatrixDetail
                              from enable in srmMatrixDetail.DefaultIfEmpty()
                              where mainRole.BusinessRole == businessRoleName && (enable.POrg == pOrg && enable.PGrp == pGroup)
                              select new
                              {
                                  mainRole.Id,
                                  mainRole.SRMRoles,
                                  enable.SRMTechRole,
                                  mainRole.SUSRoles,
                                  mainRole.ADGroups,
                                  mainRole.BIRoles,
                                  enable.BIPGrpRole,
                                  enable.BIPOrgRole
                              }).ToList().Distinct().Select(i =>
                              new EntitlementReconcilation()
                              {
                                  Id = i.Id,
                                  Entitlement = string.Concat(i.SRMRoles, ",", i?.SRMTechRole),
                                  EntitlementDescription = i.SUSRoles,
                                  Functions = string.Concat(i.BIRoles, ",", i.BIPOrgRole, ",", i.BIPGrpRole),
                                  ProvisionedThrough = i.ADGroups
                       }).ToList();
        }
        public List<EntitlementReconcilation> GetEAMEntitlements(long reconcilationId, long endpointId)
        {
            return (from ent in CurrentContext.EntitlementReconcilations
                    join eamUsrRole in CurrentContext.SAPEamNovaUserRoleInfo on ent.EntitlementId equals eamUsrRole.Id
                    where ent.AccountReconcilationId == reconcilationId && ent.EndpointSystemId == endpointId && ent.IsDeleted == false && eamUsrRole.IsActive 
                    select new 
                    {
                        Id = ent.Id,
                        EntitlementId = ent.EntitlementId,
                        Entitlement = ent.Entitlement,
                        EntitlementDescription = ent.EntitlementDescription+"(Matrix:"+eamUsrRole.NovaPositionMatrixType+")",
                        LastAttestedOn = ent.LastAttestedOn,
                        EndpointSystemId = ent.EndpointSystemId,
                        EndpointUserId = ent.EndpointUserId,
                        AccountName = ent.AccountName,
                        AccountReconcilationId = ent.AccountReconcilationId,
                        EntitlementType = (int)EntitlementType.BusinessRole,
                        Functions = ent.Functions,
                        IsSensitiveAccess = ent.IsSensitiveAccess,
                        IsSODConflict = ent.IsSODConflict,
                        ValidToDate = ent.ValidToDate,
                        UserId = ent.UserId,
                        ValidFromDate = ent.ValidFromDate,
                        EndpointType = ent.EndpointType
                    }
                    ).AsEnumerable().Select(ent => new EntitlementReconcilation()
                    {
                        Id = ent.Id,
                        EntitlementId = ent.EntitlementId,
                        Entitlement = ent.Entitlement,
                        EntitlementDescription = ent.EntitlementDescription,
                        LastAttestedOn = ent.LastAttestedOn,
                        EndpointSystemId = ent.EndpointSystemId,
                        EndpointUserId = ent.EndpointUserId,
                        AccountName = ent.AccountName,
                        AccountReconcilationId = ent.AccountReconcilationId,
                        EntitlementType = ent.EntitlementType,
                        Functions = ent.Functions,
                        IsSensitiveAccess = ent.IsSensitiveAccess,
                        IsSODConflict = ent.IsSODConflict,
                        ValidToDate = ent.ValidToDate,
                        UserId = ent.UserId,
                        ValidFromDate = ent.ValidFromDate,
                        EndpointType = ent.EndpointType
                    }).ToList();
        }
        public List<EntitlementReconcilation> GetEAMTechRoleByBusinessRoles(long endpointSystemId,string userAccountName, string businessRole)
        {
            if(string.IsNullOrWhiteSpace(userAccountName) || string.IsNullOrWhiteSpace(businessRole))
            {
                return new List<EntitlementReconcilation>();
            }
            this.CurrentContext.Database.CommandTimeout = 0;
            var applnParam = new SqlParameter("@ENDPOINTSYSTEMID", endpointSystemId);
            var usrNameParam = new SqlParameter("@AccountName", System.Data.SqlDbType.VarChar, 512);
            usrNameParam.Value = userAccountName;
            var businessRoleParam = new SqlParameter("@BUSINESSROLE", System.Data.SqlDbType.VarChar, 2000);
            businessRoleParam.Value = businessRole;
            object[] param = { applnParam, usrNameParam, businessRoleParam };
            return CurrentContext.Database.SqlQuery<EntitlementReconcilation>("Exec DBO.USP_EAM_TECHNICALROLE_FROMBUSINESSROLE @ENDPOINTSYSTEMID,@AccountName,@BUSINESSROLE", param).ToList();
        }

        public List<EntitlementReconcilation> GetEntitlements(long endpointId,string accountName)
        {
            return (from ent in CurrentContext.EntitlementReconcilations
                    join account in CurrentContext.AccountReconcilations on ent.AccountReconcilationId equals account.Id
                    where account.AccountName == accountName && account.EndpointSystemId == endpointId
                    select new
                    {
                        ent.Entitlement,
                        ent.EntitlementDescription
                    }).ToList().Distinct().Select(i => new EntitlementReconcilation()
                    {
                        Entitlement = i.Entitlement,
                        EntitlementDescription = i.EntitlementDescription
                    }).ToList();
        }
        public List<EntitlementReconcilation> GetSAPPoratlEntitlements(long reconcilationId, long endpointId, EntitlementType entitlementType)
        {
            if(entitlementType == EntitlementType.Role)
            {
                return (from ent in CurrentContext.EntitlementReconcilations
                        join epRoles in CurrentContext.SAPEPRoleInfos on ent.Entitlement equals epRoles.RoleName
                        where ent.AccountReconcilationId == reconcilationId && ent.EndpointSystemId == endpointId && ent.IsDeleted == false
                        && epRoles.EndPointSystemId == endpointId
                        select new
                        {
                            Id = ent.Id,
                            EntitlementId = ent.EntitlementId,
                            Entitlement = ent.Entitlement,
                            EntitlementDescription = ent.EntitlementDescription,
                            LastAttestedOn = ent.LastAttestedOn,
                            EndpointSystemId = ent.EndpointSystemId,
                            EndpointUserId = ent.EndpointUserId,
                            AccountName = ent.AccountName,
                            AccountReconcilationId = ent.AccountReconcilationId,
                            EntitlementType = ent.EntitlementType,
                            Functions = epRoles.IsXLUploaded ? "1":"0",
                            IsSensitiveAccess = ent.IsSensitiveAccess,
                            IsSODConflict = ent.IsSODConflict,
                            ValidToDate = ent.ValidToDate,
                            UserId = ent.UserId,
                            ValidFromDate = ent.ValidFromDate,
                            EndpointType = ent.EndpointType,
                        }
                    ).AsEnumerable().Select(ent => new EntitlementReconcilation()
                    {
                        Id = ent.Id,
                        EntitlementId = ent.EntitlementId,
                        Entitlement = ent.Entitlement,
                        EntitlementDescription = ent.EntitlementDescription,
                        LastAttestedOn = ent.LastAttestedOn,
                        EndpointSystemId = ent.EndpointSystemId,
                        EndpointUserId = ent.EndpointUserId,
                        AccountName = ent.AccountName,
                        AccountReconcilationId = ent.AccountReconcilationId,
                        EntitlementType = ent.EntitlementType,
                        Functions = ent.Functions,
                        IsSensitiveAccess = ent.IsSensitiveAccess,
                        IsSODConflict = ent.IsSODConflict,
                        ValidToDate = ent.ValidToDate,
                        UserId = ent.UserId,
                        ValidFromDate = ent.ValidFromDate,
                        EndpointType = ent.EndpointType
                    }).ToList();
            }
            else if (entitlementType == EntitlementType.Group)
            {
                return (from ent in CurrentContext.EntitlementReconcilations
                        join epGroup in CurrentContext.SAPEPGroupInfos on ent.Entitlement equals epGroup.GroupName 
                        where ent.AccountReconcilationId == reconcilationId && ent.EndpointSystemId == endpointId && ent.IsDeleted == false &&
                        epGroup.EndPointSystemId == endpointId
                        select new
                        {
                            Id = ent.Id,
                            EntitlementId = ent.EntitlementId,
                            Entitlement = ent.Entitlement,
                            EntitlementDescription = ent.EntitlementDescription,
                            LastAttestedOn = ent.LastAttestedOn,
                            EndpointSystemId = ent.EndpointSystemId,
                            EndpointUserId = ent.EndpointUserId,
                            AccountName = ent.AccountName,
                            AccountReconcilationId = ent.AccountReconcilationId,
                            EntitlementType = ent.EntitlementType,
                            Functions = epGroup.IsXLUploaded ? "1" : "0",
                            IsSensitiveAccess = ent.IsSensitiveAccess,
                            IsSODConflict = ent.IsSODConflict,
                            ValidToDate = ent.ValidToDate,
                            UserId = ent.UserId,
                            ValidFromDate = ent.ValidFromDate,
                            EndpointType = ent.EndpointType,
                        }
                    ).AsEnumerable().Select(ent => new EntitlementReconcilation()
                    {
                        Id = ent.Id,
                        EntitlementId = ent.EntitlementId,
                        Entitlement = ent.Entitlement,
                        EntitlementDescription = ent.EntitlementDescription,
                        LastAttestedOn = ent.LastAttestedOn,
                        EndpointSystemId = ent.EndpointSystemId,
                        EndpointUserId = ent.EndpointUserId,
                        AccountName = ent.AccountName,
                        AccountReconcilationId = ent.AccountReconcilationId,
                        EntitlementType = ent.EntitlementType,
                        Functions = ent.Functions,
                        IsSensitiveAccess = ent.IsSensitiveAccess,
                        IsSODConflict = ent.IsSODConflict,
                        ValidToDate = ent.ValidToDate,
                        UserId = ent.UserId,
                        ValidFromDate = ent.ValidFromDate,
                        EndpointType = ent.EndpointType
                    }).ToList();
            }
            return new List<EntitlementReconcilation>();
        }

        public List<string> GetActiveAccountsForCriticalReport(long endpointId, string accountName = null)
        {
            if (!string.IsNullOrWhiteSpace(accountName))
                accountName = null;

            var types = new []{(int)EntitlementType.Role,(int)EntitlementType.Position };
            return (from ent in CurrentContext.EntitlementReconcilations
                    join ac in CurrentContext.AccountReconcilations on ent.AccountReconcilationId equals ac.Id
                    where ac.EndpointSystemId == endpointId && ent.EndpointSystemId == endpointId && types.Contains(ent.EntitlementType)
                    && ac.IsDeleted == false && ent.IsDeleted == false
                    && (accountName == null ? true: ac.AccountName == accountName)
                    select ac.AccountName
                    ).Distinct().ToList();
        }
        public List<string> GetActiveAccountsCriticalReport(long endpointId, string accountName = null)
        {
            if (!string.IsNullOrWhiteSpace(accountName))
                accountName = null;

            var types = new[] { (int)EntitlementType.Role, (int)EntitlementType.Position };
            return (from ent in CurrentContext.EntitlementReconcilations
                    join ac in CurrentContext.AccountReconcilations on ent.AccountReconcilationId equals ac.Id
                    where ac.EndpointSystemId == endpointId && ent.EndpointSystemId == endpointId && types.Contains(ent.EntitlementType) &&
                    ac.IsDeleted == false && ent.IsDeleted == false
                    && (accountName == null ? true : ac.AccountName == accountName)
                    select ac.AccountName
                    ).Distinct().ToList();
        }
        public bool GenerateReportForUser(long endpointid, string username)
        {
            bool success = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(username) && endpointid > 0)
                {
                    SqlParameter sqlParamUserLogon = new SqlParameter();
                    sqlParamUserLogon.ParameterName = "@USERNAME";
                    sqlParamUserLogon.SqlDbType = System.Data.SqlDbType.NVarChar;
                    sqlParamUserLogon.Value = username;

                    SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
                    sqlParamEndpoinSystemId.ParameterName = "@ENDPOINTID";
                    sqlParamEndpoinSystemId.SqlDbType = System.Data.SqlDbType.BigInt;
                    sqlParamEndpoinSystemId.Value = endpointid;

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    parameters.Add(sqlParamUserLogon);
                    parameters.Add(sqlParamEndpoinSystemId);

                    var m_iacmOperator = new IACMDbOperator("USP_CRITICALTCODEREPORT", System.Data.CommandType.StoredProcedure, parameters);
                    var m_executor = new QueryExecutor(m_iacmOperator);
                    m_executor.ExecuteQuery();
                    success = true;
                }
            }
            catch
            {
            }

            return success;
        }

        public EntitlementReconcilation GetEndpointUserObjectByParameter(long endpointSystemId, long userId, string entitlement, int entitlementType)
        {
            return CurrentContext.EntitlementReconcilations.FirstOrDefault(i=> i.EndpointSystemId == endpointSystemId && i.EndpointUserId == userId && i.Entitlement == entitlement && i.EntitlementType == entitlementType);
        }
    }
}

