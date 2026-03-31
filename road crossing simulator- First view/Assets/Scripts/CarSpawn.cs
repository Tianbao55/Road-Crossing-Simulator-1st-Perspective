using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;           // Object to spawn
    public Transform pointA;             // Start point
    public Transform pointB;             // End point

    [Header("Control")]
    public float spawnInterval = 2f;     // Time between spawns (seconds)
    public int maxCount = 10;             // Max objects in scene

    private float timer;
    private int currentCount = 0;

    public bool canSpawn = true;

    void Update()
    {
        if (!canSpawn) return;

        if (currentCount >= maxCount) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    void SpawnObject()
    {
        Vector3 randomPos = GetRandomPositionBetween(pointA.position, pointB.position);

        Instantiate(prefab, randomPos, Quaternion.identity);

        currentCount++;
    }

    Vector3 GetRandomPositionBetween(Vector3 a, Vector3 b)
    {
        return new Vector3(
            Random.Range(Mathf.Min(a.x, b.x), Mathf.Max(a.x, b.x)),
            Random.Range(Mathf.Min(a.y, b.y), Mathf.Max(a.y, b.y)),
            Random.Range(Mathf.Min(a.z, b.z), Mathf.Max(a.z, b.z))
        );
    }

    public void StartSpawning() => canSpawn = true;
    public void StopSpawning() => canSpawn = false;
}
