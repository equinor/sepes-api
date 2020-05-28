using AutoMapper;
using Sepes.Infrastructure.Dto;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class StudyMapping : Profile
    {
        public StudyMapping()
        {
            CreateMap<Study, StudyDto>();
        }
    }
}
