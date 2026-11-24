using UnityEngine;

/// <summary>
/// Instruction texts shown by the HUD.
/// Kept as data so they can be swapped for localized versions later.
/// </summary>
[CreateAssetMenu(fileName = "MeasurementHudTexts", menuName = "AR Measure/HUD Texts")]
public class MeasurementHudTexts : ScriptableObject
{
    [SerializeField] private string searchingSurface = "Move your phone slowly to find a surface";
    [SerializeField] private string placingA = "Aim at the start point and press +";
    [SerializeField] private string placingB = "Aim at the end point and press +";
    [SerializeField] private string locked = "Press + to start a new measurement";

    public string SearchingSurface => searchingSurface;
    public string PlacingA => placingA;
    public string PlacingB => placingB;
    public string Locked => locked;
}
