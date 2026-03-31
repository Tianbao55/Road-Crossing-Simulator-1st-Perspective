using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float currentSpeed = 0f;
    public float minX = -330f; // Minimum X position
    private float maxX = -280f;  // Maximum X position
    private float inimaxX;

    public Animator animator;

    private bool canMove = true;

    public GameParameter gameParameter;
    public float baseTriggerX = -300f;
    public float segmentLength = 43.7f;
    public int maxTriggers = 10;
    private int currentTriggerIndex = 0;
    private float prevX;

    public float elapsedTime = 0f;
    public bool timerStarted = false;

    public TCP tcpReceiver;
    private float externalSpeed = 0f;

    public RoundControl roundControl;

    public CarSpawn carSpawner;
    private Vector3 pointAStartPos;
    private Vector3 pointBStartPos;

    public CarSpawn2 carSpawner2;
    private Vector3 pointStartPos;



    void Start()
    {
        // Get the Animator component attached to the character
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }


        // Initial animation state: Idle
        animator.SetBool("Walk", false);
        prevX = transform.position.x;

        pointAStartPos = carSpawner.pointA.transform.position;
        pointBStartPos = carSpawner.pointB.transform.position;
        pointStartPos = carSpawner2.spawnPoint.transform.position;

        inimaxX = maxX;
    }

    void Update()
    {
        maxX = inimaxX + (roundControl.maxRounds - 1) * 43.7f; // Extend maxX based on the number of rounds
        maxTriggers = roundControl.maxRounds;

        if (!canMove || !GameManager.instance.GameStart) return;

        // Update external speed from TCP
        if (tcpReceiver != null)
            externalSpeed = tcpReceiver.externalSpeed;
        // Use externalSpeed to replace moveSpeed
        currentSpeed = externalSpeed;

        // // Use Vertical axis (W/S) instead of Horizontal (A/D)
        // float inputZ = Input.GetAxisRaw("Vertical"); // +1 forward, -1 backward

        float inputZ = 1.0f;

        bool isMoving = Mathf.Abs(inputZ) > 0.01f;
        animator.SetBool("Walk", isMoving);

        if (isMoving)
        {
            // Always face forward (+X direction)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            // Use currentSpeed (external or default) instead of moveSpeed
            Vector3 movement = new Vector3(inputZ, 0, 0) * currentSpeed * Time.deltaTime;
            transform.position += movement;

            // Clamp X position
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }

        // ===== Trigger system (multi-trigger) =====
        if (currentTriggerIndex < maxTriggers)
        {
            float triggerX = baseTriggerX + currentTriggerIndex * segmentLength;

            if (prevX < triggerX && transform.position.x >= triggerX)
            {
                carSpawner.pointA.transform.position = new Vector3(
                    pointAStartPos.x + currentTriggerIndex * segmentLength,
                    pointAStartPos.y,
                    pointAStartPos.z
                );

                carSpawner.pointB.transform.position = new Vector3(
                    pointBStartPos.x + currentTriggerIndex * segmentLength,
                    pointBStartPos.y,
                    pointBStartPos.z
                );

                carSpawner2.spawnPoint.transform.position = new Vector3(
                    pointStartPos.x + currentTriggerIndex * segmentLength,
                    pointStartPos.y,
                    pointStartPos.z
                );

                HandleTrigger(currentTriggerIndex);
                currentTriggerIndex++;
            }
        }

        prevX = transform.position.x;

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

    public void HandleTrigger(int index)
    {
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
                    int rand = Random.Range(0, 3);

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
                    else
                    {
                        if (gameParameter.carSpawner != null)
                            gameParameter.carSpawner.StartSpawning();
                        if (gameParameter.carSpawner2 != null)
                            gameParameter.carSpawner2.StartSpawning();
                    }
                    break;
            }

            gameParameter.OnDirectionChanged(indexToUse);

            Debug.Log("Trigger #" + index +
                      " at player x = " + transform.position.x);
        }
    }
}
