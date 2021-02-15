using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Interfaces;
using Sepes.Infrastructure.Response.Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class DatasetClassificationUtils
    {
        public static DatasetClassification GetClassificationCode(string classification)
        {
            return Enum.Parse<DatasetClassification>(classification);           
        }
        public static string GetRestrictionText(DatasetClassification classification)
        {
            return classification switch
            {
                DatasetClassification.Open => DatasetConstants.DATASET_RESTRICTION_TEXT_OPEN,
                DatasetClassification.Internal => DatasetConstants.DATASET_RESTRICTION_TEXT_INTERNAL,
                DatasetClassification.Restricted => DatasetConstants.DATASET_RESTRICTION_TEXT_RESTRICTED,
                _ => throw new Exception($"Could not resolve restriction text for classification code: {classification}"),
            };
        }
       public static DatasetClassification GetLowestClassification(List<IHasDataClassification> items)
        {
            var lowestClassification = DatasetClassification.Open;

            foreach (var cur in items)
            {
                var classificationCode = GetClassificationCode(cur.Classification);

                if (classificationCode > lowestClassification)
                {
                    lowestClassification = classificationCode;
                }
            }

            return lowestClassification;
        }

        public static void SetRestrictionProperties(AvailableDatasets dto)
        {
            var addedDatasets = dto.Datasets.Where(ds => ds.AddedToSandbox).ToList<IHasDataClassification>();
            var lowestClassification = GetLowestClassification(addedDatasets);
            dto.Classification = lowestClassification.ToString();
            dto.RestrictionDisplayText = GetRestrictionText(lowestClassification);
        }

        public static void SetRestrictionProperties(SandboxDetails sandboxDetails)
        {
            var addedDatasets = sandboxDetails.Datasets.ToList<IHasDataClassification>();
            var lowestClassification = GetLowestClassification(addedDatasets);          
            sandboxDetails.RestrictionDisplayText = GetRestrictionText(lowestClassification);
        }
    }

    public enum DatasetClassification { Open, Internal, Restricted }
}
