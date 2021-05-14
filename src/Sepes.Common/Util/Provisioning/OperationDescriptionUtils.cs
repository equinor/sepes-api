using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Util.Provisioning
{
    public static class ResourceOperationDescriptionUtils
    {
        public static string CreateDescriptionForResourceOperation(string resourceType, string operationType, int resourceId = 0, int studyId = 0, int sandboxId = 0)
        {
            var description = $"{operationType} {resourceType}";

            if (resourceId > 0)
            {
                description += $" with id {resourceId}";
            }

            if (studyId > 0)
            {
                description += $" for study {studyId}";
            }

            if (sandboxId > 0)
            {
                description += $" for sandbox {sandboxId}";
            }

            return description;
        }

        public static string CreateResourceOperationErrorMessage(Exception ex)
        {
            var messageBuilder = new StringBuilder(ex.Message);

            if (ex.InnerException != null)
            {
                messageBuilder.AppendLine(CreateResourceOperationErrorMessage(ex.InnerException));
            }

            return messageBuilder.ToString();
        }
    }
}
