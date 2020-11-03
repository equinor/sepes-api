//using AutoMapper;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using Sepes.Infrastructure.Constants;
//using Sepes.Infrastructure.Dto;
//using Sepes.Infrastructure.Dto.Azure;
//using Sepes.Infrastructure.Model;
//using Sepes.Infrastructure.Model.Context;
//using Sepes.Infrastructure.Service;
//using Sepes.Infrastructure.Service.Interface;
//using Sepes.Tests.Setup;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace Sepes.Tests.Services
//{
//    public class StudyParticipantServiceTests
//    {
//        public ServiceCollection Services { get; private set; }
//        public ServiceProvider ServiceProvider { get; protected set; }

//        public StudyParticipantServiceTests()
//        {
//            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
//            Services.AddTransient<IStudyService, StudyService>();
//            ServiceProvider = Services.BuildServiceProvider();
//        }

//        void RefreshTestDb()
//        {
//            var db = ServiceProvider.GetService<SepesDbContext>();
//            db.Database.EnsureDeleted();
//            db.Database.EnsureCreated();
//        }

//        void RefreshAndPopulateTestDb()
//        {
//            RefreshTestDb();
//            var db = ServiceProvider.GetService<SepesDbContext>();

//            var currentUser = UserPopulator.Add(db, "currentuserobjectid", "currentuser", "currentuser@somedomain.com", "Current User");

//            StudyPopulator.Add(db, "Test Study 1", "Vendor for TS1", "WBS for TS1", currentUser.Id);

//            StudyPopulator.Add(db, "Test Study 2", "Vendor for TS2", "WBS for TS2", currentUser.Id);

//            StudyPopulator.Add(db, "Test Study 3", "Vendor for TS3", "WBS for TS3", currentUser.Id);

//            db.SaveChanges();

//        }

//        [Theory]
//        [InlineData(1, 2, "Sponsor Rep", ParticipantSource.Azure)]
//        [InlineData(1, 2, "Vendor Admin", ParticipantSource.Azure)]
//        [InlineData(1, 2, "Vendor Contributor", ParticipantSource.Azure)]
//        [InlineData(1, 2, "Study Viewer", ParticipantSource.Azure)]
//        [InlineData(1, 2, "Sponsor Rep", ParticipantSource.Db)]
//        [InlineData(1, 2, "Vendor Admin", ParticipantSource.Db)]
//        [InlineData(1, 2, "Vendor Contributor", ParticipantSource.Db)]
//        [InlineData(1, 2, "Study Viewer", ParticipantSource.Db)]
//        public async void AddingParticipant_ShouldSucceed(int studyId, int expectedParticipantCount, string role, string source)
//        {
//            var userObjectId = "somenewuserobjectId";
//            var userName = "newUserUsername";
//            var userEmail = userName + "@somedomain.com";
//            var userFullName = "Hey I Am The New User";

//            //SETUP
//            var adUserServiceMock = new Mock<IAzureUserService>();
//            adUserServiceMock.Setup(service => service.GetUser(It.IsAny<string>())).ReturnsAsync(new AzureUserDto() { DisplayName = userFullName, Mail = userEmail });

//            RefreshAndPopulateTestDb();

//            //GET REQUIRED SERVICES
//            var mapper = ServiceProvider.GetService<IMapper>();

//            var studyParticipantService = ServiceProvider.GetService<IStudyParticipantService>();


//            ParticipantLookupDto newParticipant = null;

//            if (source == ParticipantSource.Db)
//            {
//                var newUser = await AddUserToDb(userObjectId, userName, userName + "@somedomain.com", "User From DB");
//                newParticipant = mapper.Map<ParticipantLookupDto>(newUser);
//            }
//            else
//            {
//                newParticipant = new ParticipantLookupDto()
//                {
//                    Source = source,
//                    ObjectId = userObjectId,
//                    UserName = userName,
//                    EmailAddress = userName + "@somedomain.com",
//                    FullName = userFullName
//                };
//            }


//            var createdParticipant = await studyParticipantService.HandleAddParticipantAsync(studyId, newParticipant, role);

//            var createdStudyAsDto = await studyService.GetStudyByIdAsync(studyId);

//            Assert.Equal(expectedParticipantCount, createdStudyAsDto.Participants.Count);

//            var participantFromStudyDto = createdStudyAsDto.Participants.FirstOrDefault(p => p.UserName == userName);

//            Assert.NotNull(participantFromStudyDto);
//            Assert.Equal(role, participantFromStudyDto.Role);
//        }

//        [Theory]
//        [InlineData(1, "Sponsor Re1p")]
//        [InlineData(1, "Vendor Ad1min")]
//        [InlineData(1, "Vendor Contr1ibutor")]
//        [InlineData(1, "Study Vie1wer")]
//        public async void TestAddNewUserInvalidRole(int studyId, string role)
//        {
//            RefreshTestDb();
//            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

//            var initialStudy = new StudyCreateDto()
//            {
//                Name = "TestStudy12345",
//                Vendor = "Equinor"
//            };

//            var createdStudy = await studyService.CreateStudyAsync(initialStudy);

//            var initialUser = new AddStudyParticipantDto()
//            {
//                FullName = "John",
//                Role = role,
//                EmailAddress = "john@test.com"
//            };

//            await Assert.ThrowsAsync<ArgumentException>(() => studyService.AddNewParticipantToStudyAsync(studyId, initialUser));
//            //Assert.Equal(expected, createdParticipant.Participants.Count);
//        }


//        async Task<User> AddUserToDb(string objectId, string username, string email, string fullName)
//        {
//            var db = ServiceProvider.GetService<SepesDbContext>();

//            var existingUserWithHighestId = db.Users.OrderByDescending(u => u.Id).FirstOrDefault();
//            int nextId = 2;

//            if (existingUserWithHighestId != null)
//            {
//                nextId = existingUserWithHighestId.Id + 10;
//            }

//            var newUser = new User() { Id = nextId, ObjectId = objectId, UserName = username, EmailAddress = email, FullName = fullName, Created = DateTime.UtcNow, CreatedBy = "unittest" };
//            db.Users.Add(newUser);
//            await db.SaveChangesAsync();

//            return newUser;

//        }

//    }
//}
