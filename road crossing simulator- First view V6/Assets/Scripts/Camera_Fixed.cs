using UnityEngine;

public class Camera_Fixed : MonoBehaviour
{
    public Transform playerHead;
    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = playerHead.position + offset;
    }
}
