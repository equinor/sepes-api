﻿using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IUserService
    {
        UserDto GetCurrentUser();

        Task<UserDto> GetCurrentUserFromDb();

        Task<UserDto> GetCurrentUserWithStudyParticipants();
    }
}