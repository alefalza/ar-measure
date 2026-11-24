# AR Measure

An Android augmented reality app for measuring real-world distances, inspired by Apple's Measure app. Built with **Unity 6** and **AR Foundation / ARCore**.

> 🚧 **Status:** functional base. Two-point distance measurement works end to end; area measurement and Play Store readiness are on the [roadmap](#roadmap).

<!-- Add a short GIF or screenshots here, e.g.:
<p align="center"><img src="Docs/demo.gif" width="300"></p>
-->

## Features

- **Screen-center reticle + "+" button.** You aim with the center of the screen and confirm with a button, so your finger never covers the point being measured.
- **Live preview.** The pending point and the line follow the reticle in real time.
- **Anchored points.** Confirmed points are backed by AR anchors attached to the detected plane, so they stay on the real-world spot as ARCore refines its tracking.
- **Adaptive 3D label.** The label lies flat on the surface when the line runs along it, and stands upright facing the camera when it doesn't (ceiling to floor, wall to wall). It always reads left to right.
- **Surface feedback.** The reticle dims, the "+" button disables and the HUD gives coaching text when there's no surface under the reticle.
- **Plane visibility.** Detected planes are shown while you look for a starting point and hidden while you measure, so they don't clutter the view.
- **Metric, imperial or both**, with configurable precision.
- **Haptic tick** when a point is added. It uses Android's native haptic feedback, so it respects the user's system settings.

## Architecture

The code is split into small layers with one responsibility each. The controller orchestrates them and contains no rendering, formatting or input logic of its own.

```
Assets/Scripts
├── ARMeasurementController.cs   Orchestrator: aim → hit → anchor → state → view
├── Core/                        Pure C#, no Unity scene dependencies (unit-testable)
│   ├── MeasurementSession.cs    State machine (PlacingA → PlacingB → Locked)
│   ├── MeasurementEnums.cs
│   └── UnitFormatter.cs         Meters → display text, cached per displayed value
├── AR/
│   ├── SurfaceHitProvider.cs    Raycasts against detected planes
│   ├── PointAnchorer.cs         Creates and releases plane-attached anchors
│   └── PlaneVisibility.cs       Shows or hides plane visuals by state
├── View/
│   ├── MeasurementView.cs       Points (pooled), line and label
│   ├── MeasurementLabel.cs      Flat or upright label orientation
│   └── LabelBackground.cs       Resizes the label background to the text
├── UI/
│   ├── MeasurementHud.cs        Instructions and "+" button state
│   └── ReticleView.cs           Screen-center crosshair
├── Platform/
│   └── Haptics.cs               Android haptic feedback via JNI
└── Settings/                    ScriptableObjects (all tunable data)
    ├── MeasurementSettings.cs
    ├── UnitSettings.cs
    └── MeasurementHudTexts.cs
```

### Design decisions

- **Settings as data.** Every tunable value (offsets, colors, thresholds, units, texts) lives in ScriptableObjects, not in code or scattered across components.
- **Pure C# core.** The state machine and the unit formatter have no scene dependencies, so they can be unit-tested and extended (undo, polygon/area mode) without touching the rendering code.
- **No per-frame allocations.** Distance text is rebuilt only when the displayed value changes, TMP only regenerates its mesh on real text changes, and the label background reacts to TMP's text-changed event instead of polling.
- **Anchors are followed, not parented.** AR Foundation can destroy an anchor at any time (for example, when planes merge). Points copy their anchor's pose every frame, so a removed anchor leaves the point at its last known position instead of destroying it.
- **Event-driven UI.** The HUD and plane visibility subscribe to session state changes, so they update only when something actually changes.

## Requirements

| | |
|---|---|
| Unity | 6000.3.12f1 |
| Render pipeline | URP |
| AR | AR Foundation 6.x + Google ARCore XR Plugin |
| Input | Input System package |
| UI animations | [iTween](https://assetstore.unity.com/packages/tools/animation/itween-84) (free, Asset Store) |
| Device | Android phone with [ARCore support](https://developers.google.com/ar/devices) |

## Getting started

1. Clone the repo and open it with Unity **6000.3.12f1**.
2. Import **iTween** from the Asset Store (*Package Manager → My Assets*). It's not included in the repo.
3. Open `Assets/Scenes/DevScene.unity`.
4. In *Build Profiles*, switch to **Android**. Then:
   - *XR Plug-in Management → Android*: enable **Google ARCore**.
   - *Player → Other Settings*: **IL2CPP**, **ARM64** only, Graphics API **OpenGLES3** or **Vulkan**.
   - *Player → Other Settings → Active Input Handling*: **Input System Package (New)**.
5. Build and run on a device.

You can also test in the Editor with **XR Simulation** (*XR Plug-in Management → Standalone*). Press `R` to reset the measurement.

## How to use

1. Move the phone slowly until the reticle becomes solid.
2. Aim at the start point and press **+**.
3. Aim at the end point and press **+**. The distance is shown on the line.
4. Press **+** again to start a new measurement, or **Reset** to clear.

## Roadmap

- [ ] AR session gate: ARCore availability and install, camera permission, tracking-lost coaching
- [ ] Area mode: polygon measurement with area and perimeter
- [ ] Snapping to existing points, with haptic feedback
- [ ] Multiple simultaneous measurements, undo and clear all
- [ ] In-app unit toggle (saved in preferences)
- [ ] Localization (EN / ES)
- [ ] Screenshot and share
- [ ] Play Store release: target API 36, 16 KB page size, privacy policy, store listing

## License

[MIT](LICENSE) © 2026 Alejandro Falzarano
