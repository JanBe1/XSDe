using XSDe.Services.InputDrivers;

namespace XSDe.Tests.TestCases
{
    public class DummyInputDriverTests
    {
        [Fact]
        public void Poll_AlwaysReturnsDisconnectedEmptySnapshot()
        {
            var driver = new DummyInputDriver();

            var snapshot = driver.Poll();

            Assert.False(snapshot.IsConnected);
            Assert.Empty(snapshot.PressedButtons);
            Assert.Equal(0, snapshot.LeftThumbX);
            Assert.Equal(0, snapshot.LeftThumbY);
            Assert.Equal(0, snapshot.RightThumbX);
            Assert.Equal(0, snapshot.RightThumbY);
            Assert.Equal(0, snapshot.LeftTrigger);
            Assert.Equal(0, snapshot.RightTrigger);
        }

        [Fact]
        public void Poll_IsConsistentAcrossMultipleCalls()
        {
            var driver = new DummyInputDriver();

            var first = driver.Poll();
            var second = driver.Poll();

            Assert.Equal(first.IsConnected, second.IsConnected);
            Assert.Equal(first.PressedButtons.Count, second.PressedButtons.Count);
        }
    }
}