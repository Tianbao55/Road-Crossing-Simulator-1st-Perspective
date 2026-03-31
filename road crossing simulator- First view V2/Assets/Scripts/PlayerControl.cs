using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.0f;
    public float minX = -1.3f; // Minimum X position
    public float maxX = 10f;  // Maximum X position

    public Animator animator;

    private bool canMove = true;

    public GameParameter gameParameter;
    public float spawnTriggerX = -300f;     // trigger point
    private bool hasTriggered = false;   // trigger once
    public float elapsedTime = 0f;
    private bool timerStarted = false;

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

        if (!hasTriggered && transform.position.x >= spawnTriggerX)
        {
            hasTriggered = true;
            timerStarted = true;

            if (gameParameter != null)
            {
                int indexToUse = gameParameter.directionDropdown != null
                                 ? gameParameter.directionDropdown.value
                                 : 0;
                switch (indexToUse)
                {
                    case 0:
                        gameParameter.currentDirection = "Left";
                        if (gameParameter.carSpawner != null)
                            gameParameter.carSpawner.StartSpawning();
                        break;
                    case 1:
                        gameParameter.currentDirection = "Right";
                        if (gameParameter.carSpawner2 != null)
                            gameParameter.carSpawner2.StartSpawning();
                        break;
                    case 2:
                        gameParameter.currentDirection = "Dual";
                        if (gameParameter.carSpawner != null)
                            gameParameter.carSpawner.StartSpawning();
                        if (gameParameter.carSpawner2 != null)
                            gameParameter.carSpawner2.StartSpawning();
                        break;
                    case 3:
                        gameParameter.currentDirection = "Random";
                        int rand = Random.Range(0, 3); // 0 = Left, 1 = Right, 2 = Both

                        if (rand == 0)
                        {
                            if (gameParameter.carSpawner != null)
                                gameParameter.carSpawner.StartSpawning();
                        }
                        else if (rand == 1)
                        {
                            if (gameParameter.carSpawner2 != null)
                                gameParameter.carSpawner2.StartSpawning();
                        }
                        else // rand == 2
                        {
                            if (gameParameter.carSpawner != null)
                                gameParameter.carSpawner.StartSpawning();
                            if (gameParameter.carSpawner2 != null)
                                gameParameter.carSpawner2.StartSpawning();
                        }
                        break;
                }

                gameParameter.OnDirectionChanged(indexToUse);
                Debug.Log("Triggered spawner with index " + indexToUse
                          + " at player x = " + transform.position.x);
            }


        }

        if (timerStarted)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canMove) return;

        if (other.CompareTag("Final"))
        {
            // Trigger the win condition in the GameManager
            GameManager.instance.StopGame(true, "win");
            Debug.Log("You Win!");
        }
        else if (other.CompareTag("Car"))
        {
            // Trigger the lose condition in the GameManager
            GameManager.instance.StopGame(false, "hit");
            Debug.Log("You Lose! Hit by right car.");
        }
        else if (other.CompareTag("CarLeft"))
        {
            // Trigger the lose condition in the GameManager
            GameManager.instance.StopGame(false, "hitl");
            Debug.Log("You Lose! Hit by left car.");
        }

        canMove = false;
        animator.SetBool("Walk", false);
    }
}
