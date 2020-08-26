﻿using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyService
    {
        
        Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? includeRestricted = null);
        Task<StudyDto> GetStudyByIdAsync(int studyId);

        Task<StudyDto> CreateStudyAsync(StudyDto newStudy);

        Task<StudyDto> UpdateStudyDetailsAsync(int studyId, StudyDto newStudy);

        Task<IEnumerable<StudyListItemDto>> DeleteStudyAsync(int studyId);

        Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo);

        Task<byte[]> GetLogoAsync(int id);

        Task<StudyDto> AddParticipantToStudyAsync(int studyId, int participantId, string role);
        Task<StudyDto> RemoveParticipantFromStudyAsync(int studyId, int participantId);

        /// <summary>
        /// Makes changes to the meta data of a study.
        /// If based is null it means its a new study.
        /// This call will only succeed if based is the same as the current version of that study.
        /// This method will only update the metadata about a study and will not make changes to the azure resources.
        /// </summary>
        /// <param name="newStudy">The updated or new study</param>
        /// <param name="based">The current study</param>
        //Task<StudyDto> Save(StudyDto newStudy, StudyDto based);
    }
}
