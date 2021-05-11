//using Sepes.Infrastructure.Model.SepesSqlModels;
//using System;

//namespace Sepes.Infrastructure.Dto
//{
//    // Read only view of the current or proposed state of a Pod.
//    // Missing technical parameters like Id.
//    public class DataSet
//    {
//        // Name to show user.
//        public string displayName { get; }
//        // Link to the policy file or document.
//        public string opaPolicy { get; }
//        // Reference to where the data is stored in azure.
//        public string azureReference { get; }

//        public DataSet(string name, string opaPolicy, string azureReference)
//        {
//            this.displayName = name;
//            this.opaPolicy = opaPolicy;
//            this.azureReference = azureReference;
//        }

//        public DataSetDB ToDataSetDB()
//        {
//            return new DataSetDB(){
//                displayName = displayName,
//                opaPolicy = opaPolicy,
//                azureReference = azureReference
//            };
//        }

//        public override bool Equals(object obj)
//        {
//            return obj is DataSet dataset &&
//                   displayName == dataset.displayName &&
//                   opaPolicy == dataset.opaPolicy &&
//                   azureReference == dataset.azureReference;
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(displayName);
//        }

//    }
//}
