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
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Dataset).ToList()))
                .ReverseMap();

            //SANDBOX VARIATIONS
            CreateMap<Sandbox, SandboxDto>()
                .ReverseMap();

            //DATASET VARIATIONS
            CreateMap<Dataset, DatasetDto>()
                .ForMember(dest => dest.Studies,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study).ToList()))
                .ReverseMap();

            CreateMap<Dataset, DatasetListItemDto>();

            CreateMap<Dataset, StudySpecificDatasetDto>()
                .ForMember(dest => dest.StudyId,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study.Id)))
                .ReverseMap();
        }
    }
}
