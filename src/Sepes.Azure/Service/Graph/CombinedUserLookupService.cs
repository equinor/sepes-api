using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class CombinedUserLookupService : ICombinedUserLookupService
    {
        readonly ICompanyUserLookupService _companyUserLookupService;
        readonly IAffiliateUserLookupService _affiliateUserLookupService;

        public CombinedUserLookupService(ICompanyUserLookupService companyUserLookupService, IAffiliateUserLookupService affiliateUserLookupService)          
        {
            _companyUserLookupService = companyUserLookupService;
            _affiliateUserLookupService = affiliateUserLookupService;
        }

        public async Task<Dictionary<string, AzureUserDto>> SearchAsync(string search, int limit, CancellationToken cancellationToken = default)
        {
            var companyUserTask = _companyUserLookupService.SearchAsync(search, limit, cancellationToken);
            var affiliateUserTask = _affiliateUserLookupService.SearchAsync(search, limit, cancellationToken);

            await Task.WhenAll(companyUserTask, affiliateUserTask);

            var userDictionary = companyUserTask.Result;

            MergeUserDictionaries(userDictionary, affiliateUserTask.Result);

            return userDictionary;
        }

        void MergeUserDictionaries(Dictionary<string, AzureUserDto> target, Dictionary<string, AzureUserDto> source)
        {
            if (source == null)
            {
                return;
            }

            if (target == null)
            {
                return;
            }           

            foreach(var curKvp in source)
            {
                if (target.ContainsKey(curKvp.Key))
                {
                    continue;
                }

                target.Add(curKvp.Key, curKvp.Value);
            }
        }
    }
}
