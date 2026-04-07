using System.Net.Sockets;
using UnityEngine;

public class DegreeCal : MonoBehaviour
{
    public Transform headFront;        // Front marker of the head
    public Transform headBack;         // Back marker of the head

    // public UDPforDegree udpReceiver;

    public TCP tcp;

    public CamOfMarkers cameraController;  // Reference to the camera controller script

    // void Update()
    // {
    //     if (!GameManager.instance.GameStart) return;

    //     Vector3 headFront = tcp.Head_Front_Pos;
    //     Vector3 headBack = tcp.Head_Back_Pos;

    //     if (headFront == Vector3.zero || headBack == Vector3.zero)
    //         return;

    //     Vector3 dir = headFront - headBack;

    //     // Calculate head direction vector (from back to front marker)
    //     // Vector3 dir = headFront.position - headBack.position;

    //     // ----- Yaw calculation (horizontal rotation) -----
    //     // Project onto horizontal plane to ignore vertical movement
    //     Vector3 dirHorizontal = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
    //     float yaw = Vector3.SignedAngle(Vector3.forward, dirHorizontal, Vector3.up);

    //     // ----- Pitch calculation (vertical rotation) -----
    //     // Angle between horizontal projection and full direction vector
    //     float pitch = Vector3.SignedAngle(dirHorizontal, dir, Vector3.right);

    //     // Set camera rotation: pitch = up/down, yaw = left/right
    //     cameraController.SetCameraRotation(pitch, yaw);
    // }

    // void Update()
    // {
    //     if (!GameManager.instance.GameStart) return;

    //     Vector3 forward = ((tcp.LFHD.position + tcp.RFHD.position) / 2f) - ((tcp.LBHD.position + tcp.RBHD.position) / 2f);
    //     Vector3 right = ((tcp.RFHD.position + tcp.RBHD.position) / 2f) - ((tcp.LFHD.position + tcp.LBHD.position) / 2f);
    //     Vector3 up = Vector3.Cross(forward, right);

    //     if (forward.magnitude < 0.001f || up.magnitude < 0.001f)
    //         return;

    //     Quaternion headRotation = Quaternion.LookRotation(forward, up);
    //     Vector3 euler = headRotation.eulerAngles;

    //     float pitch = euler.x;
    //     float yaw = euler.y;
    //     float roll = euler.z;

    //     // Set camera rotation: pitch = up/down, yaw = left/right
    //     cameraController.SetCameraRotation(pitch, yaw, roll);
    // }

    void Update()
    {
        if (!GameManager.instance.GameStart) return;

        Vector3 forward = ((tcp.LFHD + tcp.RFHD) / 2f) - ((tcp.LBHD + tcp.RBHD) / 2f);
        Vector3 right = ((tcp.RFHD + tcp.RBHD) / 2f) - ((tcp.LFHD + tcp.LBHD) / 2f);
        Vector3 up = Vector3.Cross(forward, right);

        if (forward.magnitude < 0.001f || up.magnitude < 0.001f)
            return;

        Quaternion headRotation = Quaternion.LookRotation(forward, up);
        Vector3 euler = headRotation.eulerAngles;

        float pitch = euler.x;
        float yaw = euler.y;
        float roll = euler.z;

        // Set camera rotation: pitch = up/down, yaw = left/right
        cameraController.SetCameraRotation(pitch, yaw, roll);
    }
}

