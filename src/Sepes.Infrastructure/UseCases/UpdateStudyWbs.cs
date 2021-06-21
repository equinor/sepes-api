using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.UseCases
{
    public class UpdateStudyWbs
    {
        readonly ILogger _logger;
        readonly SepesDbContext _sepesDbContext;

        public async Task Invoke(Study study)
        {
            //Update resource groups for datasets
            //Get list of resource groups to update
            //Update resource for resource group tag 

            //Update sandboxes
                //Update resource groups for the sandbox
                    //Update resource 

            //Create operations in db
            //Add queue item
            //


            //Update 
        }

    }
}
