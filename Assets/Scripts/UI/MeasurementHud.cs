using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Shows the instruction for the current state and enables the "+" button
/// only while there's a surface to place a point on.
/// Updates only when the state or the surface status changes, not every frame.
/// </summary>
public class MeasurementHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button addButton;
    [SerializeField] private MeasurementHudTexts texts;

    private MeasurementState state;
    private bool surfaceFound;

    public void ShowState(MeasurementState newState)
    {
        state = newState;
        RefreshText();
    }

    public void SetSurfaceFound(bool found)
    {
        surfaceFound = found;
        addButton.interactable = found;
        RefreshText();
    }

    private void RefreshText()
    {
        // While a measurement is locked, the result matters more than coaching
        bool isSearching = !surfaceFound && state != MeasurementState.Locked;

        instructionText.text = isSearching ? texts.SearchingSurface : state switch
        {
            MeasurementState.PlacingA => texts.PlacingA,
            MeasurementState.PlacingB => texts.PlacingB,
            MeasurementState.Locked => texts.Locked,
            _ => string.Empty
        };
    }
}
