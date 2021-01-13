namespace Sepes.Infrastructure.Constants.Auth
{
    public static class AzureRoleIds
    {

        public const string READ = "acdd72a7-3385-48ef-bd42-f606fba81ae7";

        public static string CreateUrl(string resourceId, string roleId)
        {
            return $"{resourceId}/providers/Microsoft.Authorization/roleDefinitions/{roleId}";
        }
    }
}
