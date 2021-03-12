using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class DatasetClassificationUtilsTest
    {
        [Fact]
        public void SetRestrictionProperties_ShouldSetRestrictedDisplayText()
        {
            var list = new List<SandboxDatasetDto>();
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Open.ToString(), DatasetId = 1 });
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Internal.ToString(), DatasetId = 2 });
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Restricted.ToString(), DatasetId = 3 });
            var sandboxDetails = new SandboxDetails() { Datasets = list };
            DatasetClassificationUtils.SetRestrictionProperties(sandboxDetails);

            Assert.Equal(DatasetConstants.DATASET_RESTRICTION_TEXT_RESTRICTED, sandboxDetails.RestrictionDisplayText);
        }

        [Fact]
        public void SetRestrictionProperties_ShouldSetInternalDisplayText()
        {
            var list = new List<SandboxDatasetDto>();
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Open.ToString(), DatasetId = 1 });
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Internal.ToString(), DatasetId = 2 });
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Internal.ToString(), DatasetId = 3 });
            var sandboxDetails = new SandboxDetails() { Datasets = list };
            DatasetClassificationUtils.SetRestrictionProperties(sandboxDetails);

            Assert.Equal(DatasetConstants.DATASET_RESTRICTION_TEXT_INTERNAL, sandboxDetails.RestrictionDisplayText );
        }

        [Fact]
        public void SetRestrictionProperties_ShouldSetOpenDisplayText()
        {
            var list = new List<SandboxDatasetDto>();
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Open.ToString(), DatasetId = 1 });
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Open.ToString(), DatasetId = 2 });
            list.Add(new SandboxDatasetDto() { Classification = DatasetClassification.Open.ToString(), DatasetId = 3 });
            var sandboxDetails = new SandboxDetails() { Datasets = list };
            DatasetClassificationUtils.SetRestrictionProperties(sandboxDetails);

            Assert.Equal(DatasetConstants.DATASET_RESTRICTION_TEXT_OPEN, sandboxDetails.RestrictionDisplayText);
        }

        [Fact]
        public void SetRestrictionProperties_AvailableDatasets_ShouldSetRestrictedDisplayText()
        {
            var list = new List<AvailableDatasetItem>();

            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Open.ToString(), DatasetId = 1, AddedToSandbox = true });
            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Internal.ToString(), DatasetId = 2, AddedToSandbox = true });
            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Restricted.ToString(), DatasetId = 3, AddedToSandbox = true });

            AvailableDatasets test = new AvailableDatasets(list);
            test.Datasets = list;
            //test.Datasets = List;
            //ar sandboxDetails = new SandboxDetails() { Datasets = list };
            DatasetClassificationUtils.SetRestrictionProperties(test);

            Assert.Equal(DatasetConstants.DATASET_RESTRICTION_TEXT_RESTRICTED, test.RestrictionDisplayText);
        }

        [Fact]
        public void SetRestrictionProperties_AvailableDatasets_ShouldSetInternalDisplayText()
        {
            var list = new List<AvailableDatasetItem>();

            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Open.ToString(), DatasetId = 1, AddedToSandbox = true });
            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Internal.ToString(), DatasetId = 2, AddedToSandbox = true });
            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Internal.ToString(), DatasetId = 3, AddedToSandbox = true });

            AvailableDatasets test = new AvailableDatasets(list);
            test.Datasets = list;
            //test.Datasets = List;
            //ar sandboxDetails = new SandboxDetails() { Datasets = list };
            DatasetClassificationUtils.SetRestrictionProperties(test);

            Assert.Equal(DatasetConstants.DATASET_RESTRICTION_TEXT_INTERNAL, test.RestrictionDisplayText);
        }

        [Fact]
        public void SetRestrictionProperties_AvailableDatasets_ShouldSetOpendDisplayText()
        {
            var list = new List<AvailableDatasetItem>();

            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Open.ToString(), DatasetId = 1, AddedToSandbox = true });
            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Open.ToString(), DatasetId = 2, AddedToSandbox = true });
            list.Add(new AvailableDatasetItem() { Classification = DatasetClassification.Open.ToString(), DatasetId = 3, AddedToSandbox = true });

            AvailableDatasets test = new AvailableDatasets(list);
            test.Datasets = list;
            //test.Datasets = List;
            //ar sandboxDetails = new SandboxDetails() { Datasets = list };
            DatasetClassificationUtils.SetRestrictionProperties(test);

            Assert.Equal(DatasetConstants.DATASET_RESTRICTION_TEXT_OPEN, test.RestrictionDisplayText);
        }
    }
}
