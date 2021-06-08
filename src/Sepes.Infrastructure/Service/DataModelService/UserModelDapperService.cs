﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class UserModelDapperService : DapperModelServiceBase, IUserModelService
    {
        public UserModelDapperService(IConfiguration configuration, ILogger<UserModelDapperService> logger)
            : base(configuration, logger)
        {

        }

        public async Task<UserDto> GetByIdAsync(int userId)
        {
            var query = BasicQueryWithUserIdArg();
            return await RunDapperQuerySingleAsync<UserDto>(query, new { userId });
        }

        public async Task<UserDto> GetByObjectIdAsync(string objectId)
        {
            var query = BasicQueryWithObjectIdArg();
            var user = await RunDapperQuerySingleAsync<UserDto>(query, new { objectId });
            return user;
        }

        public async Task TryCreate(string objectId, string userName, string emailAddress, string fullName, string createdBy)
        {
            try
            {
                var insertStatement = $"IF NOT EXISTS ({ExistQueryWithObjectIdArg()}) BEGIN";
                insertStatement += $" INSERT INTO [dbo].[Users] ([FullName], [UserName], [EmailAddress], [ObjectId], [Created], [CreatedBy], [Updated], [UpdatedBy])";
                insertStatement += $" VALUES('{fullName}','{userName}','{emailAddress}','{objectId}', GETUTCDATE(), '{createdBy}', GETUTCDATE(), '{createdBy}')";
                insertStatement += $" END";

                await ExecuteAsync(insertStatement, new { objectId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create user {userName}");
            }
        }

        string BasicQuery()
        {
            return $"SELECT [Id], [FullName], [UserName], [EmailAddress], [ObjectId] FROM [dbo].[Users]";
        }

        string ExistsQuery()
        {
            return $"SELECT [Id] FROM [dbo].[Users]";
        }

        string BasicQueryWithUserIdArg()
        {
            return BasicQueryWithUserIdArg(BasicQuery());
        }

        string BasicQueryWithObjectIdArg()
        {
            return BasicQueryWithObjectIdArg(BasicQuery());
        }

        string ExistQueryWithObjectIdArg()
        {
            return BasicQueryWithObjectIdArg(ExistsQuery());
        }

        string BasicQueryWithUserIdArg(string basedOn)
        {
            return $"{basedOn} WHERE [Id] = @userId";
        }

        string BasicQueryWithObjectIdArg(string basedOn)
        {
            return $"{basedOn} WHERE [ObjectId] = @objectId";
        }
    }
}