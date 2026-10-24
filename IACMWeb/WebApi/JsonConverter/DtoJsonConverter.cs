using DI.Chakra.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.Web
{
    public class DtoJsonConverter : JsonConverter
    {
        private static readonly string IEndpointConfiguration_FullName = typeof(IEndpointConfiguration).FullName;
        private static readonly string IEndpointEntitlementInfo_FullName = typeof(IEndpointEntitlementInfo).FullName;
        private static readonly string IConfigurationMaster_FullName = typeof(IConfigurationMaster).FullName;
        private static readonly string PresentationWorkflowCriteriaBase_FullName = typeof(Dto.Presentation.PresentationWorkflowCriteriaBase).FullName;
        private static readonly string IXLData_FullName = typeof(IXLData).FullName;
        private static readonly string ABSPresentationUserEntitlement_FullName = typeof(Dto.Presentation.PresentationUserEntitlement).FullName; 
        public override bool CanConvert(Type objectType)
        {
            if (objectType.FullName == IEndpointConfiguration_FullName
                ||
                objectType.FullName == IEndpointEntitlementInfo_FullName
                ||
                objectType.FullName == IConfigurationMaster_FullName
                ||
                objectType.FullName == PresentationWorkflowCriteriaBase_FullName
                 ||
                objectType.FullName == IXLData_FullName
                  ||
                objectType.FullName == ABSPresentationUserEntitlement_FullName
                )
            {
                return true;
            }
            return false;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (objectType.FullName == IEndpointConfiguration_FullName
                ||
                objectType.FullName == IEndpointEntitlementInfo_FullName
                ||
                objectType.FullName == IConfigurationMaster_FullName
                ||
                objectType.FullName == PresentationWorkflowCriteriaBase_FullName
                 ||
                objectType.FullName == IXLData_FullName
                  ||
                objectType.FullName == ABSPresentationUserEntitlement_FullName
                )
            {
                JObject jObject = JObject.Load(reader);

                var assemblyQualifiedTypeName = jObject["$type"].ToString();
                Type type = Type.GetType(assemblyQualifiedTypeName);

                return serializer.Deserialize(jObject.CreateReader(), type);
            }

            throw new NotSupportedException(string.Format("Type {0} unexpected.", objectType));
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotSupportedException("In custom DtoJsonConverter ... Deserialization is not required / implemented");
        }
    }
} 