Code that can be used for Unity 2021.3 VR projects
==================================================

ActionBasedTeleportationProvider.cs
-----------------------------------
**Push forward on right thumbstick to target teleportation, then release to teleport to any object that has a TeleportationArea Component with the Interaction Layer Mask that matches the XR Ray Interator**

XRRig > Camera Offset > RightHand Controller
* disable XRRayInteractor

XRRig > Camera Offset > RightHand Controller / XR Ray Interactor
* Selection Configuration / Keep Selected: False
* Raycast Configuration / Hit Closest Only: True
* Raycast Configuration / Line Type: Projectile Curve
* Interaction Layer Mask = Teleport

XRRig Add Component Teleportation Provider (Action-Based)
* Add Ray Visual and Ray Interactor
* Teleport Actions
  * Reference: XRI Right Hand Teleport Mode Activate
    * in config, change to Action Type: Value/Vector2
    * in config, remove Interactions
    * for testing: in config add 2D Vector for IKJL

Teleport Start Event -> RightHand Controller / XRRayInteractor.enable = true

Teleport End Event -> RightHand Controller / XRRayInteractor.enable = false

Bugs:
* This will teleport to any object that is Interactable and an the Interaction Layer connected to the XR Ray Interactor


ActionBasedThrusterProvider.cs
------------------------------
**Hit the thruster button to fly upwards. It can be set to fly up when the controller is flat (normal) or in the direction of the controller (superman) or away from the controller (ironman).**

XRRig
* Add a RigidBody
 * Set Drag to 1
 * Constraints, Freeze all rotation
* Add a Collider

XRRig Add XRRig Add Component Thruster Provider (Action-Based)
* Thruster Action
  * Reference: XRI RightHand Interaction/Activate Value
    * for testing: in config add binding to Left Button [Mouse]
* Thruster Direction Action
  * Reference: XRI RightHand Rotation


Reformable.cs
-------------
**An attempt to replicate Geometry Nodes from blender. Requires ProBuilder 5.0.4.**


ChairReformable.cs
------------------
**Toy example of a Reformable**

Generates variations of a couch/chair based on [this blender tutorial](https://www.youtube.com/watch?v=WC4wZahtUwA)
Selecting the cube will generate a default couch, additional selections will randomize the values, some which 
