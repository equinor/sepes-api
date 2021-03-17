using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class ValidationUtilsTest
    {
        [Fact]
        public void ThrowIfValidationErrors_NoErrorsShouldRun()
        {
            var errors = new List<string>();
            ValidationUtils.ThrowIfValidationErrors("prefix", errors);
        }

        [Fact]
        public void ThrowIfValidationErrors_NoErrorsShouldThrow()
        {
            var errors = new List<string>();
            ValidationUtils.ThrowIfValidationErrors("prefix", null);
        }

        [Fact]
        public void ThrowIfValidationErrors_ShouldThrowWithCorrectError()
        {
            var errors = new List<string>();
            errors.Add("Error1");
            errors.Add("Error2");
            var ex = Assert.Throws<Exception>(() => ValidationUtils.ThrowIfValidationErrors("prefix", errors));
            Assert.Equal("prefix: Error1\r\nError2\r\n", ex.Message);
        }
    }
}
