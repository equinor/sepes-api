namespace Sepes.Infrastructure.Constants
{
    public static class AzureVmOperatingSystemConstants
    {
        public static class Windows
        {
            public static class Server2019DataCenter
            {
                public const string Publisher = "MicrosoftWindowsServer";
                public const string Offer = "WindowsServer";
                public const string Sku = "2019-Datacenter";
            }
            public static class Server2019DataCenterCore
            {
                public const string Publisher = "MicrosoftWindowsServer";
                public const string Offer = "WindowsServer";
                public const string Sku = "2019-Datacenter-Core";
            }
            public static class Server2016DataCenter
            {
                public const string Publisher = "MicrosoftWindowsServer";
                public const string Offer = "WindowsServer";
                public const string Sku = "2016-Datacenter";
            }
            public static class Server2016DataCenterCore
            {
                public const string Publisher = "MicrosoftWindowsServer";
                public const string Offer = "WindowsServer";
                public const string Sku = "2016-Datacenter-Server-Core";
            }
        }

        public static class Linux
        {
            public static class RedHat7LVM
            {
                public const string Publisher = "RedHat";
                public const string Offer = "RHEL";
                public const string Sku = "7-LVM";
            }
            public static class UbuntuServer1804LTS
            {
                public const string Publisher = "Canonical";
                public const string Offer = "UbuntuServer";
                public const string Sku = "18.04-LTS";
            }
            public static class Debian10
            {
                public const string Publisher = "Debian";
                public const string Offer = "debian-10";
                public const string Sku = "10";
            }
            public static class CentOS75
            {
                public const string Publisher = "OpenLogic";
                public const string Offer = "CentOS";
                public const string Sku = "7.5";
            }
        }
    }
}
