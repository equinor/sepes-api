using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Util
{
    public static class CloudResourceCRUDUtil
    {
        public static CloudResourceCRUDResult CreateResultFromIResource(IResource resource)
        {
            return new CloudResourceCRUDResult() { Resource = resource, Success = true };
        }

        public static CloudResourceCRUDResult CreateResultFromIResource(Resource resource)
        {
            return new CloudResourceCRUDResult() { NetworkResource = resource, Success = true };
        }
    }
}
