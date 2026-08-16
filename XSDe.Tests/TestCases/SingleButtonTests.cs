using Vortice.XInput;
using XSDe.Models.Enums;
using XSDe.Services.InputDrivers;

namespace XSDe.Tests.TestCases
{
    public class SingleButtonTests
    {
        public static TheoryData<GamepadButtons, XButton> ButtonMappings => new()
        {
            { GamepadButtons.A, XButton.A },
            { GamepadButtons.B, XButton.B },
            { GamepadButtons.X, XButton.X },
            { GamepadButtons.Y, XButton.Y },
            { GamepadButtons.DPadUp, XButton.DPadUp },
            { GamepadButtons.DPadDown, XButton.DPadDown },
            { GamepadButtons.DPadLeft, XButton.DPadLeft },
            { GamepadButtons.DPadRight, XButton.DPadRight },
            { GamepadButtons.LeftShoulder, XButton.LeftShoulder },
            { GamepadButtons.RightShoulder, XButton.RightShoulder },
            { GamepadButtons.LeftThumb, XButton.LeftThumb },
            { GamepadButtons.RightThumb, XButton.RightThumb },
            { GamepadButtons.Start, XButton.Start },
            { GamepadButtons.Back, XButton.Back }
        };

        [Theory]
        [MemberData(nameof(ButtonMappings))]
        public void SingleButton_IsMappedToExactlyOneXButton(
            GamepadButtons input,
            XButton expected)
        {
            var gamepad = new Gamepad
            {
                Buttons = input
            };

            var result = XInputDriver.MapButtons(gamepad);

            Assert.Single(result);
            Assert.Contains(expected, result);
        }
    }
}
