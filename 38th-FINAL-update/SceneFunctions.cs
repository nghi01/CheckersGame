using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFunctions : MonoBehaviour
{

  public GameObject QuitButton;
  public GameObject BackButton;
  public GameObject StartButton;

  public void QuitGame()
  {
    Application.Quit();
  }

  public void Play()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
  }

  public void Back()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
  }
}
