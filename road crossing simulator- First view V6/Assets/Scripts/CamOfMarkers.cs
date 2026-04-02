using UnityEngine;

public class CamOfMarkers : MonoBehaviour
{
    // Reference to the player's transform
    // Used to follow the player's position
    public Transform player;

    // Offset position of the camera relative to the player
    // Determines where the camera sits around the player
    public Vector3 offset = new Vector3(-1f, 1f, 1f);

    // Pitch rotation (up and down)
    private float xRotation = 0f;

    // Yaw rotation (left and right)
    private float yRotation = 0f;

    // Roll rotation (tilt)
    private float zRotation = 0f;

    void Start()
    {
        // Initialize rotation values using the current camera rotation
        // This ensures the camera keeps its starting orientation
        xRotation = transform.localEulerAngles.x;
        yRotation = transform.localEulerAngles.y;
        zRotation = transform.localEulerAngles.z;
    }

    void Update()
    {
        // Stop updating camera if the game has not started
        if (!GameManager.instance.GameStart) return;

        // Limit the vertical rotation (pitch) to prevent camera flipping
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);

        // Update camera position so it follows the player
        transform.position = player.position + offset;
    }

    // Function used to update camera rotation externally
    // For example: data from motion capture, markers, or other tracking systems
    public void SetCameraRotation(float pitch, float yaw, float roll = 0f)
    {
        xRotation = pitch;
        yRotation = yaw;
        zRotation = roll;
    }
}