using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Graph
{
    public class UserFromGroupRequest : IUserFromGroupRequest
    {
        readonly string _groupId;
        readonly int _limit;
        readonly IUserFromGroupLookupService _service;        

        public string SearchText { get; private set; }
        public string SearchName { get; private set; }

        public UserFromGroupRequest(string searchName, IUserFromGroupLookupService service, string groupId, string searchText, int limit)
        {
            SearchName = searchName;
            SearchText = searchText;

            _service = service ?? throw new ArgumentNullException(nameof(service));
            _groupId = groupId ?? throw new ArgumentNullException(nameof(groupId));                  
          
            _limit = limit;
        }   

        public async Task<Dictionary<string, AzureUserDto>> StartRequest(CancellationToken cancellationToken = default)
        {
            return await _service.SearchInGroupAsync(_groupId, SearchText, _limit, cancellationToken);
        }
    }
}
