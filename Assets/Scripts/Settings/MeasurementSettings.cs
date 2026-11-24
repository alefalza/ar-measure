using UnityEngine;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Tunable values for AR hits, point placement and the 3D label.
/// Shared by the controller, the view and the label prefab.
/// </summary>
[CreateAssetMenu(fileName = "MeasurementSettings", menuName = "AR Measure/Measurement Settings")]
public class MeasurementSettings : ScriptableObject
{
    [Header("AR Hits")]
    [Tooltip("Trackable types accepted when raycasting against the real world.")]
    [SerializeField] private TrackableType hitTrackableTypes = TrackableType.PlaneWithinPolygon;

    [Header("Points")]
    [Tooltip("Rotation offset applied to point prefabs to align them with the detected AR surface. " +
             "Default (90, 0, 0) works for prefabs whose up-axis points in Z (ARCore convention).")]
    [SerializeField] private Vector3 surfaceRotationOffset = new(90f, 0f, 0f);

    [Header("Label")]
    [Tooltip("Offset perpendicular to the line and parallel to the surface, " +
             "so the label sits visually beside the LineRenderer without moving in depth.")]
    [SerializeField] private float labelSideOffset = 0.02f;

    [Tooltip("Max angle (degrees) between the line and the surface for the label to lie flat on it. " +
             "Above this (e.g. ceiling to floor), the label stands upright facing the camera.")]
    [SerializeField, Range(0f, 90f)] private float flatLabelMaxAngle = 20f;

    [Tooltip("Extra size added to the label background around the text bounds.")]
    [SerializeField] private Vector2 labelBackgroundPadding = new(0.02f, 0.01f);

    [Header("Reticle")]
    [Tooltip("Crosshair color while a surface is under the screen center.")]
    [SerializeField] private Color reticleFoundColor = Color.white;

    [Tooltip("Crosshair color while searching for a surface.")]
    [SerializeField] private Color reticleSearchingColor = new(1f, 1f, 1f, 0.35f);

    [Tooltip("Scale punch applied to the crosshair when a point is added.")]
    [SerializeField] private float reticlePunchAmount = 0.3f;
    [SerializeField] private float reticlePunchTime = 0.3f;

    public TrackableType HitTrackableTypes => hitTrackableTypes;
    public Vector3 SurfaceRotationOffset => surfaceRotationOffset;
    public float LabelSideOffset => labelSideOffset;
    public float FlatLabelMaxAngle => flatLabelMaxAngle;
    public Vector2 LabelBackgroundPadding => labelBackgroundPadding;
    public Color ReticleFoundColor => reticleFoundColor;
    public Color ReticleSearchingColor => reticleSearchingColor;
    public float ReticlePunchAmount => reticlePunchAmount;
    public float ReticlePunchTime => reticlePunchTime;
}
