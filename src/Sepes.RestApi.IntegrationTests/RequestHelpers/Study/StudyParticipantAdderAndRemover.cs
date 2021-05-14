using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class StudyParticipantAdderAndRemover
    {
        static async Task<ApiConversation<ParticipantLookupDto, TResponse>> Add<TResponse>(RestHelper restHelper, int studyId, string role, ParticipantLookupDto requestDto)         
        {         
            var response = await restHelper.Put<TResponse, ParticipantLookupDto>($"api/studies/{studyId}/participants/{role}", requestDto);

            return new ApiConversation<ParticipantLookupDto, TResponse>(requestDto, response);
        }

        public static async Task<ApiConversation<ParticipantLookupDto, StudyParticipantDto>> AddAndExpectSuccess(RestHelper restHelper, int studyId, string role, ParticipantLookupDto requestDto)
        {
            return await Add<StudyParticipantDto>(restHelper, studyId, role, requestDto);
        }

        public static async Task<ApiConversation<ParticipantLookupDto, ErrorResponse>> AddAndExpectFailure(RestHelper restHelper, int studyId, string role, ParticipantLookupDto requestDto)
        {
            return await Add<ErrorResponse>(restHelper, studyId, role, requestDto);
        }

        static async Task<ApiConversation<TResponse>> Remove<TResponse>(RestHelper restHelper, int studyId, int userId, string role)
        {            
            var response = await restHelper.Delete<TResponse>($"api/studies/{studyId}/participants/{userId}/{role}");
            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<StudyParticipantDto>> RemoveAndExpectSuccess(RestHelper restHelper, int studyId, int userId, string role)
        {
            return await Remove<StudyParticipantDto>(restHelper, studyId, userId, role);
        }

        public static async Task<ApiConversation<ErrorResponse>> RemoveAndExpectFailure(RestHelper restHelper, int studyId, int userId, string role)
        {
            return await Remove<ErrorResponse>(restHelper, studyId, userId, role);
        }

        public static ParticipantLookupDto CreateParticipantLookupDto()
        {
            return new ParticipantLookupDto()
            {
                Source = ParticipantSource.Db,
                DatabaseId = TestUserConstants.COMMON_NEW_PARTICIPANT_DB_ID,
                EmailAddress = TestUserConstants.COMMON_NEW_PARTICIPANT_EMAIL,
                FullName = TestUserConstants.COMMON_NEW_PARTICIPANT_FULL_NAME,
                ObjectId = TestUserConstants.COMMON_NEW_PARTICIPANT_OBJECTID,
                UserName = TestUserConstants.COMMON_NEW_PARTICIPANT_UPN
            };
        }
    } 
}
