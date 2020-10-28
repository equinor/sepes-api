using AutoMapper;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Util;
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
                    source => source.MapFrom(x => x.Resources))
                     .ForMember(dest => dest.StudyName,
                    source => source.MapFrom(x => x.Study.Name));

            CreateMap<SandboxDto, Sandbox>();

            CreateMap<SandboxCreateDto, Sandbox>();

            CreateMap<SandboxResource, SandboxResourceLightDto>()
            .ForMember(dest => dest.Name, source => source.MapFrom(x => x.ResourceName))
             .ForMember(dest => dest.LastKnownProvisioningState, source => source.MapFrom(x => x.LastKnownProvisioningState))
             .ForMember(dest => dest.Type, source => source.MapFrom(x => AzureResourceTypeUtil.GetUserFriendlyName(x)))
              .ForMember(dest => dest.Status, source => source.MapFrom(x => AzureResourceStatusUtil.ResourceStatus(x)))
                .ForMember(dest => dest.LinkToExternalSystem, source => source.MapFrom<SandboxResourceExternalLinkResolver>())
              ;


            //CLOUD RESOURCE

            CreateMap<SandboxResource, SandboxResourceDto>()
                .ForMember(dest => dest.Tags, source => source.MapFrom(x => AzureResourceTagsFactory.TagStringToDictionary(x.Tags)))
                .ForMember(dest => dest.SandboxName, source => source.MapFrom(s => s.Sandbox.Name))
            .ForMember(dest => dest.StudyName, source => source.MapFrom(s => s.Sandbox.Study.Name));


            CreateMap<SandboxResourceDto, SandboxResource>()
                .ForMember(dest => dest.Tags, source => source.MapFrom(x => AzureResourceTagsFactory.TagDictionaryToString(x.Tags)));

            CreateMap<SandboxResourceOperation, SandboxResourceOperationDto>();
            CreateMap<SandboxResourceOperationDto, SandboxResourceOperation>();


            //USERS/PARTICIPANTS

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<User, ParticipantLookupDto>()
                    .ForMember(dest => dest.Source, source => source.MapFrom(s => ParticipantSource.Db))
                    .ForMember(dest => dest.DatabaseId, source => source.MapFrom(s => s.Id));

            CreateMap<Microsoft.Graph.User, ParticipantLookupDto>()
                    .ForMember(dest => dest.FullName, source => source.MapFrom(x => x.DisplayName))
                    .ForMember(dest => dest.EmailAddress, source => source.MapFrom(x => x.Mail))
                    .ForMember(dest => dest.ObjectId, source => source.MapFrom(x => x.Id))
                    .ForMember(dest => dest.UserName, source => source.MapFrom(x => x.UserPrincipalName))
                    .ForMember(dest => dest.Source, source => source.MapFrom(s => ParticipantSource.Azure));

            CreateMap<StudyParticipant, StudyParticipantDto>()
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(x => x.User.EmailAddress))
                .ForMember(dest => dest.FullName, source => source.MapFrom(x => x.User.FullName))
                .ForMember(dest => dest.UserName, source => source.MapFrom(x => x.User.UserName))
                .ForMember(dest => dest.Role, source => source.MapFrom(x => x.RoleName));

            //AZURE
            CreateMap<IResource, AzureResourceDto>();

            CreateMap<IResourceGroup, AzureResourceGroupDto>()
                 .ForMember(dest => dest.ProvisioningState, source => source.MapFrom(x => x.ProvisioningState));

            CreateMap<CreateVmUserInputDto, VmSettingsDto>();

            CreateMap<SandboxResourceDto, VmDto>()
                .ForMember(dest => dest.Name, source => source.MapFrom(x => x.ResourceName))
                 .ForMember(dest => dest.Region, source => source.MapFrom(x => RegionStringConverter.Convert(x.Region).Name));

            CreateMap<SandboxResource, VmDto>()
           .ForMember(dest => dest.Name, source => source.MapFrom(x => x.ResourceName))
            .ForMember(dest => dest.Region, source => source.MapFrom(x => RegionStringConverter.Convert(x.Region).Name))
            .ForMember(dest => dest.Status, source => source.MapFrom(x => AzureResourceStatusUtil.ResourceStatus(x)))
            .ForMember(dest => dest.OperatingSystem, source => source.MapFrom(x => AzureVmUtil.GetOsName(x)))
                   .ForMember(dest => dest.LinkToExternalSystem, source => source.MapFrom<SandboxResourceExternalLinkResolver>());
        }
    }
}
