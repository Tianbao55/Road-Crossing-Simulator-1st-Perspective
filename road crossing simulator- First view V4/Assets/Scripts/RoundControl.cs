using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundControl : MonoBehaviour
{
    public int maxRounds = 2;
    private int currentRound = 1;
    public GameObject buildings;
    public GameObject endingPoint;

    public GameParameter gameParameter;
    public Vector3 initialPosition;

    void Start()
    {
        initialPosition = endingPoint.transform.position;
        currentRound = 1;
    }

    void Update()
    {
        maxRounds = gameParameter.RoundNum;
        endingPoint.transform.position = new Vector3(
            initialPosition.x + (maxRounds - 1) * 43.7f,
            initialPosition.y,
            initialPosition.z
            );
    }

    public void StartCreatingSegments()
    {
        currentRound = 1;
        StopAllCoroutines();
        StartCoroutine(CreateSegment());
    }
    private IEnumerator CreateSegment()
    {
        while (currentRound < maxRounds)
        {
            yield return new WaitForSeconds(5f); // Wait for 5 seconds before creating the next segment

            Instantiate(
                buildings,
                new Vector3(
                    buildings.transform.position.x + currentRound * 43.7f,
                    buildings.transform.position.y,
                    buildings.transform.position.z
                ),
                Quaternion.identity
            );

            currentRound++;
        }
    }
}
