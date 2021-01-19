﻿using AutoMapper;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.Study;
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
            CreateMap<Study, StudyDto>()
          .ForMember(dest => dest.Participants, source => source.MapFrom(x => x.StudyParticipants));    

            CreateMap<Study, StudyDetailsDto>()
                .ForMember(dest => dest.Datasets, source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Dataset).Where(d=> d.Deleted.HasValue == false || (d.Deleted.HasValue && d.Deleted.Value == false)).ToList()))
                .ForMember(dest => dest.Sandboxes, source => source.MapFrom(x => x.Sandboxes.Where(sb => sb.Deleted.HasValue == false || (sb.Deleted.HasValue && sb.Deleted.Value == false)).ToList()))
                .ForMember(dest => dest.Participants, source => source.MapFrom(x => x.StudyParticipants));


            CreateMap<Study, StudyListItemDto>();
            CreateMap<StudyDto, Study>();
            CreateMap<StudyCreateDto, Study>();

            //DATASET
            CreateMap<Dataset, DatasetDto>()
                .ForMember(dest => dest.Studies,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study).ToList()))
                .ReverseMap();

            CreateMap<Dataset, DatasetListItemDto>();

            CreateMap<Dataset, DatasetDto>()
                 .ForMember(dest => dest.StorageAccountLink, source => source.MapFrom<DatasetStorageExternalLinkResolver>());

            CreateMap<StudyDataset, StudyDatasetDto>()
                  .ForMember(dest => dest.Id, source => source.MapFrom(x => x.Dataset.Id))
                        .ForMember(dest => dest.Name, source => source.MapFrom(x => x.Dataset.Name))
                .ForMember(dest => dest.DataId, source => source.MapFrom(x => x.Dataset.DataId))                
                .ForMember(dest => dest.Classification, source => source.MapFrom(x => x.Dataset.Classification));

            CreateMap<SandboxDataset, SandboxDatasetDto>()
                .ForMember(dest => dest.DatasetId, source => source.MapFrom(x => x.Dataset.Id))
                .ForMember(dest => dest.Name, source => source.MapFrom(x => x.Dataset.Name))
                .ForMember(dest => dest.Classification, source => source.MapFrom(x => x.Dataset.Classification))
                .ForMember(dest => dest.SandboxName, opt =>
                {
                    opt.PreCondition(src => (!src.Sandbox.Deleted.HasValue));
                    opt.MapFrom(src =>

                        src.Sandbox.Name
                    );
                })
                .ForMember(dest => dest.SandboxId, source => source.MapFrom(x => x.Sandbox.Id))
                .ForMember(dest => dest.StudyId, source => source.MapFrom(x => x.Sandbox.StudyId));

            /*
            CreateMap<Dataset, StudySpecificDatasetDto>()
                .ForMember(dest => dest.StudyNo,
                    source => source.MapFrom(x => x.StudyDatasets.Select(y => y.Study.Id)))
                .ReverseMap();
*/
            CreateMap<DatasetCreateUpdateInputBaseDto, Dataset>();

            //SANDBOX
            CreateMap<Sandbox, SandboxDto>()
                 .ForMember(dest => dest.StudyName, source => source.MapFrom(x => x.Study.Name))
                  .ForMember(dest => dest.CurrentPhase, source => source.MapFrom<SandboxPhaseNameResolver>());
           

            CreateMap<Sandbox, SandboxListItemDto>();

            CreateMap<Sandbox, SandboxDetailsDto>()
         .ForMember(dest => dest.Resources, source => source.MapFrom(x => x.Resources))
         .ForMember(dest => dest.StudyName, source => source.MapFrom(x => x.Study.Name))
         .ForMember(dest => dest.Datasets, source => source.MapFrom(x => x.SandboxDatasets))
         .ForMember(dest => dest.LinkToCostAnalysis, source => source.MapFrom<SandboxResourceExternalCostAnalysis>())
           .ForMember(dest => dest.CurrentPhase, source => source.MapFrom<SandboxPhaseNameResolver>());

            /*
            CreateMap<Sandbox, DatasetSandboxDto>()
                 .ForMember(dest => dest.Id, source => source.MapFrom(x => x.Id))
                     .ForMember(dest => dest.Name, source => source.MapFrom(x => x.Name));

            CreateMap<DatasetSandbox, DatasetSandboxDto>()
                 .ForMember(dest => dest.Id, source => source.MapFrom(x => x.Id))
                     .ForMember(dest => dest.Name, source => source.MapFrom(x => x.Name));*/

            CreateMap<SandboxDto, Sandbox>();

            CreateMap<SandboxCreateDto, Sandbox>();

            CreateMap<CloudResource, SandboxResourceLightDto>()
            .ForMember(dest => dest.Name, source => source.MapFrom(x => x.ResourceName))
             .ForMember(dest => dest.LastKnownProvisioningState, source => source.MapFrom(x => x.LastKnownProvisioningState))
             .ForMember(dest => dest.Type, source => source.MapFrom(x => AzureResourceTypeUtil.GetUserFriendlyName(x)))
              .ForMember(dest => dest.Status, source => source.MapFrom(x => AzureResourceStatusUtil.ResourceStatus(x)))
                .ForMember(dest => dest.LinkToExternalSystem, source => source.MapFrom<SandboxResourceExternalLinkResolver>())
                .ForMember(dest => dest.RetryLink, source => source.MapFrom<SandboxResourceRetryLinkResolver>());
            

            //CLOUD RESOURCE

            CreateMap<CloudResource, CloudResourceDto>()
                .ForMember(dest => dest.Tags, source => source.MapFrom(x => AzureResourceTagsFactory.TagStringToDictionary(x.Tags)))
                .ForMember(dest => dest.SandboxName, source => source.MapFrom(s => s.Sandbox.Name))
            .ForMember(dest => dest.StudyName, source => source.MapFrom(s => s.Sandbox.Study.Name));


            CreateMap<CloudResourceDto, CloudResource>()
                .ForMember(dest => dest.Tags, source => source.MapFrom(x => AzureResourceTagsFactory.TagDictionaryToString(x.Tags)));

            CreateMap<CloudResourceOperation, CloudResourceOperationDto>();
            CreateMap<CloudResourceOperationDto, CloudResourceOperation>();

            //USERS/PARTICIPANTS

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<User, ParticipantLookupDto>()
                    .ForMember(dest => dest.Source, source => source.MapFrom(s => ParticipantSource.Db))
                    .ForMember(dest => dest.DatabaseId, source => source.MapFrom(s => s.Id));

            CreateMap<Microsoft.Graph.User, AzureUserDto>();           

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
                .ForMember(dest => dest.Role, source => source.MapFrom(x => x.RoleName))
                    .ForMember(dest => dest.Study, source => source.MapFrom(x => x.Study)); ;

            //AZURE
            CreateMap<IResource, AzureResourceDto>();

            CreateMap<IResourceGroup, AzureResourceGroupDto>()
                 .ForMember(dest => dest.ProvisioningState, source => source.MapFrom(x => x.ProvisioningState));

            CreateMap<IStorageAccount, AzureStorageAccountDto>()
                .ForMember(dest => dest.ProvisioningState, source => source.MapFrom(x => x.ProvisioningState));

            CreateMap<CreateVmUserInputDto, VmSettingsDto>();

            CreateMap<CloudResourceDto, VmDto>()
                .ForMember(dest => dest.Name, source => source.MapFrom(x => x.ResourceName))
                 .ForMember(dest => dest.Region, source => source.MapFrom(x => RegionStringConverter.Convert(x.Region).Name));

            CreateMap<CloudResource, VmDto>()
           .ForMember(dest => dest.Name, source => source.MapFrom(x => x.ResourceName))
            .ForMember(dest => dest.Region, source => source.MapFrom(x => RegionStringConverter.Convert(x.Region).Name))
            .ForMember(dest => dest.Status, source => source.MapFrom(x => AzureResourceStatusUtil.ResourceStatus(x)))
            .ForMember(dest => dest.OperatingSystem, source => source.MapFrom(x => AzureVmUtil.GetOsName(x)))
                   .ForMember(dest => dest.LinkToExternalSystem, source => source.MapFrom<SandboxResourceExternalLinkResolver>());


            CreateMap<VmRuleDto, NsgRuleDto>()                
                      .ForMember(dest => dest.Protocol, source => source.MapFrom(x => x.Protocol))
                  .ForMember(dest => dest.Description, source => source.MapFrom(x => x.Description));


            CreateMap<AzureRegionDto, LookupDto>();

            CreateMap<VmSize, VmSizeDto>()
                  .ForMember(dest => dest.Name, source => source.MapFrom(x => x.Key));

            CreateMap<VmSize, VmSizeLookupDto>()
               .ForMember(dest => dest.DisplayValue, source => source.MapFrom(x => x.DisplayText));

            CreateMap<DiskSize, VmDiskLookupDto>()
               .ForMember(dest => dest.DisplayValue, source => source.MapFrom(x => x.DisplayText));

            CreateMap<Region, LookupDto>()
             .ForMember(dest => dest.Key, source => source.MapFrom(x => x.Key))
              .ForMember(dest => dest.DisplayValue, source => source.MapFrom(x => x.Name));
        }
    }
}
