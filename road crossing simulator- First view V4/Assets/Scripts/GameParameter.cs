using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameParameter : MonoBehaviour
{
    public CarMove car;
    public Dropdown directionDropdown;
    public CarSpawn carSpawner;
    public CarSpawn2 carSpawner2;
    public string currentDirection = "Left";

    public int RoundNum;

    public void PrintParameter(string value)
    {
        float newSpeed;
        if (float.TryParse(value, out newSpeed))
        {
            car.moveSpeed = newSpeed / 3.6f; // Convert km/h to m/s
            Debug.Log("Car speed updated to: " + car.moveSpeed);
        }
        else
        {
            Debug.LogWarning("Invalid input! Please enter a number.");
        }
    }

    public void OnDirectionChanged(int index)
    {
        switch (index)
        {
            case 0:
                currentDirection = "Left";
                break;
            case 1:
                currentDirection = "Right";
                break;
            case 2:
                currentDirection = "Dual";
                break;
            case 3:
                currentDirection = "Random";
                break;
        }


        Debug.Log("Direction selected: " + currentDirection);
    }

    public void SetRoundNumber(string value)
    {
        int newRound;

        if (int.TryParse(value, out newRound))
        {
            RoundNum = newRound;
            Debug.Log("Round number set to: " + RoundNum);
        }
        else
        {
            Debug.LogWarning("Invalid round number!");
        }
    }

}
