using UnityEngine;
using TMPro;
using System;

public class RealTimeUI : MonoBehaviour
{
    public Transform player;
    public TMP_Text positionText;

    public CarSpawn carSpawner;
    public CarSpawn2 carSpawner2;

    public GameParameter gameParameter;
    public PlayerControl playerControl;
    public RoundControl roundControl;

    void Start()
    {
        if (positionText != null)
        {
            positionText.color = new Color32(10, 10, 10, 255);
        }
    }

    void Update()
    {
        if (player == null || positionText == null || gameParameter == null)
            return;

        Vector3 pos = player.position;
        float speed = playerControl.currentSpeed;
        long posixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int round = roundControl.maxRounds;


        string text =
            $"Player Position\nX: {pos.x:F2} Y: {pos.y:F2} Z: {pos.z:F2}\n";
        text += $"Player Speed: {speed:F2} m/s\n";
        text += $"POSIX Time: {posixTime}\n";
        text += $"Elapsed Time after Trigger: {playerControl.elapsedTime:F2} s\n";
        text += $"Total Round Number: {round}\n";

        Vector3 car1pos = Vector3.zero;
        Vector3 car2pos = Vector3.zero;

        switch (gameParameter.currentDirection)
        {
            case "Left":

                if (carSpawner != null && carSpawner.currentCar != null)
                {
                    car1pos = carSpawner.currentCar.transform.position;
                    text +=
                        $"\nLeft Car Position\nX: {car1pos.x:F2} Y: {car1pos.y:F2} Z: {car1pos.z:F2}";
                }
                break;

            case "Right":

                if (carSpawner2 != null && carSpawner2.currentCar != null)
                {
                    car2pos = carSpawner2.currentCar.transform.position;
                    text +=
                        $"\nRight Car Position\nX: {car2pos.x:F2} Y: {car2pos.y:F2} Z: {car2pos.z:F2}";
                }
                break;

            case "Dual":
            case "Random":

                if (carSpawner != null && carSpawner.currentCar != null)
                {
                    car1pos = carSpawner.currentCar.transform.position;
                    text +=
                        $"\nLeft Car Position\nX: {car1pos.x:F2} Y: {car1pos.y:F2} Z: {car1pos.z:F2}";
                }

                if (carSpawner2 != null && carSpawner2.currentCar != null)
                {
                    car2pos = carSpawner2.currentCar.transform.position;
                    text +=
                        $"\nRight Car Position\nX: {car2pos.x:F2} Y: {car2pos.y:F2} Z: {car2pos.z:F2}";
                }
                break;
        }


        positionText.text = text;
    }
}

