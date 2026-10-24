using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors
{
    public class FormExecutorFactory
    {

        public static FormExecutorBase Get(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger)
        {
            if (currentRequest.AccountType == AccountType.Shared)
            {
                return GetSharedAccount(endpoint, currentRequest, logger);
            }
            else if (currentRequest.AccountType == AccountType.Service)
            {
                return GetServiceAccount(endpoint, currentRequest, logger);
            }
            else if (currentRequest.AccountType == AccountType.Secondary)
            {
                return GetSecondaryAccount(endpoint, currentRequest, logger);
            }
            else
            {
                return GetUserAccount(endpoint, currentRequest, logger);
            }
        }

        static FormExecutorBase GetUserAccount(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger)
        {
            FormExecutorBase formExecutor = null;
            switch ((EndpointType)endpoint.EndpointSystemTypeId)
            {

                case EndpointType.ActiveDirectory:
                    if (endpoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                    {
                        formExecutor = new User.SPLocal(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAPLocal)
                    {
                        formExecutor = new User.SAPLocal(endpoint, currentRequest, logger);
                    }
                    break;

                case EndpointType.Office365:
                    formExecutor = new User.O365(endpoint, currentRequest, logger);
                    break;

                case EndpointType.SAP:

                    if (endpoint.ApplicationType == ApplicationType.HRMS_FLINS)
                    {
                        formExecutor = new User.FLINS(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.FLINS_BI)
                    {
                        formExecutor = new User.FLINSBI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_GRC)
                    {
                        formExecutor = new User.GRC(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_PI)
                    {
                        formExecutor = new User.PI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_CRM)
                    {
                        formExecutor = new User.MSSLCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        formExecutor = new User.MSSLISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_CRM)
                    {
                        formExecutor = new User.EBSCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        formExecutor = new User.EBSISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SRP)
                    {
                        formExecutor = new User.SRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SUS)
                    {
                        formExecutor = new User.SRM(endpoint, currentRequest, logger);
                    }
                    else
                    {
                        //formExecutor = new SAP(endpoint, currentRequest);
                    }
                    break;

                case EndpointType.SAPEnterprisePortal:

                    if (endpoint.ApplicationType == ApplicationType.SAP_EP)
                    {
                        formExecutor = new User.SAPEP(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.CE)
                    {
                        formExecutor = new User.SAPCE(endpoint, currentRequest, logger);

                    }
                    break;

                case EndpointType.GEMS:
                    formExecutor = new User.GEMS(endpoint, currentRequest, logger);
                    break;

                case EndpointType.NPAC:
                    formExecutor = new User.NPAC(endpoint, currentRequest, logger);
                    break;

                case EndpointType.InhouseSystems:
                    formExecutor = new User.GeneralFormExecutor(endpoint, currentRequest, logger);
                    break;

                case EndpointType.OneCard:
                    formExecutor = new User.SMS(endpoint, currentRequest, logger);
                    break;

                case EndpointType.SRM:
                    formExecutor = new User.SRM(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SSIDS:
                    formExecutor = new User.SSIDS(endpoint, currentRequest, logger);
                    break;
                case EndpointType.IDMS:
                    formExecutor = new User.IDMS(endpoint, currentRequest, logger);
                    break;
                case EndpointType.VPN:
                    formExecutor = new User.VPNBase(endpoint, currentRequest, logger);
                    break;
                case EndpointType.EAM_NOVA:
                    formExecutor = new User.NOVA(endpoint, currentRequest, logger);
                    break;
                case EndpointType.MicrosoftTeams:
                    formExecutor = new SkypeForBusinessBase(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SAPHANA:
                     if (endpoint.ApplicationType == ApplicationType.MSSL)
                    {
                        formExecutor = new User.SAPHANAMSSL(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS)
                    {
                        formExecutor = new User.SAPHANAEBS(endpoint, currentRequest, logger);
                    }
                    break;
                default:
                    break;
            }

            return formExecutor;
        }

        static FormExecutorBase GetSharedAccount(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger)
        {
            FormExecutorBase formExecutor = null;

            switch ((EndpointType)endpoint.EndpointSystemTypeId)
            {

                case EndpointType.ActiveDirectory:
                    if (endpoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                    {
                        formExecutor = new Shared.SPLocal(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAPLocal)
                    {
                        formExecutor = new Shared.SAPLocal(endpoint, currentRequest, logger);
                    }
                    break;

                case EndpointType.Office365:
                    formExecutor = new Shared.O365(endpoint, currentRequest, logger);
                    break;

                case EndpointType.SAP:

                    if (endpoint.ApplicationType == ApplicationType.HRMS_FLINS)
                    {
                        formExecutor = new Shared.FLINS(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.FLINS_BI)
                    {
                        formExecutor = new Shared.FLINSBI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_GRC)
                    {
                        formExecutor = new Shared.GRC(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_PI)
                    {
                        formExecutor = new Shared.PI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_CRM)
                    {
                        formExecutor = new Shared.MSSLCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        formExecutor = new Shared.MSSLISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_CRM)
                    {
                        formExecutor = new Shared.EBSCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        formExecutor = new Shared.EBSISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SRP)
                    {
                        formExecutor = new Shared.SRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SUS)
                    {
                        formExecutor = new Shared.SRM(endpoint, currentRequest, logger);
                    }
                    else
                    {
                        ////formExecutor = new SAP(endpoint);
                    }
                    break;

                case EndpointType.SAPEnterprisePortal:

                    if (endpoint.ApplicationType == ApplicationType.SAP_EP)
                    {
                        formExecutor = new Shared.SAPEP(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.CE)
                    {
                        formExecutor = new Shared.SAPCE(endpoint, currentRequest, logger);
                    }

                    break;

                case EndpointType.GEMS:
                    formExecutor = new Shared.GEMS(endpoint, currentRequest, logger);
                    break;

                case EndpointType.SRM:
                    formExecutor = new Shared.SRM(endpoint, currentRequest, logger);
                    break;

                case EndpointType.InhouseSystems:
                    formExecutor = new User.GeneralFormExecutor(endpoint, currentRequest, logger);
                    break;
                case EndpointType.EAM_NOVA:
                    formExecutor = new User.NOVA(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SAPHANA:
                    if (endpoint.ApplicationType == ApplicationType.MSSL)
                    {
                        formExecutor = new Shared.SAPHANAMSSL(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS)
                    {
                        formExecutor = new Shared.SAPHANAEBS(endpoint, currentRequest, logger);
                    }
                    break;
                default:
                    break;
            }

            return formExecutor;
        }

        static FormExecutorBase GetServiceAccount(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger)
        {
            FormExecutorBase formExecutor = null;
            switch ((EndpointType)endpoint.EndpointSystemTypeId)
            {

                case EndpointType.ActiveDirectory:
                    if (endpoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                    {
                        formExecutor = new Service.SPLocal(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAPLocal)
                    {
                        formExecutor = new Service.SAPLocal(endpoint, currentRequest, logger);
                    }
                    break;

                case EndpointType.SAP:

                    if (endpoint.ApplicationType == ApplicationType.HRMS_FLINS)
                    {
                        formExecutor = new Service.FLINS(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.FLINS_BI)
                    {
                        formExecutor = new Service.FLINSBI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_GRC)
                    {
                        formExecutor = new Service.GRC(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_PI)
                    {
                        formExecutor = new Service.PI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_CRM)
                    {
                        formExecutor = new Shared.MSSLCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        formExecutor = new Shared.MSSLISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_CRM)
                    {
                        formExecutor = new Shared.EBSCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        formExecutor = new Shared.EBSISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SRP)
                    {
                        formExecutor = new Service.SRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SUS)
                    {
                        formExecutor = new Service.SRM(endpoint, currentRequest, logger);
                    }
                    else
                    {
                        ////formExecutor = new SAP(endpoint);
                    }
                    break;

                case EndpointType.SAPEnterprisePortal:

                    if (endpoint.ApplicationType == ApplicationType.SAP_EP)
                    {
                        formExecutor = new Service.SAPEP(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.CE)
                    {
                        formExecutor = new Service.SAPCE(endpoint, currentRequest, logger);
                    }

                    break;

                case EndpointType.GEMS:
                    formExecutor = new Service.GEMS(endpoint, currentRequest, logger);
                    break;

                case EndpointType.SRM:
                    formExecutor = new Service.SRM(endpoint, currentRequest, logger);
                    break;

                case EndpointType.InhouseSystems:
                    formExecutor = new User.GeneralFormExecutor(endpoint, currentRequest, logger);
                    break;
                case EndpointType.EAM_NOVA:
                    formExecutor = new User.NOVA(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SAPHANA:
                    if (endpoint.ApplicationType == ApplicationType.MSSL)
                    {
                        formExecutor = new Shared.SAPHANAMSSL(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS)
                    {
                        formExecutor = new Shared.SAPHANAEBS(endpoint, currentRequest, logger);
                    }
                    break;
                default:
                    break;
            }

            return formExecutor;
        }

        static FormExecutorBase GetSecondaryAccount(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger)
        {
            FormExecutorBase formExecutor = null;
            switch ((EndpointType)endpoint.EndpointSystemTypeId)
            {

                case EndpointType.ActiveDirectory:
                    if (endpoint.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                    {
                        formExecutor = new Secondary.SPLocal(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAPLocal)
                    {
                        formExecutor = new Secondary.SAPLocal(endpoint, currentRequest, logger);
                    }
                    break;

                case EndpointType.SAP:

                    if (endpoint.ApplicationType == ApplicationType.HRMS_FLINS)
                    {
                        formExecutor = new Secondary.FLINS(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.FLINS_BI)
                    {
                        formExecutor = new Secondary.FLINSBI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_GRC)
                    {
                        formExecutor = new Secondary.GRC(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_PI)
                    {
                        formExecutor = new Secondary.PI(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_CRM)
                    {
                        formExecutor = new Shared.MSSLCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        formExecutor = new Shared.MSSLISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_CRM)
                    {
                        formExecutor = new Shared.EBSCRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        formExecutor = new Shared.EBSISU(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SRP)
                    {
                        formExecutor = new Secondary.SRM(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.SAP_SUS)
                    {
                        formExecutor = new Secondary.SRM(endpoint, currentRequest, logger);
                    }
                    else
                    {
                        ////formExecutor = new SAP(endpoint);
                    }
                    break;

                case EndpointType.SAPEnterprisePortal:

                    if (endpoint.ApplicationType == ApplicationType.SAP_EP)
                    {
                        formExecutor = new Secondary.SAPEP(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.CE)
                    {
                        formExecutor = new Secondary.SAPCE(endpoint, currentRequest, logger);
                    }

                    break;

                case EndpointType.GEMS:
                    formExecutor = new Secondary.GEMS(endpoint, currentRequest, logger);
                    break;
                case EndpointType.NPAC:
                    formExecutor = new Secondary.NPAC(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SRM:
                    formExecutor = new Secondary.SRM(endpoint, currentRequest, logger);
                    break;

                case EndpointType.InhouseSystems:
                    formExecutor = new User.GeneralFormExecutor(endpoint, currentRequest, logger);
                    break;
                case EndpointType.EAM_NOVA:
                    formExecutor = new User.NOVA(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SAPHANA:
                    if (endpoint.ApplicationType == ApplicationType.MSSL)
                    {
                        formExecutor = new Shared.SAPHANAMSSL(endpoint, currentRequest, logger);
                    }
                    else if (endpoint.ApplicationType == ApplicationType.EBS)
                    {
                        formExecutor = new Shared.SAPHANAEBS(endpoint, currentRequest, logger);
                    }
                    break;
                default:
                    break;
            }

            return formExecutor;
        }

    }
}
