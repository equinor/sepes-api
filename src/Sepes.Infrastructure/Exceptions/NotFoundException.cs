using System;
using System.Text;

namespace Sepes.Infrastructure.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException() : base("Not found") { }
        public NotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected NotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public static NotFoundException CreateForEntity(string entityName, int id, Exception inner = null)
        {
            return new NotFoundException($"{entityName} with id {id} not found!", inner);
        }

        public static NotFoundException CreateForEntityCustomDescr(string entityName, string fieldsAndValuesCustom, Exception inner = null)
        {
            return new NotFoundException($"{entityName} with {fieldsAndValuesCustom} not found!", inner);
        }

        public static NotFoundException CreateForEntityByName(string entityName, string name, Exception inner = null)
        {
            return new NotFoundException($"{entityName} with name {name} not found!", inner);
        }
        public static NotFoundException CreateForEntityByOtherFieldName(string entityName, string field, string fieldValue, Exception inner = null)
        {
            return new NotFoundException($"{entityName} with {field} = {fieldValue} not found!", inner);
        }      


        public static NotFoundException CreateForAzureResource(string resourceName, string resourceGroupName = null, Exception inner = null)
        {
            var sbExMessage = new StringBuilder($"Azure resource with name {resourceName}");

            if (!String.IsNullOrWhiteSpace(resourceGroupName))
            {
                sbExMessage.Append($" in resource group {resourceGroupName}");
            }

            sbExMessage.Append(" not found");

            return new NotFoundException(sbExMessage.ToString(), inner);
        }
    }
}
