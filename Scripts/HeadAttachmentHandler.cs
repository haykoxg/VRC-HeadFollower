﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HeadAttachmentHandler : UdonSharpBehaviour
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

    private Vector3[] posOnlyOffsets;
    private Vector3[] posOffsets;
    private Quaternion[] rotOffsets;

    private Vector3[] smoothPosOffsets;
    private Quaternion[] smoothRotOffsets;
    private Vector3[] currentSmoothedPositions;
    private Quaternion[] currentSmoothedRotations;

    private Vector3[] rotOnlyPosOffsets;
    private Quaternion[] rotOnlyRotOffsets;

    private bool initialized = false;

    void Start()
    {
        if (!Networking.LocalPlayer.IsValid()) return;

        // Snap this object to the player's head
        VRCPlayerApi.TrackingData headData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        transform.SetPositionAndRotation(headData.position, headData.rotation);

        InitializeOffsets();
    }

    void InitializeOffsets()
    {
        VRCPlayerApi.TrackingData headData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 headPos = headData.position;
        Quaternion headRot = headData.rotation;

        // Position-only
        posOnlyOffsets = new Vector3[positionOnlyObjects.Length];
        for (int i = 0; i < positionOnlyObjects.Length; i++)
        {
            if (positionOnlyObjects[i] == null) continue;
            posOnlyOffsets[i] = positionOnlyObjects[i].position - headPos;
        }

        // Position + Rotation
        posOffsets = new Vector3[positionAndRotationObjects.Length];
        rotOffsets = new Quaternion[positionAndRotationObjects.Length];
        for (int i = 0; i < positionAndRotationObjects.Length; i++)
        {
            if (positionAndRotationObjects[i] == null) continue;
            posOffsets[i] = Quaternion.Inverse(headRot) * (positionAndRotationObjects[i].position - headPos);
            rotOffsets[i] = Quaternion.Inverse(headRot) * positionAndRotationObjects[i].rotation;
        }

        // Smoothed
        smoothPosOffsets = new Vector3[smoothedObjects.Length];
        smoothRotOffsets = new Quaternion[smoothedObjects.Length];
        currentSmoothedPositions = new Vector3[smoothedObjects.Length];
        currentSmoothedRotations = new Quaternion[smoothedObjects.Length];
        for (int i = 0; i < smoothedObjects.Length; i++)
        {
            if (smoothedObjects[i] == null) continue;
            smoothPosOffsets[i] = Quaternion.Inverse(headRot) * (smoothedObjects[i].position - headPos);
            smoothRotOffsets[i] = Quaternion.Inverse(headRot) * smoothedObjects[i].rotation;
            currentSmoothedPositions[i] = smoothedObjects[i].position;
            currentSmoothedRotations[i] = smoothedObjects[i].rotation;
        }

        // Rotation-only
        rotOnlyPosOffsets = new Vector3[rotationOnlyObjects.Length];
        rotOnlyRotOffsets = new Quaternion[rotationOnlyObjects.Length];
        for (int i = 0; i < rotationOnlyObjects.Length; i++)
        {
            if (rotationOnlyObjects[i] == null) continue;
            rotOnlyPosOffsets[i] = rotationOnlyObjects[i].position - headPos;
            rotOnlyRotOffsets[i] = Quaternion.Inverse(headRot) * rotationOnlyObjects[i].rotation;
        }

        initialized = true;
    }

    void LateUpdate()
    {
        if (!initialized || !Networking.LocalPlayer.IsValid()) return;

        VRCPlayerApi.TrackingData headData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 headPos = headData.position;
        Quaternion headRot = headData.rotation;

        // Position-only
        for (int i = 0; i < positionOnlyObjects.Length; i++)
        {
            if (positionOnlyObjects[i] == null) continue;
            positionOnlyObjects[i].position = headPos + posOnlyOffsets[i];
        }

        // Position + Rotation
        for (int i = 0; i < positionAndRotationObjects.Length; i++)
        {
            if (positionAndRotationObjects[i] == null) continue;
            positionAndRotationObjects[i].position = headPos + headRot * posOffsets[i];
            positionAndRotationObjects[i].rotation = headRot * rotOffsets[i];
        }

        // Smoothed
        for (int i = 0; i < smoothedObjects.Length; i++)
        {
            if (smoothedObjects[i] == null) continue;

            Vector3 targetPos = headPos + headRot * smoothPosOffsets[i];
            Quaternion targetRot = headRot * smoothRotOffsets[i];

            currentSmoothedPositions[i] = Vector3.Lerp(currentSmoothedPositions[i], targetPos, Time.deltaTime * smoothingSpeed);
            currentSmoothedRotations[i] = Quaternion.Slerp(currentSmoothedRotations[i], targetRot, Time.deltaTime * smoothingSpeed);

            smoothedObjects[i].position = currentSmoothedPositions[i];
            smoothedObjects[i].rotation = currentSmoothedRotations[i];
        }

        // Rotation-only
        for (int i = 0; i < rotationOnlyObjects.Length; i++)
        {
            if (rotationOnlyObjects[i] == null) continue;
            rotationOnlyObjects[i].position = headPos + rotOnlyPosOffsets[i];
            rotationOnlyObjects[i].rotation = headRot * rotOnlyRotOffsets[i];
        }
    }
}
