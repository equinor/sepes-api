using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Constants
{
    public class AzureVmInvalidUsernames
    {
        public static readonly string[] invalidUsernamesWindows = { "1", "123", "a", "actuser", "adm", "admin", "admin1",
            "admin2", "administrator", "aspnet", "backup", "console", "david", "guest", "john", "owner", "root", "server",
            "sql", "support_388945a0", "support"," sys", "test", "test1", "test2", "test3", "user", "user1", "user2" };

        public static readonly string[] invalidUsernamesLinux = {"1", "123", "a", "actuser", "adm", "admin", "admin1", "admin2",
            "administrator" ,"aspnet", "backup", "console", "david", "guest", "john", "owner", "root", "server", "sql", "support_388945a0",
            "support", "sys", "test", "test1", "test2", "test3", "user", "user1", "user2", "user3", "user4", "user5", "video"};
    }
}
