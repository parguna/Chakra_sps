using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public interface IIMProcessor
    {
        ProvisioningResult CreateAccount(ProvisioningParamCollection fields);

        ProvisioningResult DeleteAccount(ProvisioningParamCollection fields);

        ProvisioningResult LockAccount(ProvisioningParamCollection fields);

        ProvisioningResult DisableAccount(ProvisioningParamCollection fields);

        ProvisioningResult UnLockAccount(ProvisioningParamCollection fields);

        ProvisioningResult EnableAccount(ProvisioningParamCollection fields);


        ProvisioningResult AddEntitlement(ProvisioningParamCollection fields);

        ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields);

        ProvisioningResult ResetPassword(ProvisioningParamCollection fields);

        ProvisioningResult UpdateAccount(ProvisioningParamCollection fields);

        ProvisioningResult UpdateAccountValidity(ProvisioningParamCollection fields);

        ProvisioningResult UpdateEntitlementValidity(ProvisioningParamCollection fields);

        ProvisioningResult AddEntitlementForOneWayTrustUser(ProvisioningParamCollection fields);

        ProvisioningResult RemoveEntitlementForOneWayTrustUser(ProvisioningParamCollection fields);

    }
}
