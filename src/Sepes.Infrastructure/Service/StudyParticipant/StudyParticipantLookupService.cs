using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantLookupService : StudyParticipantBaseService, IStudyParticipantLookupService
    {
        readonly IAzureUserService _azureUserService;
        readonly IConfiguration _configuration;

        public StudyParticipantLookupService(SepesDbContext db,
            ILogger<StudyParticipantLookupService> logger,
            IMapper mapper,
            IUserService userService,
            IAzureUserService azureUserService,
            IStudyModelService studyModelService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceReadService cloudResourceReadService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService,
            IConfiguration configuration)
            : base(db, mapper, logger, userService, studyModelService, provisioningQueueService, cloudResourceReadService, cloudResourceOperationCreateService, cloudResourceOperationUpdateService)
        {
            _azureUserService = azureUserService;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ParticipantLookupDto>> GetLookupAsync(string searchText, int limit = 30, CancellationToken cancellationToken = default)
        {
            if (await _userService.IsMockUser())
            {
                var listWithMockUser = new List<ParticipantLookupDto>();
                listWithMockUser.Add(new ParticipantLookupDto
                {
                    ObjectId = _configuration[ConfigConstants.CYPRESS_MOCK_USER],
                    FullName = "Mock user",
                    UserName = "Mock User",
                    EmailAddress = "Mock@user.com",
                    Source = "Azure"
                });
                return listWithMockUser;
            }
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<ParticipantLookupDto>();
            }
            Task<List<Microsoft.Graph.User>> usersFromAzureAdTask = null;
            try
            {
                usersFromAzureAdTask = _azureUserService.SearchUsersAsync(searchText, limit, cancellationToken);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Could not get user list from Azure. Use only list from DB instead");
            }

            var usersFromDbTask = _db.Users.Where(u => u.EmailAddress.StartsWith(searchText) || u.FullName.StartsWith(searchText) || u.ObjectId.Equals(searchText)).ToListAsync(cancellationToken);

            await Task.WhenAll(usersFromDbTask, usersFromAzureAdTask);

            var usersFromDb = _mapper.Map<IEnumerable<ParticipantLookupDto>>(usersFromDbTask.Result);
            var usersFromDbAsDictionary = new Dictionary<string, ParticipantLookupDto>();

            foreach (var curUserFromDb in usersFromDb)
            {
                if (string.IsNullOrWhiteSpace(curUserFromDb.ObjectId))
                {
                    continue;
                }

                if (!usersFromDbAsDictionary.ContainsKey(curUserFromDb.ObjectId))
                {
                    usersFromDbAsDictionary.Add(curUserFromDb.ObjectId, curUserFromDb);
                }
            }

            if (usersFromAzureAdTask.IsCompletedSuccessfully)
            {
                var usersFromAzureAd = _mapper.Map<IEnumerable<ParticipantLookupDto>>(usersFromAzureAdTask.Result).ToList();

                foreach (var curAzureUser in usersFromAzureAd)
                {
                    if (!usersFromDbAsDictionary.ContainsKey(curAzureUser.ObjectId))
                    {
                        usersFromDbAsDictionary.Add(curAzureUser.ObjectId, curAzureUser);
                    }
                }
            }

            return usersFromDbAsDictionary.OrderBy(o => o.Value.FullName).Select(o => o.Value);
        }
    }
}
