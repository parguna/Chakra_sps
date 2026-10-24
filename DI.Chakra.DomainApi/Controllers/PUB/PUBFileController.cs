using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers 
{
    [RoutePrefix("PUBFile")]
    public class PUBFileController : BaseController<DomainModel.PUBFile, EntityModel.PUBFile, Dto.PUBFile>
    {

        [HttpPost]
        [Route("GetLazyLoadPUBFiles")]
        public PresentationGridPagedData<PUBFile> GetLazyLoadPUBFiles(PresentationGridPagedDataParameter param)
        {
            var result = new PresentationGridPagedData<PUBFile>();
            var val = DomainModel.PUBFile.GetLazyLoadPUBFiles(param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression);

            result.Data = DtoMapList(val.Data);
            result.TotalCount = val.TotalCount;
            return result;
        }

    }
}