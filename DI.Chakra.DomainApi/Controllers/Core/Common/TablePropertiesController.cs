using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("TableProperties")]
    public class TablePropertiesController : ApiController
    {
        [HttpGet]
        [Route("GetByTableName")]
        public List<Dto.TableProperties> GetByTableName(string tableName)
        {
            return DtoMapList(DomainModel.TableProperties.GetByTableName(tableName));
        }
        [HttpGet]
        [Route("GetTableList")]
        public IList<string> GetTableList()
        {
            return DomainModel.TableProperties.GetTableList();
        }
        [HttpGet]
        [Route("GetPropertyList")]
        public List<string> GetPropertyList(string tableName)
        {
            return DomainModel.TableProperties.GetPropertyList(tableName);
        }
        public Dto.TableProperties DtoMap(DomainModel.TableProperties tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.TableProperties, Dto.TableProperties>(tModel);
        }
        public List<Dto.TableProperties> DtoMapList(IList<DomainModel.TableProperties> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
