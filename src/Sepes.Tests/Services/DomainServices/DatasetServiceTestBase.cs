using Sepes.Infrastructure.Model;

namespace Sepes.Tests.Services
{
    public class DatasetServiceTestBase : ServiceTestBase
    {


        public DatasetServiceTestBase()
            : base()
        {
          
        }    


        protected async void SeedTestDatabase(int datasetId)
        {
            var db = GetDatabase();

            Dataset dataset = new Dataset()
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
        }

           
    }
}
