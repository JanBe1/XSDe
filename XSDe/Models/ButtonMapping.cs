using XSDe.Models.Enums;

namespace XSDe.Models
{
    /// <summary>
    /// Button mapping class that represents mapping of button to corresponding action.
    /// </summary>
    public class ButtonMapping
    {
        /// <summary>
        /// Unique identifier for the button mapping.
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Button that triggers the action.
        /// </summary>
        public XButton Button { get; set; }

        /// <summary>
        /// Optional modifier button that needs to be pressed along with the main button for the action to be triggered.
        /// </summary>
        public XButton? Modifier { get; set; }

        /// <summary>
        /// Action type to be performed when the button is pressed.
        /// </summary>
        public ActionTypes ActionType { get; set; }

        /// <summary>
        /// Action parameter, like Launch path for AppLaunch action, command for SystemCommand action, etc.
        /// </summary>
        public string? Parameter { get; set; }

        /// <summary>
        /// Indicates whether the button press is a long press.
        /// </summary>
        public bool IsLongPress { get; set; } = false;

        /// <summary>
        /// Duration in milliseconds that defines a long press. If the button is held down for this duration or longer, it will be considered a long press.
        /// </summary>
        public int LongPressMilliseconds { get; set; } = 600;

        /// <summary>
        /// Display name for the button mapping, used for UI representation.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
    }
}
