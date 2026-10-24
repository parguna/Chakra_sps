using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model;
using DI.Chakra.Model.SAP;
using DI.Chakra.Infrastructure.Enums;
using System.Data.Entity;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class PasswordPolicyDA
    {
        public IList<PasswordPolicy> FindAllPolicies()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    return _context.PasswordPolicies.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ChangePassword(string _NewPwd, string username)
        {
            //string query = "DI_spChangePassword";
            int result = 0;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<User> pwdChangingUser = oIACMDBContext.Users.Where(usr => usr.UserName == username).ToList();
                    if (pwdChangingUser.Count() > 0)
                    {
                        pwdChangingUser[0].Password = _NewPwd;
                        result = oIACMDBContext.SaveChanges();
                    }
                    if (result > 0)
                        return true;
                    return false;
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetIACMLastPasswordChangeByEndpointUser(long EndPointSystemId, string UserName)
        {
            try
            {
                string PwdLastModifiedDate = string.Empty; ;

                using (IACMDBContext _context = new IACMDBContext())
                {
                    long _endPointSystemTypeId = _context.EndpointSystems.FirstOrDefault(eps => eps.Id == EndPointSystemId).EndpointSystemTypeId.Value;
                    EndpointType _endPointName = _context.EndpointSystemTypes.FirstOrDefault(eps => eps.Id == _endPointSystemTypeId).EndpointSystemTypeName;

                    DateTime? IACMLastPasswordChange = null; // DateTime.Now;
                    if (_endPointName == EndpointType.ActiveDirectory)
                    {
                        ADSUserInfo userInfo = null;
                        userInfo = _context.ADSUserInfos.FirstOrDefault(usr => usr.sAMAccountName == UserName && usr.EndPointSystemId == EndPointSystemId);
                        if (userInfo != null)
                            IACMLastPasswordChange = userInfo.IACMLastPasswordChange;
                    }
                    else if (_endPointName == EndpointType.SAP)
                    {
                        SAPUserinfo userInfo = null;
                        userInfo = _context.SAPUserinfos.FirstOrDefault(usr => usr.UserLogonName == UserName && usr.EndPointSystemId == EndPointSystemId);
                        if (userInfo != null)
                            IACMLastPasswordChange = userInfo.IACMLastPasswordChange;
                    }
                    else if (_endPointName == EndpointType.LotusNotes)
                    {
                        LotusUserInfo userInfo = null;
                        userInfo = _context.LotusUserInfos.FirstOrDefault(usr => usr.ShortName == UserName && usr.EndPointSystemId == EndPointSystemId);
                        if (userInfo != null)
                            IACMLastPasswordChange = userInfo.IACMLastPasswordChange;
                    }
                    else if (_endPointName == EndpointType.SAPEnterprisePortal)
                    {
                        SAPEPUserInfo userInfo = null;
                        userInfo = _context.SAPEPUserInfos.FirstOrDefault(usr => usr.UserName == UserName && usr.EndPointSystemId == EndPointSystemId);
                        if (userInfo != null)
                            IACMLastPasswordChange = userInfo.IACMLastPasswordChange;
                    }
                    if (IACMLastPasswordChange == null)
                        PwdLastModifiedDate = null;
                    else
                        PwdLastModifiedDate = IACMLastPasswordChange.Value.ToString();
                }

                return PwdLastModifiedDate;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Retreiving  the IACMLastPasswordChange", ex);
                throw ex;
            }
        }

        public void UpdateIACMLastPasswordChange(int EndpointId, string UserName)
        {
            try
            {
                int recordsAffected = -1;
                using (IACMDBContext _context = new IACMDBContext())
                {
                    long _endPointSystemTypeId = _context.EndpointSystems.FirstOrDefault(eps => eps.Id == EndpointId).EndpointSystemTypeId.Value;
                    EndpointType _endPointName = _context.EndpointSystemTypes.FirstOrDefault(eps => eps.Id == _endPointSystemTypeId).EndpointSystemTypeName;

                    if (_endPointName == EndpointType.ActiveDirectory)
                    {
                        ADSUserInfo userInfo = null;
                        userInfo = _context.ADSUserInfos.FirstOrDefault(usr => usr.sAMAccountName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            userInfo.IACMLastPasswordChange = DateTime.Now;
                            recordsAffected = _context.SaveChanges();
                        }
                    }
                    else if (_endPointName == EndpointType.SAP)
                    {
                        SAPUserinfo userInfo = null;
                        userInfo = _context.SAPUserinfos.FirstOrDefault(usr => usr.UserLogonName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            userInfo.IACMLastPasswordChange = DateTime.Now;
                            recordsAffected = _context.SaveChanges();
                        }
                    }
                    else if (_endPointName == EndpointType.LotusNotes)
                    {
                        LotusUserInfo userInfo = null;
                        userInfo = _context.LotusUserInfos.FirstOrDefault(usr => usr.ShortName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            userInfo.IACMLastPasswordChange = DateTime.Now;
                            recordsAffected = _context.SaveChanges();
                        }
                    }
                    else if (_endPointName == EndpointType.SAPEnterprisePortal)
                    {
                        SAPEPUserInfo userInfo = null;
                        userInfo = _context.SAPEPUserInfos.FirstOrDefault(usr => usr.UserName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            userInfo.IACMLastPasswordChange = DateTime.Now;
                            recordsAffected = _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while updating IACM Last Password Change.", ex);
                throw ex;
            }
        }

        public void SavePasswordPolicyEndPoint(List<PasswordPolicyEndpoint> _PasswordPolicyEndPoint)
        {
            try
            {
                long policyId = _PasswordPolicyEndPoint.FirstOrDefault().PolicyId;
                using (IACMDBContext Context = new IACMDBContext())
                {
                    List<PasswordPolicyEndpoint> policyEndPoints = Context.PasswordPolicyEndpoints.Where(p => p.PolicyId == policyId).ToList();
                    if (policyEndPoints.Count > 0)
                    {
                        foreach (PasswordPolicyEndpoint policyEndPoint in policyEndPoints)
                        {
                            Context.PasswordPolicyEndpoints.Remove(policyEndPoint);
                        }
                    }
                    foreach (PasswordPolicyEndpoint endPoint in _PasswordPolicyEndPoint)
                    {
                        Context.PasswordPolicyEndpoints.Add(endPoint);
                    }
                    Context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Saving the Group", ex);
                throw ex;
            }
        }

        public void SavePolicy(PasswordPolicy _PasswordPolicy)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    if (_PasswordPolicy.Id == 0)
                    {
                        Context.PasswordPolicies.Attach(_PasswordPolicy);
                        Context.Entry(_PasswordPolicy).State = EntityState.Added;
                        Context.SaveChanges();
                    }
                    else
                    {
                        PasswordPolicy editPolicy = Context.PasswordPolicies.Where(policy => policy.Id == _PasswordPolicy.Id).First();
                        if (editPolicy == null)
                            return;
                        Context.Entry(editPolicy).CurrentValues.SetValues(_PasswordPolicy);
                        Context.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Saving the Group", ex);
                throw ex;
            }
        }

        public PasswordPolicy GetPolicy(long PolicyId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    return _context.PasswordPolicies
                        .FirstOrDefault(pp => pp.Id == PolicyId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeletePasswordPolicy(int _PolicyId)
        {
            int RecordCount = 0;
            PasswordPolicy delPolicy = new PasswordPolicy();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    delPolicy = _context.PasswordPolicies.FirstOrDefault(pp => pp.Id == _PolicyId);
                    if (delPolicy != null)
                    {
                        _context.PasswordPolicies.Remove(delPolicy);
                        RecordCount = _context.SaveChanges();
                    }
                }
                if (RecordCount > 0)
                    return true;
                return false;
            }

            catch (Exception)
            {
                throw;
            }
        }
        public IList<EndPointSystemType> GetAllEndPointType()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystemTypes
                        .Include("EndPoints").Where(eps => eps.IsActive == true).ToList();

                }
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,ex.Message, ex);
                throw ex;
            }
        }
        public List<EndPoint> GetAllEndPointByType(int EndPointSystemTypeId, string HostName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<EndPoint> list = new List<EndPoint>();
                    if (!string.IsNullOrEmpty(HostName) && EndPointSystemTypeId > -1)
                        list = _context.EndpointSystems.Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId && s.DisplayName == HostName).ToList();
                    else
                    {
                        if (EndPointSystemTypeId > -1)
                            list = _context.EndpointSystems.Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId).ToList();
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the EndPointSystem", ex);
                throw ex;
            }

        }

        public PasswordPolicy FindAllPoliciesById(int EndpointSystemTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from policy in _context.PasswordPolicies
                            join epPolicy in _context.PasswordPolicyEndpoints
                                on policy.Id equals epPolicy.PolicyId
                            join ep in _context.EndpointSystems
                                on epPolicy.EndpointSystemId equals ep.Id
                            where ep.EndpointSystemTypeId == EndpointSystemTypeId
                            select policy).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }
        public PasswordPolicy FindAllPoliciesById(int EndpointSystemTypeId, int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from policy in _context.PasswordPolicies
                            join epPolicy in _context.PasswordPolicyEndpoints
                                on policy.Id equals epPolicy.PolicyId
                            join ep in _context.EndpointSystems
                                on epPolicy.EndpointSystemId equals ep.Id
                            where ep.EndpointSystemTypeId == EndpointSystemTypeId && ep.Id == EndpointSystemId
                            select policy).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public PasswordPolicy FindAllPoliciesByEndpointSystemId(long EndpointSystemId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return (from policy in _context.PasswordPolicies
                        join epPolicy in _context.PasswordPolicyEndpoints
                            on policy.Id equals epPolicy.PolicyId
                        join ep in _context.EndpointSystems
                            on epPolicy.EndpointSystemId equals ep.Id
                        where ep.Id == EndpointSystemId
                        select policy).FirstOrDefault();
            }
        }


        public List<PasswordPolicyEndpoint> GetPasswordPolicyEndpointByPolicyId(long PolicyId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    return _context.PasswordPolicyEndpoints.Include(lq=> lq.Endpoint)
                        .Where(pp => pp.PolicyId == PolicyId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
