# TrackStepVR project notes

## Purpose

TrackStepVR is a Windows .NET 10 WinForms app that reads foot trackers through SteamVR/OpenVR and sends a forward movement value to VRChat over OSC.

## Project layout

- `Program.cs` starts the WinForms application.
- `MainForm.cs` owns the UI behavior, settings, calibration prompts, and shutdown flow.
- `MainForm.Designer.cs` contains the WinForms layout and control initialization. Keep it consistent with `MainForm.cs` when changing UI text or controls.
- `TrackerService.cs` owns the background SteamVR/OpenVR tracking loop, foot calibration and state, movement calculation, and OSC output.
- `openvr_api.cs` and `openvr_api.dll` are generated/vendor OpenVR bindings. Avoid hand-editing them; warnings there are generally upstream binding warnings.
- `TrackStepVR.csproj` is the project file; `TrackStepVR.slnx` is the solution.

## Behavior to preserve

- The user calibrates with both feet grounded, then raises the left and right foot separately so the app can identify tracker slots.
- Ground height is captured per foot. Recalibration turns movement off.
- Forward swing is measured against the headset's horizontal facing direction, not the room's fixed Z axis. The UI's World Z value is diagnostic only.
- Movement is opt-in and can be toggled with the UI button or T while the main window is focused.
- Movement speed, lift sensitivity, and movement trigger settings are configurable and saved under the app's local user data directory.
- OSC output is a float to `127.0.0.1:9000` at `/input/Vertical`. Send zero when movement is disabled, tracking is lost, or the service shuts down. UDP sends do not confirm VRChat received the packet.
- If headset or foot tracking becomes invalid, disable movement and send zero until tracking recovers; movement should remain off after recovery.
- Keep tracking and OSC work off the UI thread. Marshal UI updates through WinForms' UI thread and await service shutdown before disposing it.

## Change guidance

- Keep movement thresholds, smoothing, calibration timing, and OSC behavior understandable and centralized in `TrackerService.cs`.
- Avoid silently changing defaults or calibration prompts; explain user-visible behavior changes in the UI/help text.
- When changing a setting, update its UI readout, persistence, and service setter together.
- When changing controls, update the designer and event wiring consistently.
- Do not modify generated build output under `bin/` or `obj/` as source changes.
- Prefer focused changes and preserve the current x64 Windows Forms target unless the user asks otherwise.

## Build

Build from the project directory with:

```powershell
dotnet build TrackStepVR.csproj
```

The project targets `net10.0-windows`, WinForms, and x64. Publishing creates the executable and required runtime files for sharing; distribute the complete publish folder, not only the `.exe`.
