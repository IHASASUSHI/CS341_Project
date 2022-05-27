using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    Image timerBar;
    public float maxTime = 5f;
    float timeLeft;
    public GameOverScreen GameOverScreen;
    public GameObject board;

    public GameObject controller;
    public TMP_Text scoreText;
    public TMP_Text waveText;

    private int score;
    private float wave;

    // Start is called before the first frame update
    void Start()
    {
        timerBar = GetComponent<Image>();
        timeLeft = maxTime / 2;
        wave = 1f;
        score = 0;
        waveText.text = "Wave: " + wave;
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            if (!(timerBar.fillAmount < 1f))
            {
                //controller.GetComponent<BeFractioned>().WipeBoard();
                //controller.GetComponent<BeFractioned>().StartGame();
                timeLeft = maxTime / 2;
                wave += 1f;
                waveText.text = "Wave: " + wave;
            }
            //timeLeft -= Time.deltaTime * (1.01f * wave);
            timeLeft -= .002f + (0.00025f * (wave - 1f)); ;
            timerBar.fillAmount = timeLeft / maxTime;
        }
        else
        {
            board.SetActive(false);
            GameOverScreen.Setup();
        }
    }

    public void IncreaseScore(int increase)
    {
        score += increase;
        scoreText.text = "Score: " + score;
    }

    public void IncreaseTime(float increase)
    {
        timeLeft += increase + (0.1f * (wave - 1f));
    }
}
