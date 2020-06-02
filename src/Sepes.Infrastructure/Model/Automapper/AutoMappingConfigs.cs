using AutoMapper;
using Sepes.Infrastructure.Dto;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class AutoMappingConfigs : Profile
    {
        public AutoMappingConfigs()
        {
            CreateMap<Study, StudyDto>();

        }
    }
}
