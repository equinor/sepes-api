using Sepes.Common.Dto.VirtualMachine;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineValidationService
    {
        string CalculateName(string studyName, string sandboxName, string userPrefix);
        VmUsernameValidateDto CheckIfUsernameIsValidOrThrow(VmUsernameDto input);
    }
}
