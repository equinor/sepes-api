﻿using Microsoft.Graph;
using Microsoft.Identity.Web;
using Sepes.Azure.Service.Interface;

namespace Sepes.RestApi.Services.GraphApi
{
    public class GraphServiceProvider : IGraphServiceProvider
    {
        readonly ITokenAcquisition tokenAcquisition;

        public GraphServiceProvider(ITokenAcquisition tokenAcquisition)
        {
            this.tokenAcquisition = tokenAcquisition;
        }
        public GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, "https://graph.microsoft.com/v1.0/");
        }
    }
}
