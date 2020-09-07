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
                    .ForMember(dest => dest.Participants,source => source.MapFrom(x => x.StudyParticipants))
                    ;

            CreateMap<StudyDto, Study>();
            CreateMap<StudyCreateDto, Study>();

            //DATASET
            CreateMap<Dataset, DatasetDto>()
                .ForMember(dest => dest.Studies,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study).ToList()))
                .ReverseMap();

            CreateMap<Dataset, DatasetListItemDto>();

            CreateMap<Dataset, DataSetsForStudyDto>();

            CreateMap<Dataset, StudySpecificDatasetDto>()
                .ForMember(dest => dest.StudyNo,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study.Id)))
                .ReverseMap();

            //SANDBOX
            CreateMap<Sandbox, SandboxDto>()
                 .ForMember(dest => dest.Resources,
                    source => source.MapFrom(x => x.Resources));

            CreateMap<SandboxResource, SandboxResourceLightDto>()
                    .ForMember(dest => dest.Name,source => source.MapFrom(x => x.ResourceName))
                     .ForMember(dest => dest.Status, source => source.MapFrom(x => x.LastKnownProvisioningState))
                     .ForMember(dest => dest.Type, source => source.MapFrom(x => x.ResourceType));

            CreateMap<SandboxDto, Sandbox>();
              

            CreateMap<SandboxCreateDto, Sandbox>();
          

            //STUDY PARTICIPANTS
            CreateMap<User, ParticipantDto>()
                .ReverseMap();
            CreateMap<User, ParticipantListItemDto>()
                    .ForMember(dest => dest.Name, source => source.MapFrom(x => x.FullName));

            CreateMap<ParticipantDto, User>();

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<StudyParticipant, StudyParticipantDto>()
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(x => x.User.EmailAddress))
                .ForMember(dest => dest.FullName, source => source.MapFrom(x => x.User.FullName))
                .ForMember(dest => dest.UserName, source => source.MapFrom(x => x.User.UserName))
                  .ForMember(dest => dest.Role, source => source.MapFrom(x => x.RoleName));

            //CLOUD RESOURCE
            CreateMap<SandboxResource, SandboxResourceDto>()
                .ReverseMap();

            CreateMap<SandboxResourceOperation, SandboxResourceOperationDto>()
                .ReverseMap();
        }
    }
}
