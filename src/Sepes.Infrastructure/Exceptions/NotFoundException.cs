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

        public static NotFoundException CreateForIdentity(string entityName, int id, Exception inner = null)
        {
            return new NotFoundException($"{entityName} with id {id} not found!", inner);
        }

        public static NotFoundException CreateForIdentityByName(string entityName, string name, Exception inner = null)
        {
            return new NotFoundException($"{entityName} with name {name} not found!", inner);
        }

        public static NotFoundException CreateForAzureResource(string resourceName, string resourceGroupName = null, Exception inner = null)
        {
            StringBuilder sbExMessage = new StringBuilder($"Azure resource with name {resourceName}");

            if (!String.IsNullOrWhiteSpace(resourceGroupName))
            {
                sbExMessage.Append($" in resource group {resourceGroupName}");
            }

            sbExMessage.Append(" not found");

            return new NotFoundException(sbExMessage.ToString(), inner);
        }
    }
}
