using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Tests.Setup
{
    public class AzureResourceTagsFactory_Factory
    {
        public static IConfiguration GetConfiguration(ServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();

            return config;
        }
    }
}
