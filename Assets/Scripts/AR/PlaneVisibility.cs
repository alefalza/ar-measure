using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Shows detected planes only while the user looks for where to start (Point A),
/// and hides them while measuring so they don't cover the scene.
/// Only the plane GameObjects are deactivated: planes keep being tracked,
/// so raycasts and anchors still work.
/// </summary>
[RequireComponent(typeof(ARPlaneManager))]
public class PlaneVisibility : MonoBehaviour
{
    private ARPlaneManager planeManager;
    private bool visible = true;

    private void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable() => planeManager.trackablesChanged.AddListener(OnPlanesChanged);

    private void OnDisable() => planeManager.trackablesChanged.RemoveListener(OnPlanesChanged);

    public void ShowForState(MeasurementState state)
    {
        SetVisible(state == MeasurementState.PlacingA);
    }

    public void SetVisible(bool value)
    {
        if (visible == value) return;

        visible = value;
        planeManager.SetTrackablesActive(visible);
    }

    // Planes detected while hidden spawn active from the prefab, so match them to the current state
    private void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (visible) return;

        foreach (ARPlane plane in args.added)
            plane.gameObject.SetActive(false);
    }
}
