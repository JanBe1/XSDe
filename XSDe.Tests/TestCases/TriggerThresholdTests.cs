using System;
using System.Collections.Generic;
using System.Text;
using Vortice.XInput;
using XSDe.Models.Enums;
using XSDe.Services.InputDrivers;

namespace XSDe.Tests.TestCases
{
    public class TriggerThresholdTests
    {
        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void LeftTrigger_UsesStrictThreshold(
            int offset,
            bool expectedPressed)
        {
            byte value = (byte)(Gamepad.TriggerThreshold + offset);
            var gamepad = new Gamepad
            {
                LeftTrigger = value
            };

            var result = XInputDriver.MapButtons(gamepad);

            Assert.Equal(
                expectedPressed,
                result.Contains(XButton.LeftTrigger));
        }
    }
}
