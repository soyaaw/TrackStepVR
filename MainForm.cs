using System.Text.Json;

namespace TrackStepVR
{
    public partial class MainForm : Form
    {
        private TrackerService? trackerService;
        private bool loadingSettings;
        private bool applyingPreset;
        private bool shutdownInProgress;
        private bool allowClose;
        private long lastUiUpdateTicks;
        private volatile bool closing;
        private Form? calibrationPrompt;

        public MainForm()
        {
            InitializeComponent();
            LoadUserSettings();
            movementButton.Click += movementButton_Click;
            recalibrateButton.Click += recalibrateButton_Click;
            FormClosing += MainForm_FormClosing;
        }

        private void connectButton_Click(object? sender, EventArgs e)
        {
            if (trackerService?.IsRunning == true) return;

            trackerService?.Dispose();
            trackerService = new TrackerService();
            trackerService.SetMovementSpeed(movementSpeedTrackBar.Value);
            trackerService.SetRaiseSensitivity(raiseSensitivityTrackBar.Value);
            trackerService.SetMovementTrigger(movementTriggerTrackBar.Value);
            trackerService.ConfirmCalibrationStep = ShowCalibrationPrompt;
            trackerService.Updated += TrackerService_Updated;
            connectButton.Enabled = false;
            connectionLabel.Text = "Starting SteamVR…";
            connectionDot.ForeColor = Color.FromArgb(218, 153, 48);
            leftStatus.Text = rightStatus.Text = "Waiting for tracker data";
            leftTrackerLabel.Text = rightTrackerLabel.Text = "Tracker slot: --";
            trackerService.Start();
        }

        private bool ShowCalibrationPrompt(string message)
        {
            if (closing || IsDisposed || !IsHandleCreated) return false;
            using var completed = new ManualResetEventSlim(false);
            bool accepted = false;
            void ShowPrompt()
            {
                if (closing) { completed.Set(); return; }
                var prompt = new Form
                {
                    Text = "TrackStepVR calibration",
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    ClientSize = new Size(420, 150),
                    MinimizeBox = false,
                    MaximizeBox = false,
                    ShowInTaskbar = false
                };
                var label = new Label { Text = message, AutoSize = false, Dock = DockStyle.Fill, Padding = new Padding(16) };
                var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
                var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
                var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
                buttons.Controls.Add(cancel); buttons.Controls.Add(ok);
                prompt.Controls.Add(label); prompt.Controls.Add(buttons);
                prompt.AcceptButton = ok; prompt.CancelButton = cancel;
                calibrationPrompt = prompt;
                prompt.FormClosed += (_, _) =>
                {
                    accepted = prompt.DialogResult == DialogResult.OK && !closing;
                    calibrationPrompt = null;
                    completed.Set();
                };
                prompt.Show(this);
            }
            try
            {
                BeginInvoke(ShowPrompt);
                completed.Wait();
            }
            catch (InvalidOperationException) { return false; }
            return accepted;
        }

        private void TrackerService_Updated(TrackerUpdate update)
        {
            if (IsDisposed || !IsHandleCreated) return;
            long now = Environment.TickCount64;
            if (now - lastUiUpdateTicks < 50) return;
            lastUiUpdateTicks = now;
            try
            {
                BeginInvoke(new Action(() =>
                {
                    if (IsDisposed) return;
                    connectionLabel.Text = update.Status;
                    connectionDot.ForeColor = update.Ready
                        ? Color.FromArgb(42, 158, 105)
                        : update.Status.Contains("Could not", StringComparison.OrdinalIgnoreCase) ||
                          update.Status.Contains("failed", StringComparison.OrdinalIgnoreCase) ||
                          update.Status.Contains("lost", StringComparison.OrdinalIgnoreCase)
                            ? Color.FromArgb(194, 68, 68)
                            : Color.FromArgb(218, 153, 48);
                    connectButton.Enabled = update.CanRetry;
                    movementButton.Enabled = update.Ready;
                    recalibrateButton.Enabled = update.Ready;
                    movementButton.Text = update.MovementEnabled ? "Disable movement" : "Enable movement";
                    int inputPercent = Math.Clamp((int)Math.Round(update.ForwardInput * 100), 0, 100);
                    inputProgressBar.Value = inputPercent;
                    inputMeterValue.Text = $"{inputPercent}%";
                    if (update.LeftTrackerIndex is uint leftIndex)
                        leftTrackerLabel.Text = $"Tracker slot: {leftIndex}";
                    if (update.RightTrackerIndex is uint rightIndex)
                        rightTrackerLabel.Text = $"Tracker slot: {rightIndex}";

                    if (update.Left is { } left)
                    {
                        leftStatus.Text = left.Planted ? "PLANTED" : "AIR";
                        leftStatus.ForeColor = left.Planted ? Color.FromArgb(42, 158, 105) : Color.FromArgb(218, 153, 48);
                        leftYLabel.Text = $"Height Y: {left.Y:F3} m";
                        leftZLabel.Text = $"World Z: {left.Z:F3} m";
                    }
                    else
                    {
                        leftStatus.Text = "Waiting for tracker data";
                        leftYLabel.Text = "Height Y: --";
                        leftZLabel.Text = "World Z: --";
                    }

                    if (update.Right is { } right)
                    {
                        rightStatus.Text = right.Planted ? "PLANTED" : "AIR";
                        rightStatus.ForeColor = right.Planted ? Color.FromArgb(42, 158, 105) : Color.FromArgb(218, 153, 48);
                        rightYLabel.Text = $"Height Y: {right.Y:F3} m";
                        rightZLabel.Text = $"World Z: {right.Z:F3} m";
                    }
                    else
                    {
                        rightStatus.Text = "Waiting for tracker data";
                        rightYLabel.Text = "Height Y: --";
                        rightZLabel.Text = "World Z: --";
                    }

                    if (update.Ready)
                        footerLabel.Text = $"OSC packets sent to 127.0.0.1:9000 · /input/Vertical · Movement {(update.MovementEnabled ? "ON" : "OFF")}";
                    else if (update.Status.Contains("lost", StringComparison.OrdinalIgnoreCase))
                        footerLabel.Text = "Tracker poses unavailable · movement is off · OSC sends zero input";
                    else if (update.Status.Contains("Calibration", StringComparison.OrdinalIgnoreCase) ||
                             update.Status.Contains("Recalibration", StringComparison.OrdinalIgnoreCase))
                        footerLabel.Text = "Follow the calibration prompt · movement is off";
                    else if (update.CanRetry)
                        footerLabel.Text = "OSC output stopped · fix the issue, then connect again";
                    else
                        footerLabel.Text = "Connect SteamVR trackers, then follow the calibration prompts.";
                }));
            }
            catch (InvalidOperationException) { }
        }

        private void movementButton_Click(object? sender, EventArgs e)
        {
            ToggleMovement();
        }

        private void recalibrateButton_Click(object? sender, EventArgs e)
        {
            trackerService?.RequestRecalibration();
        }

        private void movementSpeedTrackBar_Scroll(object? sender, EventArgs e)
        {
            MarkPresetCustom();
            ApplyCurrentSettings();
        }

        private void raiseSensitivityTrackBar_Scroll(object? sender, EventArgs e)
        {
            MarkPresetCustom();
            ApplyCurrentSettings();
        }

        private void movementTriggerTrackBar_Scroll(object? sender, EventArgs e)
        {
            MarkPresetCustom();
            ApplyCurrentSettings();
        }

        private void resetButton_Click(object? sender, EventArgs e)
        {
            presetComboBox.SelectedItem = "Normal";
            ApplyPresetValues("Normal");
            footerLabel.Text = "Settings reset to defaults and saved.";
        }

        private void helpButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(this,
                "TrackStepVR quick help\n\n" +
                "Connect & calibrate: keep both feet on the floor for the first prompt, then lift only the requested foot for each prompt.\n\n" +
                "The live input bar shows the current /input/Vertical value from 0 to 100%. Presets apply starter settings; changing a slider switches to Custom.\n\n" +
                "Forward movement follows the headset's horizontal facing direction. World Z is shown only as a tracker position reading; it does not determine forward movement by itself.\n\n" +
                "Movement speed: caps the strength of the forward input sent to VRChat. VRChat determines the avatar's actual speed.\n\n" +
                "Raise leg sensitivity: the height above ground needed to mark a foot as AIR. Lower centimeters detect smaller lifts.\n\n" +
                "Movement trigger: minimum forward swing speed before movement input starts. Lower values trigger more easily.\n\n" +
                "Press T while this window is focused to toggle movement. The button shows whether movement is on or off.\n\n" +
                "Recalibrate ground: stand normally on the floor and capture a new ground level. Movement turns off during recalibration.\n\n" +
                "OSC target: 127.0.0.1:9000 · /input/Vertical. The app sends UDP packets but cannot confirm VRChat received them.",
                "TrackStepVR help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadUserSettings()
        {
            loadingSettings = true;
            try
            {
                string path = Path.Combine(Application.LocalUserAppDataPath, "settings.json");
                if (File.Exists(path))
                {
                    AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path));
                    if (settings != null)
                    {
                        movementSpeedTrackBar.Value = Math.Clamp(settings.MovementSpeedPercent,
                            movementSpeedTrackBar.Minimum, movementSpeedTrackBar.Maximum);
                        raiseSensitivityTrackBar.Value = Math.Clamp(settings.RaiseSensitivityCm,
                            raiseSensitivityTrackBar.Minimum, raiseSensitivityTrackBar.Maximum);
                        movementTriggerTrackBar.Value = Math.Clamp(settings.MovementTriggerHundredths,
                            movementTriggerTrackBar.Minimum, movementTriggerTrackBar.Maximum);
                        presetComboBox.SelectedItem = presetComboBox.Items.Contains(settings.PresetName)
                            ? settings.PresetName : "Custom";
                    }
                }
            }
            catch (Exception ex)
            {
                footerLabel.Text = $"Could not load saved settings: {ex.Message}";
            }
            finally { loadingSettings = false; }
            UpdateSettingReadouts();
        }

        private void SaveUserSettings()
        {
            try
            {
                Directory.CreateDirectory(Application.LocalUserAppDataPath);
                string path = Path.Combine(Application.LocalUserAppDataPath, "settings.json");
                var settings = new AppSettings
                {
                    MovementSpeedPercent = movementSpeedTrackBar.Value,
                    RaiseSensitivityCm = raiseSensitivityTrackBar.Value,
                    MovementTriggerHundredths = movementTriggerTrackBar.Value,
                    PresetName = presetComboBox.SelectedItem?.ToString() ?? "Custom"
                };
                string temporaryPath = path + ".tmp";
                File.WriteAllText(temporaryPath,
                    JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
                File.Move(temporaryPath, path, overwrite: true);
            }
            catch (Exception ex)
            {
                footerLabel.Text = $"Could not save settings: {ex.Message}";
            }
        }

        private void presetComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (loadingSettings) return;
            string selected = presetComboBox.SelectedItem?.ToString() ?? "Custom";
            if (selected == "Custom")
            {
                SaveUserSettings();
                return;
            }
            ApplyPresetValues(selected);
        }

        private void ApplyPresetValues(string name)
        {
            (int speed, int lift, int trigger) = name switch
            {
                "Gentle" => (55, 6, 8),
                "Responsive" => (100, 1, 3),
                _ => (80, 3, 5)
            };
            applyingPreset = true;
            movementSpeedTrackBar.Value = speed;
            raiseSensitivityTrackBar.Value = lift;
            movementTriggerTrackBar.Value = trigger;
            applyingPreset = false;
            ApplyCurrentSettings();
        }

        private void MarkPresetCustom()
        {
            if (!loadingSettings && !applyingPreset && presetComboBox.SelectedItem?.ToString() != "Custom")
                presetComboBox.SelectedItem = "Custom";
        }

        private void ApplyCurrentSettings()
        {
            UpdateSettingReadouts();
            trackerService?.SetMovementSpeed(movementSpeedTrackBar.Value);
            trackerService?.SetRaiseSensitivity(raiseSensitivityTrackBar.Value);
            trackerService?.SetMovementTrigger(movementTriggerTrackBar.Value);
            SaveUserSettings();
        }

        private void UpdateSettingReadouts()
        {
            movementSpeedValue.Text = $"{movementSpeedTrackBar.Value}%";
            raiseSensitivityValue.Text = $"{raiseSensitivityTrackBar.Value} cm";
            movementTriggerValue.Text = $"{movementTriggerTrackBar.Value / 100f:F2} m/s";
        }

        private void ToggleMovement()
        {
            if (trackerService == null || !movementButton.Enabled) return;
            bool enabled = trackerService.ToggleMovement();
            movementButton.Text = enabled ? "Disable movement" : "Enable movement";
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.T || !movementButton.Enabled) return;
            ToggleMovement();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (allowClose) return;
            e.Cancel = true;
            if (shutdownInProgress) return;
            shutdownInProgress = true;
            closing = true;
            calibrationPrompt?.Close();
            Enabled = false;
            if (trackerService != null)
            {
                // Request cancellation first so a calibration prompt can no longer start
                // another tracking step while the form is closing.
                trackerService.Stop();
                try { await trackerService.StopAsync(); }
                catch (Exception ex) { footerLabel.Text = $"Shutdown warning: {ex.Message}"; }
                finally
                {
                    trackerService.Dispose();
                    trackerService = null;
                }
            }
            allowClose = true;
            Close();
        }
    }

    internal sealed class AppSettings
    {
        public int MovementSpeedPercent { get; set; } = 80;
        public int RaiseSensitivityCm { get; set; } = 3;
        public int MovementTriggerHundredths { get; set; } = 5;
        public string PresetName { get; set; } = "Normal";
    }
}
