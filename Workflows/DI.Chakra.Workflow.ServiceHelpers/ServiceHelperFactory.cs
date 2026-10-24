
using DI.Chakra.Workflow.ServiceHelpers.Attestation;
using DI.Chakra.Workflow.ServiceHelpers.Provisioning;

namespace DI.Chakra.Workflow.ServiceHelpers
{
    public class ServiceHelperFactory
    {

        public static IAttestattionServiceHelper GetAttestationServiceHelper()
        {
            return (IAttestattionServiceHelper)new AttestationApprovalWFServiceHelper();
        }

        public static IProvisioningServiceHelper GetProvisioningServiceHelper()
        {
            return (IProvisioningServiceHelper)new ProvisioningApprovalWFServiceHelper();
        }

    }
}
