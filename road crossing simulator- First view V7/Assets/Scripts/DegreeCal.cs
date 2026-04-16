using System.Net.Sockets;
using UnityEngine;
using System;

public class DegreeCal : MonoBehaviour
{
    // public Transform headFront;        // Front marker of the head
    // public Transform headBack;         // Back marker of the head

    // public UDPforDegree udpReceiver;

    public TCP tcp;

    public CamOfMarkers cameraController;  // Reference to the camera controller script

    public float pitch;
    public float yaw;
    public float roll;

    private Quaternion calibrationOffset = Quaternion.identity;
    private bool isCalibrated = false;
    public Quaternion worldReference = Quaternion.Euler(0f, 90f, 0f);

    // void Update()
    // {
    //     if (!GameManager.instance.GameStart) return;

    //     Vector3 forward = ((tcp.LFHD + tcp.RFHD) / 2f) - ((tcp.LBHD + tcp.RBHD) / 2f);
    //     Vector3 right = ((tcp.RFHD + tcp.RBHD) / 2f) - ((tcp.LFHD + tcp.LBHD) / 2f);
    //     Vector3 up = Vector3.Cross(right, forward);

    //     if (forward.magnitude < 0.001f || up.magnitude < 0.001f)
    //         return;

    //     Quaternion headRotation = Quaternion.LookRotation(forward, up);
    //     Vector3 euler = headRotation.eulerAngles;

    //     // pitch = euler.x + 30;
    //     // yaw = -euler.y + 360;
    //     // roll = euler.z;

    //     pitch = NormalizeAngle(euler.x);
    //     yaw = NormalizeAngle(euler.y);
    //     roll = NormalizeAngle(euler.z);

    //     // Set camera rotation: pitch = up/down, yaw = left/right
    //     cameraController.SetCameraRotation(pitch, yaw, roll);
    // }

    // float NormalizeAngle(float angle)
    // {
    //     if (angle > 180f) angle -= 360f;
    //     return angle;
    // }
    void Update()
    {
        if (!GameManager.instance.GameStart) return;

        // ===============================
        // 1. Build head coordinate system
        // ===============================
        Vector3 forward =
            ((tcp.LFHD + tcp.RFHD) / 2f) -
            ((tcp.LBHD + tcp.RBHD) / 2f);

        Vector3 right =
            ((tcp.RFHD + tcp.RBHD) / 2f) -
            ((tcp.LFHD + tcp.LBHD) / 2f);

        Vector3 up = Vector3.Cross(right, forward);

        if (forward.magnitude < 0.001f || up.magnitude < 0.001f)
            return;

        // ===============================
        // 2. Convert to Quaternion
        // ===============================
        Quaternion headRotation = Quaternion.LookRotation(forward, up);

        // ===============================
        // 3. Apply calibration
        // ===============================
        if (isCalibrated)
        {
            headRotation = calibrationOffset * headRotation;
        }

        // ===============================
        // 4. Convert to Euler 
        // ===============================
        Vector3 euler = headRotation.eulerAngles;

        pitch = NormalizeAngle(euler.x);
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        yaw = NormalizeAngle(euler.y);
        roll = NormalizeAngle(euler.z);

        // ===============================
        // 5. Send to camera
        // ===============================
        cameraController.SetCameraRotation(pitch, yaw, roll);

        // ===============================
        // 6. Calibration trigger
        // ===============================
        if (Input.GetKeyDown(KeyCode.C))
        {
            Calibrate();
            Debug.Log("Camera Calibrated");
        }
    }

    // ===============================
    // Calibration function
    // ===============================
    public void Calibrate()
    {
        Vector3 forward =
            ((tcp.LFHD + tcp.RFHD) / 2f) -
            ((tcp.LBHD + tcp.RBHD) / 2f);

        Vector3 right =
            ((tcp.RFHD + tcp.RBHD) / 2f) -
            ((tcp.LFHD + tcp.LBHD) / 2f);

        Vector3 up = Vector3.Cross(right, forward);

        Quaternion currentRotation = Quaternion.LookRotation(forward, up);

        calibrationOffset = worldReference * Quaternion.Inverse(currentRotation);

        isCalibrated = true;
    }

    // ===============================
    // Angle normalization
    // ===============================
    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

}

// using UnityEngine;

// public class HeadCamera6DOF : MonoBehaviour
// {
//     public TCPData tcp;                 // 你的 marker 数据
//     public Transform cameraTransform;   // 主摄像机

//     private Quaternion rotationOffset = Quaternion.identity;
//     private bool calibrated = false;

//     void Update()
//     {
//         if (!GameManager.instance.GameStart) return;

//         Vector3 forward = GetForward();
//         Vector3 right = GetRight();
//         Vector3 up = Vector3.Cross(right, forward);

//         if (forward.magnitude < 0.001f || up.magnitude < 0.001f)
//             return;

//         Quaternion headRot = Quaternion.LookRotation(forward, up);

//         if (calibrated)
//         {
//             headRot = rotationOffset * headRot;
//         }

//         cameraTransform.rotation = headRot;
//     }

//     // =========================
//     // CALIBRATION (按键触发)
//     // =========================
//     public void CalibrateRotation()
//     {
//         Vector3 forward = GetForward();
//         Vector3 right = GetRight();
//         Vector3 up = Vector3.Cross(right, forward);

//         if (forward.magnitude < 0.001f || up.magnitude < 0.001f)
//             return;

//         Quaternion headRot = Quaternion.LookRotation(forward, up);

//         // camera 已经对齐 world forward => identity
//         Quaternion targetRot = Quaternion.identity;

//         rotationOffset = targetRot * Quaternion.Inverse(headRot);

//         calibrated = true;
//     }

//     // =========================
//     // MARKER → HEAD AXES
//     // =========================
//     Vector3 GetForward()
//     {
//         return ((tcp.LFHD + tcp.RFHD) / 2f)
//              - ((tcp.LBHD + tcp.RBHD) / 2f);
//     }

//     Vector3 GetRight()
//     {
//         return ((tcp.RFHD + tcp.RBHD) / 2f)
//              - ((tcp.LFHD + tcp.LBHD) / 2f);
//     }
// }
