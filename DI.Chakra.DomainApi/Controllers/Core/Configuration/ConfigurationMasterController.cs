using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ConfigurationMaster")]
    public class ConfigurationMasterController : BaseController<DomainModel.ConfigurationMaster, EntityModel.ConfigurationMaster, Dto.ConfigurationMaster>
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("GetByConfigKey")]
        public Dto.IConfigurationMaster GetByConfigKey(string configKey, Product product)
        {
            return AutoMapper.Mapper.Map<DomainModel.IConfigurationMaster, Dto.IConfigurationMaster>(DomainModel.ConfigurationMaster.GetByConfigKey(configKey, product));
        }


        [HttpGet]
        [Route("GetConfigKey")]
        [AllowAnonymous]
        public string GetConfigKey(string configKey, Product product)
        {
            return DomainModel.ConfigurationMaster.GetConfigKey(configKey, product);
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string configKey, Product product)
        {
            return DomainModel.ConfigurationMaster.DoesConfigKeyAlreadyExists(configKey, product);
        }

        [HttpGet]
        [Route("GetConfigurationWithUIDisplayByProduct")]
        public List<Dto.IConfigurationMaster> GetConfigurationWithUIDisplayByProduct(Product product)
        {
            return AutoMapper.Mapper.Map<List<DomainModel.IConfigurationMaster>, List<Dto.IConfigurationMaster>>(DomainModel.ConfigurationMaster.GetConfigurationWithUIDisplayByProduct(product).ToList());
        }

        [HttpGet]
        [Route("GetByProduct")]
        public Dto.IConfigurationMaster GetByProduct(long id, Product product)
        {
            return AutoMapper.Mapper.Map<DomainModel.IConfigurationMaster, Dto.IConfigurationMaster>(DomainModel.ConfigurationMaster.GetByProduct(id, product));
        }

        [HttpGet]
        [Route("DeleteConfiguration")]
        public void DeleteConfiguration(long id, Product product)
        {
            DomainModel.ConfigurationMaster.DeleteConfiguration(id, product);
        }

        [HttpPost]
        [Route("SaveConfiguration")]
        public void SaveConfiguration([FromBody]Dto.IConfigurationMaster configuration, Product product)
        {
            DomainModel.ConfigurationMaster.SaveConfiguration(AutoMapper.Mapper.Map<Dto.IConfigurationMaster, DomainModel.IConfigurationMaster>(configuration), product,User.Identity.Name);
        }
    }
}
