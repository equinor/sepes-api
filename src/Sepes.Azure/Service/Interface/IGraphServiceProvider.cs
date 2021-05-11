using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Azure.Service.Interface
{
    public interface IGraphServiceProvider
    {
        GraphServiceClient GetGraphServiceClient(string[] scopes);
    }
}
