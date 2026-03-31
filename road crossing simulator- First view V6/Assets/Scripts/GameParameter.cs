using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// GameParameter handles user input for vehicle speed, driving direction, and round number.
/// Updates the car and game settings based on UI values.
/// </summary>
public class GameParameter : MonoBehaviour
{
    [Header("Vehicle & UI References")]
    public CarMove car;                // Reference to the car being controlled
    public Dropdown directionDropdown; // Dropdown UI for direction selection
    public CarSpawn carSpawner;        // Reference to the car spawner

    [Header("Game Settings")]
    public string currentDirection;    // Selected driving direction
    public int RoundNum;               // Number of rounds for the game

    /// <summary>
    /// Update the car speed based on input string (km/h -> m/s conversion)
    /// </summary>
    /// <param name="value">String input from UI</param>
    public void PrintParameter(string value)
    {
        float newSpeed;
        if (float.TryParse(value, out newSpeed))
        {
            car.moveSpeed = newSpeed / 3.6f; // Convert km/h to m/s
            Debug.Log("Car speed updated to: " + car.moveSpeed + " m/s");
        }
        else
        {
            Debug.LogWarning("Invalid input! Please enter a number.");
        }
    }

    /// <summary>
    /// Update the currentDirection based on dropdown selection
    /// </summary>
    /// <param name="index">Dropdown index</param>
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
            default:
                currentDirection = "Left";
                Debug.LogWarning("Invalid dropdown index! Defaulting to Left.");
                break;
        }

        Debug.Log("Direction selected: " + currentDirection);
    }

    /// <summary>
    /// Set the number of rounds based on UI input
    /// </summary>
    /// <param name="value">String input from UI</param>
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
            Debug.LogWarning("Invalid round number! Please enter an integer.");
        }
    }
}