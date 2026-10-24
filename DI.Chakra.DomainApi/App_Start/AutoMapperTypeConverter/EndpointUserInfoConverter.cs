using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.DomainApi
{
    public class EndpointUserInfoDtoConverter : TypeConverter<DomainModel.IEndpointUserInfo, Dto.IEndpointUserInfo>
    {
        protected override Dto.IEndpointUserInfo ConvertCore(DomainModel.IEndpointUserInfo source)
        {
            if (source is DomainModel.ADSUserInfo) return Mapper.Map<Dto.ADSUserInfo>(source);
            if (source is DomainModel.SAPUserinfo) return Mapper.Map<Dto.SAPUserinfo>(source);


            return null;
        }

    }

    public class EndpointUserInfoDomainConverter : TypeConverter<Dto.IEndpointUserInfo, DomainModel.IEndpointUserInfo>
    {
        protected override DomainModel.IEndpointUserInfo ConvertCore(Dto.IEndpointUserInfo source)
        {
            if (source is Dto.ADSUserInfo) return Mapper.Map<DomainModel.ADSUserInfo>(source);
            if (source is Dto.SAPUserinfo) return Mapper.Map<DomainModel.SAPUserinfo>(source);

            return null;
        }

    }
}