using AutoMapper;
using Microsoft.Graph;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
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
                    .ForMember(dest => dest.Participants, source => source.MapFrom(x => x.StudyParticipants));


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

            CreateMap<SandboxDto, Sandbox>();

            CreateMap<SandboxCreateDto, Sandbox>();

            CreateMap<SandboxResource, SandboxResourceLightDto>()
            .ForMember(dest => dest.Name, source => source.MapFrom(x => x.ResourceName))
             .ForMember(dest => dest.LastKnownProvisioningState, source => source.MapFrom(x => x.LastKnownProvisioningState))
             .ForMember(dest => dest.Type, source => source.MapFrom(x => x.ResourceType))
              .ForMember(dest => dest.Status, source => source.MapFrom(x => x.Status))
             ;

            //CLOUD RESOURCE

            CreateMap<SandboxResource, SandboxResourceDto>()
                .ForMember(dest => dest.Tags, source => source.MapFrom(x => AzureResourceTagsFactory.TagStringToDictionary(x.Tags)))
                .ForMember(dest=> dest.SandboxName, source => source.MapFrom(s=> s.Sandbox.Name));


            CreateMap<SandboxResourceDto, SandboxResource>()
                .ForMember(dest => dest.Tags, source => source.MapFrom(x => AzureResourceTagsFactory.TagDictionaryToString(x.Tags)));

            CreateMap<SandboxResourceOperation, SandboxResourceOperationDto>()
                .ReverseMap();

            //USERS/PARTICIPANTS

            CreateMap<User, ParticipantDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<User, ParticipantListItemDto>()
                    .ForMember(dest => dest.Name, source => source.MapFrom(x => x.FullName));

            CreateMap<User, AzureADUserDto>()
                    .ForMember(dest => dest.DisplayName, source => source.MapFrom(x => x.FullName))
                    .ForMember(dest => dest.Mail, source => source.MapFrom(x => x.EmailAddress))
                    ;

            //Graph API: TODO: CHANGE target tp ParticipantListItemDto
            CreateMap<Microsoft.Graph.User, ParticipantListItemDto>()
                .ForMember(dest => dest.Name, source => source.MapFrom(x => x.DisplayName))
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(x => x.Mail))
                .ForMember(dest => dest.Id, source => source.MapFrom(x => 1))
                .ForMember(dest => dest.UserName, source => source.MapFrom(x => x.Mail))
                .ForMember(dest => dest.Source, source => source.MapFrom(x => x.Mail))
                .ForMember(dest => dest.AzureObjectId, source => source.MapFrom(x => x.Mail))
                ;
            ;
            CreateMap<Microsoft.Graph.User, ParticipantListItemDto>().ReverseMap();
            CreateMap<AzureADUserDto, ParticipantListItemDto>().ReverseMap();
            CreateMap<AzureADUserDto, ParticipantListItemDto>()
                .ForMember(dest => dest.Name, source => source.MapFrom(x => x.DisplayName))
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(x => x.Mail));
            ;

            CreateMap<ParticipantListItemDto, AzureADUserDto>()
                .ForMember(dest => dest.DisplayName, source => source.MapFrom(x => x.Name))
                .ForMember(dest => dest.Mail, source => source.MapFrom(x => x.EmailAddress))
                .ForMember(dest => dest.Id, source => source.MapFrom(x => x.Name))
                .ForMember(dest => dest.MobilePhone, source => source.MapFrom(x => x.EmailAddress));
            ;

            CreateMap<Microsoft.Graph.User, AzureADUserDto>().ReverseMap()
            ;

            CreateMap<ParticipantListItemDto, AzureADUserDto>()
            ;

            CreateMap<User, UserCreateDto>()
              .ReverseMap();

            CreateMap<StudyParticipant, StudyParticipantDto>()
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(x => x.User.EmailAddress))
                .ForMember(dest => dest.FullName, source => source.MapFrom(x => x.User.FullName))
                .ForMember(dest => dest.UserName, source => source.MapFrom(x => x.User.UserName))
                  .ForMember(dest => dest.Role, source => source.MapFrom(x => x.RoleName));         

         

            //AZURE
            CreateMap<IResource, AzureResourceDto>();

            CreateMap<IResourceGroup, AzureResourceGroupDto>()
                 .ForMember(dest => dest.ProvisioningState, source => source.MapFrom(x => x.ProvisioningState));
        }
    }
}
