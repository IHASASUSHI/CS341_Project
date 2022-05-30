using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject start;
    public GameObject board;
    public GameObject timer;
    public GameObject score;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton()
    {
        board.SetActive(true);
        score.SetActive(true);
        timer.SetActive(true);
        start.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
