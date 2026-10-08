namespace TrackStepVR
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            headerPanel = new Panel();
            titleLabel = new Label();
            subtitleLabel = new Label();
            connectionPanel = new Panel();
            connectionDot = new Label();
            connectionLabel = new Label();
            connectButton = new Button();
            leftCard = new Panel();
            leftTitle = new Label();
            leftTrackerLabel = new Label();
            leftStatus = new Label();
            leftYLabel = new Label();
            leftZLabel = new Label();
            rightCard = new Panel();
            rightTitle = new Label();
            rightTrackerLabel = new Label();
            rightStatus = new Label();
            rightYLabel = new Label();
            rightZLabel = new Label();
            movementButton = new Button();
            recalibrateButton = new Button();
            resetButton = new Button();
            helpButton = new Button();
            inputMeterPanel = new Panel();
            inputMeterTitle = new Label();
            inputProgressBar = new ProgressBar();
            inputMeterValue = new Label();
            sensitivityPanel = new Panel();
            presetTitle = new Label();
            presetComboBox = new ComboBox();
            movementSpeedTitle = new Label();
            movementSpeedHint = new Label();
            movementSpeedTrackBar = new TrackBar();
            movementSpeedValue = new Label();
            raiseSensitivityTitle = new Label();
            raiseSensitivityHint = new Label();
            raiseSensitivityTrackBar = new TrackBar();
            raiseSensitivityValue = new Label();
            movementTriggerTitle = new Label();
            movementTriggerHint = new Label();
            movementTriggerTrackBar = new TrackBar();
            movementTriggerValue = new Label();
            footerLabel = new Label();
            headerPanel.SuspendLayout();
            connectionPanel.SuspendLayout();
            leftCard.SuspendLayout();
            rightCard.SuspendLayout();
            inputMeterPanel.SuspendLayout();
            sensitivityPanel.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(29, 36, 49);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(820, 105);
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(28, 17);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(110, 37);
            titleLabel.Text = "TrackStepVR";
            // 
            // subtitleLabel
            // 
            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font("Segoe UI", 9.5F);
            subtitleLabel.ForeColor = Color.FromArgb(187, 198, 216);
            subtitleLabel.Location = new Point(32, 62);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(265, 17);
            subtitleLabel.Text = "Foot tracking and movement controls";
            // 
            // connectionPanel
            // 
            connectionPanel.BackColor = Color.White;
            connectionPanel.Controls.Add(connectionDot);
            connectionPanel.Controls.Add(connectionLabel);
            connectionPanel.Controls.Add(connectButton);
            connectionPanel.Location = new Point(28, 126);
            connectionPanel.Name = "connectionPanel";
            connectionPanel.Size = new Size(764, 58);
            // 
            // connectionDot
            // 
            connectionDot.AutoSize = true;
            connectionDot.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            connectionDot.ForeColor = Color.FromArgb(218, 153, 48);
            connectionDot.Location = new Point(17, 15);
            connectionDot.Name = "connectionDot";
            connectionDot.Size = new Size(22, 25);
            connectionDot.Text = "●";
            // 
            // connectionLabel
            // 
            connectionLabel.AutoSize = true;
            connectionLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            connectionLabel.ForeColor = Color.FromArgb(44, 52, 66);
            connectionLabel.Location = new Point(47, 19);
            connectionLabel.Name = "connectionLabel";
            connectionLabel.Size = new Size(225, 19);
            connectionLabel.Text = "Tracking service not connected";
            //
            // connectButton
            //
            connectButton.Font = new Font("Segoe UI", 9F);
            connectButton.Location = new Point(586, 11);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(160, 36);
            connectButton.Text = "Connect & calibrate";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // leftCard
            // 
            leftCard.BackColor = Color.White;
            leftCard.Controls.Add(leftTitle);
            leftCard.Controls.Add(leftTrackerLabel);
            leftCard.Controls.Add(leftStatus);
            leftCard.Controls.Add(leftYLabel);
            leftCard.Controls.Add(leftZLabel);
            leftCard.Location = new Point(28, 205);
            leftCard.Name = "leftCard";
            leftCard.Size = new Size(370, 177);
            // 
            // leftTitle
            // 
            leftTitle.AutoSize = true;
            leftTitle.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
            leftTitle.ForeColor = Color.FromArgb(44, 52, 66);
            leftTitle.Location = new Point(19, 16);
            leftTitle.Name = "leftTitle";
            leftTitle.Size = new Size(81, 25);
            leftTitle.Text = "Left foot";
            //
            // leftTrackerLabel
            //
            leftTrackerLabel.AutoSize = true;
            leftTrackerLabel.Font = new Font("Segoe UI", 9F);
            leftTrackerLabel.ForeColor = Color.FromArgb(112, 120, 134);
            leftTrackerLabel.Location = new Point(247, 22);
            leftTrackerLabel.Name = "leftTrackerLabel";
            leftTrackerLabel.Size = new Size(83, 15);
            leftTrackerLabel.Text = "Tracker slot: --";
            // 
            // leftStatus
            // 
            leftStatus.AutoSize = true;
            leftStatus.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            leftStatus.ForeColor = Color.FromArgb(112, 120, 134);
            leftStatus.Location = new Point(22, 57);
            leftStatus.Name = "leftStatus";
            leftStatus.Size = new Size(81, 15);
            leftStatus.Text = "Waiting for data";
            // 
            // leftYLabel
            // 
            leftYLabel.AutoSize = true;
            leftYLabel.Font = new Font("Segoe UI", 10F);
            leftYLabel.ForeColor = Color.FromArgb(74, 82, 96);
            leftYLabel.Location = new Point(22, 96);
            leftYLabel.Name = "leftYLabel";
            leftYLabel.Size = new Size(71, 19);
            leftYLabel.Text = "Height Y: --";
            // 
            // leftZLabel
            // 
            leftZLabel.AutoSize = true;
            leftZLabel.Font = new Font("Segoe UI", 10F);
            leftZLabel.ForeColor = Color.FromArgb(74, 82, 96);
            leftZLabel.Location = new Point(22, 126);
            leftZLabel.Name = "leftZLabel";
            leftZLabel.Size = new Size(75, 19);
            leftZLabel.Text = "World Z: --";
            // 
            // rightCard
            // 
            rightCard.BackColor = Color.White;
            rightCard.Controls.Add(rightTitle);
            rightCard.Controls.Add(rightTrackerLabel);
            rightCard.Controls.Add(rightStatus);
            rightCard.Controls.Add(rightYLabel);
            rightCard.Controls.Add(rightZLabel);
            rightCard.Location = new Point(422, 205);
            rightCard.Name = "rightCard";
            rightCard.Size = new Size(370, 177);
            // 
            // rightTitle
            // 
            rightTitle.AutoSize = true;
            rightTitle.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
            rightTitle.ForeColor = Color.FromArgb(44, 52, 66);
            rightTitle.Location = new Point(19, 16);
            rightTitle.Name = "rightTitle";
            rightTitle.Size = new Size(92, 25);
            rightTitle.Text = "Right foot";
            //
            // rightTrackerLabel
            //
            rightTrackerLabel.AutoSize = true;
            rightTrackerLabel.Font = new Font("Segoe UI", 9F);
            rightTrackerLabel.ForeColor = Color.FromArgb(112, 120, 134);
            rightTrackerLabel.Location = new Point(247, 22);
            rightTrackerLabel.Name = "rightTrackerLabel";
            rightTrackerLabel.Size = new Size(83, 15);
            rightTrackerLabel.Text = "Tracker slot: --";
            // 
            // rightStatus
            // 
            rightStatus.AutoSize = true;
            rightStatus.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            rightStatus.ForeColor = Color.FromArgb(112, 120, 134);
            rightStatus.Location = new Point(22, 57);
            rightStatus.Name = "rightStatus";
            rightStatus.Size = new Size(81, 15);
            rightStatus.Text = "Waiting for data";
            // 
            // rightYLabel
            // 
            rightYLabel.AutoSize = true;
            rightYLabel.Font = new Font("Segoe UI", 10F);
            rightYLabel.ForeColor = Color.FromArgb(74, 82, 96);
            rightYLabel.Location = new Point(22, 96);
            rightYLabel.Name = "rightYLabel";
            rightYLabel.Size = new Size(71, 19);
            rightYLabel.Text = "Height Y: --";
            // 
            // rightZLabel
            // 
            rightZLabel.AutoSize = true;
            rightZLabel.Font = new Font("Segoe UI", 10F);
            rightZLabel.ForeColor = Color.FromArgb(74, 82, 96);
            rightZLabel.Location = new Point(22, 126);
            rightZLabel.Name = "rightZLabel";
            rightZLabel.Size = new Size(75, 19);
            rightZLabel.Text = "World Z: --";
            // 
            // movementButton
            // 
            movementButton.Enabled = false;
            movementButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            movementButton.Location = new Point(28, 680);
            movementButton.Name = "movementButton";
            movementButton.Size = new Size(210, 42);
            movementButton.Text = "Enable movement";
            movementButton.UseVisualStyleBackColor = true;
            // 
            // recalibrateButton
            // 
            recalibrateButton.Enabled = false;
            recalibrateButton.Font = new Font("Segoe UI", 10F);
            recalibrateButton.Location = new Point(252, 680);
            recalibrateButton.Name = "recalibrateButton";
            recalibrateButton.Size = new Size(190, 42);
            recalibrateButton.Text = "Recalibrate ground";
            recalibrateButton.UseVisualStyleBackColor = true;
            //
            // resetButton
            //
            resetButton.Font = new Font("Segoe UI", 10F);
            resetButton.Location = new Point(454, 680);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(150, 42);
            resetButton.Text = "Reset defaults";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            //
            // helpButton
            //
            helpButton.Font = new Font("Segoe UI", 10F);
            helpButton.Location = new Point(616, 680);
            helpButton.Name = "helpButton";
            helpButton.Size = new Size(176, 42);
            helpButton.Text = "Help";
            helpButton.UseVisualStyleBackColor = true;
            helpButton.Click += helpButton_Click;
            //
            // inputMeterPanel
            //
            inputMeterPanel.BackColor = Color.White;
            inputMeterPanel.Controls.Add(inputMeterTitle);
            inputMeterPanel.Controls.Add(inputProgressBar);
            inputMeterPanel.Controls.Add(inputMeterValue);
            inputMeterPanel.Location = new Point(28, 392);
            inputMeterPanel.Name = "inputMeterPanel";
            inputMeterPanel.Size = new Size(764, 44);
            //
            // inputMeterTitle
            //
            inputMeterTitle.AutoSize = true;
            inputMeterTitle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            inputMeterTitle.ForeColor = Color.FromArgb(44, 52, 66);
            inputMeterTitle.Location = new Point(17, 13);
            inputMeterTitle.Name = "inputMeterTitle";
            inputMeterTitle.Size = new Size(138, 15);
            inputMeterTitle.Text = "Forward input · headset direction";
            //
            // inputProgressBar
            //
            inputProgressBar.Location = new Point(260, 11);
            inputProgressBar.Name = "inputProgressBar";
            inputProgressBar.Size = new Size(410, 22);
            inputProgressBar.Maximum = 100;
            inputProgressBar.Value = 0;
            //
            // inputMeterValue
            //
            inputMeterValue.AutoSize = true;
            inputMeterValue.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            inputMeterValue.ForeColor = Color.FromArgb(44, 52, 66);
            inputMeterValue.Location = new Point(683, 13);
            inputMeterValue.Name = "inputMeterValue";
            inputMeterValue.Size = new Size(24, 15);
            inputMeterValue.Text = "0%";
            //
            // sensitivityPanel
            //
            sensitivityPanel.BackColor = Color.White;
            sensitivityPanel.Controls.Add(movementSpeedTitle);
            sensitivityPanel.Controls.Add(movementSpeedHint);
            sensitivityPanel.Controls.Add(movementSpeedTrackBar);
            sensitivityPanel.Controls.Add(movementSpeedValue);
            sensitivityPanel.Controls.Add(raiseSensitivityTitle);
            sensitivityPanel.Controls.Add(raiseSensitivityHint);
            sensitivityPanel.Controls.Add(raiseSensitivityTrackBar);
            sensitivityPanel.Controls.Add(raiseSensitivityValue);
            sensitivityPanel.Controls.Add(movementTriggerTitle);
            sensitivityPanel.Controls.Add(movementTriggerHint);
            sensitivityPanel.Controls.Add(movementTriggerTrackBar);
            sensitivityPanel.Controls.Add(movementTriggerValue);
            sensitivityPanel.Controls.Add(presetTitle);
            sensitivityPanel.Controls.Add(presetComboBox);
            sensitivityPanel.Location = new Point(28, 448);
            sensitivityPanel.Name = "sensitivityPanel";
            sensitivityPanel.Size = new Size(764, 216);
            //
            // presetTitle
            //
            presetTitle.AutoSize = true;
            presetTitle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            presetTitle.ForeColor = Color.FromArgb(44, 52, 66);
            presetTitle.Location = new Point(17, 14);
            presetTitle.Name = "presetTitle";
            presetTitle.Size = new Size(41, 15);
            presetTitle.Text = "Preset";
            //
            // presetComboBox
            //
            presetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            presetComboBox.Font = new Font("Segoe UI", 9F);
            presetComboBox.Items.AddRange(new object[] { "Custom", "Gentle", "Normal", "Responsive" });
            presetComboBox.Location = new Point(73, 8);
            presetComboBox.Name = "presetComboBox";
            presetComboBox.Size = new Size(160, 23);
            presetComboBox.SelectedIndex = 2;
            presetComboBox.SelectedIndexChanged += presetComboBox_SelectedIndexChanged;
            //
            // movementSpeedTitle
            //
            movementSpeedTitle.AutoSize = true;
            movementSpeedTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            movementSpeedTitle.ForeColor = Color.FromArgb(44, 52, 66);
            movementSpeedTitle.Location = new Point(17, 50);
            movementSpeedTitle.Name = "movementSpeedTitle";
            movementSpeedTitle.Size = new Size(105, 19);
            movementSpeedTitle.Text = "Movement speed";
            //
            // movementSpeedHint
            //
            movementSpeedHint.AutoSize = true;
            movementSpeedHint.Font = new Font("Segoe UI", 8F);
            movementSpeedHint.ForeColor = Color.FromArgb(112, 120, 134);
            movementSpeedHint.Location = new Point(18, 73);
            movementSpeedHint.Name = "movementSpeedHint";
            movementSpeedHint.Size = new Size(211, 13);
            movementSpeedHint.Text = "Controls strength of forward input";
            //
            // movementSpeedTrackBar
            //
            movementSpeedTrackBar.Location = new Point(260, 41);
            movementSpeedTrackBar.Minimum = 0;
            movementSpeedTrackBar.Maximum = 100;
            movementSpeedTrackBar.Value = 80;
            movementSpeedTrackBar.TickFrequency = 10;
            movementSpeedTrackBar.TickStyle = TickStyle.BottomRight;
            movementSpeedTrackBar.Name = "movementSpeedTrackBar";
            movementSpeedTrackBar.Size = new Size(410, 52);
            movementSpeedTrackBar.Scroll += movementSpeedTrackBar_Scroll;
            //
            // movementSpeedValue
            //
            movementSpeedValue.AutoSize = true;
            movementSpeedValue.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            movementSpeedValue.ForeColor = Color.FromArgb(44, 52, 66);
            movementSpeedValue.Location = new Point(683, 55);
            movementSpeedValue.Name = "movementSpeedValue";
            movementSpeedValue.Size = new Size(42, 19);
            movementSpeedValue.Text = "80%";
            //
            // raiseSensitivityTitle
            //
            raiseSensitivityTitle.AutoSize = true;
            raiseSensitivityTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            raiseSensitivityTitle.ForeColor = Color.FromArgb(44, 52, 66);
            raiseSensitivityTitle.Location = new Point(17, 107);
            raiseSensitivityTitle.Name = "raiseSensitivityTitle";
            raiseSensitivityTitle.Size = new Size(147, 19);
            raiseSensitivityTitle.Text = "Raise leg sensitivity";
            //
            // raiseSensitivityHint
            //
            raiseSensitivityHint.AutoSize = true;
            raiseSensitivityHint.Font = new Font("Segoe UI", 8F);
            raiseSensitivityHint.ForeColor = Color.FromArgb(112, 120, 134);
            raiseSensitivityHint.Location = new Point(18, 130);
            raiseSensitivityHint.Name = "raiseSensitivityHint";
            raiseSensitivityHint.Size = new Size(215, 13);
            raiseSensitivityHint.Text = "Lower cm value detects smaller lifts";
            //
            // raiseSensitivityTrackBar
            //
            raiseSensitivityTrackBar.Location = new Point(260, 91);
            raiseSensitivityTrackBar.Minimum = 1;
            raiseSensitivityTrackBar.Maximum = 8;
            raiseSensitivityTrackBar.Value = 3;
            raiseSensitivityTrackBar.TickFrequency = 1;
            raiseSensitivityTrackBar.TickStyle = TickStyle.BottomRight;
            raiseSensitivityTrackBar.Name = "raiseSensitivityTrackBar";
            raiseSensitivityTrackBar.Size = new Size(410, 52);
            raiseSensitivityTrackBar.Scroll += raiseSensitivityTrackBar_Scroll;
            //
            // raiseSensitivityValue
            //
            raiseSensitivityValue.AutoSize = true;
            raiseSensitivityValue.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            raiseSensitivityValue.ForeColor = Color.FromArgb(44, 52, 66);
            raiseSensitivityValue.Location = new Point(683, 106);
            raiseSensitivityValue.Name = "raiseSensitivityValue";
            raiseSensitivityValue.Size = new Size(39, 19);
            raiseSensitivityValue.Text = "3 cm";
            //
            // movementTriggerTitle
            //
            movementTriggerTitle.AutoSize = true;
            movementTriggerTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            movementTriggerTitle.ForeColor = Color.FromArgb(44, 52, 66);
            movementTriggerTitle.Location = new Point(17, 164);
            movementTriggerTitle.Name = "movementTriggerTitle";
            movementTriggerTitle.Size = new Size(117, 19);
            movementTriggerTitle.Text = "Movement trigger";
            //
            // movementTriggerHint
            //
            movementTriggerHint.AutoSize = true;
            movementTriggerHint.Font = new Font("Segoe UI", 8F);
            movementTriggerHint.ForeColor = Color.FromArgb(112, 120, 134);
            movementTriggerHint.Location = new Point(18, 187);
            movementTriggerHint.Name = "movementTriggerHint";
            movementTriggerHint.Size = new Size(213, 13);
            movementTriggerHint.Text = "Minimum swing speed to start movement";
            //
            // movementTriggerTrackBar
            //
            movementTriggerTrackBar.Location = new Point(260, 148);
            movementTriggerTrackBar.Minimum = 1;
            movementTriggerTrackBar.Maximum = 20;
            movementTriggerTrackBar.Value = 5;
            movementTriggerTrackBar.TickFrequency = 2;
            movementTriggerTrackBar.TickStyle = TickStyle.BottomRight;
            movementTriggerTrackBar.Name = "movementTriggerTrackBar";
            movementTriggerTrackBar.Size = new Size(410, 52);
            movementTriggerTrackBar.Scroll += movementTriggerTrackBar_Scroll;
            //
            // movementTriggerValue
            //
            movementTriggerValue.AutoSize = true;
            movementTriggerValue.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            movementTriggerValue.ForeColor = Color.FromArgb(44, 52, 66);
            movementTriggerValue.Location = new Point(683, 163);
            movementTriggerValue.Name = "movementTriggerValue";
            movementTriggerValue.Size = new Size(61, 19);
            movementTriggerValue.Text = "0.05 m/s";
            // 
            // footerLabel
            // 
            footerLabel.AutoSize = true;
            footerLabel.Font = new Font("Segoe UI", 9F);
            footerLabel.ForeColor = Color.FromArgb(112, 120, 134);
            footerLabel.Location = new Point(28, 730);
            footerLabel.Name = "footerLabel";
            footerLabel.Size = new Size(329, 15);
            footerLabel.Text = "Controls become available when the tracking service is connected.";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(241, 244, 248);
            ClientSize = new Size(820, 758);
            Controls.Add(footerLabel);
            Controls.Add(sensitivityPanel);
            Controls.Add(inputMeterPanel);
            Controls.Add(recalibrateButton);
            Controls.Add(movementButton);
            Controls.Add(resetButton);
            Controls.Add(helpButton);
            Controls.Add(rightCard);
            Controls.Add(leftCard);
            Controls.Add(connectionPanel);
            Controls.Add(headerPanel);
            MinimumSize = new Size(840, 802);
            KeyPreview = true;
            KeyDown += MainForm_KeyDown;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TrackStepVR";
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            connectionPanel.ResumeLayout(false);
            connectionPanel.PerformLayout();
            leftCard.ResumeLayout(false);
            leftCard.PerformLayout();
            rightCard.ResumeLayout(false);
            rightCard.PerformLayout();
            inputMeterPanel.ResumeLayout(false);
            inputMeterPanel.PerformLayout();
            sensitivityPanel.ResumeLayout(false);
            sensitivityPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel headerPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private Panel connectionPanel;
        private Label connectionDot;
        private Label connectionLabel;
        private Button connectButton;
        private Panel leftCard;
        private Label leftTitle;
        private Label leftTrackerLabel;
        private Label leftStatus;
        private Label leftYLabel;
        private Label leftZLabel;
        private Panel rightCard;
        private Label rightTitle;
        private Label rightTrackerLabel;
        private Label rightStatus;
        private Label rightYLabel;
        private Label rightZLabel;
        private Button movementButton;
        private Button recalibrateButton;
        private Button resetButton;
        private Button helpButton;
        private Panel inputMeterPanel;
        private Label inputMeterTitle;
        private ProgressBar inputProgressBar;
        private Label inputMeterValue;
        private Panel sensitivityPanel;
        private Label presetTitle;
        private ComboBox presetComboBox;
        private Label movementSpeedTitle;
        private Label movementSpeedHint;
        private TrackBar movementSpeedTrackBar;
        private Label movementSpeedValue;
        private Label raiseSensitivityTitle;
        private Label raiseSensitivityHint;
        private TrackBar raiseSensitivityTrackBar;
        private Label raiseSensitivityValue;
        private Label movementTriggerTitle;
        private Label movementTriggerHint;
        private TrackBar movementTriggerTrackBar;
        private Label movementTriggerValue;
        private Label footerLabel;
    }
}
