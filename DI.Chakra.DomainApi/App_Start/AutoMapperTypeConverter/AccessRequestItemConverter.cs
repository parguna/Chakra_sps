using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.DomainApi
{
    public class PresentationAccessRequestItemDtoConverter : TypeConverter<DomainModel.IAccessRequestItem, Dto.IAccessRequestItem>
    {
        protected override Dto.IAccessRequestItem ConvertCore(DomainModel.IAccessRequestItem source)
        {
            if (source is DomainModel.Presentation.PresentationAccessRequestItem) return Mapper.Map<Dto.Presentation.PresentationAccessRequestItem>(source);
           
            return null;
        }

    }

    public class PresentationAccessRequestItemDomainConverter : TypeConverter<Dto.IAccessRequestItem, DomainModel.IAccessRequestItem>
    {
        protected override DomainModel.IAccessRequestItem ConvertCore(Dto.IAccessRequestItem source)
        {
            if (source is Dto.Presentation.PresentationAccessRequestItem) return Mapper.Map<DomainModel.Presentation.PresentationAccessRequestItem>(source);
           
            return null;
        }

    }
}