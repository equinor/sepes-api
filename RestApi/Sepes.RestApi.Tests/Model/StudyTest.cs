using System;
using System.Collections.Generic;
using System.Net;
using Sepes.RestApi.Model;
using Xunit;

namespace Sepes.RestApi.Tests.Model
{
    public class StudyTests
    {
        [Fact]
        public void Constructor()
        {
            Study study = new Study("Teststudy", 42, new List<Pod>(), new List<User>(), new List<User>(), new List<DataSet>(), false, new int[] { 2, 5, 1 }, new int[] { 2, 4, 5 });


            Assert.Equal("Teststudy", study.studyName);
            Assert.Equal(42, study.studyId);
            Assert.Equal(new int[] { 2, 5, 1 }, study.userIds);
            Assert.Equal(new int[] { 2, 4, 5 }, study.datasetIds);
            Assert.False(study.archived);
        }
        [Fact]
        public void ConstructorInput()
        {
            var study = new StudyInput()
            {
                studyName = "Teststudy",
                studyId = 42,
                userIds = new int[] { 2, 5, 1 },
                datasetIds = new int[] { 2, 4, 5 }
            };//"Teststudy", 42, new int[] { 2, 5, 1 }, new int[] { 2, 4, 5 }


            Assert.Equal("Teststudy", study.studyName);
            Assert.Equal(42, study.studyId);
            Assert.Equal(new int[] { 2, 5, 1 }, study.userIds);
            Assert.Equal(new int[] { 2, 4, 5 }, study.datasetIds);
        }


        [Fact]
        public void TestEqualsMethods()
        {
            var user1 = new User("Name1", "test@test.com", "sponsor");
            var user2 = new User("Name1", "test@test.com", "sponsor");
            Assert.True(user1.Equals(user2));

            var rule1 = new Rule(8080, IPAddress.Parse("1.1.1.1"));
            var rule2 = new Rule(8080, IPAddress.Parse("1.1.1.1"));
            var rule3 = new Rule(10, IPAddress.Parse("1.1.1.1"));
            var rule4 = new Rule(8080, IPAddress.Parse("1.1.1.4"));
            Assert.True(rule1.Equals(rule2));
            Assert.False(rule1.Equals(rule3));
            Assert.False(rule1.Equals(rule4));

            var dataset1 = new DataSet("test", "/test", "twqt4yhqe.qe5w.ywyw5ywq.yq4e5yqe5y");
            var dataset2 = new DataSet("test", "/test", "twqt4yhqe.qe5w.ywyw5ywq.yq4e5yqe5y");
            Assert.True(dataset1.Equals(dataset2));

            var rules = new List<Rule>();
            rules.Add(rule1);
            var users = new List<User>();
            users.Add(user1);
            var datasets = new List<DataSet>();
            datasets.Add(dataset1);
            var pod = new Pod(11, "test", 1, false, rules, rules, users, datasets, datasets);
            var pods = new List<Pod>();
            pods.Add(pod);
            var study1 = new Study("Test-Study", 1, pods, users, users, datasets, false, new int[]{1, 2, 3}, new int[]{1, 2, 3});
            var study2 = new Study("Test-Study", 1, pods, users, users, datasets, false, new int[]{1, 2, 3}, new int[]{1, 2, 3});
            var study3 = new Study("Test-Study", 2, pods, users, users, datasets, false, new int[]{1, 2}, new int[]{1, 2});
            Assert.True(study1.Equals(study2));
            Assert.False(study1.Equals(study3));

            var studySmall = new Study("My study", 123);
            var studySmall2 = new Study("My study", 123, new HashSet<Pod>());
            var podsSmall = new HashSet<Pod>();
            podsSmall.Add(new Pod(1, "test", 1));
            var studySmall3 = new Study("My study", 123, podsSmall);

            Assert.True(studySmall.Equals(studySmall2));
            Assert.Equal(1, studySmall3.pods.Count);
            Assert.False(studySmall.Equals(studySmall3));
        }
    }
}
