using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<AzureADUserDto>> GetLookupAsync(string searchText, int limit = 30)
        {
            var participantsFromAzureAdTask = _azureADUsersService.SearchUsersAsync(searchText, limit);
            var participantsFromDbTask = _db.Users.Where(u => u.FullName.Contains(searchText)).ToListAsync();

            await Task.WhenAll(participantsFromAzureAdTask, participantsFromDbTask);

            var participantDtos = _mapper.Map<IEnumerable<AzureADUserDto>>(participantsFromDbTask.Result).ToList();

            var participantAzureDtos = _mapper.Map<IEnumerable<AzureADUserDto>>(participantsFromAzureAdTask.Result).ToList();
 
            foreach (var participantFromDb in participantDtos.ToList())
            {
                participantFromDb.Source = ParticipantSource.Db;
                try
                {
                    participantFromDb.DatabaseId = Int32.Parse(participantFromDb.Id);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unable to parse '{participantFromDb.Id}'");
                }
            }

            foreach (var participantFromAzure in participantAzureDtos)
            {
                var azureParticipantInDb = participantDtos.Where(i => i.ObjectId == participantFromAzure.Id).FirstOrDefault();
                if (azureParticipantInDb == null)
                {
                    participantFromAzure.Source = ParticipantSource.Azure;
                    participantDtos.Add(participantFromAzure);
                    
                }
            }

            return participantDtos.OrderBy(o => o.DisplayName).ToList();  
        }
    }
}
