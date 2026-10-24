using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.DomainApi
{
    public class EndpointEntitlementInfoDtoConverter : TypeConverter<DomainModel.IEndpointEntitlementInfo, Dto.IEndpointEntitlementInfo>
    {
        protected override Dto.IEndpointEntitlementInfo ConvertCore(DomainModel.IEndpointEntitlementInfo source)
        {

            if (source is DomainModel.ADSGroupInfo)
            {
                DomainModel.ADSGroupInfo domainModelADSGroupInfo = (DomainModel.ADSGroupInfo)source;
                //Suresh - Setting to NULL as temp fix to circular referrence issue.
                domainModelADSGroupInfo.ADSUsers = null;
                return Mapper.Map<Dto.ADSGroupInfo>(domainModelADSGroupInfo);
            }
            else if (source is DomainModel.InhousePermission)
            {
                DomainModel.InhousePermission domainModelInhousePermission = (DomainModel.InhousePermission)source;
                //domainModelInhousePermission.INHUsers = null;
                return Mapper.Map<Dto.InhousePermission>(source);
            }
            else if (source is DomainModel.SAPRoleInfo)
            {
                DomainModel.SAPRoleInfo domainModelSAPRoleInfo = (DomainModel.SAPRoleInfo)source;
                //Suresh - Setting to NULL as temp fix to circular referrence issue.
                domainModelSAPRoleInfo.SAPUserinfo = null;
                return Mapper.Map<Dto.SAPRoleInfo>(domainModelSAPRoleInfo);
            }
            else if (source is DomainModel.SAPHANARoleInfo)
            {
                DomainModel.SAPHANARoleInfo domainModelSAPRoleInfo = (DomainModel.SAPHANARoleInfo)source;
                //Suresh - Setting to NULL as temp fix to circular referrence issue.
                domainModelSAPRoleInfo.SAPUserinfo = null;
                return Mapper.Map<Dto.SAPHANARoleInfo>(domainModelSAPRoleInfo);
            }
            if (source is DomainModel.IACMRoleInfo)
            {
                DomainModel.IACMRoleInfo domainModelRole = (DomainModel.IACMRoleInfo)source;
                //Suresh - Setting to NULL as temp fix to circular referrence issue.
                domainModelRole.Users = null;
                return Mapper.Map<Dto.IACMRoleInfo>(domainModelRole);
            }
            return null;
        }

    }

    public class EndpointEntitlementInfoDomainConverter : TypeConverter<Dto.IEndpointEntitlementInfo, DomainModel.IEndpointEntitlementInfo>
    {
        protected override DomainModel.IEndpointEntitlementInfo ConvertCore(Dto.IEndpointEntitlementInfo source)
        {
            if (source is Dto.ADSGroupInfo) return Mapper.Map<DomainModel.ADSGroupInfo>(source);
            else if (source is Dto.InhousePermission) return Mapper.Map<DomainModel.InhousePermission>(source);
            else if (source is Dto.SAPRoleInfo) return Mapper.Map<DomainModel.SAPRoleInfo>(source);
            else if (source is Dto.SAPHANARoleInfo) return Mapper.Map<DomainModel.SAPHANARoleInfo>(source);
            else if (source is Dto.IACMRoleInfo) return Mapper.Map<DomainModel.IACMRoleInfo>(source);
            return null;
        }

    }
}