using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra;
using DI.Chakra.Dto.Presentation;
using AutoMapper;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SkypeForBusinessProvisioningMatrix")]
    public class SkypeForBusinessMatrixController : BaseController<DomainModel.SkypeForBusinessProvisioningMatrix, EntityModel.SkypeForBusinessProvisioningMatrix, Dto.SkypeForBusinessProvisioningMatrix>
    {
        [HttpGet]
        [Route("GetAllSkypeForBusinessMatrix")]
        public List<Dto.SkypeForBusinessProvisioningMatrix> GetAllSkypeForBusinessMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SkypeForBusinessProvisioningMatrix.GetAllSkypeForBusinessMatrix(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("GetSkypeForBusinessMatrixByName")]
        public Dto.SkypeForBusinessProvisioningMatrix GetSkypeForBusinessMatrixByName(string name,EntitlementType entitlementType)
        {
            return DtoMap(DomainModel.SkypeForBusinessProvisioningMatrix.GetByName(name, entitlementType));
        }
    }
}