using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantService : IStudyParticipantService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IAzureADUsersService _azureADUsersService;

        public StudyParticipantService(SepesDbContext db, IMapper mapper, IAzureADUsersService azureADUsersService)
        {            
            _db = db;
            _mapper = mapper;
            _azureADUsersService = azureADUsersService;
        }

        public async Task<IEnumerable<ParticipantListItemDto>> GetLookupAsync(string searchText, int limit = 30)
        {
            var participantsFromAzureAdTask = _azureADUsersService.SearchUsersAsync(searchText, limit);
            var participantsFromDbTask = _db.Users.ToListAsync();

            await Task.WhenAll(participantsFromAzureAdTask, participantsFromDbTask);

            //TODO: Merge results and bake into DTO
            //Remember source: If exists in both db and azure (based on object id), then source is DB
            //remember to demove duplicates
            //Sort ??

            var participantDtos = _mapper.Map<IEnumerable<ParticipantListItemDto>>(participantsFromDb);

            return participantDtos;  
        }
    }
}
