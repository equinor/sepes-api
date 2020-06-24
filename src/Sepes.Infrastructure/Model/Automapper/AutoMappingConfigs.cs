using AutoMapper;
using Sepes.Infrastructure.Dto;
using System.Linq;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class AutoMappingConfigs : Profile
    {
        public AutoMappingConfigs()
        {
            //STUDY VARIATIONS
            CreateMap<Study, StudyListItemDto>();
            CreateMap<Study, StudyDto>()
                .ForMember(dest => dest.Datasets,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Dataset).ToList()));
            CreateMap<StudyDto, Study>();

            CreateMap<Sandbox, SandboxDto>();
            CreateMap<SandboxDto, Sandbox>();

            //DATASET VARIATIONS
            CreateMap<Dataset, DatasetDto>()
                .ForMember(dest => dest.Studies,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study).ToList()));
            CreateMap<DatasetDto, Dataset>();
            CreateMap<Dataset, DatasetListItemDto>();
        }
    }
}
