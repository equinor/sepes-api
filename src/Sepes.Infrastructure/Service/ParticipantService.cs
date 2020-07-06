using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class ParticipantService : IParticipantService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IStudyService _studyService;

        public ParticipantService(SepesDbContext db, IMapper mapper, IStudyService studyService)
        {            
            _db = db;
            _mapper = mapper;
            _studyService = studyService;
        }

        //public Task<StudyDto> CreateStudyAsync(StudyDto newStudy)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public Task<StudyDto> DeleteStudyAsync(StudyDto newStudy)
        //{
        //    throw new System.NotImplementedException();
        //}

        public async Task<IEnumerable<ParticipantListItemDto>> GetLookupAsync()
        {
            var participantsFromDb = await _db.Participants.ToListAsync();
            var participantDtos = _mapper.Map<IEnumerable<ParticipantListItemDto>>(participantsFromDb);

            return participantDtos;  
        }

        public async Task<ParticipantDto> GetByIdAsync(int id)
        {
            var participantFromDb = await GetOrThrowAsync(id);

            var participantDto = _mapper.Map<ParticipantDto>(participantFromDb);

            return participantDto;
        } 
        
        async Task<Participant> GetOrThrowAsync(int id)
        {
            var entityFromDb = await _db.Participants.FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Participant", id);
            }

            return entityFromDb;
        }

        public async Task<StudyDto> AddParticipantToStudyAsync(int studyId, int participantId, string role)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var participantFromDb = await _db.Participants.FirstOrDefaultAsync(p => p.Id == participantId);

            if (participantFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Participant", participantId);
            }

            //Check that association does not allready exist

            await VerifyRoleOrThrowAsync(role);

            var studyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, ParticipantId = participantId, RoleName = role };
            await _db.StudyParticipants.AddAsync(studyParticipant);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyByIdAsync(studyId);
        }

        public async Task<StudyDto> RemoveParticipantFromStudyAsync(int studyId, int participantId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var participantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.ParticipantId == participantId);

            if (participantFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Participant", participantId);
            }

            studyFromDb.StudyParticipants.Remove(participantFromDb);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyByIdAsync(studyId);
        }

        public async Task VerifyRoleOrThrowAsync(string roleName)
        {
            var roleExists = false;

            var roleIsPermittedForParticipant = false;

        }
    }
}
