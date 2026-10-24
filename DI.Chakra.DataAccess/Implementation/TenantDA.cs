using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;

using DI.Chakra.Infrastructure;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class TenantDA
    {
        //public List<Tenant> GetAllTenant()
        //{
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            return Context.Tenants.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Get All Teanant", ex);
        //        throw ex;
        //    }
        //}
        //public Tenant GetTenantById(int _id)
        //{
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            return Context.Tenants.Where(t => t.Id == _id).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Get All Teanant", ex);
        //        throw ex;
        //    }
        //}
        //public void SaveTenant(Tenant _tenant)
        //{
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            if (_tenant.Id == 0)
        //            {
        //                Context.Tenants.Attach(_tenant);
        //                Context.Entry(_tenant).State = EntityState.Added;
        //                Context.SaveChanges();
        //            }
        //            else
        //            {
        //                Tenant editTenant = Context.Tenants.Where(uTent => uTent.Id ==_tenant.Id).First();
        //                if (editTenant == null)
        //                    return;
        //                Context.Entry(editTenant).CurrentValues.SetValues(_tenant);
        //                Context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Saving the Tenant", ex);
        //        throw ex;
        //    }
        //}
        //public void DeleteTenant(int _id)
        //{
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            var tenant = Context.Tenants.First(atent => atent.Id == _id);

        //            if (tenant == null)
        //                return;

        //            Context.Tenants.Remove(tenant);
        //            Context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Deleting  the Tenant", ex);
        //        throw ex;
        //    }
        //}
    }
}
