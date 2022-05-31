using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public GameObject screen;
    public GameObject formula;
    public GameObject overlay;
    public Timer timer;
    bool repeat;
    // Start is called before the first frame update
    public void Setup()
    {
        /*if (repeat)
        {
            timer.IncreaseTime(timer.maxTime);
        }*/
        formula.SetActive(false);
        overlay.SetActive(false);
        gameObject.SetActive(true);
        //FindObjectOfType<AudioManager>().PlaySound("Game Over");

    }

    public void RestartButton()
    {
        timer.IncreaseTime(timer.maxTime);
        timer.reset();
        formula.SetActive(true);
        overlay.SetActive(true);
        screen.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("SampleScene");
    }
}