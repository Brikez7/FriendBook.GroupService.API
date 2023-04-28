using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using Moq;

namespace FriendBook.GroupService.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Mock<IGroupRepository> moq = new Mock<IGroupRepository>();
            moq.Setup( x=> x.Get()).Returns()
            var groupDTO = \
                Guid.NewGuid
        }
    }
}