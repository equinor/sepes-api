using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Util
{
    public static class ValidationUtils
    {
        public static void ThrowIfValidationErrors(string messagePrefix, List<string> validationErrors)
        {
            if (validationErrors != null && validationErrors.Count > 0)
            {
                var validationErrorMessageBuilder = new StringBuilder();

                foreach (var curValidation in validationErrors)
                {
                    validationErrorMessageBuilder.AppendLine(curValidation);
                }

                throw new Exception($"{messagePrefix}: {validationErrorMessageBuilder}");
            }
        }
    }
}
