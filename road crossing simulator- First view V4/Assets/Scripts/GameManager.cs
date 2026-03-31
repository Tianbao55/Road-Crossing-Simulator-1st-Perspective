using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class GameManager : MonoBehaviour
{
     public static GameManager instance;
     public bool GameStart = false;
     public GameObject StartUI;
     public GameObject ParaUI;
     public GameObject EndUI;
     public Text text;
     public CarSpawn carSpawner;
     public CarSpawn2 carSpawner2;
     public CarMove car;
     public GameParameter gameParameter;
     public UDPforDegree udpReceiver;
     public TCP tcp;
     public RoundControl roundControl;


     UdpClient udpClient;

     public string remoteIP = "192.168.1.50"; // Received computer IP
     public int remotePort = 5005;             // Target port


     private void Awake()
     {
          if (instance == null)
          {
               instance = this;
          }

          udpClient = new UdpClient();

          StartUI.SetActive(true);
          EndUI.SetActive(false);
          ParaUI.SetActive(false);
          car.moveSpeed = 10f;
     }

     public void StartGame()
     {
          GameStart = true;
          StartUI.SetActive(false);
          ParaUI.SetActive(false);
          EndUI.SetActive(false);

          if (udpReceiver != null)
               udpReceiver.OpenUDP();
          if (tcp != null)
               tcp.OpenTCP();

          if (roundControl != null)
               roundControl.StartCreatingSegments();
     }

     public void RestartGame()
     {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
     }

     public void ShowParaUI()
     {
          StartUI.SetActive(false);
          ParaUI.SetActive(true);
     }

     public void BackToStartUI()
     {
          ParaUI.SetActive(false);
          StartUI.SetActive(true);
     }

     public void StopGame(bool isWin, string reason)
     {
          GameStart = false;
          Cursor.lockState = CursorLockMode.None;
          Cursor.visible = true;

          if (udpReceiver != null)
               udpReceiver.CloseUDP();
          if (tcp != null)
               tcp.CloseTCP();

          if (isWin)
          {
               text.text = "You Win!";
          }
          else
          {
               text.text = "You Lose!";
               // Send UDP signal
               SendUDP(reason, car.moveSpeed / 100f);
          }
          EndUI.SetActive(true);
     }

     public void QuitGame()
     {
          Application.Quit();
     }

     public void SendUDP(string command, float value)
     {
          string message = value.ToString() + "," + command;

          byte[] data = Encoding.UTF8.GetBytes(message);
          udpClient.Send(data, data.Length, remoteIP, remotePort);

          Debug.Log("Message sent: " + message);
     }


}
