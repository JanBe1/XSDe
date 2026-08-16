using XSDe.Services.InputDrivers.Interfaces;
using Vortice.XInput;
using XSDe.Models.Enums;
using XSDe.Models.Records;
using System.Runtime.CompilerServices;

namespace XSDe.Services.InputDrivers
{
    /// <summary>
    /// XInputDriver class implements the IInputDriver interface and provides functionality for handling input from Xbox controllers using the XInput API.
    /// </summary>
    public sealed class XInputDriver(uint userIndex = 0) : IInputDriver
    {
        /// <summary>
        /// _lastKnownConnected field stores the last known connection status of the Xbox controller.
        /// </summary>
        private bool _lastKnownConnected;

        /// <summary>
        /// TriggerThreshold constant defines the threshold value for trigger input detection.
        /// </summary>
        private const byte TriggerThreshold = Gamepad.TriggerThreshold;

        /// <inheritdoc/>
        public ControllerSnapshot Poll()
        {
            // no state: not connected
            if (!XInput.GetState(userIndex, out var state))
            {
                _lastKnownConnected = false;
                return new ControllerSnapshot(false, [], 0, 0, 0, 0, 0, 0);
            }
            Gamepad gp = state.Gamepad;

            _lastKnownConnected = true;
            var pressed = MapButtons(state.Gamepad);

            return new ControllerSnapshot(
                IsConnected: _lastKnownConnected,
                PressedButtons: pressed,
                LeftThumbX: gp.LeftThumbX,
                LeftThumbY: gp.LeftThumbY,
                RightThumbX: gp.RightThumbX,
                RightThumbY: gp.RightThumbY,
                LeftTrigger: gp.LeftTrigger,
                RightTrigger: gp.RightTrigger
            );
        }

        /// <summary>
        /// MapButtons method maps the pressed buttons from the Gamepad state to a HashSet of XButton enums.
        /// </summary>
        /// <param name="gp">The Gamepad state.</param>
        /// <returns>A HashSet of pressed XButton enums.</returns>
        public static HashSet<XButton> MapButtons(Gamepad gp)
        {
            
            var pressed = new HashSet<XButton>();

            void MapIf(GamepadButtons flag, XButton mapped)
            {
                if (gp.Buttons.HasFlag(flag))
                {
                    pressed.Add(mapped);
                }
            }

            MapIf(GamepadButtons.A, XButton.A);
            MapIf(GamepadButtons.B, XButton.B);
            MapIf(GamepadButtons.X, XButton.X);
            MapIf(GamepadButtons.Y, XButton.Y);
            MapIf(GamepadButtons.DPadUp, XButton.DPadUp);
            MapIf(GamepadButtons.DPadDown, XButton.DPadDown);
            MapIf(GamepadButtons.DPadLeft, XButton.DPadLeft);
            MapIf(GamepadButtons.DPadRight, XButton.DPadRight);
            MapIf(GamepadButtons.LeftShoulder, XButton.LeftShoulder);
            MapIf(GamepadButtons.RightShoulder, XButton.RightShoulder);
            MapIf(GamepadButtons.LeftThumb, XButton.LeftThumb);
            MapIf(GamepadButtons.RightThumb, XButton.RightThumb);
            MapIf(GamepadButtons.Start, XButton.Start);
            MapIf(GamepadButtons.Back, XButton.Back);

            // Check triggers separately since they are analog inputs
            if (gp.LeftTrigger > TriggerThreshold) pressed.Add(XButton.LeftTrigger);
            if (gp.RightTrigger > TriggerThreshold) pressed.Add(XButton.RightTrigger);
            return pressed;
        }
    }
}