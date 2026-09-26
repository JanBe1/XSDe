using System.Diagnostics;
using XSDe.Models;
using XSDe.Models.Enums;
using XSDe.Models.Records;
using XSDe.Services.ButtonMappingServices;
using XSDe.Services.Executors;
using XSDe.Services.InputDrivers.Interfaces;

namespace XSDe.Services.BackgroundServices
{
    /// <summary>
    /// Background service that continuously polls the input driver for controller state and processes the input for the deck.
    /// </summary>
    /// <param name="driver">
    /// The input driver to poll for controller state.
    /// </param>
    /// <param name="mappingService">
    /// The service responsible for mapping controller inputs to deck actions.
    /// </param>
    public class XInputDeckService(
        IInputDriver driver, 
        IMappingService mappingService, 
        IActionExecutor actionExecutor,
        ILogger<XInputDeckService> logger) : BackgroundService
    {
        /// <summary>
        /// Around 60Hz polling rate for XInput, which is approximately every 16 milliseconds.
        /// </summary>
        private const int PollingIntervalMilliseconds = 16;

        /// <summary>
        /// The maximum number of consecutive poll failures before logging a warning. 
        /// This helps to avoid flooding the logs with warnings if the controller is disconnected or not responding.
        /// </summary>
        private const int MaxConsecutivePollFailuresBeforeWarning = 60;

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
        private Dictionary<XButton, long> _buttonPressTimestamps = new();

        /// <summary>
        /// Counts the number of consecutive poll failures to determine when to log a warning about potential controller issues.
        /// </summary>
        private int _consecutivePollFailures;

        /// <summary>
        /// Executes the background service, continuously polling the input driver for controller state and processing the input for the deck.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token to stop the background service.</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Poll the driver for controller state
                    _controllerSnapshot = driver.Poll();
                    //for logging purposes
                    _consecutivePollFailures = 0;
                    // Process the snapshot and send input to the deck
                    ProcessSnapshot(stoppingToken);
                    _previousSnapshot = _controllerSnapshot;
                    // Wait for a short interval before polling again
                }
                catch (Exception ex)
                {
                    if (_consecutivePollFailures == 1 
                        || _consecutivePollFailures % MaxConsecutivePollFailuresBeforeWarning == 0)
                    {
                        logger.LogError(ex,
                            "Error polling controller state. Consecutive failures: {ConsecutivePollFailures}",
                            _consecutivePollFailures
                            );
                    }
                }


                try
                {
                    await Task.Delay(PollingIntervalMilliseconds, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // The task was canceled, exit the loop
                    break;
                }

            }
        }

        /// <summary>
        /// Processes the controller snapshot and sends the input to the deck.
        /// </summary>
        /// <param name="stoppingToken">
        ///  The Cancellation token.
        /// </param>
        private void ProcessSnapshot(CancellationToken stoppingToken)
        {
            if (_controllerSnapshot is null || _previousSnapshot is null) return;

            var newlyPressed = _controllerSnapshot.PressedButtons
                .Except(_previousSnapshot.PressedButtons);

            foreach (var button in newlyPressed)
            {
                _buttonPressTimestamps[button] = Environment.TickCount64;
            }

            var newlyReleased = _previousSnapshot.PressedButtons
                .Except(_controllerSnapshot.PressedButtons);

            foreach (var button in newlyReleased)
            {
                if (_buttonPressTimestamps.TryGetValue(button, out var pressTimeTicks))
                {
                    var duration = Environment.TickCount64 - pressTimeTicks;

                    // Fetch short-press default to check its threshold duration
                    var baseMapping = mappingService.GetMapping(button, isLongPress: false);
                    int threshold = baseMapping?.LongPressMilliseconds ?? 600;

                    bool isLongPress = duration >= threshold;
                    var mapping = mappingService.GetMapping(button, isLongPress: isLongPress);

                    if (mapping is not null)
                    {
                        // Fire and forget non-blocking execution
                        _ = actionExecutor.ExecuteAsync(mapping, stoppingToken);
                    }

                    _buttonPressTimestamps.Remove(button);
                }
            }
        }
    }
}
