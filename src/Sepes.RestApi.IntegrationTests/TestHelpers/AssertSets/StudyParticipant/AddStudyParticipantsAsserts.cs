using Sepes.Common.Dto;
using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.StudyParticipant
{
    public static class AddStudyParticipantsAsserts
    {
        public static void ExpectSuccess(string role, ParticipantLookupDto createRequest, ApiResponseWrapper<StudyParticipantListItem> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyParticipantListItem>(responseWrapper);

            Assert.NotEqual<int>(0, responseWrapper.Content.StudyId);
            Assert.NotEqual<int>(0, responseWrapper.Content.UserId);
            Assert.Equal(role, responseWrapper.Content.Role);
            Assert.Contains(createRequest.UserName, responseWrapper.Content.UserName);
        }

        public static void ExpectSuccess(string role, StudyParticipantListItem createRequest, ApiResponseWrapper<StudyParticipantListItem> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyParticipantListItem>(responseWrapper);

            Assert.NotEqual<int>(0, responseWrapper.Content.StudyId);
            Assert.NotEqual<int>(0, responseWrapper.Content.UserId);
            Assert.Equal(role, responseWrapper.Content.Role);
            Assert.Contains(createRequest.UserName, responseWrapper.Content.UserName);
        }
    }
}
