using System.Diagnostics;
using System.Threading;
using Valve.VR;

namespace TrackStepVR;

internal sealed record FootReading(bool Planted, float Y, float Z, float ForwardSwingSpeed);
internal sealed record TrackerUpdate(string Status, bool Ready, bool MovementEnabled,
    FootReading? Left, FootReading? Right, bool CanRetry = false,
    uint? LeftTrackerIndex = null, uint? RightTrackerIndex = null, float ForwardInput = 0);

/// <summary>Owns SteamVR tracking and OSC output on one background thread.</summary>
internal sealed class TrackerService : IDisposable
{
    private readonly CancellationTokenSource stop = new();
    private readonly object stateLock = new();
    private Task? worker;
    private bool movementEnabled;
    private bool recalibrationRequested;
    private int movementSpeedPercent = 80;
    private int raiseSensitivityCm = 3;
    private int movementTriggerHundredths = 5;
    private string status = "Starting SteamVR…";

    public event Action<TrackerUpdate>? Updated;
    public Func<string, bool>? ConfirmCalibrationStep { get; set; }
    public bool IsRunning => worker is { IsCompleted: false };

    public void Start()
    {
        if (worker != null) return;
        worker = Task.Run(() => Run(stop.Token));
    }

    public bool ToggleMovement()
    {
        lock (stateLock) return movementEnabled = !movementEnabled;
    }

    public void SetMovementSpeed(int percent)
    {
        lock (stateLock) movementSpeedPercent = Math.Clamp(percent, 0, 100);
    }

    public void SetRaiseSensitivity(int centimeters)
    {
        lock (stateLock) raiseSensitivityCm = Math.Clamp(centimeters, 1, 8);
    }

    public void SetMovementTrigger(int hundredthsOfMetersPerSecond)
    {
        lock (stateLock) movementTriggerHundredths = Math.Clamp(hundredthsOfMetersPerSecond, 1, 20);
    }

    public void RequestRecalibration()
    {
        lock (stateLock)
        {
            movementEnabled = false;
            recalibrationRequested = true;
        }
    }

    public void Stop() => stop.Cancel();

    public async Task StopAsync()
    {
        stop.Cancel();
        if (worker != null)
            await worker.ConfigureAwait(false);
    }

    public void Dispose()
    {
        stop.Cancel();
        stop.Dispose();
    }

    private void Run(CancellationToken cancellationToken)
    {
        CVRSystem? vrSystem = null;
        OscSender? oscSender = null;
        try
        {
            EVRInitError error = EVRInitError.None;
            vrSystem = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);
            if (error != EVRInitError.None || vrSystem == null)
                throw new InvalidOperationException($"Could not start SteamVR: {error}");

            oscSender = new OscSender();
            SetStatus("Looking for SteamVR trackers…");
            List<uint> trackerIndices = FindConnectedTrackers(vrSystem);
            if (trackerIndices.Count < 2)
                throw new InvalidOperationException("Connect at least two SteamVR trackers, then try again.");

            if (!Ask("Stand with both feet on the floor, then choose OK to capture ground height.",
                "Calibration 1/3 · Capture ground height"))
                throw new OperationCanceledException("Calibration cancelled.");

            float[] groundReadings;
            List<uint> candidates;
            int groundAttempts = 0;
            while (true)
            {
                groundReadings = CaptureTrackerY(vrSystem, trackerIndices, 35, cancellationToken, out float[] deviations);
                candidates = FindLowestTrackers(trackerIndices, groundReadings, 2);
                if (candidates.Count < 2)
                    throw new InvalidOperationException("Could not identify two foot tracker candidates.");
                bool noisy = HasNoisyTrackers(candidates, deviations);
                if (!noisy) break;
                if (groundAttempts++ == 0 && Ask(
                    "The trackers moved during the ground reading. Keep still and choose OK to measure again, or Cancel to continue with this reading.",
                    "Calibration warning · tracker movement detected"))
                    continue;
                if (groundAttempts > 1)
                {
                    SetStatus("Calibration warning · tracker readings are still moving");
                    if (!Ask("The ground reading is still unstable. Choose OK to continue with its average, or Cancel to stop and try calibration again.",
                        "Calibration warning · unstable ground reading"))
                        throw new OperationCanceledException("Ground calibration cancelled because trackers were moving.");
                }
                break;
            }
            if (candidates.Count < 2)
                throw new InvalidOperationException("Could not identify two foot tracker candidates.");

            if (!Ask("Raise and hold your LEFT foot, then choose OK.",
                "Calibration 2/3 · Identify left foot"))
                throw new OperationCanceledException("Calibration cancelled.");
            float[] leftLift = CaptureTrackerY(vrSystem, trackerIndices, 25, cancellationToken, out _);
            if (!TryFindLiftedTracker(candidates, groundReadings, leftLift, null, GetLiftThreshold(),
                out uint leftIndex, out float leftLiftAmount))
                throw new InvalidOperationException("Could not identify the left foot. Restart and lift it clearly during calibration.");

            if (!Ask($"Left foot detected at tracker {leftIndex} (lifted {leftLiftAmount:F2} m). Lower it, then raise and hold your RIGHT foot.",
                "Calibration 3/3 · Identify right foot"))
                throw new OperationCanceledException("Calibration cancelled.");
            float[] rightLift = CaptureTrackerY(vrSystem, trackerIndices, 25, cancellationToken, out _);
            if (!TryFindLiftedTracker(candidates, groundReadings, rightLift, leftIndex, GetLiftThreshold(),
                out uint rightIndex, out float rightLiftAmount))
                throw new InvalidOperationException("Could not identify the right foot. Restart and lift it clearly during calibration.");

            float leftGroundY = groundReadings[leftIndex];
            float rightGroundY = groundReadings[rightIndex];
            SetStatus($"Calibration complete · Left slot {leftIndex}, right slot {rightIndex}");

            const float minimumPlantTime = 0.06f;
            const float liftConfirmTime = 0.08f;
            const float groundContactTolerance = 0.04f;
            const float fullForwardInputSpeed = 0.40f;
            const float maximumPlausibleSwingSpeed = 3.0f;
            const float speedFilterSeconds = 0.06f;
            const float inputReleaseSeconds = 0.08f;
            var leftFoot = new FootState();
            var rightFoot = new FootState();
            bool firstReading = true;
            float smoothedSwingSpeed = 0;
            float sentInput = 0;
            // SteamVR's HMD local forward direction, projected onto the floor.
            float forwardX = 0;
            float forwardZ = -1;
            var clock = Stopwatch.StartNew();
            double previousTime = clock.Elapsed.TotalSeconds;
            bool trackingLost = false;

            while (!cancellationToken.IsCancellationRequested)
            {
                bool recalibrate;
                lock (stateLock)
                {
                    recalibrate = recalibrationRequested;
                    recalibrationRequested = false;
                    if (recalibrate) movementEnabled = false;
                }

                if (recalibrate)
                {
                    oscSender.SendFloat("/input/Vertical", 0);
                    if (Ask("Stand with both feet on the floor, then choose OK to recalibrate ground height.",
                        "Recalibration · Capture ground height"))
                    {
                        var selected = new List<uint> { leftIndex, rightIndex };
                        float[] newGround = CaptureTrackerY(vrSystem, selected, 35, cancellationToken, out float[] deviations);
                        bool useNewGround = true;
                        if (HasNoisyTrackers(selected, deviations) &&
                            Ask("The trackers moved during ground recalibration. Keep still and choose OK to measure again, or Cancel to use this reading.",
                                "Recalibration warning · tracker movement detected"))
                        {
                            newGround = CaptureTrackerY(vrSystem, selected, 35, cancellationToken, out deviations);
                            if (HasNoisyTrackers(selected, deviations))
                                useNewGround = Ask("The ground reading is still unstable. Choose OK to use its average, or Cancel to keep the previous ground level.",
                                    "Recalibration warning · unstable ground reading");
                        }
                        if (useNewGround && float.IsFinite(newGround[leftIndex]) && float.IsFinite(newGround[rightIndex]))
                        {
                            leftGroundY = newGround[leftIndex];
                            rightGroundY = newGround[rightIndex];
                            firstReading = true;
                            SetStatus("Ground recalibrated · Movement is off");
                        }
                        else SetStatus("Recalibration failed · Tracker pose is invalid");
                    }
                    else SetStatus("Recalibration cancelled · Movement is off");
                }

                double now = clock.Elapsed.TotalSeconds;
                float deltaTime = (float)Math.Clamp(now - previousTime, 0.001, 0.1);
                previousTime = now;
                var poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
                vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, poses);
                if (!poses[leftIndex].bPoseIsValid || !poses[rightIndex].bPoseIsValid ||
                    !poses[OpenVR.k_unTrackedDeviceIndex_Hmd].bPoseIsValid)
                {
                    if (!trackingLost)
                    {
                        trackingLost = true;
                        lock (stateLock) movementEnabled = false;
                        SetStatus("VR tracking lost · movement turned off");
                    }
                    smoothedSwingSpeed = 0;
                    sentInput = 0;
                    oscSender.SendFloat("/input/Vertical", 0);
                    Publish(leftFoot, rightFoot, false, leftIndex, rightIndex, ready: false);
                    Thread.Sleep(20);
                    continue;
                }

                if (trackingLost)
                {
                    trackingLost = false;
                    SetStatus("Foot tracking restored · movement remains off");
                }

                HmdMatrix34_t lm = poses[leftIndex].mDeviceToAbsoluteTracking;
                HmdMatrix34_t rm = poses[rightIndex].mDeviceToAbsoluteTracking;
                HmdMatrix34_t hm = poses[OpenVR.k_unTrackedDeviceIndex_Hmd].mDeviceToAbsoluteTracking;
                float hmdForwardX = -hm.m2;
                float hmdForwardZ = -hm.m10;
                float horizontalLength = MathF.Sqrt(hmdForwardX * hmdForwardX + hmdForwardZ * hmdForwardZ);
                if (horizontalLength > 0.1f)
                {
                    forwardX = hmdForwardX / horizontalLength;
                    forwardZ = hmdForwardZ / horizontalLength;
                }
                float lx = lm.m3, ly = lm.m7, lz = lm.m11;
                float rx = rm.m3, ry = rm.m7, rz = rm.m11;
                float liftThreshold = GetLiftThreshold();
                if (firstReading)
                {
                    InitializeFoot(leftFoot, lx, ly, lz, leftGroundY);
                    InitializeFoot(rightFoot, rx, ry, rz, rightGroundY);
                    firstReading = false;
                }
                else
                {
                    UpdateFoot(leftFoot, lx, ly, lz, minimumPlantTime, liftThreshold,
                        liftConfirmTime, groundContactTolerance, now, deltaTime, forwardX, forwardZ);
                    UpdateFoot(rightFoot, rx, ry, rz, minimumPlantTime, liftThreshold,
                        liftConfirmTime, groundContactTolerance, now, deltaTime, forwardX, forwardZ);
                }

                bool enabled;
                float maxForwardInput;
                float forwardSpeedDeadZone;
                lock (stateLock)
                {
                    enabled = movementEnabled;
                    maxForwardInput = movementSpeedPercent / 100f;
                    forwardSpeedDeadZone = movementTriggerHundredths / 100f;
                }
                float swingSpeed = Math.Max(leftFoot.ForwardSwingSpeed, rightFoot.ForwardSwingSpeed);
                // Smooth tracker noise and discard implausible single-frame velocity spikes.
                swingSpeed = Math.Min(swingSpeed, maximumPlausibleSwingSpeed);
                float speedAlpha = 1f - MathF.Exp(-deltaTime / speedFilterSeconds);
                smoothedSwingSpeed += (swingSpeed - smoothedSwingSpeed) * speedAlpha;
                float targetInput = enabled
                    ? Math.Clamp((smoothedSwingSpeed - forwardSpeedDeadZone) /
                        (fullForwardInputSpeed - forwardSpeedDeadZone), 0, 1) * maxForwardInput
                    : 0;
                // Let input rise immediately, but decay briefly instead of cutting at one noisy frame.
                if (targetInput >= sentInput || !enabled)
                    sentInput = targetInput;
                else
                {
                    float releaseAlpha = 1f - MathF.Exp(-deltaTime / inputReleaseSeconds);
                    sentInput += (targetInput - sentInput) * releaseAlpha;
                }
                oscSender.SendFloat("/input/Vertical", sentInput);
                Publish(leftFoot, rightFoot, enabled, leftIndex, rightIndex, forwardInput: sentInput);
                Thread.Sleep(20);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            SetStatus(ex.Message);
        }
        finally
        {
            if (oscSender != null)
            {
                try { oscSender.SendFloat("/input/Vertical", 0); } catch { }
                oscSender.Dispose();
            }
            if (vrSystem != null) OpenVR.Shutdown();
            Publish(new TrackerUpdate(status, false, false, null, null, CanRetry: true));
        }
    }

    private float GetLiftThreshold()
    {
        lock (stateLock) return raiseSensitivityCm / 100f;
    }

    private bool Ask(string message, string progress)
    {
        SetStatus(progress);
        return ConfirmCalibrationStep?.Invoke(message) == true;
    }

    private void SetStatus(string value)
    {
        status = value;
        Updated?.Invoke(new TrackerUpdate(value, false, false, null, null));
    }

    private void Publish(FootState left, FootState right, bool enabled, uint leftIndex, uint rightIndex,
        bool ready = true, float forwardInput = 0)
    {
        Updated?.Invoke(new TrackerUpdate(status, ready, enabled,
            ready ? new FootReading(left.Planted, left.CurrentY, left.CurrentZ, left.ForwardSwingSpeed) : null,
            ready ? new FootReading(right.Planted, right.CurrentY, right.CurrentZ, right.ForwardSwingSpeed) : null,
            LeftTrackerIndex: leftIndex, RightTrackerIndex: rightIndex, ForwardInput: forwardInput));
    }

    private void Publish(TrackerUpdate update) => Updated?.Invoke(update);

    private static List<uint> FindConnectedTrackers(CVRSystem system)
    {
        var indices = new List<uint>();
        for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            if (system.IsTrackedDeviceConnected(i) && system.GetTrackedDeviceClass(i) == ETrackedDeviceClass.GenericTracker)
                indices.Add(i);
        return indices;
    }

    private static float[] CaptureTrackerY(CVRSystem system, List<uint> indices, int samples,
        CancellationToken token, out float[] standardDeviations)
    {
        int count = (int)OpenVR.k_unMaxTrackedDeviceCount;
        var sums = new double[count];
        var sumsSquared = new double[count];
        var counts = new int[count];
        for (int s = 0; s < samples; s++)
        {
            token.ThrowIfCancellationRequested();
            var poses = new TrackedDevicePose_t[count];
            system.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, poses);
            foreach (uint index in indices)
                if (poses[index].bPoseIsValid)
                {
                    float y = poses[index].mDeviceToAbsoluteTracking.m7;
                    sums[index] += y;
                    sumsSquared[index] += y * y;
                    counts[index]++;
                }
            Thread.Sleep(20);
        }
        var averages = new float[count];
        standardDeviations = new float[count];
        Array.Fill(averages, float.NaN);
        Array.Fill(standardDeviations, float.NaN);
        int minimumValidSamples = (int)Math.Ceiling(samples * 0.8);
        foreach (uint index in indices)
            if (counts[index] >= minimumValidSamples)
            {
                double average = sums[index] / counts[index];
                double variance = Math.Max(0, sumsSquared[index] / counts[index] - average * average);
                averages[index] = (float)average;
                standardDeviations[index] = (float)Math.Sqrt(variance);
            }
        return averages;
    }

    private static bool HasNoisyTrackers(List<uint> indices, float[] deviations) =>
        indices.Any(index => !float.IsFinite(deviations[index]) || deviations[index] > 0.015f);

    private static List<uint> FindLowestTrackers(List<uint> indices, float[] ground, int wanted)
    {
        var candidates = indices.Where(i => float.IsFinite(ground[i])).ToList();
        candidates.Sort((a, b) => ground[a].CompareTo(ground[b]));
        if (candidates.Count > wanted) candidates.RemoveRange(wanted, candidates.Count - wanted);
        return candidates;
    }

    private static bool TryFindLiftedTracker(List<uint> indices, float[] ground, float[] lifted,
        uint? excluded, float minimumLift, out uint selected, out float amount)
    {
        selected = uint.MaxValue;
        amount = 0;
        foreach (uint index in indices)
        {
            if (excluded == index || !float.IsFinite(ground[index]) || !float.IsFinite(lifted[index])) continue;
            float change = lifted[index] - ground[index];
            if (change > amount) { amount = change; selected = index; }
        }
        return selected != uint.MaxValue && amount >= minimumLift;
    }

    private static void InitializeFoot(FootState foot, float x, float y, float z, float groundY)
    {
        foot.GroundY = groundY;
        foot.Planted = false;
        foot.FloorContactSince = foot.LiftSince = -1;
        foot.PreviousDisplacementZ = foot.ForwardCompensation = foot.ForwardSwingSpeed = foot.FilteredSwingSpeed = 0;
        foot.PreviousX = foot.CurrentX = x;
        foot.PreviousY = foot.CurrentY = foot.FilteredY = y;
        foot.PreviousZ = foot.CurrentZ = z;
        foot.AddYSample(y); foot.AddYSample(y); foot.AddYSample(y);
    }

    private static void UpdateFoot(FootState foot, float x, float y, float z, float minPlant,
        float liftThreshold, float liftConfirm, float groundTolerance, double now, float dt,
        float forwardX, float forwardZ)
    {
        float filteredY = foot.FilterY(y);
        float deltaX = x - foot.PreviousX;
        float deltaZ = z - foot.PreviousZ;
        float forwardDelta = deltaX * forwardX + deltaZ * forwardZ;
        float rawSwingSpeed = !foot.Planted && forwardDelta > 0 ? forwardDelta / dt : 0;
        float speedAlpha = 1f - MathF.Exp(-dt / 0.06f);
        foot.FilteredSwingSpeed += (rawSwingSpeed - foot.FilteredSwingSpeed) * speedAlpha;
        foot.ForwardSwingSpeed = foot.FilteredSwingSpeed;
        float deltaY = filteredY - foot.PreviousY;
        foot.PreviousX = x; foot.PreviousY = filteredY; foot.PreviousZ = z;
        foot.CurrentX = x; foot.CurrentY = filteredY; foot.CurrentZ = z;
        if (!foot.Planted)
        {
            bool nearGround = Math.Abs(filteredY - foot.GroundY) <= groundTolerance;
            bool settled = Math.Abs(deltaY / dt) <= 0.15f;
            if (nearGround && settled)
            {
                if (foot.FloorContactSince < 0) foot.FloorContactSince = now;
                if (now - foot.FloorContactSince >= minPlant)
                {
                    foot.Planted = true; foot.AnchorX = x; foot.AnchorY = filteredY; foot.AnchorZ = z;
                    foot.PreviousDisplacementZ = foot.ForwardCompensation = 0;
                }
            }
            else foot.FloorContactSince = -1;
            return;
        }
        if (filteredY > foot.GroundY + liftThreshold)
        {
            if (foot.LiftSince < 0) foot.LiftSince = now;
            if (now - foot.LiftSince >= liftConfirm)
            {
                foot.Planted = false; foot.FloorContactSince = foot.LiftSince = -1;
                foot.PreviousDisplacementZ = foot.ForwardCompensation = 0;
                return;
            }
        }
        else foot.LiftSince = -1;
        float displacement = z - foot.AnchorZ;
        float movement = displacement - foot.PreviousDisplacementZ;
        foot.PreviousDisplacementZ = displacement;
        foot.ForwardCompensation = movement < 0 ? -movement : 0;
    }

}

internal sealed class FootState
{
    private readonly Queue<float> ySamples = new();
    public bool Planted;
    public float AnchorX, AnchorY, AnchorZ;
    public float CurrentX, CurrentY, CurrentZ;
    public float PreviousX, PreviousY, PreviousZ;
    public float GroundY, FilteredY, ForwardSwingSpeed, FilteredSwingSpeed;
    public double FloorContactSince = -1, LiftSince = -1;
    public float PreviousDisplacementZ, ForwardCompensation;

    public void AddYSample(float value) => ySamples.Enqueue(value);
    public float FilterY(float value)
    {
        AddYSample(value);
        if (ySamples.Count > 3) ySamples.Dequeue();
        FilteredY = ySamples.Average();
        return FilteredY;
    }
}
