using LetsChat.Services.DataAccess;
using LetsChat.Services.Hubs;
using LetsChat.Services.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace Test.LetsChat.Services
{
    [TestClass]
    public class ChatHubTests
    {
        [TestMethod]
        public void JoinRoom_ShouldNot_ErrorOnBlankNullRoomName()
        {
            var repo = new Mock<IMessageRepository>();
            var mockGroups = new Mock<IGroupManager>();
            var target = new ChatHub(repo.Object) { Groups = mockGroups.Object };

            try
            {
                target.JoinRoom(null);
                target.JoinRoom("");
            }
            catch
            {
                Assert.Fail("Null/empty string should be ignored");
            }

        }

        [TestMethod]
        public void JoinRoom_Should_CallAddToGroup()
        {
            const string roomname = "let_me_in";
            const string connectionId = "LeaveRoomTestConnection";

            var repo = new Mock<IMessageRepository>();
            var mockGroups = new Mock<IGroupManager>();
            var mockContext = new Mock<HubCallerContext>();
            mockContext.SetupGet(c => c.ConnectionId).Returns(connectionId);

            var target = new ChatHub(repo.Object) { Groups = mockGroups.Object, Context = mockContext.Object };

            //Calling this should invoke the AddToGroupAsync method only once, and for the room name we send in
            target.JoinRoom(roomname);
            mockGroups.Verify(g => g.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            mockGroups.Verify(g => g.AddToGroupAsync(It.Is<string>(i => i == connectionId), It.Is<string>(r => r == roomname), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void LeaveRoom_ShouldNot_ErrorOnBlankNullRoomName()
        {
            var repo = new Mock<IMessageRepository>();
            var mockGroups = new Mock<IGroupManager>();
            var target = new ChatHub(repo.Object) { Groups = mockGroups.Object };

            try
            {
                target.LeaveRoom(null);
                target.LeaveRoom("");
            }
            catch
            {
                Assert.Fail("Null/empty string should be ignored");
            }

        }

        [TestMethod]
        public void LeaveRoom_Should_CallRemoveFromGroup()
        {
            const string roomname = "room_to_leave";
            const string connectionId = "LeaveRoomTestConnection";

            var repo = new Mock<IMessageRepository>();
            var mockGroups = new Mock<IGroupManager>();
            var mockContext = new Mock<HubCallerContext>();
            mockContext.SetupGet(c => c.ConnectionId).Returns(connectionId);

            var target = new ChatHub(repo.Object) { Groups = mockGroups.Object, Context = mockContext.Object };

            //Calling this should invoke the AddToGroupAsync method only once, and for the room name we send in
            target.LeaveRoom(roomname);
            mockGroups.Verify(g => g.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            mockGroups.Verify(g => g.RemoveFromGroupAsync(It.Is<string>(i => i == connectionId), It.Is<string>(r => r == roomname), It.IsAny<CancellationToken>()), Times.Once);
        }


        [TestMethod]
        public void PostMessage_Should_InvokeSaveMessage()
        {
            var testMessage = new ChatMessage { ChatRoom = "service test", MessageContent = "Test Message, ya'll", UserName = "Joe Tester" };
            var repo = new Mock<IMessageRepository>();

            var target = new ChatHub(repo.Object);
            target.PostMessage(testMessage).Wait();
            repo.Verify(g => g.SaveMessage(It.IsAny<ChatMessage>()), Times.Once);
            repo.Verify(g => g.SaveMessage(It.Is<ChatMessage>(m => m == testMessage)), Times.Once);
        }
    }
}
