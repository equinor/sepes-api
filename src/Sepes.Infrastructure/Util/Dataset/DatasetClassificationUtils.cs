using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using System;
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

        public static void SetRestrictionProperties(AvailableDatasetResponseDto dto)
        {
            if(dto.AvailableDatasets.Count() > 0)
            {
                var lowestClassification = DatasetClassification.Open;

                foreach (var cur in dto.AvailableDatasets)
                {
                    if (cur.AddedToSandbox)
                    {
                        var classificationCode = GetClassificationCode(cur.Classification);

                        if(classificationCode > lowestClassification)
                        {
                            lowestClassification = classificationCode;
                        }
                    }
                }

                dto.Classification = lowestClassification.ToString();
                dto.RestrictionDisplayText = GetRestrictionText(lowestClassification);
            }           
        }
    }

    public enum DatasetClassification { Open, Internal, Restricted }
}
