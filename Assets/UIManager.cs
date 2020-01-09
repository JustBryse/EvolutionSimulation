using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text roundText;
    public Text togglePauseText;

    private SimulationManager sm;

    void Start()
    {
        sm = SimulationManager.FindObjectOfType<SimulationManager>();
        Time.timeScale = sm.simSpeed;
    }

    public void togglePause()
    {
        if (Time.timeScale == 0f) // if paused
        {
            togglePauseText.text = "pause";
            Time.timeScale = sm.simSpeed; // resume
        }
        else if (Time.timeScale == sm.simSpeed) // if resumed
        {
            togglePauseText.text = "resume";
            Time.timeScale = 0f; // pause
        }
    }

    public void exit()
    {
        Application.Quit();
    }
    
    public void setRoundText(string text)
    {
        roundText.text = text;
    }
}
