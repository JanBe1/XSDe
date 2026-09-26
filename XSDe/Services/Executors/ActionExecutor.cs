using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using XSDe.Models;
using XSDe.Models.Enums;

namespace XSDe.Services.Executors;

public class ActionExecutor : IActionExecutor
{
    private readonly ILogger<ActionExecutor> _logger;

    public ActionExecutor(ILogger<ActionExecutor> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(ButtonMapping mapping, CancellationToken cancellationToken = default)
    {
        if (mapping is null) return;

        try
        {
            _logger.LogInformation("Executing action [{ActionType}] '{DisplayName}' for button {Button}",
                mapping.ActionType, mapping.DisplayName, mapping.Button);

            switch (mapping.ActionType)
            {
                case ActionTypes.AppLaunch:
                    ExecuteAppLaunch(mapping.Parameter);
                    break;

                case ActionTypes.MusicControl:
                    ExecuteMusicControl(mapping.Parameter);
                    break;

                case ActionTypes.SystemCommand:
                    ExecuteSystemCommand(mapping.Parameter);
                    break;

                case ActionTypes.KeyboardMacro:
                    await ExecuteKeyboardMacroAsync(mapping.Parameter, cancellationToken);
                    break;

                case ActionTypes.ProfileSwitch:
                    ExecuteProfileSwitch(mapping.Parameter);
                    break;

                case ActionTypes.None:
                default:
                    _logger.LogDebug("No action configured for button {Button}", mapping.Button);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute mapping '{DisplayName}' ({Id})", mapping.DisplayName, mapping.Id);
        }
    }

    #region Action Handlers

    private void ExecuteAppLaunch(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _logger.LogWarning("AppLaunch skipped: Executable path is null or empty.");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    private void ExecuteMusicControl(string? actionParam)
    {
        if (!Enum.TryParse<MusicActions>(actionParam, true, out var musicAction))
        {
            _logger.LogWarning("Invalid MusicControl parameter: {Parameter}", actionParam);
            return;
        }

        byte vkCode = musicAction switch
        {
            MusicActions.Play => VK_MEDIA_PLAY_PAUSE,
            MusicActions.Pause => VK_MEDIA_PLAY_PAUSE,
            MusicActions.Stop => VK_MEDIA_STOP,
            MusicActions.Next => VK_MEDIA_NEXT_TRACK,
            MusicActions.Previous => VK_MEDIA_PREV_TRACK,
            MusicActions.VolumeUp => VK_VOLUME_UP,
            MusicActions.VolumeDown => VK_VOLUME_DOWN,
            MusicActions.Mute => VK_VOLUME_MUTE,
            _ => 0
        };

        if (vkCode != 0)
        {
            SendVirtualKey(vkCode);
        }
        else
        {
            _logger.LogWarning("Music action '{Action}' is not supported via OS media keys.", musicAction);
        }
    }

    private void ExecuteSystemCommand(string? command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        // Common quick command handling
        switch (command.ToLowerInvariant())
        {
            case "mute":
                SendVirtualKey(VK_VOLUME_MUTE);
                break;
            case "volup":
                SendVirtualKey(VK_VOLUME_UP);
                break;
            case "voldown":
                SendVirtualKey(VK_VOLUME_DOWN);
                break;
            default:
                // Fallback to cmd execution for raw system commands
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                break;
        }
    }

    private async Task ExecuteKeyboardMacroAsync(string? macroParam, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(macroParam)) return;

        _logger.LogInformation("Triggered KeyboardMacro: {Macro}", macroParam);
        await Task.CompletedTask; // Stub for MVP macro expansion engine
    }

    private void ExecuteProfileSwitch(string? profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName)) return;

        _logger.LogInformation("Profile switched to: {Profile}", profileName);
        // Stub: Hook into your IMappingService/ProfileManager when implemented
    }

    #endregion

    #region Win32 Native Interop

    private const byte VK_VOLUME_MUTE = 0xAD;
    private const byte VK_VOLUME_DOWN = 0xAE;
    private const byte VK_VOLUME_UP = 0xAF;
    private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
    private const byte VK_MEDIA_PREV_TRACK = 0xB1;
    private const byte VK_MEDIA_STOP = 0xB2;
    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;

    private const uint KEYEVENTF_KEYUP = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private static void SendVirtualKey(byte vkCode)
    {
        keybd_event(vkCode, 0, 0, UIntPtr.Zero);               // Key down
        keybd_event(vkCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Key up
    }

    #endregion
}