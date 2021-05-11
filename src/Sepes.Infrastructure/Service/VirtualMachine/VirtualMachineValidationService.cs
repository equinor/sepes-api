using Sepes.Common.Constants;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Common.Util;
using System.Linq;
using System.Text;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineValidationService : IVirtualMachineValidationService
    {

        public VirtualMachineValidationService() { }

        

        public string CalculateName(string studyName, string sandboxName, string userPrefix)
        {
            return AzureResourceNameUtil.VirtualMachine(studyName, sandboxName, userPrefix);
        }

        public VmUsernameValidateDto CheckIfUsernameIsValidOrThrow(VmUsernameDto input)
        {
            StringBuilder errorString = new StringBuilder("");
            StringBuilder listOfInvalidNames = new StringBuilder("");
            VmUsernameValidateDto usernameValidation = new VmUsernameValidateDto { errorMessage = "", isValid = true };
            if (input.Username.EndsWith("."))
            {
                usernameValidation.isValid = false;
                errorString.Append("Name can not end with a period(.)");
            }
            var invalidUsernames = AzureVmInvalidUsernames.invalidUsernamesWindows;
            if (input.OperativeSystemType == AzureVmConstants.LINUX)
            {
                invalidUsernames = AzureVmInvalidUsernames.invalidUsernamesLinux;
            }

            foreach (string invalidName in invalidUsernames)
            {
                if (input.Username.Equals(invalidName))
                {
                    usernameValidation.isValid = false;
                    errorString.Append($"The name: '{input.Username}' is not valid.");
                    foreach (string name in invalidUsernames)
                    {
                        listOfInvalidNames.Append(name);
                        if (name != invalidUsernames.Last())
                        {
                            listOfInvalidNames.Append(", ");
                        }
                    }
                    errorString.Append($" The following names are not allowed: {listOfInvalidNames}");
                    break;
                }
            }
            usernameValidation.errorMessage = errorString.ToString();
            return usernameValidation;
        }
    }
}
