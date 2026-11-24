using UnityEngine;
using TMPro;

/// <summary>
/// Resizes the background quad to match the TMP text bounds.
/// Reacts to TMP's text-changed event instead of polling every frame.
/// </summary>
public class LabelBackground : MonoBehaviour
{
    [SerializeField] private TextMeshPro targetText;
    [SerializeField] private MeasurementSettings settings;

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    // The event is global to all TMP texts, so filter by our target.
    // It fires after TMP regenerates the mesh, so rendered bounds are already valid here.
    private void OnTextChanged(Object changedText)
    {
        if (changedText == targetText)
            Refresh();
    }

    public void Refresh()
    {
        if (targetText == null) return;

        Vector2 textSize = targetText.GetRenderedValues(onlyVisibleCharacters: false);
        Vector2 padding = settings.LabelBackgroundPadding;

        transform.localScale = new Vector3(textSize.x + padding.x, textSize.y + padding.y, 1f);
    }
}
