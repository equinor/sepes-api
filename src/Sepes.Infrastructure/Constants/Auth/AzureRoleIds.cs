namespace Sepes.Infrastructure.Constants.Auth
{
    public static class AzureRoleIds
    {
        public const string OWNER = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635";
        public const string READ = "acdd72a7-3385-48ef-bd42-f606fba81ae7";
        public const string CONTRIBUTOR = "b24988ac-6180-42a0-ab88-20f7382dd24c";


        public static string CreateRoleDefinitionUrl(string resourceId, string roleId)
        {
            return $"{resourceId}/providers/Microsoft.Authorization/roleDefinitions/{roleId}";
        }

        public static string GetRoleIdFromDefinition(string roleDefinitionId)
        {
            var partToLookFor = "/";
            var idx = roleDefinitionId.LastIndexOf(partToLookFor);

            if(idx > 0)
            {
                return roleDefinitionId.Substring(idx+1);
            }

            return null;
        }
    }
}
