namespace Sepes.Infrastructure.Constants
{
    public class AzureResourceType
    {
        public const string ResourceGroup = "ResourceGroup";     
        public const string VirtualNetwork = "Microsoft.Network/virtualNetworks";
        public const string NetworkSecurityGroup = "Microsoft.Network/networkSecurityGroups";
        public const string StorageAccount = "Microsoft.Storage/storageAccounts";
        //TODO: Change remaining typeNames to valid Azure Types.
        public const string Bastion = "Bastion";
        public const string VirtualMachine = "VirtualMachine";
    }
}
