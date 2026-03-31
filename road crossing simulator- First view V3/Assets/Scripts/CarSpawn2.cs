using UnityEngine;

public class CarSpawn2 : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public Transform spawnPoint;
    public Vector3 moveDirection = Vector3.forward;
    public GameObject currentCar = null;


    public void StartSpawning()
    {

        SpawnCar();
    }

    void SpawnCar()
    {
        if (prefab == null || spawnPoint == null) return;

        Quaternion spawnRot = Quaternion.LookRotation(-moveDirection);
        currentCar = Instantiate(prefab, spawnPoint.position, spawnRot);
    }

}
