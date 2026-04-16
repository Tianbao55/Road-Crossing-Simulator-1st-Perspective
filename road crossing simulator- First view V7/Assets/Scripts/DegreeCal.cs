using System.Net.Sockets;
using UnityEngine;
using System;

public class DegreeCal : MonoBehaviour
{
    public TCP tcp;
    public CamOfMarkers cameraController;  // Reference to the camera controller script

    private Quaternion calibrationOffset = Quaternion.identity;
    private bool isCalibrated = false;
    public Vector3 worldReferenceEuler = new Vector3(0f, 90f, 0f);
    private Quaternion worldReference;

    void Update()
    {
        worldReference = Quaternion.Euler(worldReferenceEuler);
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
        // 4. Send to camera
        // ===============================
        cameraController.SetCameraRotation(headRotation);

        // ===============================
        // 5. Calibration trigger
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

}
