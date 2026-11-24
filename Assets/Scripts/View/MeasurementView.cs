using UnityEngine;

/// <summary>
/// Renders a measurement: endpoint markers, the connecting line and the 3D label.
/// Points are reused between measurements (activated/deactivated) instead of destroyed.
/// Confirmed points copy their anchor's pose every frame instead of being parented to it:
/// AR Foundation can destroy an anchor at any time (plane merged, tracking lost),
/// and a child point would be destroyed with it.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class MeasurementView : MonoBehaviour
{
    [SerializeField] private MeasurementSettings settings;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private MeasurementLabel labelPrefab;

    [Tooltip("Camera used to orient the label. Falls back to Camera.main if empty.")]
    [SerializeField] private Camera viewerCamera;

    private LineRenderer lineRenderer;
    private MeasurementLabel label;
    private readonly GameObject[] points = new GameObject[2];
    private readonly Transform[] anchors = new Transform[2];

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        if (viewerCamera == null)
            viewerCamera = Camera.main;

        label = Instantiate(labelPrefab);
        label.Initialize(viewerCamera.transform);
        label.Hide();
    }

    /// <summary>
    /// Instantiates the point the first time, then just moves and re-activates it.
    /// The surface rotation offset is always applied so the prefab aligns with the AR plane.
    /// </summary>
    public void PlacePoint(MeasurementPointId id, Pose pose)
    {
        Quaternion rotation = pose.rotation * Quaternion.Euler(settings.SurfaceRotationOffset);
        int index = (int)id;

        if (points[index] == null)
            points[index] = Instantiate(pointPrefab, pose.position, rotation);
        else
            points[index].transform.SetPositionAndRotation(pose.position, rotation);

        if (!points[index].activeSelf)
            points[index].SetActive(true);
    }

    /// <summary>
    /// Links a confirmed point to its anchor so it follows tracking corrections.
    /// A null anchor leaves the point in world space (anchoring unavailable).
    /// </summary>
    public void AttachPoint(MeasurementPointId id, Transform anchor)
    {
        anchors[(int)id] = anchor;
    }

    /// <summary>
    /// Moves anchored points to their anchors' current pose. Call once per frame.
    /// If an anchor was destroyed, its point simply stays at its last known position.
    /// </summary>
    public void SyncToAnchors()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null || anchors[i] == null) continue;

            Quaternion rotation = anchors[i].rotation * Quaternion.Euler(settings.SurfaceRotationOffset);
            points[i].transform.SetPositionAndRotation(anchors[i].position, rotation);
        }
    }

    public Vector3 GetPointPosition(MeasurementPointId id) => points[(int)id].transform.position;

    public void ShowMeasurement(Vector3 posA, Vector3 posB, Vector3 planeNormal, string text)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, posA);
        lineRenderer.SetPosition(1, posB);

        label.Show(posA, posB, planeNormal, text);
    }

    /// <summary>
    /// Hides everything without destroying it and forgets the anchors. Safe to call at any phase.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < points.Length; i++)
        {
            anchors[i] = null;

            if (points[i] != null)
                points[i].SetActive(false);
        }

        lineRenderer.positionCount = 0;
        label.Hide();
    }
}
