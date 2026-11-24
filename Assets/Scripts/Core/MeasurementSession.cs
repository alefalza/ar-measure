using System;

/// <summary>
/// Pure C# state machine for a two-point measurement.
/// Holds no Unity references so it can be unit-tested and later extended (undo, polygons).
/// </summary>
public class MeasurementSession
{
    public MeasurementState State { get; private set; } = MeasurementState.PlacingA;

    public event Action<MeasurementState> StateChanged;

    /// <summary>
    /// True while a point is still pending confirmation.
    /// </summary>
    public bool IsPlacing => State != MeasurementState.Locked;

    /// <summary>
    /// The point currently being placed. Only meaningful while <see cref="IsPlacing"/> is true.
    /// </summary>
    public MeasurementPointId PendingPoint =>
        State == MeasurementState.PlacingA ? MeasurementPointId.A : MeasurementPointId.B;

    /// <summary>
    /// Confirms the pending point and moves to the next phase.
    /// Has no effect once locked: call <see cref="Reset"/> to start a new measurement.
    /// </summary>
    public void Advance()
    {
        switch (State)
        {
            case MeasurementState.PlacingA:
                SetState(MeasurementState.PlacingB);
                break;
            case MeasurementState.PlacingB:
                SetState(MeasurementState.Locked);
                break;
        }
    }

    public void Reset() => SetState(MeasurementState.PlacingA);

    private void SetState(MeasurementState newState)
    {
        State = newState;
        StateChanged?.Invoke(newState);
    }
}
