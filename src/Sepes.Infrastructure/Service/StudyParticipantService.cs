using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
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

        public async Task<IEnumerable<ParticipantLookupDto>> GetLookupAsync(string searchText, int limit = 30)
        {
            var usersFromAzureAdTask = _azureADUsersService.SearchUsersAsync(searchText, limit);
            var usersFromDbTask = _db.Users.Where(u => u.EmailAddress.Contains(searchText) || u.FullName.Contains(searchText) || u.ObjectId.Equals(searchText)).ToListAsync();

            await Task.WhenAll(usersFromAzureAdTask, usersFromDbTask);

            var usersFromDb = _mapper.Map<IEnumerable<ParticipantLookupDto>>(usersFromDbTask.Result);        
            var usersFromDbAsDictionary = usersFromDb.ToDictionary(p => p.ObjectId, p => p);

            var usersFromAzureAd = _mapper.Map<IEnumerable<ParticipantLookupDto>>(usersFromAzureAdTask.Result).ToList();


            foreach (var curAzureUser in usersFromAzureAd)
            {
                if(usersFromDbAsDictionary.ContainsKey(curAzureUser.ObjectId) == false)
                {
                    usersFromDbAsDictionary.Add(curAzureUser.ObjectId, curAzureUser);
                }               
            }      

            return usersFromDbAsDictionary.OrderBy(o => o.Value.FullName).Select(o => o.Value);
        }
    }
}
