using Sepes.Common.Dto;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class MockUserFactory
    {
        public static UserDto CreateMockUser(string objectId) {
            return new UserDto(
                    objectId,
                    "mock@user.com",
                    "Mock User",
                    "mock@user.com"
                );
        }
    }
}
