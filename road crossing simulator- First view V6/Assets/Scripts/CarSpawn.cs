using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;   // Left lane car prefab
    public GameObject prefabR;  // Right lane car prefab

    [Header("Left Lane Settings")]
    public Transform pointAL;   // Left lane spawn area start
    public Transform pointBL;   // Left lane spawn area end
    public GameObject currentCarL = null; // Reference to currently spawned left lane car

    [Header("Right Lane Settings")]
    public Transform pointR;    // Right lane fixed spawn point
    public GameObject currentCarR = null; // Reference to currently spawned right lane car
    public Vector3 moveDirection = Vector3.forward; // Forward direction for right lane car

    // Public method to spawn a left lane car
    public void StartSpawning()
    {
        SpawnLeftCar();
    }

    // Spawn left lane car at random position between pointAL and pointBL
    void SpawnLeftCar()
    {
        Vector3 randomPos = GetRandomPositionBetween(pointAL.position, pointBL.position);
        currentCarL = Instantiate(prefab, randomPos, Quaternion.identity);
    }

    // Returns a random position within two points
    Vector3 GetRandomPositionBetween(Vector3 a, Vector3 b)
    {
        return new Vector3(
            Random.Range(Mathf.Min(a.x, b.x), Mathf.Max(a.x, b.x)),
            Random.Range(Mathf.Min(a.y, b.y), Mathf.Max(a.y, b.y)),
            Random.Range(Mathf.Min(a.z, b.z), Mathf.Max(a.z, b.z))
        );
    }

    // Public method to spawn a right lane car
    public void StartSpawningR()
    {
        SpawnRightCar();
    }

    // Spawn right lane car at fixed point, facing moveDirection
    void SpawnRightCar()
    {
        if (prefabR == null || pointR == null) return;

        Quaternion spawnRot = Quaternion.LookRotation(-moveDirection);
        currentCarR = Instantiate(prefabR, pointR.position, spawnRot);
    }
}