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

        public StudyParticipantService(SepesDbContext db, IMapper mapper)
        {            
            _db = db;
            _mapper = mapper;       
        }

        public async Task<IEnumerable<ParticipantListItemDto>> GetLookupAsync()
        {
            var participantsFromDb = await _db.Users.ToListAsync();
            var participantDtos = _mapper.Map<IEnumerable<ParticipantListItemDto>>(participantsFromDb);

            return participantDtos;  
        }
    }
}
