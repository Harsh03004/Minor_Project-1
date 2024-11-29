using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuFunction : MonoBehaviour
{
    public GameObject menuObj;
    public GameObject loadText;
  //  public AudioSource buttonClick;
    public GameObject black;



    public void NewGameButton()
    {
        StartCoroutine(NewGameStart());
    }

    IEnumerator NewGameStart()
    {
       // buttonClick.Play();
        menuObj.SetActive(false);
        black.SetActive(true);
        yield return new WaitForSeconds(3);
        loadText.SetActive(true);
        Debug.Log("Load Scene 1");
        SceneManager.LoadScene(1);
    }


    public void QuitGame() 
    {
        Application.Quit();
    }

    public void SetQuality (int qualityIndex) 
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen (bool isFullscreen) 
    {
        Screen.fullScreen = isFullscreen;
    }
    
}
