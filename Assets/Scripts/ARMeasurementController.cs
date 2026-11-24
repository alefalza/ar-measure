using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Orchestrates aiming, AR hits, anchoring, session state and view.
/// Points are aimed with the screen-center reticle and confirmed with the "+" button,
/// so the user's finger never covers the spot being measured.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
public class ARMeasurementController : MonoBehaviour
{
    [SerializeField] private MeasurementSettings measurementSettings;
    [SerializeField] private UnitSettings unitSettings;
    [SerializeField] private MeasurementView view;
    [SerializeField] private MeasurementHud hud;
    [SerializeField] private ReticleView reticle;
    [SerializeField] private PlaneVisibility planeVisibility;

    private readonly MeasurementSession session = new();
    private SurfaceHitProvider hitProvider;
    private PointAnchorer anchorer;
    private UnitFormatter formatter;

    private ARRaycastHit lastHit;
    private bool hasSurface;
    private Vector3 planeNormal = Vector3.up; // fallback until first hit

    private void Awake()
    {
        hitProvider = new SurfaceHitProvider(GetComponent<ARRaycastManager>(), measurementSettings.HitTrackableTypes);
        anchorer = new PointAnchorer(GetComponent<ARAnchorManager>());
        formatter = new UnitFormatter(unitSettings);
    }

    private void OnEnable()
    {
        session.StateChanged += hud.ShowState;
        session.StateChanged += planeVisibility.ShowForState;
    }

    private void OnDisable()
    {
        session.StateChanged -= hud.ShowState;
        session.StateChanged -= planeVisibility.ShowForState;
    }

    private void Start()
    {
        ResetMeasurement();
        SetSurfaceFound(false);
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            ResetMeasurement();
#endif

        view.SyncToAnchors();
        UpdateAim();

        // Anchors can shift as tracking improves, so a locked measurement is re-read every frame.
        // The formatter caches the text, so this only costs a distance and two line positions.
        if (session.State == MeasurementState.Locked)
            RefreshMeasurement();
    }

    /// <summary>
    /// Confirms and anchors the pending point at the reticle. Wired to the "+" button.
    /// Pressing it on a locked measurement starts a new one, using the reticle as Point A.
    /// </summary>
    public void AddPoint()
    {
        if (!hasSurface) return;

        if (!session.IsPlacing)
            ResetMeasurement();

        MeasurementPointId confirmedPoint = session.PendingPoint;
        PlacePendingPoint(lastHit.pose);
        view.AttachPoint(confirmedPoint, anchorer.Attach(lastHit));
        session.Advance();

        reticle.PlayConfirm();
        Haptics.Tick();
    }

    /// <summary>
    /// Clears the current measurement and starts over from Point A. Wired to the Reset button.
    /// </summary>
    public void ResetMeasurement()
    {
        view.Clear();
        anchorer.ReleaseAll();
        session.Reset();
    }

    /// <summary>
    /// Raycasts from the screen center every frame. The pending point follows the hit as a live preview;
    /// preview points are not anchored, since creating anchors every frame would be wasteful.
    /// </summary>
    private void UpdateAim()
    {
        // Read every frame instead of caching, so it stays correct if the screen rotates
        Vector2 screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f);
        bool found = hitProvider.TryGetHit(screenCenter, out ARRaycastHit hit);

        if (found != hasSurface)
            SetSurfaceFound(found);

        if (!found) return;

        lastHit = hit;

        if (session.IsPlacing)
            PlacePendingPoint(hit.pose);
    }

    private void SetSurfaceFound(bool found)
    {
        hasSurface = found;
        hud.SetSurfaceFound(found);
        reticle.SetSurfaceFound(found);
    }

    private void PlacePendingPoint(Pose pose)
    {
        planeNormal = pose.up;
        view.PlacePoint(session.PendingPoint, pose);

        if (session.PendingPoint == MeasurementPointId.B)
            RefreshMeasurement();
    }

    private void RefreshMeasurement()
    {
        Vector3 posA = view.GetPointPosition(MeasurementPointId.A);
        Vector3 posB = view.GetPointPosition(MeasurementPointId.B);
        string text = formatter.Format(Vector3.Distance(posA, posB));

        view.ShowMeasurement(posA, posB, planeNormal, text);
    }
}
