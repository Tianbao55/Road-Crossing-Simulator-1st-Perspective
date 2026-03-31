using UnityEngine;

public class CarMove : MonoBehaviour
{
    // Base movement speed of the car
    public float moveSpeed = 10f;

    // Array containing different car models
    public GameObject[] Cars;

    // Index of the randomly selected car
    int CarIndex;

    // Called once when the object is first created
    void Start()
    {
        // // Randomize the movement speed to create variation between cars
        // moveSpeed = Random.Range(moveSpeed - 3f, moveSpeed + 3f);

        // Select a random car model from the array
        CarIndex = Random.Range(0, Cars.Length);

        // Activate the selected car model
        Cars[CarIndex].SetActive(true);
    }

    // Called once per frame
    void Update()
    {
        // Move the car only when the game has started
        if (GameManager.instance.GameStart)
        {
            // Move the car forward in the negative Z direction
            // Time.deltaTime ensures movement is frame-rate independent
            transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    // Trigger event when the car collides with another collider
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Untagged") && !other.CompareTag("Car"))
        {
            Destroy(gameObject);
        }
    }
}