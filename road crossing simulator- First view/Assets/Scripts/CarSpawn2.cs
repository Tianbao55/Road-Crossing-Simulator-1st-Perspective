using UnityEngine;

public class CarSpawn2 : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;      // The car prefab to spawn
    public Transform spawnPoint;   // Fixed spawn point for the car
    public Vector3 moveDirection = Vector3.forward; // Direction the car will face and move

    [Header("Control")]
    public float spawnInterval = 2f;   // Time (in seconds) between each spawn
    public int maxCount = 10;          // Maximum number of cars in the scene

    private float timer;               // Timer to track spawn intervals
    private int currentCount = 0;      // Current number of cars spawned

    public bool canSpawn = true;       // Toggle spawning on/off

    void Update()
    {
        if (!canSpawn || currentCount >= maxCount) return;

        // Increment timer
        timer += Time.deltaTime;

        // Check if it's time to spawn a new car
        if (timer >= spawnInterval)
        {
            SpawnCar();
            timer = 0f; // Reset timer
        }
    }

    void SpawnCar()
    {
        if (prefab == null || spawnPoint == null) return;

        // --- 1. Set spawn position at the fixed point ---
        Vector3 spawnPos = spawnPoint.position;

        // --- 2. Set car rotation so it faces the movement direction ---
        Quaternion spawnRot = Quaternion.LookRotation(-moveDirection);

        // --- 3. Instantiate the car prefab at the position with the correct rotation ---
        Instantiate(prefab, spawnPos, spawnRot);

        // Increment current car count
        currentCount++;
    }

    // --- Methods to control spawning externally ---
    public void StartSpawning() => canSpawn = true;
    public void StopSpawning() => canSpawn = false;
}
