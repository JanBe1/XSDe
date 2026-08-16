using System;
using System.Collections.Generic;
using System.Text;
using Vortice.XInput;
using XSDe.Models.Enums;
using XSDe.Services.InputDrivers;

namespace XSDe.Tests.TestCases
{
    public class ComboButtonTests
    {
        [Fact]
        public void MultipleButtons_AreMappedToAllPressedButtons()
        {
            var gamepad = new Gamepad
            {
                Buttons =
                    GamepadButtons.A |
                    GamepadButtons.X |
                    GamepadButtons.DPadUp
            };

            var result = XInputDriver.MapButtons(gamepad);

            Assert.Equal(3, result.Count);
            Assert.Contains(XButton.A, result);
            Assert.Contains(XButton.X, result);
            Assert.Contains(XButton.DPadUp, result);
        }
    }
}
