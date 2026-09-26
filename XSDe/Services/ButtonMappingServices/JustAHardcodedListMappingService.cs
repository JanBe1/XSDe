using XSDe.Models;
using XSDe.Models.Enums;

namespace XSDe.Services.ButtonMappingServices
{
    /// <summary>
    /// A simple implementation of IMappingService that returns a hardcoded list of button mappings.
    /// </summary>
    public class JustAHardcodedListMappingService : IMappingService
    {
        private readonly List<ButtonMapping> _mappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JustAHardcodedListMappingService"/> class with a hardcoded list of button mappings.
        /// </summary>
        public JustAHardcodedListMappingService()
        {
            _mappings = InitializeHardcodedMappings();
        }

        /// <summary>
        /// Gets the list of all button mappings.
        /// </summary>
        /// <returns>
        /// A list of all button mappings.
        /// </returns>
        public List<ButtonMapping> GetMappings()
        {
            return _mappings;
        }

        /// <summary>
        /// Gets a specific button mapping based on the provided button, optional modifier, and long press status.
        /// </summary>
        /// <param name="button">
        /// Button to look for in the mappings.
        /// </param>
        /// <param name="modifier">
        /// Optional modifier button.
        /// </param>
        /// <param name="isLongPress">
        /// Indicates whether to look for a long press mapping.
        /// </param>
        /// <returns>
        /// The matching button mapping, or null if none found.
        /// </returns>
        public ButtonMapping? GetMapping(
            XButton button, 
            XButton? modifier = null, 
            bool isLongPress = false)
        {
            return _mappings.FirstOrDefault(m =>
                m.Button == button &&
                m.Modifier == modifier &&
                m.IsLongPress == isLongPress);
        }

        /// <summary>
        /// Initializes a hardcoded list of button mappings for demonstration purposes.
        /// </summary>
        /// <returns></returns>
        private static List<ButtonMapping> InitializeHardcodedMappings()
        {
            return new List<ButtonMapping>
            {
                new ButtonMapping
                {
                    Button = XButton.A,
                    Modifier = null,
                    ActionType = ActionTypes.AppLaunch,
                    Parameter = @"C:\Windows\notepad.exe",
                    IsLongPress = false,
                    DisplayName = "Run Notepad"
                },

                new ButtonMapping
                {
                    Button = XButton.A,
                    Modifier = null,
                    ActionType = ActionTypes.SystemCommand,
                    Parameter = "Mute",
                    IsLongPress = true,
                    LongPressMilliseconds = 600,
                    DisplayName = "Mute System"
                },

                new ButtonMapping
                {
                    Button = XButton.X,
                    Modifier = XButton.LeftShoulder,
                    ActionType = ActionTypes.MusicControl,
                    Parameter = MusicActions.Next.ToString(),
                    IsLongPress = false,
                    DisplayName = "Next Track"
                }
            };
        }
    }
}