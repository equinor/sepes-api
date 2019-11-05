using Sepes.RestApi.Model;
using Xunit;

namespace Sepes.RestApi.Tests.Model
{
    public class StudyTests
    {
        [Fact]
        public void Constructor()
        {
            var study = new Study("Teststudy", 42, new int[] { 2, 5, 1 }, new int[] { 2, 4, 5 }, false);


            Assert.Equal("Teststudy", study.studyName);
            Assert.Equal(42, study.studyId);
            Assert.Equal(new int[] { 2, 5, 1 }, study.userIds);
            Assert.Equal(new int[] { 2, 4, 5 }, study.datasetIds);
            Assert.False(study.archived);
        }
        [Fact]
        public void ConstructorInput()
        {
            var study = new StudyInput("Teststudy", 42, new int[] { 2, 5, 1 }, new int[] { 2, 4, 5 });


            Assert.Equal("Teststudy", study.studyName);
            Assert.Equal(42, study.studyId);
            Assert.Equal(new int[] { 2, 5, 1 }, study.userIds);
            Assert.Equal(new int[] { 2, 4, 5 }, study.datasetIds);
        }
    }
}
