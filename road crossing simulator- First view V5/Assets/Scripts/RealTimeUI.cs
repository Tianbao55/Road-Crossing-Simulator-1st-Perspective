using UnityEngine;
using TMPro;
using System;
using System.IO;

/// <summary>
/// RealTimeUI displays the player and car positions, speed, round info, and elapsed time in real-time on a UI text element.
/// </summary>
public class RealTimeUI : MonoBehaviour
{
    [Header("References")]
    public Transform player;                  // Player transform for position tracking
    public TMP_Text positionText;             // UI text element to display real-time info
    public CarSpawn carSpawner;               // Reference to car spawn manager
    public GameParameter gameParameter;       // Reference to game parameter manager
    public PlayerControl playerControl;       // Reference to player control script
    public RoundControl roundControl;         // Reference to round control manager

    private StreamWriter writer;
    private string filePath;

    void Start()
    {
        // Initialize text color (dark grey)
        if (positionText != null)
        {
            positionText.color = new Color32(10, 10, 10, 255);
        }
        string folder = Application.dataPath + "/DataLogs/";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        filePath = folder + "GameData_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

        writer = new StreamWriter(filePath);
        writer.WriteLine("Time,PlayerX,PlayerY,PlayerZ,Speed,Round,ElapsedTime,CarLX,CarLY,CarLZ,CarRX,CarRY,CarRZ");
    }

    void Update()
    {
        // Safety checks: ensure references are assigned
        if (player == null || positionText == null || gameParameter == null)
            return;

        // Get player position
        Vector3 pos = player.position;

        // Get player speed (from PlayerControl)
        float speed = playerControl.currentSpeed;

        // Get current time in POSIX format (seconds since Jan 1, 1970 UTC)
        long posixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Get total rounds from RoundControl
        int round = roundControl.maxRounds;

        // Build the main text string
        string text =
            $"Player Position\nX: {pos.x:F2} Y: {pos.y:F2} Z: {pos.z:F2}\n";
        text += $"Player Speed: {speed:F2} m/s\n";
        text += $"POSIX Time: {posixTime}\n";
        text += $"Elapsed Time after Trigger: {roundControl.elapsedTime:F2} s\n";
        text += $"Total Round Number: {round}\n";

        // Initialize car positions
        Vector3 car1pos = Vector3.zero;
        Vector3 car2pos = Vector3.zero;

        // Display car positions based on current direction
        switch (gameParameter.currentDirection)
        {
            case "Left":
                // Only left lane car is active
                if (carSpawner != null && carSpawner.currentCarL != null)
                {
                    car1pos = carSpawner.currentCarL.transform.position;
                    text +=
                        $"\nLeft Car Position\nX: {car1pos.x:F2} Y: {car1pos.y:F2} Z: {car1pos.z:F2}";
                }
                break;

            case "Right":
                // Only right lane car is active
                if (carSpawner != null && carSpawner.currentCarR != null)
                {
                    car2pos = carSpawner.currentCarR.transform.position;
                    text +=
                        $"\nRight Car Position\nX: {car2pos.x:F2} Y: {car2pos.y:F2} Z: {car2pos.z:F2}";
                }
                break;

            case "Dual":
            case "Random":
                // Both lanes may have cars
                if (carSpawner != null && carSpawner.currentCarL != null)
                {
                    car1pos = carSpawner.currentCarL.transform.position;
                    text +=
                        $"\nLeft Car Position\nX: {car1pos.x:F2} Y: {car1pos.y:F2} Z: {car1pos.z:F2}";
                }

                if (carSpawner != null && carSpawner.currentCarR != null)
                {
                    car2pos = carSpawner.currentCarR.transform.position;
                    text +=
                        $"\nRight Car Position\nX: {car2pos.x:F2} Y: {car2pos.y:F2} Z: {car2pos.z:F2}";
                }
                break;
        }

        // Update the UI text element
        positionText.text = text;

        string line =
        $"{posixTime}," +
        $"{pos.x},{pos.y},{pos.z}," +
        $"{speed}," +
        $"{round}," +
        $"{roundControl.elapsedTime}," +
        $"{car1pos.x},{car1pos.y},{car1pos.z}," +
        $"{car2pos.x},{car2pos.y},{car2pos.z}";

        writer.WriteLine(line);
    }

    void OnApplicationQuit()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
    }
}