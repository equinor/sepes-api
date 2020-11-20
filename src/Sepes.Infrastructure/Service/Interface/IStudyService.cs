﻿using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyService
    {        
        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync(bool? excludeHidden = null);
        Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperations userOperation);

        Task<StudyDetailsDto> GetStudyDetailsDtoByIdAsync(int studyId, UserOperations userOperation);

        Task<StudyDto> CreateStudyAsync(StudyCreateDto newStudy);

        Task<StudyDto> UpdateStudyDetailsAsync(int studyId, StudyDto newStudy);

        Task DeleteStudyAsync(int studyId);

        Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo);

        //Task<byte[]> GetLogoAsync(int id);

        Task<LogoResponseDto> GetLogoAsync(int id);
    }
}
