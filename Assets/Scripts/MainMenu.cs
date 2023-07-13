using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject info;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnContinueButtonClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnNewGameButtonClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnSettingsButtonClick()
    {
        Debug.Log("Settings");
        info.GetComponent<Text>().text = "settings";
    }

    public void OnBiblButtonClick()
    {
        Debug.Log("Bibl");
    }

    public void OnExitButtonClick()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
