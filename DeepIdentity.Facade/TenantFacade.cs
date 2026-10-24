using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeepIdentity.Model;
using DeepIdentity.Infrastructure;
using System.Collections;
using DeepIdentity.DataAccess;

using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.Facade
{
    public class TenantFacade
    {
        public TenantDA TenantDA { get; set; }

        public TenantFacade()
        {
            TenantDA = new TenantDA();
        }
        public Tenant GetTenantById(int _id)
        {
            return TenantDA.GetTenantById(_id);
        }
        public bool IsTenantIdAlreadyExists(string _name)
        {
            var tenant = TenantDA.GetAllTenant().Where(t => t.TenantId == _name).FirstOrDefault();

            if (tenant == null)
                return false;

            return true;
        }
        public IList<Tenant> GetAllTenant()
        {
            try
            {
                return TenantDA.GetAllTenant();
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        public void SaveTenant(Tenant _tenant)
        {
            try
            {
                TenantDA.SaveTenant(_tenant);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteTenant(int Id)
        {
            try
            {
                TenantDA.DeleteTenant(Id);
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    
}
