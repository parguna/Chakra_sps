using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Infrastructure.Enums;
using AutoMapper;
using DI.Chakra.Dto.Core;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationUser")]
    public class PresentationUserController : ApiController
    {
        [HttpGet]
        [Route("GetActiveEmployees")]
        public List<PresentationUser> GetActiveEmployees(string employeeType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUser.GetActiveEmployees(employeeType));
        }

        [HttpGet]
        [Route("GetEmployees")]
        public List<PresentationUser> GetEmployees(DateTime? FromDate, DateTime? ToDate, bool? IsActive)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUser.GetEmployees(FromDate, ToDate, IsActive));
        }

        [HttpGet]
        [Route("GetEmployees")]
        public List<PresentationUser> GetEmployees()
        {
            return DtoMapList(DomainModel.Presentation.PresentationUser.GetEmployees());
        }


        [HttpPost]
        [Route("GetLazyLoadFullTimeEmployees")]
        public Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationUser> GetLazyLoadFullTimeEmployees(PresentationGridPagedDataParameter param, string departmentCode = "", bool showAllNonEmployees = false)
        {
            var responseData = new Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationUser>();
            var result = DomainModel.Presentation.PresentationUser.GetLazyLoadFullTimeEmployees(param.FullTimeEmp, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, departmentCode, showAllNonEmployees);
            responseData.Data = DtoMapList(result.Data);
            responseData.TotalCount = result.TotalCount;
            return responseData;
        }

        [HttpGet]
        [Route("GetMembers")]
        public List<PresentationUser> GetMembers(long userId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUser.GetMembers(userId));
        }

        [HttpGet]
        [Route("GetNonEmployees")]
        public List<PresentationUser> GetNonEmployees()
        {
            return DtoMapList(DomainModel.Presentation.PresentationUser.GetNonEmployees());
        }
        [HttpGet]
        [Route("GetUserGroup")]
        public List<PresentationUser> GetUserGroup()
        {
            return DtoMapList(DomainModel.Presentation.PresentationUser.GetUserGroup());
        }

        [HttpGet]
        [Route("GetUserRole")]
        public List<PresentationUser> GetUserRole()
        {
            return DtoMapList(DomainModel.Presentation.PresentationUser.GetUserRole());
        }
        public DI.Chakra.Dto.Presentation.PresentationUser DtoMap(DomainModel.Presentation.PresentationUser tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationUser, DI.Chakra.Dto.Presentation.PresentationUser>(tModel);
        }

        public List<DI.Chakra.Dto.Presentation.PresentationUser> DtoMapList(IList<DomainModel.Presentation.PresentationUser> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpPost]
        [Route("DownloadFullTimeEmployees")]
        public byte[] DownloadFullTimeEmployees(PresentationGridPagedDataParameter param, string exportFileType)
        {
            return DomainModel.Presentation.PresentationUser.DownloadFullTimeEmployees(param.FullTimeEmp, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, exportFileType);
        }

        [HttpGet]
        [Route("DownloadNonEmployees")]
        public byte[] DownloadNonEmployees(bool fullTimeEmp, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression, string exportFileType, string departmentCode = "", bool showAllNonEmployees = false)
        {
            return DomainModel.Presentation.PresentationUser.DownloadNonEmployees(fullTimeEmp, startRowIndex, maximumRows, FilterExpression, sortExpression, exportFileType, departmentCode, showAllNonEmployees);
        }
    }
}