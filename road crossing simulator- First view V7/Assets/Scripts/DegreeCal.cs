using System.Net.Sockets;
using UnityEngine;
using System;

public class DegreeCal : MonoBehaviour
{
    public Transform headFront;        // Front marker of the head
    public Transform headBack;         // Back marker of the head

    // public UDPforDegree udpReceiver;

    public TCP tcp;

    public CamOfMarkers cameraController;  // Reference to the camera controller script

    public float pitch;
    public float yaw;
    public float roll;

    void Update()
    {
        if (!GameManager.instance.GameStart) return;

        Vector3 forward = ((tcp.LFHD + tcp.RFHD) / 2f) - ((tcp.LBHD + tcp.RBHD) / 2f);
        Vector3 right = ((tcp.RFHD + tcp.RBHD) / 2f) - ((tcp.LFHD + tcp.LBHD) / 2f);
        Vector3 up = Vector3.Cross(right, forward);

        if (forward.magnitude < 0.001f || up.magnitude < 0.001f)
            return;

        Quaternion headRotation = Quaternion.LookRotation(forward, up);
        Vector3 euler = headRotation.eulerAngles;

        pitch = euler.x + 30;
        yaw = -euler.y + 360;
        roll = euler.z;

        Debug.Log($"pitch:{pitch}, Yaw:{yaw}, Roll:{roll}");

        // Set camera rotation: pitch = up/down, yaw = left/right
        cameraController.SetCameraRotation(pitch, yaw, roll);
    }
}

