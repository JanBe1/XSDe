using Vortice.XInput;
using XSDe.Models.Enums;
using XSDe.Models.Records;

namespace XSDe.Services.InputDrivers.Interfaces
{
    /// <summary>
    /// IInputDriver interface defines the contract for input driver implementations, providing a common structure for handling input from various devices.
    /// </summary>
    public interface IInputDriver
    {
        /// <summary>
        /// Poll method retrieves the current state of the Xbox controller and returns a ControllerSnapshot object containing information about the controller's connection status, pressed buttons, thumbstick positions, and trigger values.
        /// </summary>
        /// <returns>
        ///  <see cref="ControllerSnapshot"/> object containing the current state of the Xbox controller.
        /// </returns>
        public ControllerSnapshot Poll();
    }
}
