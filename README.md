# Head Attachment Handler System – v1.1
	Author: Hayko_XG

Description
This system includes two UdonSharp scripts designed for attaching objects to a player's head in VRChat, with multiple behavior modes. 
You can choose between local-only tracking (only affects the current player) or global tracking (all players see head attachments on each other).
---------------------------------------------------------------------------------------------------------------------------------------------
The system supports:
- Smooth position and/or rotation
- Instant rotation tracking
- Fixed relative offsets
- Runtime head snapping
Each object’s behavior is determined by which array it's placed into.

Included Scripts
HeadAttachmentHandler.cs
Handles objects attached only for the local player.

GlobalHeadAttachmentHandler.cs
Handles objects in a networked/global way, so all players see attachments synced to everyone’s head.

Both scripts use the same array structure and logic, only the tracking scope changes.


INSTALLATION
---------------------------------------------------------------------------------------------------------------------------------------------
With Prefabs
1) Drag the prefab you want into your scene
2) Add whatever you would like in the respective GameObjects.
3) Adjust positions accordingly

Manual Installation (If you're weird ig)
1) Import the UnityPackage into your VRChat world Unity project.
2) Create an empty GameObject in the Hierarchy and add either HeadAttachmentHandler or GlobalHeadAttachmentHandler depending on what type of tracking you want. (You can find these in  the scripts folder)
3) Add child objects under this GameObject. 
4) Assign the child objects to the relevant array(s) in the script's Inspector.

IMPORTANT!! 
At runtime, the handler will snap to the player’s head and begin updating all assigned objects.
The positions of the children under the script GameObject will stay the same, only the object the script is attached to will be moved automatically.
---------------------------------------------------------------------------------------------------------------------------------------------
Array Behaviors:
Each array defines a different way of following the head. These work the same in both local and global scripts.

Position Only Objets
Objects in here will only move with the player, they will not be affected at all by rotation.
Reccomended uses are for Rain, Sfx, Fog or Screen FX

Position and Rotation Objects
Objects in this array will follow the players movement and head rotation
This is good for HUDs and particles firing from the players head.

Smoothed Objects
Objects in this array will function the exact same as the array above but will have smoothing applied. The speed of the smoothing can be adjusted at the bottom
You can use this for more styalised HUD choices.

Rotation Only Objects
Objects in here will not move with the players head, they will act like the first array, however they will only rotate arround itself, it will point in the same direction as the players head (Not pointing to where they are looking, just rotating the same)
Ill be honest, i dont know what uses this has so go nuts.
---------------------------------------------------------------------------------------------------------------------------------------------
Usage Tips
You can leave any array empty if unused. You can mix and match arrays on the same handler object.
The handler automatically finds and snaps to the head bone at start. 
Do not reposition the handler object manually in the scene — all positioning should be done relative to the handler. Reccomend you zero the object to the world.

Testing
To test in Unity:
- For Local
	You can enter play mode to see it work. Nothing else is needed really.
- For Global
	In the case of the Global script, use multiple test clients or a local multiplayer test to verify syncing.

Use crouching, head movement, or look direction to validate behavior.
---------------------------------------------------------------------------------------------------------------------------------------------
If anything is broken, contact me on discord @ [haykoxg]
---------------------------------------------------------------------------------------------------------------------------------------------
		Courtesy of Icurus Armories...
	 - NOT AFFILIATED WITH PROJECT WINGMAN-
