using AutoMapper;
using Sepes.Infrastructure.Dto;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class AutoMappingConfigs : Profile
    {
        public AutoMappingConfigs()
        {
            CreateMap<Study, StudyListItemDto>();

            CreateMap<Study, StudyDto>();
            CreateMap<StudyDto, Study>();

            CreateMap<Sandbox, SandBoxDto>();
            CreateMap<SandBoxDto, Sandbox>();

            CreateMap<Dataset, DataSetDto>();
            CreateMap<DataSetDto, Dataset>();
        }
    }
}
