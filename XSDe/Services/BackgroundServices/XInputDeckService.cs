using System.Diagnostics;
using XSDe.Models.Enums;
using XSDe.Models.Records;
using XSDe.Services.InputDrivers.Interfaces;

namespace XSDe.Services.BackgroundServices
{
    /// <summary>
    /// Background service that continuously polls the input driver for controller state and processes the input for the deck.
    /// </summary>
    /// <param name="driver">
    /// The input driver to poll for controller state.
    /// </param>
    public class XInputDeckService(IInputDriver driver) : BackgroundService
    {
        /// <summary>
        /// Around 60Hz polling rate for XInput, which is approximately every 16 milliseconds.
        /// </summary>
        private const int PollingIntervalMilliseconds = 16;

        /// <summary>
        /// Stores the previous controller snapshot to detect changes in input state.
        /// </summary>
        private ControllerSnapshot? _previousSnapshot;

        /// <summary>
        /// Stores the current controller snapshot to process input state.
        /// </summary>
        private ControllerSnapshot? _controllerSnapshot;

        /// <summary>
        /// Stores the timestamps of button presses to detect long presses and other input patterns.
        /// </summary>
        private List<TimeSpan> _buttonPressTimestamps = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Poll the driver for controller state
                _controllerSnapshot = driver.Poll();
                // Process the snapshot and send input to the deck
                ProcessSnapshot();
                // Wait for a short interval before polling again
                try
                {
                    await Task.Delay(PollingIntervalMilliseconds, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // The task was canceled, exit the loop
                    break;
                }

                _previousSnapshot = _controllerSnapshot;
            }
        }

        /// <summary>
        /// Processes the controller snapshot and sends the input to the deck.
        /// </summary>
        /// <param name="snapshot"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ProcessSnapshot()
        {
            if (this.PressedButtonsChanged(_controllerSnapshot, _previousSnapshot))
            {
                _buttonPressTimestamps.Add(DateTime.Now.TimeOfDay);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(500))
                {
                    if (this.PressedButtonsChanged(_controllerSnapshot, _previousSnapshot))
                    {
                        _buttonPressTimestamps.Add(DateTime.Now.TimeOfDay);
                        break;
                    }
                    else
                    {
                        //long press detected, handle accordingly
                    }
                }
            }

        }

        private bool IsButtonPressed(XButton button, ControllerSnapshot snapshot)
        {
            return snapshot.PressedButtons.Contains(button);
        }

        private bool IsButtonReleased(XButton button, ControllerSnapshot snapshot)
        {
            return !snapshot.PressedButtons.Contains(button);
        }

        private bool ButtonStateChanged(XButton button, ControllerSnapshot currentSnapshot, ControllerSnapshot? previousSnapshot)
        {
            if (previousSnapshot is null)
            {
                return false;
            }
            bool wasPressed = previousSnapshot.PressedButtons.Contains(button);
            bool isPressed = currentSnapshot.PressedButtons.Contains(button);
            return wasPressed != isPressed;
        }

        private bool PressedButtonsChanged(ControllerSnapshot currentSnapshot, ControllerSnapshot? previousSnapshot)
        {
            if (previousSnapshot is null)
            {
                return false;
            }
            return !currentSnapshot.PressedButtons.SetEquals(previousSnapshot.PressedButtons);
        }
    }
}
