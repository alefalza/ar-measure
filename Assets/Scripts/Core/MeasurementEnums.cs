/// <summary>
/// Phases of a two-point measurement.
/// </summary>
public enum MeasurementState
{
    PlacingA,
    PlacingB,
    Locked
}

/// <summary>
/// Identifies each endpoint of a measurement. Values are used as array indices.
/// </summary>
public enum MeasurementPointId
{
    A = 0,
    B = 1
}
