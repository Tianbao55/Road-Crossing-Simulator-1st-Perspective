using UnityEngine;

public class CarMove : MonoBehaviour
{
    public float moveSpeed = 10f;

    public GameObject[] Cars;
    int CarIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed = Random.Range(moveSpeed - 3f, moveSpeed + 3f);
        CarIndex = Random.Range(0, Cars.Length);

        Cars[CarIndex].SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GameStart)
        {
            transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Untagged") || other.gameObject.CompareTag("Car"))
        {

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
