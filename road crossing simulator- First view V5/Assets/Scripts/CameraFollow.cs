using UnityEngine;

public class FirstPersonCameraXDir : MonoBehaviour
{
    // Reference to the player's transform
    // Used to follow the player's position
    public Transform player;

    // Offset position of the camera relative to the player
    // This determines where the camera sits around the player
    public Vector3 offset = new Vector3(-1f, 1f, 1f);

    // Mouse sensitivity for camera rotation
    public float mouseSensitivity = 50f;

    // Pitch rotation (looking up and down)
    // Rotation around the X axis
    private float xRotation = 0f;

    // Yaw rotation (looking left and right)
    // Rotation around the Y axis
    private float yRotation = 0f;

    // Flag to check if the cursor has been locked
    private bool cursorLocked = false;

    void Start()
    {
        // At the start of the game, keep the cursor visible and unlocked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Stop updating camera movement if the game has not started
        if (!GameManager.instance.GameStart) return;

        // Lock the cursor when the game starts (only once)
        if (!cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cursorLocked = true;
        }

        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ----- Pitch control (look up and down) -----
        // Subtract mouseY so moving the mouse up makes the camera look up
        xRotation -= mouseY;

        // Clamp the vertical rotation to prevent flipping
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // ----- Yaw control (look left and right) -----
        yRotation += mouseX;

        // Apply the combined rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Update camera position based on player position plus offset
        transform.position = player.position + offset;
    }
}