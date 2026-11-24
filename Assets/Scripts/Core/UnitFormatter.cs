using UnityEngine;

/// <summary>
/// Converts meters into display text according to <see cref="UnitSettings"/>.
/// Caches the last string and only rebuilds it when the displayed value changes,
/// avoiding per-frame allocations during the live preview.
/// </summary>
public class UnitFormatter
{
    private const float METERS_TO_INCHES = 39.3701f;

    private readonly DisplayUnits units;
    private readonly string numberFormat;
    private readonly float precision;

    private int lastMetersKey = int.MinValue;
    private int lastInchesKey = int.MinValue;
    private string cachedText = string.Empty;

    public UnitFormatter(UnitSettings settings)
    {
        units = settings.Units;
        numberFormat = "F" + settings.Decimals;
        precision = Mathf.Pow(10f, settings.Decimals);
    }

    public string Format(float meters)
    {
        float inches = meters * METERS_TO_INCHES;

        int metersKey = Mathf.RoundToInt(meters * precision);
        int inchesKey = Mathf.RoundToInt(inches * precision);

        if (metersKey == lastMetersKey && inchesKey == lastInchesKey)
            return cachedText;

        lastMetersKey = metersKey;
        lastInchesKey = inchesKey;
        cachedText = BuildText(meters, inches);
        return cachedText;
    }

    private string BuildText(float meters, float inches)
    {
        return units switch
        {
            DisplayUnits.Metric => $"{meters.ToString(numberFormat)} m",
            DisplayUnits.Imperial => $"{inches.ToString(numberFormat)} in",
            _ => $"{meters.ToString(numberFormat)} m / {inches.ToString(numberFormat)} in"
        };
    }
}
