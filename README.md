Code that can be used for Unity 2021.3 VR projects
==================================================

ActionBasedTeleportationProvider.cs
-----------------------------------
**Push forward on right thumbstick to target teleportation, then release to teleport to any object that has a TeleportationArea Component**

XRRig > Camera Offset > RightHand Controller
* disable XRRayInteractor

XRRig > Camera Offset > RightHand Controller / XR Ray Interactor
* Selection Configuration / Keep Selected: False
* Raycast Configuration / Hit Closest Only: True
* Raycast Configuration / Line Type: Projectile Curve

XRRig Add Component Teleportation Provider

XRRig Add Component Teleportation Provider (Action-Based)
* Add Teleportation Provider, Ray Visual and Ray Interactor
* Right Hand Teleportation Action
  * Reference: XRI Right Hand Teleport Mode Activate
    * in config, change to Action Type: Value/Vector2
    * in config remove Interactions

Teleport Start Event -> RightHand Controller / XRRayInteractor.enable = true

Teleport End Event -> RightHand Controller / XRRayInteractor.enable = false
