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
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study).ToList()));
            CreateMap<DatasetDto, Dataset>();
            CreateMap<Dataset, DatasetListItemDto>();

            //SANDBOX
            CreateMap<Sandbox, SandboxDto>();
            CreateMap<SandboxDto, Sandbox>();

            //STUDY PARTICIPANTS
            CreateMap<Participant, ParticipantDto>();
            CreateMap<Participant, ParticipantListItemDto>();

        }
    }
}
