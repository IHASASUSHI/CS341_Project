using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{

    public GameObject startMenu;
    public GameObject gameBoard;
    public GameObject score;
    public GameObject wave;
    public GameObject timer;
    public GameObject timerBG;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        startMenu.SetActive(false);
        gameBoard.SetActive(true);
        score.SetActive(true);
        wave.SetActive(true);
        timer.SetActive(true);
        timerBG.SetActive(true);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
