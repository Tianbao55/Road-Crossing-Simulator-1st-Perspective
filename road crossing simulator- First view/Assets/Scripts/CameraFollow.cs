using UnityEngine;

public class FirstPersonCameraXDir : MonoBehaviour
{
    public Transform player;
    // Reference to the player transform

    public Vector3 offset = new Vector3(-1f, 1f, 1f);
    // Camera offset relative to the player position

    public float mouseSensitivity = 50f;
    // Mouse look sensitivity

    private float xRotation = 0f;
    // Pitch: up/down rotation of the camera (around X axis)

    private float yRotation = 0f;
    // Yaw: left/right rotation of the camera (around Y axis)

    void Start()
    {
        // Lock the mouse cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Do not update camera movement before the game starts
        if (!GameManager.instance.GameStart) return;

        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ----- Pitch (look up / down) -----
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // ----- Yaw (look left / right) -----
        yRotation += mouseX;

        // Apply combined rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Update camera position using player position + offset
        transform.position = player.position + offset;
    }
}

