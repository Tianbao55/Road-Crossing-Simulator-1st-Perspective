using UnityEngine;

public class CamOfMarkers : MonoBehaviour
{
    // Reference to the player's transform
    // Used to follow the player's position
    public Transform player;

    // Offset position of the camera relative to the player
    // Determines where the camera sits around the player
    public Vector3 offset = new Vector3(-1f, 1f, 1f);

    private Quaternion currentRotation;
    private Quaternion targetRotation;

    public float inputSmoothFactor = 0.1f; // Smoothing factor for rotation changes
    // Rotation smoothing speed
    public float rotationSmoothSpeed = 5f;

    void Start()
    {
        currentRotation = transform.localRotation;
        targetRotation = transform.localRotation;
    }

    void Update()
    {
        // Stop updating camera if the game has not started
        if (!GameManager.instance.GameStart) return;

        // Smooth rotation only
        currentRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            Time.deltaTime * rotationSmoothSpeed
        );

        transform.localRotation = currentRotation;

        // Update camera position so it follows the player
        transform.position = player.position + offset;
    }

    // Function used to update camera rotation externally
    // For example: data from motion capture, markers, or other tracking systems
    public void SetCameraRotation(Quaternion rotation)
    {
        targetRotation = rotation;
    }
}