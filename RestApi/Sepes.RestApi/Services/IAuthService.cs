namespace Sepes.RestApi.Services
{
    public interface IAuthService
    {
        string GenerateJSONWebToken(string AZtoken, string OldSepesToken);
    }
}
