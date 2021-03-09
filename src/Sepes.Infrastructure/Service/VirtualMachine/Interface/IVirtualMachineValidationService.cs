using Sepes.Infrastructure.Dto.VirtualMachine;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineValidationService
    {
      
        void ValidateVmPasswordOrThrow(string password);

        string CalculateName(string studyName, string sandboxName, string userPrefix);
        VmUsernameValidateDto CheckIfUsernameIsValidOrThrow(VmUsernameDto input);
    }
}
