using Sepes.Infrastructure.Model;
using Sepes.Tests.Common.Constants;

namespace Sepes.Tests.Common.ModelFactory
{
    public static class StudyModelFactory
    {
        public static Study CreateBasic(int id = StudyConstants.CREATED_BY_ME_ID, string name = StudyConstants.CREATED_BY_ME_NAME, string vendor = StudyConstants.CREATED_BY_ME_VENDOR, string wbs = StudyConstants.CREATED_BY_ME_WBS) {
            return new Study() {
                Id = id,
                Name = name,
                Vendor = vendor,
                WbsCode = wbs
            };
        } }
}
