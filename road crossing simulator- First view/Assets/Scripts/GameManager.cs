using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
     public static GameManager instance;
     public bool GameStart = false;
     public GameObject StartUI;
     public GameObject EndUI;
     public Text text;
     public CarSpawn carSpawner;

     void Update()
     {
          if (StartUI.activeSelf)
          {
               if (Input.GetKeyDown(KeyCode.Alpha1))
               {
                    StartGame();
               }
               else if (Input.GetKeyDown(KeyCode.Alpha3))
               {
                    QuitGame();
               }
          }
          else if (EndUI.activeSelf)
          {
               if (Input.GetKeyDown(KeyCode.Alpha2))
               {
                    RestartGame();
               }
               else if (Input.GetKeyDown(KeyCode.Alpha3))
               {
                    QuitGame();
               }
               else
               {
                    return;
               }
          }
     }
     private void Awake()
     {
          if (instance == null)
          {
               instance = this;
          }
          StartUI.SetActive(true);
          EndUI.SetActive(false);
          if (carSpawner != null)
               carSpawner.StopSpawning();
     }

     public void StartGame()
     {
          GameStart = true;
          StartUI.SetActive(false);

          if (carSpawner != null)
               carSpawner.StartSpawning();
     }

     public void RestartGame()
     {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
     }

     public void StopGame(bool isWin)
     {
          GameStart = false;
          if (carSpawner != null)
               carSpawner.StopSpawning();

          if (isWin)
          {
               text.text = "You Win!";
          }
          else
          {
               text.text = "You Lose!";
          }
          EndUI.SetActive(true);
     }

     public void QuitGame()
     {
          Application.Quit();
     }
}
