using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{

    [RoutePrefix("IXLData")]
    public class XLDataController:ApiController
    {
        [HttpPost]
        [Route("ProcessXL")]
        public IList<Dto.IXLData> ProcessXL([FromBody]object xmlData, string fileName, string templateName,  string errorMessage, bool partialUpload)
        {
            XLTemplates getTemplate = (XLTemplates)Enum.Parse(typeof(XLTemplates), templateName);
            return DtoMapList(DomainModel.XLData.ProcessXL(xmlData, fileName, getTemplate, ref errorMessage, User.Identity.Name, partialUpload));
        }
        [HttpPost]
        [Route("ProcessXLData")]
        public IList<Dto.IXLData> ProcessXLData(Dto.IXLData data, string templateName, bool isEditMode)
        {
            XLTemplates getTemplate = (XLTemplates)Enum.Parse(typeof(XLTemplates), templateName);
            return DtoMapList(DomainModel.XLData.ProcessXLData(DtoUnMap(data), getTemplate, User.Identity.Name, isEditMode));
        }

        [HttpPost]
        [Route("ProcessXLForEntitlment")]
        public IList<Dto.IXLData> ProcessXLForEntitlment([FromBody]object xmlData, string fileName, string templateName, string errorMessage, long endpointSystemId)
        {
            XLTemplates getTemplate = (XLTemplates)Enum.Parse(typeof(XLTemplates), templateName);
            var result = DtoMapList(DomainModel.XLData.ProcessXLForEntitlment(xmlData, fileName, getTemplate, ref errorMessage, User.Identity.Name, endpointSystemId));
            return result;
        }

        [HttpPost]
        [Route("ValidateFileColumns")]
        public string ValidateFileColumns([FromBody]object xmlData, string fileName, string templateName)
        {
            XLTemplates getTemplate = (XLTemplates)Enum.Parse(typeof(XLTemplates), templateName);
            return DomainModel.XLData.ValidateFileColumns(xmlData, fileName, getTemplate);
        }
        [HttpGet]
        [Route("GetWhoUpdatedData")]
        public string GetWhoUpdatedData(string templateName)
        {
            XLTemplates getTemplate = (XLTemplates)Enum.Parse(typeof(XLTemplates), templateName);
            return DomainModel.XLData.GetWhoUpdatedData(getTemplate);
        }
        public  Dto.IXLData DtoMap(DomainModel.IXLData tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.IXLData, Dto.IXLData>(tModel);
        }

        public DomainModel.IXLData DtoUnMap(Dto.IXLData tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<Dto.IXLData, DomainModel.IXLData>(tModel);
        }

        public IList<Dto.IXLData> DtoMapList(IList<DomainModel.IXLData> tModelList)
        {
            if (tModelList != null)
                return tModelList.Select(DtoMap).ToList();
            else
                return null;
        }

    }
}
