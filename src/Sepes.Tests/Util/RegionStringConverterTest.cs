using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class RegionStringConverterTest
    {
        [InlineData("norwayeast", "norwayeast")]
        [InlineData("westeurope", "europewest")]
        [InlineData("northeurope", "europenorth")]
        [InlineData("norwayeast", "")]
        [InlineData("norwayeast", null)]
        [InlineData("norwayeast", "abcd")]
        [Theory]
        public void Convert_ShouldContainStudyName(string expectedResult, string region)
        {
            var result = RegionStringConverter.Convert(region);
            Assert.Equal(expectedResult, result.Name);
        }
    }
}
