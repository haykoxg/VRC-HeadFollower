using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GlobalHeadAttachmentHandler : UdonSharpBehaviour
{
    [Tooltip("Objects that follow the player's HEAD position only. Good for rain or loading effects that should float in world space.")]
    public Transform[] positionOnlyObjects;

    [Tooltip("Objects that follow both position and rotation of the HEAD instantly. Use for things like camera-locked panels or HUDs.")]
    public Transform[] positionAndRotationObjects;

    [Tooltip("Objects that follow position and rotation with smoothing. Good for cinematic HUDs, floating indicators, etc.")]
    public Transform[] smoothedObjects;

    [Tooltip("Objects that rotate with the HEAD, but keep their position offset. Use for things like head-aimed indicators.")]
    public Transform[] rotationOnlyObjects;

    [Tooltip("Smoothing speed for smoothed objects. Higher = faster response.")]
    public float smoothingSpeed = 5f;

    private Quaternion targetHeadRot;
    private VRCPlayerApi localPlayer;
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
        localPlayer = Networking.LocalPlayer;

        if (localPlayer == null) return;

        Vector3 headPos = localPlayer.GetBonePosition(HumanBodyBones.Head);
        Quaternion headRot = localPlayer.GetBoneRotation(HumanBodyBones.Head);
        _transform.SetPositionAndRotation(headPos, headRot);
    }

    private void LateUpdate()
    {
        if (localPlayer == null) return;

        targetHeadRot = localPlayer.GetBoneRotation(HumanBodyBones.Head);

        // Position & Rotation with fixed smoothing
        foreach (var obj in positionAndRotationObjects)
        {
            obj.position = Vector3.Lerp(obj.position, _transform.position, 8f * Time.deltaTime);
            obj.rotation = Quaternion.Slerp(obj.rotation, targetHeadRot, 8f * Time.deltaTime);
        }

        // Position Only with fixed smoothing
        foreach (var obj in positionOnlyObjects)
        {
            obj.position = Vector3.Lerp(obj.position, _transform.position, 8f * Time.deltaTime);
        }

        // Smoothed Rotation Only (only affected by smoothingSpeed)
        foreach (var obj in smoothedObjects)
        {
            obj.rotation = Quaternion.Slerp(obj.rotation, targetHeadRot, smoothingSpeed * Time.deltaTime);
        }

        // Instant Rotation Only
        foreach (var obj in rotationOnlyObjects)
        {
            obj.rotation = targetHeadRot;
        }
    }
}
