using AutoMapper;
using Sepes.Infrastructure.Dto;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class AutoMappingConfigs : Profile
    {
        public AutoMappingConfigs()
        {
            //STUDY VARIATIONS
            CreateMap<Study, StudyListItemDto>();
            CreateMap<Study, StudyDto>();
            CreateMap<StudyDto, Study>();
            CreateMap<Sandbox, SandboxDto>();
            CreateMap<SandboxDto, Sandbox>();

            //DATASET VARIATIONS
            CreateMap<Dataset, DatasetDto>();
            CreateMap<Dataset, DatasetListItemDto>();

        }
    }
}
