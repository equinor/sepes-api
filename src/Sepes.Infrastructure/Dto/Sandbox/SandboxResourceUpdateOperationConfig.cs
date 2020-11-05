namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class SandboxResourceUpdateOperationConfig<T>
    {
        public string OperationName { get; private set; }

        public T ConfigItem { get; private set; }

        public SandboxResourceUpdateOperationConfig(string operationName, T configString)
        {
            OperationName = operationName;
            ConfigItem = configString;
        }
    }
}
