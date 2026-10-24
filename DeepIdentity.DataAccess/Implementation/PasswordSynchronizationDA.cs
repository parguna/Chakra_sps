using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using System.Data;
using System.Data.SqlClient;

using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace DeepIdentity.DataAccess
{
    public class PasswordSynchronizationDA
    {
        public List<EndpointInformation> GetEndpointsByDiUseridForPwdSync(int DIUserId)
        {
            EndpointInformation EndPointSystem = null;
            List<EndpointInformation> EndPointlist = new List<EndpointInformation>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var ADSquery = (from endpointsys in _context.EndpointSystems
                                    join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.EndpointSystemTypeId
                                    join usr in _context.ADSUserInfos on endpointsys.EndpointSystemId equals usr.EndPointSystemId
                                    where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary 
                                    || usr.AccountType == AccountType.Shared) && !usr.IsLocked && !usr.IsAccountDisabled
                                    && endpointsys.IsPasswordResetAllowed
                                    select new
                                    {
                                        Endpointsystemid = endpointsys.EndpointSystemId,
                                        EndPointSystemDescription = endpointsys.Description,
                                        endpointsys.TenantId,
                                        endpointsys.EndpointSystemTypeId,
                                        HostName = endpointsys.DisplayName,
                                        endpointsys.DisplayName,

                                        Name = endpointsystype.DisplayName,
                                        UserName = usr.sAMAccountName
                                    }).ToList();

                    var SAPquery = (from endpointsys in _context.EndpointSystems
                                    join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.EndpointSystemTypeId
                                    join usr in _context.SAPUserinfos on endpointsys.EndpointSystemId equals usr.EndPointSystemId
                                    where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary
                                    || usr.AccountType == AccountType.Shared) && !usr.IsLocked
                                    && endpointsys.IsPasswordResetAllowed
                                    select new
                                    {
                                        Endpointsystemid = endpointsys.EndpointSystemId,
                                        EndPointSystemDescription = endpointsys.Description,
                                        endpointsys.TenantId,
                                        endpointsys.EndpointSystemTypeId,
                                        HostName = endpointsys.DisplayName,
                                        endpointsys.DisplayName,

                                        Name = endpointsystype.DisplayName,
                                        UserName = usr.UserLogonName
                                    }).ToList();

                    var Lotusquery = (from endpointsys in _context.EndpointSystems
                                      join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.EndpointSystemTypeId
                                      join usr in _context.LotusUserInfos on endpointsys.EndpointSystemId equals usr.EndPointSystemId
                                      where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary
                                    || usr.AccountType == AccountType.Shared) && !usr.IsLocked
                                      && endpointsys.IsPasswordResetAllowed
                                      select new
                                      {
                                          Endpointsystemid = endpointsys.EndpointSystemId,
                                          EndPointSystemDescription = endpointsys.Description,
                                          endpointsys.TenantId,
                                          endpointsys.EndpointSystemTypeId,
                                          HostName = endpointsys.DisplayName,
                                          endpointsys.DisplayName,

                                          Name = endpointsystype.DisplayName,
                                          UserName = usr.ShortName
                                      }).ToList();

                    var SAPEPquery = (from endpointsys in _context.EndpointSystems
                                      join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.EndpointSystemTypeId
                                      join usr in _context.SAPEPUserInfos on endpointsys.EndpointSystemId equals usr.EndPointSystemId
                                      where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary
                                    || usr.AccountType == AccountType.Shared) && !usr.IsLocked
                                      && endpointsys.IsPasswordResetAllowed
                                      select new
                                      {
                                          Endpointsystemid = endpointsys.EndpointSystemId,
                                          EndPointSystemDescription = endpointsys.Description,
                                          endpointsys.TenantId,
                                          endpointsys.EndpointSystemTypeId,
                                          HostName = endpointsys.DisplayName,
                                          endpointsys.DisplayName,

                                          Name = endpointsystype.DisplayName, usr.UserName
                                      }).ToList();

                    var INHquery = (from endpointsys in _context.EndpointSystems
                                    join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.EndpointSystemTypeId
                                    join usr in _context.InhouseUserInfos on endpointsys.EndpointSystemId equals usr.EndPointSystemId
                                    where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary
                                    || usr.AccountType == AccountType.Shared) && !usr.IsLocked
                                    && endpointsys.IsPasswordResetAllowed
                                    select new
                                    {
                                        Endpointsystemid = endpointsys.EndpointSystemId,
                                        EndPointSystemDescription = endpointsys.Description,
                                        endpointsys.TenantId,
                                        endpointsys.EndpointSystemTypeId,
                                        HostName = endpointsys.DisplayName,
                                        endpointsys.DisplayName,

                                        Name = endpointsystype.DisplayName, usr.UserName
                                    }).ToList();

                    var listTotal1 = ADSquery.Union(SAPquery);
                    var listTotal2 = listTotal1.Union(Lotusquery);
                    var listTotal3 = listTotal2.Union(SAPEPquery);
                    var listTotal = listTotal3.Union(INHquery);

                    if (listTotal.Count() > 0)
                    {
                        foreach (var tmpList in listTotal)
                        {
                            EndPointSystem = new EndpointInformation();
                            EndPointSystem.Endpointsystemid = CheckNull.AgainstInt(tmpList.Endpointsystemid);
                            EndPointSystem.Endpointsystemdescription = CheckNull.AgainstString(tmpList.EndPointSystemDescription);
                            EndPointSystem.TenantId = CheckNull.AgainstString(tmpList.TenantId); // AgainstInt
                            EndPointSystem.EndpointsystemType = CheckNull.AgainstString(tmpList.Name);
                            EndPointSystem.EndpointSystemTypeId = CheckNull.AgainstInt(tmpList.EndpointSystemTypeId);
                            EndPointSystem.HostName = CheckNull.AgainstString(tmpList.HostName);
                            EndPointSystem.UserName = CheckNull.AgainstString(tmpList.UserName);
                            EndPointSystem.Status = string.Empty;
                            EndPointlist.Add(EndPointSystem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the EndPointSystem", ex);
                throw ex;
            }
            return EndPointlist;
        }
        public EndPoint GetEndPointSystemByKey(int EndPointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //return (from ep in _context.EndpointSystems
                    //            .Include("EndPointSystemType")

                    //        where ep.EndpointSystemId == EndPointSystemId
                    //        select ep).FirstOrDefault<EndPoint>();
                    return _context.EndpointSystems.Include("EndPointSystemType").Where(ep => ep.EndpointSystemId == EndPointSystemId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the EndpointSystem by Id", ex);
                throw ex;
            }            
        }
        public MailTemplate GetMailTemplate(string TemplateName)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.MailTemplate.Where(temp => temp.TemplateName == TemplateName).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Fetching Get MailTemplete", ex);
                throw ex;
            }
        }
        public EndpointType GetEndPointTypeName(int EndpointTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType eType = new EndPointSystemType();
                    eType = _context.EndpointSystemTypes.Find(EndpointTypeId);
                    return eType.EndpointSystemTypeName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
      
    }
}
