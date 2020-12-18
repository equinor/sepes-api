using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Threading.Tasks;

namespace Sepes.Tests.Services
{
    public class DatasetServiceTestBase : ServiceTestBase
    {


        public DatasetServiceTestBase()
            : base()
        {
          
        }   


        protected async Task<SepesDbContext> RefreshAndSeedTestDatabase(int datasetId)
        {
            var db = await ClearTestDatabase();           

            var dataset = new Dataset()
            {
                Id = datasetId,
                Name = "TestDataset",
                Description = "For Testing",
                Location = "Norway West",
                Classification = "Internal",
                StorageAccountName = "testdataset",
                LRAId = 1337,
                DataId = 420,
                SourceSystem = "SAP",
                CountryOfOrigin = "Norway",
                StudyId = null
            };

            db.Datasets.Add(dataset);
            await db.SaveChangesAsync();
            return db;
        }           
    }
}
