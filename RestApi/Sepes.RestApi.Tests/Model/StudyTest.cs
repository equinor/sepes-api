using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Text.Json;
using Sepes.RestApi.Model;
using Xunit;

namespace Sepes.RestApi.Tests.Model
{
    public class StudyTests
    {
        [Fact]
        public void Constructor()
        {
            Study study = new Study("Teststudy", 42, new List<Pod>(), new List<User>(), new List<User>(), new List<DataSet>(), false);


            Assert.Equal("Teststudy", study.studyName);
            Assert.Equal(42, study.studyId);
            Assert.False(study.archived);
        }
        [Fact]
        public void ConstructorInput()
        {
            var study = new StudyInput()
            {
                studyName = "Teststudy",
                studyId = 42
            };

            Assert.Equal("Teststudy", study.studyName);
            Assert.Equal(42, study.studyId);
        }


        [Fact]
        public void TestEqualsMethod()
        {
            var user1 = new User("Name1", "test@test.com", "sponsor");
            var rule1 = new Rule(8080, IPAddress.Parse("1.1.1.1"));
            var dataset1 = new DataSet("test", "/test", "twqt4yhqe.qe5w.ywyw5ywq.yq4e5yqe5y");

            var rules = new List<Rule>();
            rules.Add(rule1);
            var users = new List<User>();
            users.Add(user1);
            var datasets = new List<DataSet>();
            datasets.Add(dataset1);

            var pod = new Pod(11, "test", 1, false, rules, rules, datasets, datasets);
            var pods = new List<Pod>();
            pods.Add(pod);

            var study1 = new Study("Test-Study", 1, pods, users, users, datasets, false);
            var sameAsStudy1 = new Study("Test-Study", 1, pods, users, users, datasets, false);
            var differentStudy = new Study("Test-Study3", 3, pods);
            Assert.True(study1.Equals(sameAsStudy1));
            Assert.False(study1.Equals(differentStudy));
        }

        [Fact]
        public void TestEqualityForConversions()
        {
            var rule1 = new Rule(8080, IPAddress.Parse("1.1.1.1"));
            var rule2 = new Rule(400, IPAddress.Parse("1.1.1.1"));
            var rules = new List<Rule>();
            rules.Add(rule1);
            rules.Add(rule2);

            var pod = new Pod(11, "test", 1, false, rules, rules, null, null);
            var pods = new List<Pod>();
            pods.Add(pod);

            var study1 = new Study("Test-Study", 1, pods, null, null, new List<DataSet>(), false);
            var study2 = study1.ToStudyInput();
            var study3 = new Study("Test-Study", 2, pods);

            Assert.True(study1.Equals(study2.ToStudy()));
            Assert.False(study1.Equals(study3));
        }

        [Fact]
        public void TestDBModelConversion()
        {
            var pod1 = new Pod(1, "pod1", 1);
            var pod2 = new Pod(2, "pod2", 1);
            var pods = new List<Pod>();
            pods.Add(pod1);
            pods.Add(pod2);
            
            var study = new Study("study", 1, pods);
            var jsonData = JsonSerializer.Serialize<Study>(study);

            StudyDB studyDB = JsonSerializer.Deserialize<StudyDB>(jsonData);

            Assert.True(study.Equals(studyDB.ToStudy()));
        }

        [Fact]
        public void TestReplacePodsMethod()
        {
            var pod1 = new Pod(1, "test1", 1);
            var pod2 = new Pod(2, "test2", 1);
            var pods = new List<Pod>();
            pods.Add(pod1);
            pods.Add(pod2);

            var study1 = new Study("test", 1, pods);

            pods.Add(new Pod(3, "test3", 1));

            var study2 = study1.ReplacePods(pods);

            Assert.NotEqual(study1, study2);
            Assert.NotEqual(study1.pods, study2.pods);
            Assert.Equal(study2.pods, pods.ToImmutableHashSet());
        }
    }
}
