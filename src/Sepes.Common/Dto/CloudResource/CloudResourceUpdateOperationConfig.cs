namespace Sepes.Common.Dto.Sandbox
{
    public class CloudResourceUpdateOperationConfig<T>
    {
        public string OperationName { get; private set; }

        public T ConfigItem { get; private set; }

        public CloudResourceUpdateOperationConfig(string operationName, T configString)
        {
            OperationName = operationName;
            ConfigItem = configString;
        }
    }
}
