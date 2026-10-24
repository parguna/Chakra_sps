using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.DomainApi
{
    public class ConfigurationMasterDtoConverter : TypeConverter<DomainModel.IConfigurationMaster, Dto.IConfigurationMaster>
    {
        protected override Dto.IConfigurationMaster ConvertCore(DomainModel.IConfigurationMaster source)
        {
            if (source is DomainModel.ConfigurationMaster) return Mapper.Map<Dto.ConfigurationMaster>(source);
            else if (source is DomainModel.ConfigurationMasterIM) return Mapper.Map<Dto.ConfigurationMasterIM>(source);
            else if (source is DomainModel.ConfigurationMasterPasswordManagement) return Mapper.Map<Dto.ConfigurationMasterPasswordManagement>(source);
            else if (source is DomainModel.ConfigurationMasterCompliance) return Mapper.Map<Dto.ConfigurationMasterCompliance>(source);
            else if (source is DomainModel.ConfigurationMasterPIM) return Mapper.Map<Dto.ConfigurationMasterPIM>(source);
            else if (source is DomainModel.ConfigurationMasterDGM) return Mapper.Map<Dto.ConfigurationMasterDGM>(source);
            return null;
        }

    }

    public class ConfigurationMasterDomainConverter : TypeConverter<Dto.IConfigurationMaster, DomainModel.IConfigurationMaster>
    {
        protected override DomainModel.IConfigurationMaster ConvertCore(Dto.IConfigurationMaster source)
        {
            if (source is Dto.ConfigurationMaster) return Mapper.Map<DomainModel.ConfigurationMaster>(source);
            else if (source is Dto.ConfigurationMasterIM) return Mapper.Map<DomainModel.ConfigurationMasterIM>(source);
            else if (source is Dto.ConfigurationMasterPasswordManagement) return Mapper.Map<DomainModel.ConfigurationMasterPasswordManagement>(source);
            else if (source is Dto.ConfigurationMasterCompliance) return Mapper.Map<DomainModel.ConfigurationMasterCompliance>(source);
            else if (source is Dto.ConfigurationMasterPIM) return Mapper.Map<DomainModel.ConfigurationMasterPIM>(source);
            else if (source is Dto.ConfigurationMasterDGM) return Mapper.Map<DomainModel.ConfigurationMasterDGM>(source);
            return null;
        }
    }
}