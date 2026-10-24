using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System.Collections.Generic;
using System.Linq;

namespace DeepIdentity.Facade
{
    public class ProvisioningFormFacade
    {
        ProvisioningFormDA provisioningFormDA = null;

        public ProvisioningFormFacade()
        {
            provisioningFormDA = new ProvisioningFormDA();
        }

        public void AddProvisioningFormConfig(IProvisioningForm EPProvisioningForm)
        {
            provisioningFormDA.AddProvisioningFormConfig(EPProvisioningForm);
        }

        public void UpdateProvisioningFormConfig(IProvisioningForm EPProvisioningForm)
        {
            provisioningFormDA.UpdateProvisioningFormConfig(EPProvisioningForm);
        }

        public ProvisioningForm GetProvisioningFormByID(long FormID, bool deepLoad)
        {
            return provisioningFormDA.GetProvisioningFormByID(FormID, deepLoad);
        }
        public IProvisioningForm GetProvisioningFormConfiguration(ProvisioningForm ProvisioningForm)
        {
            return provisioningFormDA.GetProvisioningForm(ProvisioningForm);
        }
        public IList<ProvisioningForm> GetAllProvisioningForms()
        {
            return provisioningFormDA.GetAllProvisioningForms();
        }
        public IList<ProvisioningFormEndpointSystemType> GetAllProvisioningFormsEndpointSystemType(int EndpointSystemTypeId, int EndpointSystem)
        {
            return provisioningFormDA.GetAllProvisioningFormsEndpointSystemType(EndpointSystemTypeId, EndpointSystem);
        }
        public IList<ProvisioningForm> GetProvisioningFormsByEndPointType(int endpointTypeId)
        {
            return provisioningFormDA.GetProvisioningFormsByEndPointType(endpointTypeId);
        }
       
        public bool DeleteProvisioningForm(long _ProvisioningFormID)
        {
            return provisioningFormDA.DeleteProvisioningForm(_ProvisioningFormID);
        }
        
     

        public List<ProvisioningFormGeneralData> GetFormGeneralData(long FormID)
        {
            return provisioningFormDA.GetFormGeneralData(FormID);
        }
        public ProvisioningFormAD GetProvisioningFormAD(long FormID)
        {
            return provisioningFormDA.GetProvisioningFormAD(FormID);
        }
        public ProvisioningFormSAP GetProvisioningFormSAP(long FormID)
        {
            return provisioningFormDA.GetProvisioningFormSAP(FormID);
        }
        public ProvisioningFormSAPEP GetProvisioningFormSAPEP(long FormID)
        {
            return provisioningFormDA.GetProvisioningFormSAPEP(FormID);
        }
        public ProvisioningFormLNS GetProvisioningFormLotusNote(long FormID)
        {
            return provisioningFormDA.GetProvisioningFormLotusNote(FormID);
        }
        
        public ProvisioningFormOracleEBS GetProvisioningFormOracleEBS(long FormID)
        {
            return provisioningFormDA.GetProvisioningFormOracleEBS(FormID);
        }

        public ProvisioningForm GetProvisioningFormByEndpoint(int endpointId, AccountType accountType)
        {
            try
            {
                return provisioningFormDA.GetProvisioningFormByEndpoint(endpointId, accountType);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetProvisioningForms(string searchString, int EndpointTypeId)
        {
            try
            {
                return provisioningFormDA.GetProvisioningForms(searchString, EndpointTypeId);

            }
            catch
            {
                throw;
            }
        }

        public ProvisioningForm GetProvisioningFormByName(string FormName)
        {
            return provisioningFormDA.GetProvisioningFormByName(FormName);
        }

        public IList<DefaultProvisioningFormGeneralData> loadDefaultProvisioningGeneralAttributes(int endpointTypeId)
        {
            return provisioningFormDA.loadDefaultProvisioningGeneralAttributes(endpointTypeId);
        }


        public ProvisioningFormLINUX GetProvisioningFormLinux(long FormId)
        {
            return provisioningFormDA.GetProvisioningFormLinux(FormId);
        }

        public ProvisioningFormORCLUM GetProvisioningFormORACLEUM(long FormId)
        {
            return provisioningFormDA.GetProvisioningFormORCLUM(FormId);
        }

        public ProvisioningForm GetCreateProvisioningFormByEndpoint(int endpointId, AccountType accountType)
        {
            try
            {
                return provisioningFormDA.GetCreateProvisioningFormByEndpoint(endpointId, accountType);
            }
            catch
            {
                throw;
            }
        }

    }
}
