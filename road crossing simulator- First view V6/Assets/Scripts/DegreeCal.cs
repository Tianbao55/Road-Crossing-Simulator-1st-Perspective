using System.Net.Sockets;
using UnityEngine;

public class DegreeCal : MonoBehaviour
{
    public Transform headFront;        // Front marker of the head
    public Transform headBack;         // Back marker of the head

    // public UDPforDegree udpReceiver;

    public TCP tcp;

    public CamOfMarkers cameraController;  // Reference to the camera controller script

    void Update()
    {
        if (!GameManager.instance.GameStart) return;

        Vector3 headFront = tcp.Head_Front_Pos;
        Vector3 headBack = tcp.Head_Back_Pos;

        if (headFront == Vector3.zero || headBack == Vector3.zero)
            return;

        Vector3 dir = headFront - headBack;

        // Calculate head direction vector (from back to front marker)
        // Vector3 dir = headFront.position - headBack.position;

        // ----- Yaw calculation (horizontal rotation) -----
        // Project onto horizontal plane to ignore vertical movement
        Vector3 dirHorizontal = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
        float yaw = Vector3.SignedAngle(Vector3.forward, dirHorizontal, Vector3.up);

        // ----- Pitch calculation (vertical rotation) -----
        // Angle between horizontal projection and full direction vector
        float pitch = Vector3.SignedAngle(dirHorizontal, dir, Vector3.right);

        // Set camera rotation: pitch = up/down, yaw = left/right
        cameraController.SetCameraRotation(pitch, yaw);
    }
}

// Version:2.0 (with baseline)
// using UnityEngine;

// public class DegreeCal : MonoBehaviour
// {
//     public Transform headFront;
//     public Transform headBack;

//     public CamOfMarkers cameraController;

//     private Vector3 baselineDir;
//     private bool calibrated = false;

//     // Update is called once per frame
//     void Update()
//     {
//         if (!GameManager.instance.GameStart) return;
//         Vector3 dir = headFront.position - headBack.position;
//         dir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;

//         if (Input.GetKeyDown(KeyCode.C))
//         {
//             baselineDir = dir;
//             calibrated = true;
//             Debug.Log("Calibration Done");
//         }

//         if (!calibrated) return;

//         float yaw = Vector3.SignedAngle(baselineDir, dir, Vector3.up);
//         cameraController.SetCameraRotation(0f, yaw);

//     }
// }
