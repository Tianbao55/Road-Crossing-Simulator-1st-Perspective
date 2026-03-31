using UnityEngine;
using System.Collections;

/// <summary>
/// RoundControl manages game rounds, building segments, trigger points, car spawning, and timers.
/// It communicates with PlayerControl and GameParameter to handle player progression and obstacle spawning.
/// </summary>
public class RoundControl : MonoBehaviour
{
    [Header("Round Settings")]
    public int maxRounds = 2;             // Total rounds for the game
    private int currentRound = 1;         // Current round being created

    [Header("Building Segments")]
    public GameObject[] buildings;          // Prefab for building segment
    public GameObject endingPoint;        // Endpoint for current round
    public Vector3 initialPosition;       // Initial position of endingPoint

    [Header("Trigger & Spawning")]
    public CarSpawn carSpawner;           // Reference to CarSpawn component
    public GameParameter gameParameter;   // Reference to GameParameter component
    public float baseTriggerX = -300f;   // X position for the first trigger
    public float segmentLength = 64f;  // Distance between triggers
    private int currentTriggerIndex = 0;  // Tracks the next trigger to check
    private Vector3 pointAStartPos;      // Original position of left lane spawn point
    private Vector3 pointBStartPos;      // Original position of right lane spawn point
    private Vector3 pointRStartPos;      // Original position of center/right spawn point
    private float inimaxX;               // Store the original maxX of the player

    [Header("Timer")]
    public float elapsedTime = 0f;        // Elapsed game time since first trigger
    public bool timerStarted = false;     // Whether timer has started

    public static RoundControl instance;  // Singleton instance
    public PlayerControl playerControl;   // Reference to player
    private int buildingNum;                // Index for building prefab selection

    public Vector3 iniPosition;       // Initial position of the building
    public GameObject Cube;

    void Awake()
    {
        instance = this;

        // Store initial positions of car spawn points
        if (carSpawner != null)
        {
            pointAStartPos = carSpawner.pointAL.position;
            pointBStartPos = carSpawner.pointBL.position;
            pointRStartPos = carSpawner.pointR.position;
        }
    }

    void Start()
    {
        currentRound = 1;

        // Store initial ending point position
        initialPosition = endingPoint.transform.position;

        iniPosition = Cube.transform.position;

        // Store initial maxX from player
        inimaxX = playerControl.maxX;
    }

    void Update()
    {
        // Update max rounds dynamically from GameParameter
        maxRounds = gameParameter.RoundNum;

        // Update player's maximum X position according to the number of rounds
        playerControl.maxX = inimaxX + (maxRounds - 1) * segmentLength;

        // Move ending point further along X-axis depending on rounds
        endingPoint.transform.position = new Vector3(
            initialPosition.x + (maxRounds - 1) * segmentLength,
            initialPosition.y,
            initialPosition.z
        );

        // Increment timer if it has started
        if (timerStarted)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Start creating building segments for the rounds
    /// </summary>
    public void StartCreatingSegments()
    {
        currentRound = 1;
        StopAllCoroutines();               // Stop any previous building generation
        StartCoroutine(CreateSegment());   // Start creating segments
    }

    /// <summary>
    /// Coroutine to instantiate building segments at intervals
    /// </summary>
    private IEnumerator CreateSegment()
    {
        while (currentRound < maxRounds)
        {
            yield return new WaitForSeconds(5f); // Wait 5 seconds before creating the next segment
            buildingNum = Random.Range(0, buildings.Length); // Randomly select a building prefab
            // Instantiate a building segment at the new position
            // Instantiate(
            //     buildings[buildingNum],
            //     new Vector3(
            //         buildings[0].transform.position.x + currentRound * segmentLength,
            //         buildings[0].transform.position.y,
            //         buildings[0].transform.position.z
            //     ),
            //     Quaternion.identity
            // );
            Instantiate(
                buildings[buildingNum],
                new Vector3(
                    iniPosition.x + currentRound * segmentLength,
                    iniPosition.y,
                    iniPosition.z
                ),
                Quaternion.identity
            );

            currentRound++;
        }
    }

    /// <summary>
    /// Called by PlayerControl to check if a trigger has been crossed
    /// </summary>
    /// <param name="playerX">Player's current X position</param>
    public void CheckTriggers(float playerX)
    {
        if (currentTriggerIndex >= maxRounds) return;

        float triggerX = baseTriggerX + currentTriggerIndex * segmentLength;

        // If player crosses the trigger
        if (playerX >= triggerX)
        {
            // Move the car spawn points forward for the next segment
            carSpawner.pointAL.position = pointAStartPos + Vector3.right * currentTriggerIndex * segmentLength;
            carSpawner.pointBL.position = pointBStartPos + Vector3.right * currentTriggerIndex * segmentLength;
            carSpawner.pointR.position = pointRStartPos + Vector3.right * currentTriggerIndex * segmentLength;

            HandleTrigger(currentTriggerIndex);  // Handle car spawning logic
            currentTriggerIndex++;
        }

        // Start the timer after the first trigger
        if (!timerStarted)
            timerStarted = true;
    }

    /// <summary>
    /// Handles spawning cars based on the current direction selection in GameParameter
    /// </summary>
    /// <param name="index">Trigger index (unused in logic, only for debugging)</param>
    private void HandleTrigger(int index)
    {
        if (gameParameter == null || carSpawner == null) return;

        int indexToUse = gameParameter.directionDropdown != null
                         ? gameParameter.directionDropdown.value
                         : 0;

        switch (indexToUse)
        {
            case 0: // Left lane only
                gameParameter.currentDirection = "Left";
                carSpawner.StartSpawning();
                break;

            case 1: // Right lane only
                gameParameter.currentDirection = "Right";
                carSpawner.StartSpawningR();
                break;

            case 2: // Dual lane
                gameParameter.currentDirection = "Dual";
                carSpawner.StartSpawning();
                carSpawner.StartSpawningR();
                break;

            case 3: // Random
                gameParameter.currentDirection = "Random";
                int rand = Random.Range(0, 3);
                if (rand == 0) carSpawner.StartSpawning();
                else if (rand == 1) carSpawner.StartSpawningR();
                else { carSpawner.StartSpawning(); carSpawner.StartSpawningR(); }
                break;
        }

        // Notify GameParameter of the new direction
        gameParameter.OnDirectionChanged(indexToUse);

        Debug.Log($"Trigger #{index} handled at playerX = {baseTriggerX + index * segmentLength}");
    }

    /// <summary>
    /// Resets triggers, timer, and spawn points for a new game
    /// </summary>
    public void ResetTriggers()
    {
        currentTriggerIndex = 0;
        timerStarted = false;
        elapsedTime = 0f;

        // Reset car spawn points to original positions
        carSpawner.pointAL.position = pointAStartPos;
        carSpawner.pointBL.position = pointBStartPos;
        carSpawner.pointR.position = pointRStartPos;
    }
}

