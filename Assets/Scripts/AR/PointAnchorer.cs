using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine;

/// <summary>
/// Creates and releases AR anchors for confirmed measurement points.
/// Anchored points follow ARCore's tracking corrections instead of drifting
/// away from the real-world spot where they were placed.
/// </summary>
public class PointAnchorer
{
    private readonly ARAnchorManager anchorManager;
    private readonly List<ARAnchor> anchors = new();

    public PointAnchorer(ARAnchorManager anchorManager)
    {
        this.anchorManager = anchorManager;
    }

    /// <summary>
    /// Attaches an anchor to the plane that was hit and returns its transform.
    /// Returns null when anchoring isn't possible, so the caller can keep the point in world space.
    /// </summary>
    public Transform Attach(ARRaycastHit hit)
    {
        if (!CanAttach(hit, out ARPlane plane)) return null;

        ARAnchor anchor = anchorManager.AttachAnchor(plane, hit.pose);
        if (anchor == null) return null;

        anchors.Add(anchor);
        return anchor.transform;
    }

    /// <summary>
    /// Removes every anchor created by this instance that still exists.
    /// </summary>
    public void ReleaseAll()
    {
        foreach (ARAnchor anchor in anchors)
        {
            if (anchor != null)
                anchorManager.TryRemoveAnchor(anchor);
        }

        anchors.Clear();
    }

    // The manager disables itself on devices without anchor support,
    // and not every provider supports attaching anchors to planes
    private bool CanAttach(ARRaycastHit hit, out ARPlane plane)
    {
        plane = hit.trackable as ARPlane;

        return plane != null
            && anchorManager.enabled
            && anchorManager.descriptor is { supportsTrackableAttachments: true };
    }
}
