using UnityEngine;

public class CamOfMarkers : MonoBehaviour
{
    public Transform player;

    public Vector3 offset = new Vector3(-1f, 1f, 1f);

    private float xRotation = 0f;
    private float yRotation = 0f;

    // private bool cursorLocked = false;

    void Start()
    {
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        transform.localRotation = transform.localRotation;
        xRotation = transform.localEulerAngles.x;
        yRotation = transform.localEulerAngles.y;

    }

    void Update()
    {
        if (!GameManager.instance.GameStart) return;

        // if (!cursorLocked)
        // {
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        //     cursorLocked = true;
        // }

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        transform.position = player.position + offset;
    }

    public void SetCameraRotation(float pitch, float yaw)
    {
        xRotation = pitch;
        yRotation = yaw;
    }
}

// using UnityEngine;

// public class CamOfMarkers : MonoBehaviour
// {
//     public Transform player;
//     // Reference to the player transform

//     public Vector3 offset = new Vector3(-1f, 1f, 1f);
//     // Camera offset relative to the player position


//     private float xRotation = 0f;
//     // Pitch: up/down rotation of the camera (around X axis)

//     private float yRotation = 0f;
//     // Yaw: left/right rotation of the camera (around Y axis)

//     private bool cursorLocked = false;

//     void Start()
//     {
//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;
//     }

//     void Update()
//     {

//         // Do not update camera movement before the game starts
//         if (!GameManager.instance.GameStart) return;

//         // Lock cursor once when game starts
//         if (!cursorLocked)
//         {
//             Cursor.lockState = CursorLockMode.Locked;
//             Cursor.visible = false;
//             cursorLocked = true;
//         }

//         // xRotation = 20f;
//         // yRotation = 45f;
//         yRotation += 30f * Time.deltaTime;

//         // ----- Pitch (look up / down) -----
//         xRotation = Mathf.Clamp(xRotation, -90f, 90f);

//         // Apply combined rotation to the camera
//         transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

//         // Update camera position using player position + offset
//         transform.position = player.position + offset;
//     }

//     public void SetCameraRotation(float pitch, float yaw)
//     {
//         xRotation = pitch;
//         yRotation = yaw;
//     }
// }

