using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class SandboxSeed
    {
        public static async Task<Sandbox> Create(
                int studyId,
                string name = SandboxConstants.NAME,
                string region = SandboxConstants.REGION,
                SandboxPhase phase = SandboxPhase.Open)
        {
            var sandbox = SandboxBasic(studyId, name, region, phase);

            //Todo: add resources

            return await SliceFixture.InsertAsync(sandbox);
        }

        static Sandbox SandboxBasic(int studyId, string name, string region, SandboxPhase phase = SandboxPhase.Open)
        {
            var sb = new Sandbox()
            {
                StudyId = studyId,
                Name = name,
                Region = region,
                PhaseHistory = new List<SandboxPhaseHistory>(),
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow,
                TechnicalContactEmail = "seedcreator@somesystem.com",
                TechnicalContactName = "Seed"
                
            };

            int counter = 0;

            for (var curPhase = SandboxPhase.Open; curPhase <= phase; curPhase++)
            {
                sb.PhaseHistory.Add(new SandboxPhaseHistory() { Counter = counter, Phase = curPhase, CreatedBy = "seed" });
                counter++;
            }

            return sb;
        }
      
    }
}
