using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.DomainApi
{
    public class UserEntitlementDtoConverter : TypeConverter<DomainModel.Presentation.PresentationUserEntitlement, Dto.Presentation.PresentationUserEntitlement>
    {

        protected override Dto.Presentation.PresentationUserEntitlement ConvertCore(DomainModel.Presentation.PresentationUserEntitlement source)
        {
            try
            {
                if (source is DomainModel.PresentationADSUserEntitlement) return Mapper.Map<Dto.PresentationADSUserEntitlement>(source);
                if (source is DomainModel.PresentationInhouseUserEntitlement) return Mapper.Map<Dto.PresentationInhouseUserEntitlement>(source);
                if (source is DomainModel.PresentationSAPEPUserEntitlement) return Mapper.Map<Dto.PresentationSAPEPUserEntitlement>(source);
                if (source is DomainModel.PresentationSAPUserEntitlement) return Mapper.Map<Dto.PresentationSAPUserEntitlement>(source);
                if (source is DomainModel.PresentationIACMUserEntitlement) return Mapper.Map<Dto.PresentationIACMUserEntitlement>(source);
                if (source is DomainModel.PresentationSkypeForBusinessUserEntitlement) return Mapper.Map<Dto.PresentationSkypeForBusinessUserEntitlement>(source);
                if (source is DomainModel.PresentationVPNUserEntitlement) return Mapper.Map<Dto.PresentationVPNUserEntitlement>(source);
                if (source is DomainModel.PresentationOffice365UserEntitlement) return Mapper.Map<Dto.PresentationOffice365UserEntitlement>(source);
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public class UserEntitlementDomainConverter : TypeConverter<Dto.Presentation.PresentationUserEntitlement, DomainModel.Presentation.PresentationUserEntitlement>
    {
        protected override DomainModel.Presentation.PresentationUserEntitlement ConvertCore(Dto.Presentation.PresentationUserEntitlement source)
        {
            try
            {
                if (source is Dto.PresentationADSUserEntitlement) return Mapper.Map<DomainModel.PresentationADSUserEntitlement>(source);
                if (source is Dto.PresentationInhouseUserEntitlement) return Mapper.Map<DomainModel.PresentationInhouseUserEntitlement>(source);
                if (source is Dto.PresentationSAPEPUserEntitlement) return Mapper.Map<DomainModel.PresentationSAPEPUserEntitlement>(source);
                if (source is Dto.PresentationSAPUserEntitlement) return Mapper.Map<DomainModel.PresentationSAPUserEntitlement>(source);
                if (source is Dto.PresentationIACMUserEntitlement) return Mapper.Map<DomainModel.PresentationIACMUserEntitlement>(source);
                if (source is Dto.PresentationVPNUserEntitlement) return Mapper.Map<DomainModel.PresentationVPNUserEntitlement>(source);
                if (source is Dto.PresentationSkypeForBusinessUserEntitlement) return Mapper.Map<DomainModel.PresentationSkypeForBusinessUserEntitlement>(source);
                if (source is Dto.PresentationOffice365UserEntitlement) return Mapper.Map<DomainModel.PresentationOffice365UserEntitlement>(source);
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}