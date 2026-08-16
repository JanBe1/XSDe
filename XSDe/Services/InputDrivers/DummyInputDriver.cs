using XSDe.Models.Records;
using XSDe.Services.InputDrivers.Interfaces;

namespace XSDe.Services.InputDrivers
{
    /// <summary>
    /// DummyInputDriver class implements the IInputDriver interface and provides a dummy implementation for testing purposes.
    /// </summary>
    public class DummyInputDriver : IInputDriver
    {
        public ControllerSnapshot Poll() => new(
            IsConnected: false,
            PressedButtons: [],
            LeftThumbX: 0, LeftThumbY: 0, RightThumbX: 0, RightThumbY: 0,
            LeftTrigger: 0, RightTrigger: 0);
    }
}
