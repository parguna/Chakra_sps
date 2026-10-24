using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class IMProcessor : IIMProcessor
    {
        public virtual ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public virtual ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult OUChangeDeleteAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public virtual ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public virtual ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public virtual ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public virtual ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public virtual ProvisioningResult OUChangeRemoveEntitlement(ProvisioningParamCollection fields) 
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public virtual ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult UpdateAccountValidity(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult UpdateEntitlementValidity(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult AddEntitlementForOneWayTrustUser(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual ProvisioningResult RemoveEntitlementForOneWayTrustUser(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public virtual object RunTheMethod(Action methodName)
        {
            return null;
        }

        public virtual ProvisioningResult AddMailBoxPermissions(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

    }
}
