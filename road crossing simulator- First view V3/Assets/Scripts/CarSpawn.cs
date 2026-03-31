using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public Transform pointA;
    public Transform pointB;
    public GameObject currentCar = null;


    public void StartSpawning()
    {

        SpawnObject();
    }

    void SpawnObject()
    {
        Vector3 randomPos = GetRandomPositionBetween(pointA.position, pointB.position);
        currentCar = Instantiate(prefab, randomPos, Quaternion.identity);
    }

    Vector3 GetRandomPositionBetween(Vector3 a, Vector3 b)
    {
        return new Vector3(
            Random.Range(Mathf.Min(a.x, b.x), Mathf.Max(a.x, b.x)),
            Random.Range(Mathf.Min(a.y, b.y), Mathf.Max(a.y, b.y)),
            Random.Range(Mathf.Min(a.z, b.z), Mathf.Max(a.z, b.z))
        );
    }

}

