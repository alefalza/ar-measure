using UnityEngine;
using TMPro;

/// <summary>
/// Lives on the label prefab. Places the label at the midpoint of the measurement line.
/// Lies flat on the surface when the line runs along it; otherwise (ceiling to floor,
/// wall to wall) stands upright facing the viewer, since there is no single plane to lie on.
/// Instantiated by MeasurementView at runtime.
/// </summary>
public class MeasurementLabel : MonoBehaviour
{
    private const float MIN_LINE_SQR_LENGTH = 0.0001f;

    [SerializeField] private TextMeshPro labelText;
    [SerializeField] private MeasurementSettings settings;

    private Transform viewer;
    private Vector3 posA;
    private Vector3 posB;
    private Vector3 planeNormal = Vector3.up;

    /// <summary>
    /// Injects the transform used to orient the label towards the user.
    /// </summary>
    public void Initialize(Transform viewerTransform)
    {
        viewer = viewerTransform;
    }

    /// <summary>
    /// Stores the line endpoints, updates the text and orients the label.
    /// </summary>
    public void Show(Vector3 pointA, Vector3 pointB, Vector3 surfaceNormal, string text)
    {
        posA = pointA;
        posB = pointB;
        planeNormal = surfaceNormal.normalized;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // Only assign when the value changes, so TMP doesn't rebuild its mesh every frame
        if (labelText.text != text)
            labelText.text = text;

        UpdatePose();
    }

    /// <summary>
    /// Hides the label without destroying it so it can be reused on the next measurement.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // The viewer keeps moving after the measurement is locked, so the orientation is refreshed every frame
    private void LateUpdate()
    {
        UpdatePose();
    }

    private void UpdatePose()
    {
        Vector3 lineDir = posB - posA;
        if (lineDir.sqrMagnitude < MIN_LINE_SQR_LENGTH) return;

        lineDir.Normalize();
        Vector3 midpoint = (posA + posB) * 0.5f;

        Vector3 sideDir;
        Quaternion rotation = LiesOnSurface(lineDir)
            ? ComputeFlatRotation(lineDir, out sideDir)
            : ComputeUprightRotation(lineDir, midpoint, out sideDir);

        transform.SetPositionAndRotation(midpoint + sideDir * settings.LabelSideOffset, rotation);
    }

    /// <summary>
    /// True when the angle between the line and the surface is within the flat-label threshold.
    /// </summary>
    private bool LiesOnSurface(Vector3 lineDir)
    {
        float maxSin = Mathf.Sin(settings.FlatLabelMaxAngle * Mathf.Deg2Rad);
        return Mathf.Abs(Vector3.Dot(lineDir, planeNormal)) <= maxSin;
    }

    /// <summary>
    /// Lies flat on the surface, parallel to the line, reading left-to-right from the viewer.
    /// Axis convention shared by both modes: forward = facing direction, up = reading direction.
    /// </summary>
    private Quaternion ComputeFlatRotation(Vector3 lineDir, out Vector3 sideDir)
    {
        Vector3 along = Vector3.ProjectOnPlane(lineDir, planeNormal).normalized;

        if (Vector3.Dot(along, viewer.right) < 0f)
            along = -along;

        // Perpendicular to the line but still on the surface, so the label doesn't move in depth
        sideDir = Vector3.Cross(along, planeNormal).normalized;
        return Quaternion.LookRotation(planeNormal, along);
    }

    /// <summary>
    /// Stands upright facing the viewer, with the text horizontal on screen,
    /// shifted to the viewer's right of the line.
    /// </summary>
    private Quaternion ComputeUprightRotation(Vector3 lineDir, Vector3 midpoint, out Vector3 sideDir)
    {
        Vector3 toViewer = (viewer.position - midpoint).normalized;

        sideDir = Vector3.Cross(lineDir, toViewer).normalized;
        if (Vector3.Dot(sideDir, viewer.right) < 0f)
            sideDir = -sideDir;

        return Quaternion.LookRotation(toViewer, viewer.right);
    }
}
