using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using Sepes.Tests.Services;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class AzureResourceTagsFactoryTest : ServiceTestBase
    {
        [Fact]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue()
        {

            var config = AzureResourceTagsFactory_Factory.GetConfiguration(_serviceProvider);
            List<StudyParticipant> participants = new List<StudyParticipant>();
            participants.Add(new StudyParticipant() { RoleName = "Study Owner" });
            var study = new Study() { Name = "Study1", WbsCode = "123", StudyParticipants = participants };
            var sandbox = new Sandbox() { Study = study, Name = "Sandbox1" };

            var res = AzureResourceTagsFactory.SandboxResourceTags(config, study, sandbox);

            Dictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("txt", "notepad.exe");

            Assert.Equal(expectedResult, res);
        }
    }
}
