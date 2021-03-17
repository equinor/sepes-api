using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class SoftDeleteUtilTest
    {
        [Fact]
        public void MarkAsDeleted_ShouldMarkAsDeletedWithCorrectName()
        {
            var deletedBy = "John";
            var itemToBeDeleted = new CloudResource { };
            SoftDeleteUtil.MarkAsDeleted(itemToBeDeleted, deletedBy);

            Assert.Equal(itemToBeDeleted.DeletedBy, deletedBy);
            Assert.True(SoftDeleteUtil.IsMarkedAsDeleted(itemToBeDeleted));
        }

        [Fact]
        public void MarkAsDeleted_ShouldMarkAsDeletedWithCorrectName2()
        {
            var deletedBy = new User { UserName = "John" };
            var itemToBeDeleted = new CloudResource { };
            SoftDeleteUtil.MarkAsDeleted(itemToBeDeleted, deletedBy);

            Assert.Equal(itemToBeDeleted.DeletedBy, deletedBy.UserName);
            Assert.True(SoftDeleteUtil.IsMarkedAsDeleted(itemToBeDeleted));
        }
        [Fact]
        public void MarkAsDeleted_ShouldMarkAsDeletedWithCorrectName3()
        {
            var deletedBy = new UserDto { UserName = "John" };
            var itemToBeDeleted = new CloudResource { };
            SoftDeleteUtil.MarkAsDeleted(itemToBeDeleted, deletedBy);

            Assert.Equal(itemToBeDeleted.DeletedBy, deletedBy.UserName);
            Assert.True(SoftDeleteUtil.IsMarkedAsDeleted(itemToBeDeleted));
        }
    }
}
