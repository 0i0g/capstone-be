using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Configurations.Filter
{
    public class RequiredPropertiesContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            foreach (var contractProperty in contract.Properties)
            {
                if (contractProperty.AttributeProvider != null &&
                    contractProperty.PropertyType is { IsValueType: true } && contractProperty.AttributeProvider
                        .GetAttributes(typeof(RequiredAttribute), inherit: true).Any())
                {
                    contractProperty.Required = Required.Always;
                }
            }

            return contract;
        }
    }
}
