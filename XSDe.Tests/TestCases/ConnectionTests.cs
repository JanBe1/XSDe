using System;
using System.Collections.Generic;
using System.Text;
using XSDe.Services.InputDrivers;

namespace XSDe.Tests.TestCases
{
    public class ConnectionTests
    {
        [Fact]
        public void DisconnectedController_ShouldNotBeAbleToSendInput()
        {
            // Arrange - use an invalid user index to simulate a disconnected controller
            var controller = new XInputDriver(userIndex: 99);
            // Act
            var snapshot = controller.Poll();
            // Assert
            Assert.False(snapshot.IsConnected);
            Assert.Empty(snapshot.PressedButtons);
            Assert.Equal(0, snapshot.LeftThumbX);
            Assert.Equal(0, snapshot.LeftThumbY);
            Assert.Equal(0, snapshot.RightThumbX);
            Assert.Equal(0, snapshot.RightThumbY);
            Assert.Equal(0, snapshot.LeftTrigger);
            Assert.Equal(0, snapshot.RightTrigger);
        }
    }
}
