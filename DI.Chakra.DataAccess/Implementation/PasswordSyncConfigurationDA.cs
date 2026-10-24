using DI.Chakra.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DI.Chakra.DataAccess
{
    public class PasswordSyncConfigurationDA
    {

        public void CreatePasswordSyncConfiguration(PasswordSyncConfiguration pwdSyncConfig)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.PasswordSyncConfigurations.Add(pwdSyncConfig);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void CreatePasswordSyncConfigurationDetail(PasswordSyncConfigurationDetail pwdSyncConfigDetail)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.PasswordSyncConfigurationDetails.Add(pwdSyncConfigDetail);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePasswordSyncConfiguration(PasswordSyncConfiguration pwdSyncConfig)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var configEntry = _context.Entry(pwdSyncConfig);
                    if (configEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<PasswordSyncConfiguration>();
                        PasswordSyncConfiguration attachedConfig = set.Find(pwdSyncConfig.Id);

                        if (attachedConfig != null)
                        {
                            var attachedEntry = _context.Entry(attachedConfig);
                            attachedEntry.CurrentValues.SetValues(pwdSyncConfig);
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

        public void UpdatePasswordSyncConfigurationDetail(PasswordSyncConfigurationDetail pwdSyncConfigDetail)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var configDetailEntry = _context.Entry(pwdSyncConfigDetail);
                    if (configDetailEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<PasswordSyncConfigurationDetail>();
                        PasswordSyncConfigurationDetail attachedconfigDetail = set.Find(pwdSyncConfigDetail.Id);

                        if (attachedconfigDetail != null)
                        {
                            var attachedEntry = _context.Entry(attachedconfigDetail);
                            attachedEntry.CurrentValues.SetValues(pwdSyncConfigDetail);
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

        public PasswordSyncConfiguration GetPasswordSyncConfiguration(bool deepLoad)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    if (!deepLoad)
                    {
                        return _context.PasswordSyncConfigurations.FirstOrDefault();
                    }
                    return (from pwdSyncConfig in _context.PasswordSyncConfigurations
                        .Include("PasswordSyncConfigurationDetails")
                        select pwdSyncConfig).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        //public List<PasswordSyncConfigGridData> GetPasswordSyncConfigDetails()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return (from endpoint in _context.EndpointSystems
        //                    join endpointType in _context.EndpointSystemTypes
        //                        on endpoint.EndpointSystemTypeId equals endpointType.Id
        //                    join pwdSyncConfigDetail in _context.PasswordSyncConfigurationDetails
        //                        on endpoint.Id equals pwdSyncConfigDetail.EndpointSystemId into detail
        //                    from pwdSyncConfigDetail in detail.DefaultIfEmpty()
        //                    select new PasswordSyncConfigGridData
        //                    {
        //                        AllowSync = pwdSyncConfigDetail.AllowSync,
        //                        ConfigId = pwdSyncConfigDetail.ConfigId,
        //                        ConfigDetailId = pwdSyncConfigDetail.Id,
        //                        EndPointSystemDescription = endpoint.Description,
        //                        EndPointSystemId = endpoint.Id,
        //                        EndPointSystemTypeId = endpointType.Id,
        //                        EndpointSystemTypeName = endpointType.DisplayName,
        //                        HostName = endpoint.DisplayName
        //                    }).ToList();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

    }
}
