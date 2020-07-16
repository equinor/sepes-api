using Sepes.Infrastructure.Dto;
using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceTagsFactory
    {
        public static Dictionary<string, string> CreateTags(string studyName, StudyDto study, SandboxDto sandbox)
        {
            var tags = CreateBaseTags(studyName);
            tags.Add("WBS", study.WbsCode);
            tags.Add("StudyOwnerName", study.OwnerName);
            tags.Add("StudyOwnerEmail", study.OwnerEmail);
            tags.Add("SandboxName", sandbox.Name);
            tags.Add("TechnicalContactName", sandbox.TechnicalContactName);
            tags.Add("TechnicalContactEmail", sandbox.TechnicalContactEmail);
          
            return tags;      
        }

        public static Dictionary<string, string> CreateUnitTestTags(string studyName)
        {
            var tags = CreateBaseTags(studyName);
            tags.Add("IsUnitTest", "true");
            return tags;
            // var tags = new Dictionary<string, string>() { { "CreatedByMachine", Environment.MachineName } };
        }

        static Dictionary<string, string> CreateBaseTags(string studyName)
        {
           return new Dictionary<string, string>() { { "CreatedByMachine", Environment.MachineName }, { "StudyName",studyName } };
        }

    }
}
