using System;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Management;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Deployment;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.ResourceGroup;
using Microsoft.Azure.Management.ResourceManager.Fluent.GenericResource;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Model;
//using Newtonsoft.Json.Linq;

namespace Sepes.RestApi.Services
{

    public class AzPod : IAzPod
    {

        public string CreatePod(int podID, string podName, string podTag, IAzure azure)//Change to long form so function prompt is more descriptive
        {
            //throw new NotImplementedException();
            //return will likely be in form of Pod model
            //if(!hasresourcegroup()){
                //Create ResourceGroup
                Console.WriteLine("Creating a resource group with name: " + podName);

                var resourceGroup = azure.ResourceGroups
                        .Define(podName)
                        .WithRegion(Region.EuropeNorth)
                        .WithTag("Group",podTag) //Group is whatever we name the key as later.
                        .Create();

                Console.WriteLine("Created a resource group with name: " + podName);
                return "ok boi, its done";
            //}

        }

    }

}
