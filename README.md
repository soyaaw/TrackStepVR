# TrackStepVR

TrackStepVR uses two SteamVR foot trackers to generate forward movement input for VRChat. It reads tracker movement through OpenVR and sends OSC input to VRChat.

## Download

Get the latest Windows release from the [TrackStepVR Releases page](https://github.com/soyaaw/TrackStepVR/releases/latest).

## Requirements

- Windows 10 or newer
- SteamVR
- Two SteamVR-compatible foot trackers
- VRChat with OSC enabled
- .NET 10 SDK to build from source

## How it works

1. Start SteamVR and connect both foot trackers.
2. Start TrackStepVR and choose **Connect and calibrate**.
3. Stand normally for the ground-height reading.
4. Raise and hold your left foot when prompted.
5. Lower it, then raise and hold your right foot when prompted.
6. Enable movement with the button or press **T** while the TrackStepVR window is focused.

Forward movement follows the headset's horizontal facing direction. The displayed World Z value is only a tracker position reading; it does not determine movement by itself.

## Controls

- **Movement speed** controls the maximum strength of the forward input.
- **Raise leg sensitivity** controls how high a foot must rise before it is marked as AIR.
- **Movement trigger** controls the minimum forward swing speed needed to start movement.
- **Recalibrate ground** captures a new floor height and turns movement off.
- The live input meter shows the current value being sent to VRChat.

Movement is turned off if tracking is lost. It remains off after tracking returns until you enable it again.

## VRChat OSC

TrackStepVR sends a float value through UDP to:

```text
127.0.0.1:9000
/input/Vertical
```

Enable OSC in VRChat before testing. TrackStepVR can send UDP packets but cannot confirm that VRChat received them.

## Build from source

Open `TrackStepVR.slnx` in Visual Studio, or run this from the project folder:

```powershell
dotnet build TrackStepVR.csproj
```

The project targets `net10.0-windows`, Windows Forms, and x64.

## Publish for sharing

Use Visual Studio's **Publish** action and select a folder target. Publish for `win-x64`, then share the complete generated publish folder. The folder contains the executable and required OpenVR/runtime files; do not share only the `.exe`.

## Project files

- `MainForm.cs` and `MainForm.Designer.cs` contain the user interface.
- `TrackerService.cs` contains SteamVR tracking, calibration, movement, and OSC logic.
- `Program.cs` starts the application.
- `openvr_api.cs` and `openvr_api.dll` provide the OpenVR bindings.
- `AGENTS.md` contains project-specific development notes.
