using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.0f;
    public float minX = -1.3f; // Minimum X position
    public float maxX = 10f;  // Maximum X position

    public Animator animator;

    private bool canMove = true;

    void Start()
    {
        // Get the Animator component attached to the character
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Initial animation state: Idle
        animator.SetBool("Walk", false);
    }

    void Update()
    {
        // if (!canMove || !GameManager.instance.GameStart) return;
        // // Read horizontal input (A/D or Left/Right arrow keys)
        // // GetAxisRaw is used to avoid input smoothing
        // float inputX = Input.GetAxisRaw("Horizontal"); // +1 or -1

        // // Check whether the character is moving
        // bool isMoving = Mathf.Abs(inputX) > 0.01f;

        // // Update animation state
        // animator.SetBool("Walk", isMoving);

        // if (isMoving)
        // {
        //     // Instantly rotate the character to face the movement direction
        //     if (inputX > 0)
        //     {
        //         // Face positive X direction
        //         transform.rotation = Quaternion.Euler(0, 0f, 0);
        //     }
        //     else if (inputX < 0)
        //     {
        //         // Face negative X direction
        //         transform.rotation = Quaternion.Euler(0, 180f, 0);
        //     }

        //     // Move the character along the X axis
        //     Vector3 movement = new Vector3(inputX, 0, 0) * moveSpeed * Time.deltaTime;
        //     transform.position += movement;

        //     // Clamp X position to limit left/right movement
        //     Vector3 pos = transform.position;
        //     pos.x = Mathf.Clamp(pos.x, minX, maxX);
        //     transform.position = pos;
        // }

        if (!canMove || !GameManager.instance.GameStart) return;

        // Use Vertical axis (W/S) instead of Horizontal (A/D)
        float inputZ = Input.GetAxisRaw("Vertical"); // +1 forward, -1 backward

        bool isMoving = Mathf.Abs(inputZ) > 0.01f;
        animator.SetBool("Walk", isMoving);

        if (isMoving)
        {
            // Always face forward (+X direction)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            // Move forward/backward along X axis
            Vector3 movement = new Vector3(inputZ, 0, 0) * moveSpeed * Time.deltaTime;
            transform.position += movement;

            // Clamp X position
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canMove) return;

        if (other.CompareTag("Final"))
        {
            // Trigger the win condition in the GameManager
            GameManager.instance.StopGame(true);
            Debug.Log("You Win!");
        }
        else if (other.CompareTag("Car"))
        {
            // Trigger the lose condition in the GameManager
            GameManager.instance.StopGame(false);
            Debug.Log("You Lose!");
        }

        canMove = false;
        animator.SetBool("Walk", false);
    }
}
