using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.ReportData;
using DI.Chakra.Model.SAP;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DI.Chakra.DataAccess
{
    public class ReconcilationDA
    {

        public long CreateAccountReconcilationRecord(AccountReconcilation acctRecon)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.AccountReconcilations.Add(acctRecon);
                    _context.SaveChanges();
                    return acctRecon.Id;
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddOrUpdateAccountInReconTable(AccountReconcilation _accountReconcilation)
        {
            using (IACMDBContext context = new IACMDBContext())
            {
                AccountReconcilation reconcilation = context.AccountReconcilations.FirstOrDefault(ar => ar.EndpointSystemId == _accountReconcilation.EndpointSystemId
                    && ar.EndpointUserId == _accountReconcilation.EndpointUserId);

                if (reconcilation == null)
                {
                    _accountReconcilation.CreatedDate = DateTime.Now;
                    context.AccountReconcilations.Attach(_accountReconcilation);
                    context.Entry(_accountReconcilation).State = EntityState.Added;
                    context.SaveChanges();
                }
                else
                {
                    var accReqEntry = context.Entry(_accountReconcilation);
                    if (accReqEntry.State == EntityState.Detached)
                    {
                       
                        var attachedEntry = context.Entry(reconcilation);
                        attachedEntry.CurrentValues.SetValues(_accountReconcilation);
                    }
                    context.SaveChanges();
                }



            }
            }
        public void DeleteReconAccount(long id)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                var ac = _context.AccountReconcilations.First(u => u.Id == id);
                _context.AccountReconcilations.Remove(ac);
                _context.SaveChanges();
            }
        }
        public void SaveReconcilationRecord(AccountReconcilation _accountReconcilation)
        {
            using (IACMDBContext context = new IACMDBContext())
            {
                if (_accountReconcilation.Id == 0)
                {
                    context.AccountReconcilations.Attach(_accountReconcilation);
                    context.Entry(_accountReconcilation).State = EntityState.Added;
                    context.SaveChanges();
                }
                else
                {
                    AccountReconcilation editRecon = context.AccountReconcilations.Where(ar => ar.Id == _accountReconcilation.Id).First();
                    if (editRecon == null)
                        return;
                    context.Entry(editRecon).CurrentValues.SetValues(_accountReconcilation);
                    context.SaveChanges();
                }
            }
        }
        public void SaveEntitlementReconcilationRecord(EntitlementReconcilation _entitlementReconcilation)
        {
            using (IACMDBContext context = new IACMDBContext())
            {
                if (_entitlementReconcilation.Id == 0)
                {
                    context.EntitlementReconcilations.Attach(_entitlementReconcilation);
                    context.Entry(_entitlementReconcilation).State = EntityState.Added;
                    context.SaveChanges();
                }
                else
                {
                    EntitlementReconcilation editRecon = context.EntitlementReconcilations.Where(ar => ar.Id == _entitlementReconcilation.Id).First();
                    if (editRecon == null)
                        return;
                    context.Entry(editRecon).CurrentValues.SetValues(_entitlementReconcilation);
                    context.SaveChanges();
                }
            }
        }
        public void AddReconcilationRecord(AccountReconcilation _accountReconcilation)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                _context.AccountReconcilations.Add(_accountReconcilation);
                _context.SaveChanges();
            }

        }
        public void UpdateAccountReconcilationRecord(AccountReconcilation acctRecon)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var accReqEntry = _context.Entry(acctRecon);
                    if (accReqEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<AccountReconcilation>();
                        AccountReconcilation attachedAccReq = set.Find(acctRecon.Id);

                        if (attachedAccReq != null)
                        {
                            var attachedEntry = _context.Entry(attachedAccReq);
                            attachedEntry.CurrentValues.SetValues(acctRecon);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        public void AddEntitlementReconcilationRecord(EntitlementReconcilation _entitlementReconcilation)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                _context.EntitlementReconcilations.Add(_entitlementReconcilation);
                _context.SaveChanges();
            }
        }
        public void UpdateEntitlementReconcilationRecord(EntitlementReconcilation entRecon)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var accReqEntry = _context.Entry(entRecon);
                    if (accReqEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<EntitlementReconcilation>();
                        EntitlementReconcilation attachedAccReq = set.Find(entRecon.Id);

                        if (attachedAccReq != null)
                        {
                            var attachedEntry = _context.Entry(attachedAccReq);
                            attachedEntry.CurrentValues.SetValues(entRecon);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        public void CreateEntitlementReconcilationRecord(EntitlementReconcilation entRecon)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.EntitlementReconcilations.Add(entRecon);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        public AccountReconcilation GetAccountReconcilationRecordById(long acctReconId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AccountReconcilations.Include("EndPoint").Include("EndPoint.EndPointSystemType").FirstOrDefault(item => item.Id == acctReconId);
                }
            }
            catch
            {
                throw;
            }
        }

        public EntitlementReconcilation GetEntitlementReconcilationRecordById(long entReconId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EntitlementReconcilations.FirstOrDefault(item => item.Id == entReconId);
                }
            }
            catch
            {
                throw;
            }
        }
        //public IList<AccountReconcilation> GetAccountReconcilationByUser(int UserId)
        //{
        //    try
        //    {
        //        return GetAccountReconcilationByUser(UserId);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public IList<AccountReconcilation> GetAccountReconcilationByUser(long UserId, bool isLocked = false)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (!isLocked)
                        return _context.AccountReconcilations.Include("EndPoint").Include("EndPoint.EndPointSystemType").Where(item => item.UserId == UserId && !item.IsLocked).ToList();
                    else

                        return _context.AccountReconcilations.Include("EndPoint").Include("EndPoint.EndPointSystemType").Where(item => item.UserId == UserId && item.IsLocked).ToList();
                }
            }
            catch
            {
                throw;
            }
        }
        public IList<AccountReconcilation> GetAccountReconcilationsBySystemId(int SystemId, bool isLocked = false)
        {
            try
            {
                IList<AccountReconcilation> lst = null;
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if ((!isLocked) &&(SystemId !=0))
                    {
                        lst = _context.AccountReconcilations.Include("EndPoint").Include("EndPoint.EndPointSystemType").Where(item => item.EndpointSystemId == SystemId && !item.IsLocked).ToList();

                    }
                    if ((isLocked) && (SystemId != 0))
                    {
                        lst = _context.AccountReconcilations.Include("EndPoint").Include("EndPoint.EndPointSystemType").Where(item => item.EndpointSystemId == SystemId && item.IsLocked).ToList();
                       
                    }
                    else
                    {
                        lst = _context.AccountReconcilations.Include("EndPoint").Include("EndPoint.EndPointSystemType").Where(item => 1 == 1).ToList();
                    }
                    return lst;
                }
            }
            catch
            {
                throw;
            }
        }
        public AccountReconcilation GetAccountReconcilationRecordByUser(long endpointSystemId, long endpointUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AccountReconcilations.Include("EndPoint").Include("EndPoint.EndPointSystemType").FirstOrDefault(item => item.EndpointUserId == endpointUserId && item.EndpointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }

        public EntitlementReconcilation GetEntitlementReconcilationRecordByUser(long endpointSystemId, long endpointUserId, string entitlement, EntitlementType entitlementType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EntitlementReconcilations.FirstOrDefault(item => item.EndpointUserId == endpointUserId && item.EndpointSystemId == endpointSystemId &&
                           item.Entitlement == entitlement && item.EntitlementType == (int)entitlementType);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<EntitlementReconcilation> GetEntitlementReconcilationsByUser(long userId, long endpointSystemId, long endpointUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EntitlementReconcilations.Where(item => item.UserId == userId &&
                           item.EndpointUserId == endpointUserId && item.EndpointSystemId == endpointSystemId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void CreateOUChangeSnapshot(OUChangeSnapshot snapshot)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.OUChangeSnapshots.Add(snapshot);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<EntitlementReconcilation> GetEntitilementReconcilationListForEndpoint(EndPoint _endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.SAP)
                    {
                        var RolesList = (from sapUser in _context.SAPUserinfos
                                         from sapRole in sapUser.SAPRoleInfo
                                         where sapUser.EndPointSystemId == _endpoint.Id
                                         select new
                                         {
                                             EndpointSystemId = _endpoint.Id,
                                             EndpointUserId = sapUser.Id,
                                             EnteredDate = DateTime.Now,
                                             Entitlement = sapRole.ROLE_CODE,
                                             EntitlementType = EntitlementType.Role,
                                             UserId = sapUser.DIUserId,
                                             Version = sapRole.Version.Value,
                                             AccountName = sapUser.UserLogonName
                                         }).ToList()
                                        .Select(Reconcil => new EntitlementReconcilation
                                        {
                                            EndpointSystemId = Reconcil.EndpointSystemId,
                                            EndpointUserId = Reconcil.EndpointUserId,
                                            EnteredDate = Reconcil.EnteredDate,
                                            Entitlement = Reconcil.Entitlement,
                                            EntitlementType = (int)Reconcil.EntitlementType,
                                            UserId = Reconcil.UserId,
                                            Version = (byte)Reconcil.Version,
                                            AccountName = Reconcil.AccountName
                                        }).ToList();


                        var positions = (from sapUser in _context.SAPUserinfos
                                         from sapPosition in _context.SAPHRP1001Entries
                                         where sapUser.EndPointSystemId == _endpoint.Id &&
                                         sapPosition.EndpointSystemId == _endpoint.Id &&
                                         sapPosition.RelatedObjectCode == sapUser.UserLogonName &&
                                         sapPosition.RelatedObjectType == "US"
                                         select new
                                         {
                                             EndpointSystemId = _endpoint.Id,
                                             EndpointUserId = sapUser.Id,
                                             EnteredDate = DateTime.Now,
                                             Entitlement = sapPosition.ActualObjectCode,
                                             EntitlementType = (int)EntitlementType.Position,
                                             UserId = sapUser.DIUserId, sapUser.Version,
                                             AccountName = sapUser.UserLogonName
                                         }).ToList()
                                        .Select(Reconcil => new EntitlementReconcilation
                                        {
                                            EndpointSystemId = Reconcil.EndpointSystemId,
                                            EndpointUserId = Reconcil.EndpointUserId,
                                            EnteredDate = Reconcil.EnteredDate,
                                            Entitlement = Reconcil.Entitlement,
                                            EntitlementType = Reconcil.EntitlementType,
                                            UserId = Reconcil.UserId,
                                            Version = (byte)Reconcil.Version,
                                            AccountName = Reconcil.AccountName
                                        }).ToList();

                        return RolesList.Union(positions).ToList();
                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
                    {
                        return (from adsUser in _context.ADSUserInfos
                            from adsGroup in adsUser.ADSGroups
                            where adsUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                EndpointSystemId = _endpoint.Id,
                                EndpointUserId = adsUser.Id,
                                EnteredDate = DateTime.Now,
                                Entitlement = adsGroup.name,
                                EntitlementType = EntitlementType.Group,
                                UserId = adsUser.DIUserId,
                                Version = adsGroup.Version.Value,
                                AccountName = adsUser.sAMAccountName
                            }).ToList()
                            .Select(Reconcil => new EntitlementReconcilation
                            {
                                EndpointSystemId = Reconcil.EndpointSystemId,
                                EndpointUserId = Reconcil.EndpointUserId,
                                EnteredDate = Reconcil.EnteredDate,
                                Entitlement = Reconcil.Entitlement,
                                EntitlementType = (int)Reconcil.EntitlementType,
                                UserId = Reconcil.UserId,
                                Version = (byte)Reconcil.Version,
                                AccountName = Reconcil.AccountName
                            }).ToList();

                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
                    {
                        return (from lnsUser in _context.LotusUserInfos
                            from lnsGroup in lnsUser.LotusGroupInfos
                            where lnsUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                EndpointSystemId = _endpoint.Id,
                                EndpointUserId = lnsUser.Id,
                                EnteredDate = DateTime.Now,
                                Entitlement = lnsGroup.Group,
                                EntitlementType = EntitlementType.Group,
                                UserId = lnsUser.DIUserId,
                                Version = lnsGroup.Version.Value,
                                AccountName = lnsUser.InternetAddress
                            }).ToList()
                            .Select(Reconcil => new EntitlementReconcilation
                            {
                                EndpointSystemId = Reconcil.EndpointSystemId,
                                EndpointUserId = Reconcil.EndpointUserId,
                                EnteredDate = Reconcil.EnteredDate,
                                Entitlement = Reconcil.Entitlement,
                                EntitlementType = (int)Reconcil.EntitlementType,
                                UserId = Reconcil.UserId,
                                Version = (byte)Reconcil.Version,
                                AccountName = Reconcil.AccountName
                            }).ToList();

                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.InhouseSystems)
                    {
                        return (from inhouseUser in _context.InhouseUserInfos
                            where inhouseUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                EndpointSystemId = _endpoint.Id,
                                EndpointUserId = inhouseUser.Id,
                                EnteredDate = DateTime.Now,
                                Entitlement = inhouseUser.Permission,
                                EntitlementType = EntitlementType.Role,
                                UserId = inhouseUser.DIUserId, inhouseUser.Version,
                                AccountName = inhouseUser.UserName
                            }).ToList()
                            .Select(Reconcil => new EntitlementReconcilation
                            {
                                EndpointSystemId = Reconcil.EndpointSystemId,
                                EndpointUserId = Reconcil.EndpointUserId,
                                EnteredDate = Reconcil.EnteredDate,
                                Entitlement = Reconcil.Entitlement,
                                EntitlementType = (int)Reconcil.EntitlementType,
                                UserId = Reconcil.UserId,
                                Version = (byte)Reconcil.Version,
                                AccountName = Reconcil.AccountName
                            }).ToList();
                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                    {
                        var groups = (from sapepUser in _context.SAPEPUserInfos
                            from sapepGroup in sapepUser.SAPEPGroups
                            where sapepUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                EndpointSystemId = _endpoint.Id,
                                EndpointUserId = sapepUser.Id,
                                EnteredDate = DateTime.Now,
                                Entitlement = sapepGroup.GroupUniqueName,
                                EntitlementType = EntitlementType.Group,
                                UserId = sapepUser.DIUserId, sapepUser.Version,
                                AccountName = sapepUser.UserName
                            }).ToList()
                            .Select(Reconcil => new EntitlementReconcilation
                            {
                                EndpointSystemId = Reconcil.EndpointSystemId,
                                EndpointUserId = Reconcil.EndpointUserId,
                                EnteredDate = Reconcil.EnteredDate,
                                Entitlement = Reconcil.Entitlement,
                                EntitlementType = (int)Reconcil.EntitlementType,
                                UserId = Reconcil.UserId,
                                Version = (byte)Reconcil.Version,
                                AccountName = Reconcil.AccountName
                            }).ToList();

                        var roles = (from sapepUser in _context.SAPEPUserInfos
                            from sapepRole in sapepUser.SAPEPRoles
                            where sapepUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                EndpointSystemId = _endpoint.Id,
                                EndpointUserId = sapepUser.Id,
                                EnteredDate = DateTime.Now,
                                Entitlement = sapepRole.UniqueRoleName,
                                EntitlementType = EntitlementType.Role,
                                UserId = sapepUser.DIUserId, sapepUser.Version,
                                AccountName = sapepUser.UserName
                            }).ToList()
                            .Select(Reconcil => new EntitlementReconcilation
                            {
                                EndpointSystemId = Reconcil.EndpointSystemId,
                                EndpointUserId = Reconcil.EndpointUserId,
                                EnteredDate = Reconcil.EnteredDate,
                                Entitlement = Reconcil.Entitlement,
                                EntitlementType = (int)Reconcil.EntitlementType,
                                UserId = Reconcil.UserId,
                                Version = (byte)Reconcil.Version,
                                AccountName = Reconcil.AccountName
                            }).ToList();

                        return roles.Union(groups).ToList();
                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.VPN)
                    {
                        return null;
                    }
                }

                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AccountReconcilation> GetAccountReconcilationListForEndpoint(EndPoint _endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.SAP)
                    {
                        return (from sapUser in _context.SAPUserinfos
                                where sapUser.EndPointSystemId == _endpoint.Id
                                select new
                                {
                                    AccountName = sapUser.UserLogonName, EndpointSystemId = _endpoint.Id,
                                    EndpointUserId = sapUser.Id,
                                    EnteredDate = DateTime.Now,
                                    UserId = sapUser.DIUserId,
                                    Version = (byte)sapUser.Version
                                }).ToList()
                                .Select(Reconil => new AccountReconcilation
                                {
                                    AccountName = Reconil.AccountName,
                                    EndpointSystemId = Reconil.EndpointSystemId,
                                    EndpointUserId = Reconil.EndpointUserId,
                                    CreatedDate = Reconil.EnteredDate,
                                    UserId = Reconil.UserId,
                                    Version = Reconil.Version
                                }).ToList();

                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
                    {
                        return (from adsUser in _context.ADSUserInfos
                            where adsUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                AccountName = adsUser.sAMAccountName, EndpointSystemId = _endpoint.Id,
                                EndpointUserId = adsUser.Id,
                                EnteredDate = DateTime.Now,
                                UserId = adsUser.DIUserId,
                                Version = (byte)adsUser.Version
                            }).ToList()
                            .Select(Reconil => new AccountReconcilation
                            {
                                AccountName = Reconil.AccountName,
                                EndpointSystemId = Reconil.EndpointSystemId,
                                EndpointUserId = Reconil.EndpointUserId,
                                CreatedDate = Reconil.EnteredDate,
                                UserId = Reconil.UserId,
                                Version = Reconil.Version
                            }).ToList();
                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
                    {
                        return (from lnsUser in _context.LotusUserInfos
                            where lnsUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                AccountName = lnsUser.InternetAddress, EndpointSystemId = _endpoint.Id,
                                EndpointUserId = lnsUser.Id,
                                EnteredDate = DateTime.Now,
                                UserId = lnsUser.DIUserId,
                                Version = (byte)lnsUser.Version
                            }).ToList()
                            .Select(Reconil => new AccountReconcilation
                            {
                                AccountName = Reconil.AccountName,
                                EndpointSystemId = Reconil.EndpointSystemId,
                                EndpointUserId = Reconil.EndpointUserId,
                                CreatedDate = Reconil.EnteredDate,
                                UserId = Reconil.UserId,
                                Version = Reconil.Version
                            }).ToList();
                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.InhouseSystems)
                    {
                        return (from inhouseUser in _context.InhouseUserInfos
                            where inhouseUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                AccountName = inhouseUser.UserName, EndpointSystemId = _endpoint.Id,
                                EndpointUserId = inhouseUser.Id,
                                EnteredDate = DateTime.Now,
                                UserId = inhouseUser.DIUserId,
                                Version = (byte)inhouseUser.Version
                            }).ToList()
                            .Select(Reconil => new AccountReconcilation
                            {
                                AccountName = Reconil.AccountName,
                                EndpointSystemId = Reconil.EndpointSystemId,
                                EndpointUserId = Reconil.EndpointUserId,
                                CreatedDate = Reconil.EnteredDate,
                                UserId = Reconil.UserId,
                                Version = Reconil.Version
                            }).ToList();
                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                    {
                        return (from sapepUser in _context.SAPEPUserInfos
                            where sapepUser.EndPointSystemId == _endpoint.Id
                            select new
                            {
                                AccountName = sapepUser.UserName, EndpointSystemId = _endpoint.Id,
                                EndpointUserId = sapepUser.Id,
                                EnteredDate = DateTime.Now,
                                UserId = sapepUser.DIUserId,
                                Version = (byte)sapepUser.Version
                            }).ToList()
                            .Select(Reconil => new AccountReconcilation
                            {
                                AccountName = Reconil.AccountName,
                                EndpointSystemId = Reconil.EndpointSystemId,
                                EndpointUserId = Reconil.EndpointUserId,
                                CreatedDate = Reconil.EnteredDate,
                                UserId = Reconil.UserId,
                                Version = Reconil.Version
                            }).ToList();
                    }
                    if (_endpoint.EndpointSystemTypeId == (int)EndpointType.VPN)
                    {
                        return null;
                    }
                }

                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ResolveAllExceptionsForFirstTimeDiscovery(EndPoint _endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(string.Format("Delete From DI_M_EntitlementReconcilation where EndpointSystemId={0}", _endpoint.Id));
                    _context.Database.ExecuteSqlCommand(string.Format("Delete From DI_M_AccountReconcilation where EndpointSystemId={0}", _endpoint.Id));

                    List<AccountReconcilation> accountReconcilations = GetAccountReconcilationListForEndpoint(_endpoint);

                    SQLBulkUploader<AccountReconcilation> sqlAccountReconcilation = null;
                    string tableName = "DI_M_AccountReconcilation";
                    sqlAccountReconcilation = new SQLBulkUploader<AccountReconcilation>(tableName);
                    sqlAccountReconcilation.Flush(accountReconcilations);

                    List<EntitlementReconcilation> entitlementReconcilations = GetEntitilementReconcilationListForEndpoint(_endpoint);
                    tableName = "DI_M_EntitlementReconcilation";
                    SQLBulkUploader<EntitlementReconcilation> sqlEntitlmentReconcilation = new SQLBulkUploader<EntitlementReconcilation>(tableName);
                    sqlEntitlmentReconcilation.Flush(entitlementReconcilations);

                    string updateQuery = "update entRecon set entRecon.AccountReconcilationId = acctRecon.AccountReconcilationId from DI_M_EntitlementReconcilation entRecon join DI_M_AccountReconcilation acctRecon on entRecon.EndpointUserId = acctRecon.EndpointUserId AND entRecon.EndpointSystemId = acctRecon.EndpointSystemId";

                    _context.Database.ExecuteSqlCommand(updateQuery);

                }
            }
            catch
            {
                throw;
            }
        }

        public List<ReconcilationExceptionForEntitlmentsReport> GetEntitlementReconcilationAddedExceptions(int _EndpointSystemId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                EndPoint endPoint = new EndPointDA().GetEndPointSystemByKey(_EndpointSystemId);

                if (endPoint.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
                {
                    return (from endPointUser in _context.ADSUserInfos
                            from adsGroups in endPointUser.ADSGroups
                            where endPointUser.EndPointSystemId == _EndpointSystemId &&
                            _context.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == adsGroups.name) == null
                            select new ReconcilationExceptionForEntitlmentsReport
                            {
                                AccountName = endPointUser.sAMAccountName,
                                DisplayName = endPointUser.displayName,
                                EndpointSystemId = _EndpointSystemId,
                                EndpointUserId = endPointUser.Id,
                                CreatedDate = endPointUser.CreatedDate,
                                UserId = endPointUser.DIUserId,
                                Entitlement = adsGroups.name,
                                EntitlementType = (int)EntitlementType.Group,
                                ExceptionType = ExceptionTypes.Added,
                                Version = endPointUser.Version
                            }).ToList();
                }
                if (endPoint.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
                {
                    return (from endPointUser in _context.LotusUserInfos
                        from lnsGroups in endPointUser.LotusGroupInfos
                        where endPointUser.EndPointSystemId == _EndpointSystemId &&
                              _context.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == lnsGroups.Group) == null
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = endPointUser.Name,
                            DisplayName = endPointUser.FullName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = endPointUser.Id,
                            CreatedDate = endPointUser.CreatedDate,
                            UserId = endPointUser.DIUserId,
                            Entitlement = lnsGroups.Group,
                            EntitlementType = (int)EntitlementType.Group,
                            ExceptionType = ExceptionTypes.Added,
                            Version = endPointUser.Version
                        }).ToList();
                }
                if (endPoint.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                {
                    var groups = (from endPointUser in _context.SAPEPUserInfos
                        from sapepGroups in endPointUser.SAPEPGroups
                        where endPointUser.EndPointSystemId == _EndpointSystemId && sapepGroups.IsXLUploaded &&
                              _context.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapepGroups.GroupUniqueName) == null
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = endPointUser.UserName,
                            DisplayName = endPointUser.DisplayName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = endPointUser.Id,
                            CreatedDate = endPointUser.CreatedDate.Value,
                            UserId = endPointUser.DIUserId,
                            Entitlement = sapepGroups.GroupUniqueName,
                            EntitlementType = (int)EntitlementType.Group,
                            ExceptionType = ExceptionTypes.Added,
                            Version = endPointUser.Version
                        }).ToList();

                    var roles = (from endPointUser in _context.SAPEPUserInfos
                        from sapepRoles in endPointUser.SAPEPRoles
                        where endPointUser.EndPointSystemId == _EndpointSystemId && sapepRoles.IsXLUploaded &&
                              _context.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapepRoles.UniqueRoleName) == null
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = endPointUser.UserName,
                            DisplayName = endPointUser.DisplayName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = endPointUser.Id,
                            CreatedDate = endPointUser.CreatedDate.Value,
                            UserId = endPointUser.DIUserId,
                            Entitlement = sapepRoles.UniqueRoleName,
                            EntitlementType = (int)EntitlementType.Role,
                            ExceptionType = ExceptionTypes.Added,
                            Version = endPointUser.Version
                        }).ToList();

                    return roles.Union(groups).ToList();
                }
                if (endPoint.EndpointSystemTypeId == (int)EndpointType.SAP)
                {
                    var roles = (from endPointUser in _context.SAPUserinfos
                        from sapRoles in endPointUser.SAPRoleInfo
                        where endPointUser.EndPointSystemId == _EndpointSystemId &&
                              _context.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id && entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapRoles.ROLE_CODE) == null
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = endPointUser.UserLogonName,
                            DisplayName = endPointUser.Firstname + " " + endPointUser.Lastname,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = endPointUser.Id,
                            CreatedDate = endPointUser.CreatedDate,
                            UserId = endPointUser.DIUserId,
                            Entitlement = sapRoles.ROLE_CODE,
                            EntitlementType = (int)EntitlementType.Role,
                            ExceptionType = ExceptionTypes.Added,
                            Version = endPointUser.Version
                        }).ToList();

                    var positions = (from endPointUser in _context.SAPUserinfos
                        join sapPositions in _context.SAPHRP1001Entries
                            on new { p1 = endPointUser.UserLogonName, p2 = endPointUser.EndPointSystemId } equals
                            new { p1 = sapPositions.RelatedObjectCode, p2 = sapPositions.EndpointSystemId }
                        where endPointUser.EndPointSystemId == _EndpointSystemId && sapPositions.RelatedObjectType == "US" &&
                              _context.EntitlementReconcilations.FirstOrDefault(entitlementReconcile => entitlementReconcile.EndpointUserId == endPointUser.Id &&
                                                                                                        entitlementReconcile.EndpointSystemId == endPointUser.EndPointSystemId && entitlementReconcile.Entitlement == sapPositions.ActualObjectCode) == null
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = endPointUser.UserLogonName,
                            DisplayName = endPointUser.Firstname + " " + endPointUser.Lastname,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = endPointUser.Id,
                            CreatedDate = endPointUser.CreatedDate,
                            UserId = endPointUser.DIUserId,
                            Entitlement = sapPositions.ActualObjectCode,
                            EntitlementType = (int)EntitlementType.Position,
                            ExceptionType = ExceptionTypes.Added,
                            Version = endPointUser.Version
                        }).ToList();

                    return roles.Union(positions).ToList();
                }
            }

            return null;
        }

        public List<ReconcilationExceptionForEntitlmentsReport> GetEntitlementReconcilationRemovedExceptions(int _EndpointSystemId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                EndPoint endPoint = new EndPointDA().GetEndPointSystemByKey(_EndpointSystemId);

                if (endPoint.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
                {

                    return (from entitlementReconcile in _context.EntitlementReconcilations
                            join usr in _context.Users
                                on entitlementReconcile.UserId equals usr.Id into detail
                            from usr in detail.DefaultIfEmpty()
                            where entitlementReconcile.EndpointSystemId == _EndpointSystemId && entitlementReconcile.IsDeleted == false &&
                            _context.ADSUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                !item.ADSGroups.Any(item1 => item1.name == entitlementReconcile.Entitlement))
                            select new ReconcilationExceptionForEntitlmentsReport
                            {
                                AccountName = entitlementReconcile.AccountName,
                                DisplayName = usr.KnownAs,
                                EndpointSystemId = _EndpointSystemId,
                                EndpointUserId = entitlementReconcile.EndpointUserId,
                                CreatedDate = entitlementReconcile.EnteredDate,
                                UserId = entitlementReconcile.UserId,
                                Entitlement = entitlementReconcile.Entitlement,
                                EntitlementType = entitlementReconcile.EntitlementType,
                                Version = entitlementReconcile.Version,
                                ExceptionType = ExceptionTypes.Removed
                            }).ToList();
                }
                if (endPoint.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
                {
                    return (from entitlementReconcile in _context.EntitlementReconcilations
                        join usr in _context.Users
                            on entitlementReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where entitlementReconcile.EndpointSystemId == _EndpointSystemId && entitlementReconcile.IsDeleted == false &&
                              _context.LotusUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                                                  !item.LotusGroupInfos.Any(item1 => item1.Group == entitlementReconcile.Entitlement))
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = entitlementReconcile.AccountName,
                            DisplayName = usr.KnownAs,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = entitlementReconcile.EndpointUserId,
                            CreatedDate = entitlementReconcile.EnteredDate,
                            UserId = entitlementReconcile.UserId,
                            Entitlement = entitlementReconcile.Entitlement,
                            EntitlementType = entitlementReconcile.EntitlementType,
                            ExceptionType = ExceptionTypes.Removed,
                            Version = entitlementReconcile.Version
                        }).ToList();
                }
                if (endPoint.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                {
                    var groups = (from entitlementReconcile in _context.EntitlementReconcilations
                        join usr in _context.Users
                            on entitlementReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where entitlementReconcile.EndpointSystemId == _EndpointSystemId && entitlementReconcile.IsDeleted == false &&
                              entitlementReconcile.EntitlementType == (int)EntitlementType.Group &&
                              _context.SAPEPUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                                                  !item.SAPEPGroups.Any(item1 => item1.GroupUniqueName == entitlementReconcile.Entitlement))
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = entitlementReconcile.AccountName,
                            DisplayName = usr.KnownAs,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = entitlementReconcile.EndpointUserId,
                            CreatedDate = entitlementReconcile.EnteredDate,
                            UserId = entitlementReconcile.UserId,
                            Entitlement = entitlementReconcile.Entitlement,
                            EntitlementType = entitlementReconcile.EntitlementType,
                            ExceptionType = ExceptionTypes.Removed,
                            Version = entitlementReconcile.Version
                        }).ToList();

                    var roles = (from entitlementReconcile in _context.EntitlementReconcilations
                        join usr in _context.Users
                            on entitlementReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where entitlementReconcile.EndpointSystemId == _EndpointSystemId && entitlementReconcile.IsDeleted == false &&
                              entitlementReconcile.EntitlementType == (int)EntitlementType.Role &&
                              _context.SAPEPUserInfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                                                  !item.SAPEPRoles.Any(item1 => item1.UniqueRoleName == entitlementReconcile.Entitlement))
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = entitlementReconcile.AccountName,
                            DisplayName = usr.KnownAs,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = entitlementReconcile.EndpointUserId,
                            CreatedDate = entitlementReconcile.EnteredDate,
                            UserId = entitlementReconcile.UserId,
                            Entitlement = entitlementReconcile.Entitlement,
                            EntitlementType = entitlementReconcile.EntitlementType,
                            ExceptionType = ExceptionTypes.Removed,
                            Version = entitlementReconcile.Version
                        }).ToList();

                    return roles.Union(groups).ToList();
                }
                if (endPoint.EndpointSystemTypeId == (int)EndpointType.SAP)
                {
                    var roles = (from entitlementReconcile in _context.EntitlementReconcilations
                        join usr in _context.Users
                            on entitlementReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where entitlementReconcile.EndpointSystemId == _EndpointSystemId && entitlementReconcile.IsDeleted == false &&
                              entitlementReconcile.EntitlementType == (int)EntitlementType.Role &&
                              _context.SAPUserinfos.Any(item => item.Id == entitlementReconcile.EndpointUserId &&
                                                                !item.SAPRoleInfo.Any(item1 => item1.ROLE_CODE == entitlementReconcile.Entitlement))
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = entitlementReconcile.AccountName,
                            DisplayName = usr.KnownAs,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = entitlementReconcile.EndpointUserId,
                            CreatedDate = entitlementReconcile.EnteredDate,
                            UserId = entitlementReconcile.UserId,
                            Entitlement = entitlementReconcile.Entitlement,
                            EntitlementType = entitlementReconcile.EntitlementType,
                            ExceptionType = ExceptionTypes.Removed,
                            Version = entitlementReconcile.Version
                        }).ToList();

                    var positions = (from entitlementReconcile in _context.EntitlementReconcilations
                        join usr in _context.Users
                            on entitlementReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where entitlementReconcile.EndpointSystemId == _EndpointSystemId && entitlementReconcile.IsDeleted == false &&
                              entitlementReconcile.EntitlementType == (int)EntitlementType.Position &&
                              !_context.SAPHRP1001Entries.Any(item => item.RelatedObjectCode == entitlementReconcile.AccountName &&
                                                                      item.RelatedObjectType == "US" && item.EndpointSystemId == entitlementReconcile.EndpointSystemId &&
                                                                      item.ActualObjectCode == entitlementReconcile.Entitlement)
                        select new ReconcilationExceptionForEntitlmentsReport
                        {
                            AccountName = entitlementReconcile.AccountName,
                            DisplayName = usr.KnownAs,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = entitlementReconcile.EndpointUserId,
                            CreatedDate = entitlementReconcile.EnteredDate,
                            UserId = entitlementReconcile.UserId,
                            Entitlement = entitlementReconcile.Entitlement,
                            EntitlementType = entitlementReconcile.EntitlementType,
                            ExceptionType = ExceptionTypes.Removed,
                            Version = entitlementReconcile.Version
                        }).ToList();

                    return roles.Union(positions).ToList();
                }
            }

            return null;
        }

        public List<ReconcilationExceptionReport> GetAccountReconcilationAddedExceptions(int _EndpointSystemId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return (from Account in _context.AccountReconcilations
                        where Account.EndpointSystemId == _EndpointSystemId && Account.ExceptionType == ExceptionTypes.Added && Account.IsManuallyAdded == false
                        select new ReconcilationExceptionReport
                        {
                            AccountName = Account.AccountName,
                            DisplayName = Account.FullName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = Account.EndpointUserId,
                            CreatedDate = Account.CreatedDate,
                            UserId = Account.UserId,
                            Version = Account.Version,
                            ExceptionType = ExceptionTypes.Added
                        }
                        ).ToList();
            }
        }

        public List<ReconcilationExceptionReport> GetAccountReconcilationRemovedExceptions(int _EndpointSystemId)
        {
            EndPoint endpoint = new EndPointDA().GetEndPointSystemByKey(_EndpointSystemId);
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (endpoint.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
                {
                    return (from accountReconcile in _context.AccountReconcilations
                            join usr in _context.Users
                                on accountReconcile.UserId equals usr.Id into detail
                            from usr in detail.DefaultIfEmpty()
                            where accountReconcile.EndpointSystemId == _EndpointSystemId && accountReconcile.IsDeleted == false &&
                            !_context.ADSUserInfos.Any(item => item.Id == accountReconcile.EndpointUserId &&
                                item.EndPointSystemId == accountReconcile.EndpointSystemId)
                            select new ReconcilationExceptionReport
                            {
                                AccountName = accountReconcile.AccountName,
                                DisplayName = usr.DisplayName,
                                EndpointSystemId = _EndpointSystemId,
                                EndpointUserId = accountReconcile.EndpointUserId,
                                CreatedDate = accountReconcile.CreatedDate,
                                UserId = accountReconcile.UserId,
                                Version = accountReconcile.Version,
                                ExceptionType = ExceptionTypes.Removed
                            }
                           ).ToList();
                }
                if (endpoint.EndpointSystemTypeId == (int)EndpointType.SAP)
                {
                    return (from accountReconcile in _context.AccountReconcilations
                        join usr in _context.Users
                            on accountReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where accountReconcile.EndpointSystemId == _EndpointSystemId && accountReconcile.IsDeleted == false &&
                              !_context.SAPUserinfos.Any(item => item.Id == accountReconcile.EndpointUserId &&
                                                                 item.EndPointSystemId == accountReconcile.EndpointSystemId)
                        select new ReconcilationExceptionReport
                        {
                            AccountName = accountReconcile.AccountName,
                            DisplayName = usr.DisplayName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = accountReconcile.EndpointUserId,
                            CreatedDate = accountReconcile.CreatedDate,
                            UserId = accountReconcile.UserId,
                            Version = accountReconcile.Version,
                            ExceptionType = ExceptionTypes.Removed
                        }
                        ).ToList();
                }
                if (endpoint.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
                {
                    return (from accountReconcile in _context.AccountReconcilations
                        join usr in _context.Users
                            on accountReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where accountReconcile.EndpointSystemId == _EndpointSystemId && accountReconcile.IsDeleted == false &&
                              !_context.LotusUserInfos.Any(item => item.Id == accountReconcile.EndpointUserId &&
                                                                   item.EndPointSystemId == accountReconcile.EndpointSystemId)
                        select new ReconcilationExceptionReport
                        {
                            AccountName = accountReconcile.AccountName,
                            DisplayName = usr.DisplayName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = accountReconcile.EndpointUserId,
                            CreatedDate = accountReconcile.CreatedDate,
                            UserId = accountReconcile.UserId,
                            Version = accountReconcile.Version,
                            ExceptionType = ExceptionTypes.Removed
                        }
                        ).ToList();
                }
                if (endpoint.EndpointSystemTypeId == (int)EndpointType.InhouseSystems)
                {
                    return (from accountReconcile in _context.AccountReconcilations
                        join usr in _context.Users
                            on accountReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where accountReconcile.EndpointSystemId == _EndpointSystemId && accountReconcile.IsDeleted == false &&
                              !_context.InhouseUserInfos.Any(item => item.Id == accountReconcile.EndpointUserId &&
                                                                     item.EndPointSystemId == accountReconcile.EndpointSystemId)
                        select new ReconcilationExceptionReport
                        {
                            AccountName = accountReconcile.AccountName,
                            DisplayName = usr.DisplayName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = accountReconcile.EndpointUserId,
                            CreatedDate = accountReconcile.CreatedDate,
                            UserId = accountReconcile.UserId,
                            Version = accountReconcile.Version,
                            ExceptionType = ExceptionTypes.Removed
                        }
                        ).ToList();
                }
                if (endpoint.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                {
                    return (from accountReconcile in _context.AccountReconcilations
                        join usr in _context.Users
                            on accountReconcile.UserId equals usr.Id into detail
                        from usr in detail.DefaultIfEmpty()
                        where accountReconcile.EndpointSystemId == _EndpointSystemId && accountReconcile.IsDeleted == false &&
                              !_context.SAPEPUserInfos.Any(item => item.Id == accountReconcile.EndpointUserId &&
                                                                   item.EndPointSystemId == accountReconcile.EndpointSystemId)
                        select new ReconcilationExceptionReport
                        {
                            AccountName = accountReconcile.AccountName,
                            DisplayName = usr.DisplayName,
                            EndpointSystemId = _EndpointSystemId,
                            EndpointUserId = accountReconcile.EndpointUserId,
                            CreatedDate = accountReconcile.CreatedDate,
                            UserId = accountReconcile.UserId,
                            Version = accountReconcile.Version,
                            ExceptionType = ExceptionTypes.Removed
                        }
                        ).ToList();
                }
            }

            return null;
        }

        

        public List<AccountReconciliationReport> GetResolvedAccountExceptions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from AccountReconciliation in _context.AccountReconcilations
                            join endpoint in _context.EndpointSystems on AccountReconciliation.EndpointSystemId equals endpoint.Id
                            where AccountReconciliation.IsManuallyAdded
                            select new AccountReconciliationReport
                            {
                                AccountName = AccountReconciliation.AccountName,
                                Comments = AccountReconciliation.Comments,
                                EndpointSystemName = endpoint.DisplayName,
                                EndpointSystemType = endpoint.EndPointSystemType.DisplayName,
                                EnteredDate = AccountReconciliation.CreatedDate,
                                ResolvedBy = AccountReconciliation.ResolvedBy,
                                ExceptionType = AccountReconciliation.ExceptionType
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EntitlementReconciliationReport> GetResolvedEntitlementExceptions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from recon in _context.EntitlementReconcilations
                            join endpoint in _context.EndpointSystems on recon.EndpointSystemId equals endpoint.Id
                            where recon.IsManuallyAdded
                            select new EntitlementReconciliationReport
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OUTransferSnapshotGridData> GetOUChangeSnapshotByUser(List<int> actionUserIds)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<OUTransferSnapshotGridData> data = new List<OUTransferSnapshotGridData>();
                    IList<OUTransferSnapshotGridData> ouchangesnapshot = new List<OUTransferSnapshotGridData>();
                    if (actionUserIds != null && actionUserIds.Count > 0)
                    {
                        foreach (int userid in actionUserIds)
                        {
                            var list = (from OUC in _context.OUChangeSnapshots
                                        join Endpnt in _context.EndpointSystems
                                            on OUC.EndpointSystemId equals Endpnt.Id
                                        where OUC.UserId == userid
                                        select new OUTransferSnapshotGridData
                                        {
                                            SnapshotId = OUC.Id,
                                            EndpointSystemTypeID = Endpnt.EndpointSystemTypeId.Value
                                        }).ToList();
                            foreach (OUTransferSnapshotGridData lst in list)
                            {
                                ouchangesnapshot.Add(lst);
                            }
                        }
                    }
                    else
                    {
                        ouchangesnapshot = (from OUC in _context.OUChangeSnapshots
                                            join Endpnt in _context.EndpointSystems
                                                on OUC.EndpointSystemId equals Endpnt.Id
                                            select new OUTransferSnapshotGridData
                                            {
                                                SnapshotId = OUC.Id,
                                                EndpointSystemTypeID = Endpnt.EndpointSystemTypeId.Value
                                            }).ToList();
                    }

                    foreach (OUTransferSnapshotGridData item in ouchangesnapshot)
                    {
                        if (item.EndpointSystemTypeID == Convert.ToInt32(EndpointType.ActiveDirectory))
                        {
                            var account = (from OUC in _context.OUChangeSnapshots
                                           join user in _context.Users
                                               on OUC.UserId equals user.Id
                                           join Endpnt in _context.EndpointSystems
                                               on OUC.EndpointSystemId equals Endpnt.Id
                                           join endpntacc in _context.ADSUserInfos
                                                on OUC.EndpointUserId equals endpntacc.Id
                                           where OUC.Id == item.SnapshotId
                                           select new OUTransferSnapshotGridData
                                           {
                                               UserName = user.DisplayName,
                                               EmployeeNo = user.EmployeeId,
                                               EmailID = user.EmailId,
                                               EndpointsystemName = Endpnt.DisplayName,
                                               EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
                                               AccountName = endpntacc.sAMAccountName,
                                               EndpointUserID = OUC.EndpointUserId,
                                               Entitlement = OUC.Entitlement,
                                               EntitlementType = OUC.EntitlementType,
                                               CreatedDate = OUC.EnteredDate
                                           }).FirstOrDefault();
                            //foreach (OUTransferSnapshotGridData accitem in account)
                            //{
                            //    data.Add(item);
                            //}
                            if (account != null)
                            {
                                data.Add(account);
                            }
                        }
                        if (item.EndpointSystemTypeID == Convert.ToInt32(EndpointType.LotusNotes))
                        {
                            var account = (from OUC in _context.OUChangeSnapshots
                                           join user in _context.Users
                                               on OUC.UserId equals user.Id
                                           join Endpnt in _context.EndpointSystems
                                               on OUC.EndpointSystemId equals Endpnt.Id
                                           join endpntacc in _context.LotusUserInfos
                                                on OUC.EndpointUserId equals endpntacc.Id
                                           where OUC.Id == item.SnapshotId
                                           select new OUTransferSnapshotGridData
                                           {
                                               UserName = user.DisplayName,
                                               EmployeeNo = user.EmployeeId,
                                               EmailID = user.EmailId,
                                               EndpointsystemName = Endpnt.DisplayName,
                                               EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
                                               AccountName = endpntacc.InternetAddress,
                                               EndpointUserID = OUC.EndpointUserId,
                                               Entitlement = OUC.Entitlement,
                                               EntitlementType = OUC.EntitlementType,
                                               CreatedDate = OUC.EnteredDate
                                           });
                            foreach (OUTransferSnapshotGridData accitem in account)
                            {
                                if (accitem != null)
                                {
                                    data.Add(accitem);
                                }
                            }
                        }

                        if (item.EndpointSystemTypeID == Convert.ToInt32(EndpointType.InhouseSystems))
                        {
                            var account = (from OUC in _context.OUChangeSnapshots
                                           join user in _context.Users
                                               on OUC.UserId equals user.Id
                                           join Endpnt in _context.EndpointSystems
                                               on OUC.EndpointSystemId equals Endpnt.Id
                                           join endpntacc in _context.InhouseUserInfos
                                                on OUC.EndpointUserId equals endpntacc.Id
                                           where OUC.Id == item.SnapshotId
                                           select new OUTransferSnapshotGridData
                                           {
                                               UserName = user.DisplayName,
                                               EmployeeNo = user.EmployeeId,
                                               EmailID = user.EmailId,
                                               EndpointsystemName = Endpnt.DisplayName,
                                               EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
                                               AccountName = endpntacc.UserName,
                                               EndpointUserID = OUC.EndpointUserId,
                                               Entitlement = OUC.Entitlement,
                                               EntitlementType = OUC.EntitlementType,
                                               CreatedDate = OUC.EnteredDate
                                           });
                            foreach (OUTransferSnapshotGridData accitem in account)
                            {
                                if (accitem != null)
                                {
                                    data.Add(accitem);
                                }
                            }
                        }
                        if (item.EndpointSystemTypeID == Convert.ToInt32(EndpointType.SAP))
                        {
                            var account = (from OUC in _context.OUChangeSnapshots
                                           join user in _context.Users
                                               on OUC.UserId equals user.Id
                                           join Endpnt in _context.EndpointSystems
                                               on OUC.EndpointSystemId equals Endpnt.Id
                                           join endpntacc in _context.SAPUserinfos
                                                on OUC.EndpointUserId equals endpntacc.Id
                                           where OUC.Id == item.SnapshotId
                                           select new OUTransferSnapshotGridData
                                           {
                                               UserName = user.DisplayName,
                                               EmployeeNo = user.EmployeeId,
                                               EmailID = user.EmailId,
                                               EndpointsystemName = Endpnt.DisplayName,
                                               EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
                                               AccountName = endpntacc.UserLogonName,
                                               EndpointUserID = OUC.EndpointUserId,
                                               Entitlement = OUC.Entitlement,
                                               EntitlementType = OUC.EntitlementType,
                                               CreatedDate = OUC.EnteredDate
                                           });
                            foreach (OUTransferSnapshotGridData accitem in account)
                            {
                                if (accitem != null)
                                {
                                    data.Add(accitem);
                                }
                            }
                        }
                        if (item.EndpointSystemTypeID == Convert.ToInt32(EndpointType.SAPEnterprisePortal))
                        {
                            var account = (from OUC in _context.OUChangeSnapshots
                                           join user in _context.Users
                                               on OUC.UserId equals user.Id
                                           join Endpnt in _context.EndpointSystems
                                               on OUC.EndpointSystemId equals Endpnt.Id
                                           join endpntacc in _context.SAPEPUserInfos
                                                on OUC.EndpointUserId equals endpntacc.Id
                                           where OUC.Id == item.SnapshotId
                                           select new OUTransferSnapshotGridData
                                           {
                                               UserName = user.DisplayName,
                                               EmployeeNo = user.EmployeeId,
                                               EmailID = user.EmailId,
                                               EndpointsystemName = Endpnt.DisplayName,
                                               EndpointSystemType = Endpnt.EndPointSystemType.DisplayName,
                                               AccountName = endpntacc.UserName,
                                               EndpointUserID = OUC.EndpointUserId,
                                               Entitlement = OUC.Entitlement,
                                               EntitlementType = OUC.EntitlementType,
                                               CreatedDate = OUC.EnteredDate
                                           });
                            foreach (OUTransferSnapshotGridData accitem in account)
                            {
                                if (accitem != null)
                                {
                                    data.Add(accitem);
                                }
                            }
                        }
                    }

                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
