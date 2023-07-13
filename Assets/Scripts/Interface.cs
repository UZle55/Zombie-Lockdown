using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public GameObject dice;
    public GameObject pausePanel;
    public GameObject healthSlider;
    public GameObject infectionSlider;
    public GameObject map;
    public GameObject movesCount;
    public GameObject movesCountBackground;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movesCountBackground.GetComponent<Image>().color = new Color(0.937255f, 0.7803922f, 0.4039216f, movesCount.GetComponent<Text>().color.a);
    }

    public void OnPauseButtonClick()
    {
        pausePanel.SetActive(true);
    }

    public void OnContinueButtonClick()
    {
        pausePanel.SetActive(false);
    }

    public void OnPlayAgainButtonClick()
    {
        Debug.Log("again");
    }

    public void OnAudioButtonClick()
    {
        Debug.Log("audio");
    }

    public void OnBiblButtonClick()
    {
        Debug.Log("bibl");
    }

    public void OnExitButtonClick()
    {
        SceneManager.LoadScene("Menu");
        Manager.CanRollDice = true;
        Manager.PlayerMovesCount = 0;
        Manager.Finished();
    }

    public void OnFinishStepButtonClick()
    {
        if(Manager.PlayerState != Manager.PlayerGameState.Moving && Manager.PlayerState != Manager.PlayerGameState.Finishing)
        {
            Manager.StartFinishing();
            map.GetComponent<Map>().UnHighLightCells();
        }
    }

    public void SetInfection(int infection)
    {
        infectionSlider.GetComponent<Slider>().value = infection;
    }

    public void SetHealth(int health)
    {
        healthSlider.GetComponent<Slider>().value = health;
    }
}
