using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Screen-center crosshair: shows where the next point goes and whether there's a surface under it.
/// </summary>
public class ReticleView : MonoBehaviour
{
    [SerializeField] private Graphic crosshair;
    [SerializeField] private MeasurementSettings settings;

    public void SetSurfaceFound(bool found)
    {
        crosshair.color = found ? settings.ReticleFoundColor : settings.ReticleSearchingColor;
    }

    /// <summary>
    /// Short scale punch confirming a point was added.
    /// </summary>
    public void PlayConfirm()
    {
        // Stop and reset first so fast consecutive taps don't accumulate scale
        iTween.Stop(crosshair.gameObject);
        crosshair.rectTransform.localScale = Vector3.one;
        iTween.PunchScale(crosshair.gameObject, Vector3.one * settings.ReticlePunchAmount, settings.ReticlePunchTime);
    }
}
