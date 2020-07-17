using AutoMapper;
using Sepes.Infrastructure.Dto;
using System.Linq;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class AutoMappingConfigs : Profile
    {
        public AutoMappingConfigs()
        {
            //STUDY
            CreateMap<Study, StudyListItemDto>();

            CreateMap<Study, StudyDto>()
                .ForMember(dest => dest.Datasets,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Dataset).ToList()))
                    .ForMember(dest => dest.Participants,
                    source => source.MapFrom(x => x.StudyParticipants.Select(y => y.Participant).ToList()));
            CreateMap<StudyDto, Study>();      

            //DATASET
            CreateMap<Dataset, DatasetDto>()
                .ForMember(dest => dest.Studies,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study).ToList()))
                .ReverseMap();

            CreateMap<Dataset, DatasetListItemDto>();

            CreateMap<Dataset, StudySpecificDatasetDto>()
                .ForMember(dest => dest.StudyNo,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study.Id)))
                .ReverseMap();

            //SANDBOX
            CreateMap<Sandbox, SandboxDto>()
                .ReverseMap();

            //STUDY PARTICIPANTS
            CreateMap<Participant, ParticipantDto>()
                .ReverseMap();
            CreateMap<Participant, ParticipantListItemDto>();

            CreateMap<ParticipantDto, Participant>();

            //CLOUD RESOURCE
            CreateMap<SandboxResource, SandboxResourceDto>()
                .ReverseMap();
        }
    }
}
