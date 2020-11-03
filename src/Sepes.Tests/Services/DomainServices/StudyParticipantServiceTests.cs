using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services
{
    public class StudyParticipantServiceTests
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public StudyParticipantServiceTests()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<IStudyService, StudyService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

    

        [Theory]
        [InlineData(1, 2, "Sponsor Rep", ParticipantSource.Azure)]
        [InlineData(1, 2, "Vendor Admin", ParticipantSource.Azure)]
        [InlineData(1, 2, "Vendor Contributor", ParticipantSource.Azure)]
        [InlineData(1, 2, "Study Viewer", ParticipantSource.Azure)]
        [InlineData(1, 2, "Sponsor Rep", ParticipantSource.Db)]
        [InlineData(1, 2, "Vendor Admin", ParticipantSource.Db)]
        [InlineData(1, 2, "Vendor Contributor", ParticipantSource.Db)]
        [InlineData(1, 2, "Study Viewer", ParticipantSource.Db)]
        public async void AddingParticipant_ShouldSucceed(int studyId, int expectedParticipantCount, string role, string source)
        {
            //SETUP
            var userObjectId = "somenewuserobjectId";
            var userName = "newUserUsername";
            var userEmail = userName + "@somedomain.com";
            var userFullName = "Hey I Am The New User";

            RefreshAndPopulateTestDb();
           
            var adUserServiceMock = new Mock<IAzureUserService>();
            adUserServiceMock.Setup(service => service.GetUser(It.IsAny<string>())).ReturnsAsync(new AzureUserDto() { DisplayName = userFullName, Mail = userEmail });

            //GET REQUIRED SERVICES
            var db = ServiceProvider.GetService<SepesDbContext>();
            var mapper = ServiceProvider.GetService<IMapper>();
            var userService = ServiceProvider.GetService<IUserService>();

            var studyParticipantService = new StudyParticipantService(db, mapper, userService, adUserServiceMock.Object);

            ParticipantLookupDto newParticipant = null;

            if (source == ParticipantSource.Db)
            {
                var newUser = await AddUserToDb(userObjectId, userName, userName + "@somedomain.com", "User From DB");
                newParticipant = mapper.Map<ParticipantLookupDto>(newUser);
            }
            else
            {
                newParticipant = new ParticipantLookupDto()
                {
                    Source = source,
                    ObjectId = userObjectId,
                    UserName = userName,
                    EmailAddress = userName + "@somedomain.com",
                    FullName = userFullName
                };
            }


            var createdParticipant = await studyParticipantService.HandleAddParticipantAsync(studyId, newParticipant, role);
            Assert.NotNull(createdParticipant);
            Assert.Equal(role, createdParticipant.Role);
            Assert.Equal(expectedParticipantCount, createdParticipant.Study.Participants.Count);            
       
        }

        [Theory]
        [InlineData(1, "Sponsor Re1p", ParticipantSource.Azure)]
        [InlineData(1, "Vendor Ad1min", ParticipantSource.Azure)]
        [InlineData(1, "Vendor Contr1ibutor", ParticipantSource.Azure)]
        [InlineData(1, "Study Vie1wer", ParticipantSource.Azure)]
        [InlineData(1, "Sponsor Re1p", ParticipantSource.Db)]
        [InlineData(1, "Vendor Ad1min", ParticipantSource.Db)]
        [InlineData(1, "Vendor Contr1ibutor", ParticipantSource.Db)]
        [InlineData(1, "Study Vie1wer", ParticipantSource.Db)]
        public async void TestAddNewUserInvalidRole(int studyId, string role, string source)
        {
            //SETUP
            var userObjectId = "somenewuserobjectId";
            var userName = "newUserUsername";
            var userEmail = userName + "@somedomain.com";
            var userFullName = "Hey I Am The New User";

            RefreshAndPopulateTestDb();

            var adUserServiceMock = new Mock<IAzureUserService>();
            adUserServiceMock.Setup(service => service.GetUser(It.IsAny<string>())).ReturnsAsync(new AzureUserDto() { DisplayName = userFullName, Mail = userEmail });

            //GET REQUIRED SERVICES
            var db = ServiceProvider.GetService<SepesDbContext>();
            var mapper = ServiceProvider.GetService<IMapper>();
            var userService = ServiceProvider.GetService<IUserService>();

            var studyParticipantService = new StudyParticipantService(db, mapper, userService, adUserServiceMock.Object);

            ParticipantLookupDto newParticipant = null;

            if (source == ParticipantSource.Db)
            {
                var newUser = await AddUserToDb(userObjectId, userName, userName + "@somedomain.com", "User From DB");
                newParticipant = mapper.Map<ParticipantLookupDto>(newUser);
            }
            else
            {
                newParticipant = new ParticipantLookupDto()
                {
                    Source = source,
                    ObjectId = userObjectId,
                    UserName = userName,
                    EmailAddress = userName + "@somedomain.com",
                    FullName = userFullName
                };
            }

            await Assert.ThrowsAsync<ArgumentException>(() => studyParticipantService.HandleAddParticipantAsync(studyId, newParticipant, role));          
        }

        void RefreshTestDb()
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        void RefreshAndPopulateTestDb()
        {
            RefreshTestDb();
            var db = ServiceProvider.GetService<SepesDbContext>();

            var currentUser = UserPopulator.Add(db, "currentuserobjectid", "currentuser", "currentuser@somedomain.com", "Current User");

            StudyPopulator.Add(db, "Test Study 1", "Vendor for TS1", "WBS for TS1", currentUser.Id);

            StudyPopulator.Add(db, "Test Study 2", "Vendor for TS2", "WBS for TS2", currentUser.Id);

            StudyPopulator.Add(db, "Test Study 3", "Vendor for TS3", "WBS for TS3", currentUser.Id);

            db.SaveChanges();

        }


        async Task<User> AddUserToDb(string objectId, string username, string email, string fullName)
        {
            var db = ServiceProvider.GetService<SepesDbContext>();

            var existingUserWithHighestId = db.Users.OrderByDescending(u => u.Id).FirstOrDefault();
            int nextId = 2;

            if (existingUserWithHighestId != null)
            {
                nextId = existingUserWithHighestId.Id + 10;
            }

            var newUser = new User() { Id = nextId, ObjectId = objectId, UserName = username, EmailAddress = email, FullName = fullName, Created = DateTime.UtcNow, CreatedBy = "unittest" };
            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            return newUser;

        }

    }
}
