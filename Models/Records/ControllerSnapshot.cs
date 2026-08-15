using XSDe.Models.Enums;

namespace XSDe.Models.Records
{
    /// <summary>
    /// ControllerSnapshot record represents a snapshot of the current state of an Xbox controller, including its connection status, pressed buttons, thumbstick positions, and trigger values.
    /// </summary>
    /// <param name="IsConnected">Indicates whether the controller is connected.</param>
    /// <param name="PressedButtons">The set of buttons that are currently pressed.</param>
    /// <param name="LeftThumbX">The x-coordinate of the left thumbstick.</param>
    /// <param name="LeftThumbY">The y-coordinate of the left thumbstick.</param>
    /// <param name="RightThumbX">The x-coordinate of the right thumbstick.</param>
    /// <param name="RightThumbY">The y-coordinate of the right thumbstick.</param>
    /// <param name="LeftTrigger">The value of the left trigger.</param>
    /// <param name="RightTrigger">The value of the right trigger.</param>
    public sealed record ControllerSnapshot (
        bool IsConnected,
        HashSet<XButton> PressedButtons,
        short LeftThumbX,
        short LeftThumbY,
        short RightThumbX,
        short RightThumbY,
        byte LeftTrigger,
        byte RightTrigger
    );
}
