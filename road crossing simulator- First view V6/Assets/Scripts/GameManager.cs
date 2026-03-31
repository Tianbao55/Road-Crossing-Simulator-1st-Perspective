using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

/// <summary>
/// GameManager handles the overall game state, UI, vehicle control,
/// network communication (UDP/TCP), and round management.
/// </summary>
public class GameManager : MonoBehaviour
{
     // Singleton instance for global access
     public static GameManager instance;

     [Header("Game State")]
     public bool GameStart = false;

     [Header("UI Panels")]
     public GameObject StartUI;
     public GameObject ParaUI;
     public GameObject EndUI;
     public Text text; // Display win/lose message

     [Header("Game Objects")]
     public CarSpawn carSpawner;
     public CarMove car;
     public GameParameter gameParameter;
     // public UDPforDegree udpReceiver;
     public TCP tcp;
     public RoundControl roundControl;
     public RealTimeUI realTimeUI;

     // UDP client for sending messages
     private UdpClient udpClient;
     public string remoteIP = "192.168.1.50"; // Target computer IP
     public int remotePort = 5005;             // Target port

     private void Awake()
     {
          // Initialize singleton
          if (instance == null)
               instance = this;

          // Create UDP client
          udpClient = new UdpClient();

          // Initialize UI
          StartUI.SetActive(true);
          ParaUI.SetActive(false);
          EndUI.SetActive(false);

          // Initialize car speed and round count
          car.moveSpeed = 10f;
          gameParameter.currentDirection = "Left";
          gameParameter.RoundNum = 2;
     }

     /// <summary>
     /// Start the game: hide UI, open network, start rounds
     /// </summary>
     public void StartGame()
     {
          GameStart = true;

          StartUI.SetActive(false);
          ParaUI.SetActive(false);
          EndUI.SetActive(false);

          // Open UDP and TCP connections if available
          // udpReceiver?.OpenUDP();
          tcp?.OpenTCP();
          tcp.sendZeroToDFlow = true;

          // Start creating game rounds
          roundControl?.ResetTriggers();
          roundControl?.StartCreatingSegments();
          // Start real-time UI updates
          realTimeUI.isLogging = true;
     }

     /// <summary>
     /// Restart the current scene/game
     /// </summary>
     public void RestartGame()
     {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
     }

     /// <summary>
     /// Show parameter UI
     /// </summary>
     public void ShowParaUI()
     {
          StartUI.SetActive(false);
          ParaUI.SetActive(true);
     }

     /// <summary>
     /// Return to start UI
     /// </summary>
     public void BackToStartUI()
     {
          ParaUI.SetActive(false);
          StartUI.SetActive(true);
     }

     /// <summary>
     /// Stop the game and show win/lose UI
     /// </summary>
     /// <param name="isWin">True if player wins</param>
     /// <param name="reason">Reason for loss</param>
     public void StopGame(bool isWin, string reason)
     {
          GameStart = false;

          // Update UI
          if (isWin)
          {
               text.text = "You Win!";
          }
          else
          {
               text.text = "You Lose!";
               // Send UDP signal with failure reason
               // SendUDP(reason, car.moveSpeed / 100f);
          }

          EndUI.SetActive(true);
          roundControl.timerStarted = false; // Stop round timer

          Invoke("StopTCP", 0.1f);

          realTimeUI.isLogging = false;
     }

     /// <summary>
     /// Quit the application
     /// </summary>
     public void QuitGame()
     {
          Application.Quit();
     }

     void StopTCP()
     {
          tcp.sendZeroToDFlow = false;
          Thread.Sleep(50);
          tcp.CloseTCP();
     }

     // /// <summary>
     // /// Send a UDP message in format: "value,command"
     // /// </summary>
     // /// <param name="command">Command string</param>
     // /// <param name="value">Value (float)</param>
     // public void SendUDP(string command, float value)
     // {
     //      string message = value.ToString() + "," + command;
     //      byte[] data = Encoding.UTF8.GetBytes(message);

     //      udpClient.Send(data, data.Length, remoteIP, remotePort);

     //      Debug.Log("Message sent: " + message);
     // }
}