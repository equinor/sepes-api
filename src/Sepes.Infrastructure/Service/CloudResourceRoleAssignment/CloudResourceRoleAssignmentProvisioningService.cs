//using AutoMapper;
//using Microsoft.Extensions.Logging;
//using Sepes.Infrastructure.Dto;
//using Sepes.Infrastructure.Model.Context;
//using Sepes.Infrastructure.Service.Interface;
//using Sepes.Infrastructure.Util;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;


//namespace Sepes.Infrastructure.Service
//{
//    public class CloudResourceRoleAssignmentProvisioningService : CloudResourceRoleAssignmentServiceBase, ICloudResourceRoleAssignmentProvisioningService
//    {
//        public CloudResourceRoleAssignmentProvisioningService(
//            SepesDbContext db,
//            IMapper mapper,
//            IUserService userService,
//            ILogger<CloudResourceRoleAssignmentUpdateService> logger)
//              : base(db, mapper, userService, logger)
//        {

//        }

//        public async Task<CloudResourceOperationDto> DefineAndOrderForSandbox(int sandboxId)
//        {
//           //var studyParticipants

//            //Generate desired roles list

//            //Create update operation

//            //Return update operation

//            //(user will add to queue)
//        }

//        public async Task<CloudResourceOperationDto> DefineAndOrderForStudy(int sandboxId)
//        {
//            //foreach sandbox in study
//            //call method above
//            //add to queue           
//        }


//        public async Task CalculateAndEnforce(int resourceId, HashSet<string> rolesUserShouldHave)
//        {
//            var currentUser = await _userService.GetCurrentUserAsync();

//            //Delete those not relevant
//            var existing = await GetByResourceAndPrincipalAsync(resourceId, principalId);

//            foreach(var curExisting in existing)
//            {
//                if(rolesUserShouldHave.Contains(curExisting.RoleId) == false)
//                {
//                    SoftDeleteUtil.MarkAsDeleted(curExisting, currentUser);
//                    await _db.SaveChangesAsync();
//                }
//            }

//            foreach(var curShouldExist in rolesUserShouldHave)
//            {
//                await AddInternalAsync(resourceId, principalId, curShouldExist);
//            }           
//        }

     
//    }
//}
