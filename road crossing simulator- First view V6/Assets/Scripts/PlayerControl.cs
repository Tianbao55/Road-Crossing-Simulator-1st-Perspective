using UnityEngine;

/// <summary>
/// Controls the player movement, animation, and collision detection.
/// Notifies RoundControl when triggers are crossed.
/// </summary>
public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float currentSpeed = 3f;       // Default movement speed (m/s)
    public float minX = -330f;            // Minimum X position (back boundary)
    public float maxX = -280f;            // Maximum X position (front boundary)
    private Animator animator;            // Reference to Animator component
    private bool canMove = true;          // Whether the player can move

    [Header("External Speed Input")]
    public TCP tcpReceiver;               // Optional external speed source (from TCP)

    public CarMove car;

    void Start()
    {
        // Get the Animator component if not assigned
        animator = animator ?? GetComponentInChildren<Animator>();

        // Initial animation state: not walking
        animator.SetBool("Walk", false);
    }

    void Update()
    {
        // Do nothing if the game hasn't started or movement is disabled
        if (!canMove || !GameManager.instance.GameStart) return;

        // Update speed from external TCP source if available
        if (tcpReceiver != null)
            currentSpeed = tcpReceiver.externalSpeed;

        // Move the player forward along the X-axis
        Vector3 movement = Vector3.right * currentSpeed * Time.deltaTime;
        transform.position += movement;

        // Clamp X position within minX and maxX boundaries
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Set walking animation
        animator.SetBool("Walk", true);

        // Notify RoundControl about the current player X position
        RoundControl.instance?.CheckTriggers(transform.position.x);
    }

    /// <summary>
    /// Detect collisions with finish line or cars
    /// </summary>
    /// <param name="other">Collider of the object collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!canMove) return;

        switch (other.tag)
        {
            case "Final":
                // Player reaches finish line → win
                GameManager.instance.StopGame(true, "win");
                break;

            case "Car":
                // Player hits right lane car → lose
                tcpReceiver.platform.posX = -car.moveSpeed / 20f; // Example of setting platform state on collision
                GameManager.instance.StopGame(false, "hit");
                break;

            case "CarLeft":
                // Player hits left lane car → lose
                tcpReceiver.platform.posX = car.moveSpeed / 20f; // Example of setting platform state on collision
                GameManager.instance.StopGame(false, "hitl");
                break;
        }

        // Stop movement and walking animation after collision
        canMove = false;
        animator.SetBool("Walk", false);
    }
}
