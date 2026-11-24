using UnityEngine;

public enum DisplayUnits
{
    Metric,
    Imperial,
    Both
}

/// <summary>
/// How distances are displayed to the user.
/// </summary>
[CreateAssetMenu(fileName = "UnitSettings", menuName = "AR Measure/Unit Settings")]
public class UnitSettings : ScriptableObject
{
    [SerializeField] private DisplayUnits units = DisplayUnits.Both;
    [SerializeField, Range(0, 4)] private int decimals = 2;

    public DisplayUnits Units => units;
    public int Decimals => decimals;
}
