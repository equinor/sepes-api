﻿using Sepes.Common.Dto.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyListService
    {        
        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync();          
    }
}
