using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Wraps ARRaycastManager to answer a single question: what real-world surface is under this screen point?
/// Isolated so fallbacks (estimated planes, depth) can be added without touching the controller.
/// </summary>
public class SurfaceHitProvider
{
    private readonly ARRaycastManager raycastManager;
    private readonly TrackableType trackableTypes;
    private readonly List<ARRaycastHit> hits = new();

    public SurfaceHitProvider(ARRaycastManager raycastManager, TrackableType trackableTypes)
    {
        this.raycastManager = raycastManager;
        this.trackableTypes = trackableTypes;
    }

    /// <summary>
    /// Returns the closest hit. The full hit (not just the pose) is exposed
    /// because anchoring needs the trackable that was hit.
    /// </summary>
    public bool TryGetHit(Vector2 screenPosition, out ARRaycastHit hit)
    {
        if (raycastManager.Raycast(screenPosition, hits, trackableTypes))
        {
            hit = hits[0];
            return true;
        }

        hit = default;
        return false;
    }
}
