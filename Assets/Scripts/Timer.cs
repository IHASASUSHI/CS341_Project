using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Image timerBar;
    public float maxTime = 5f;
    float timeLeft;
    public GameObject gameoverText;
    public GameOverScreen GameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        gameoverText.SetActive(false);
        timerBar = GetComponent<Image>();
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        }
        else
        {
            //gameoverText.SetActive(true);
            FindObjectOfType<AudioManager>().PlaySound("Game Over");
            GameOverScreen.Setup();
        }
    }
}
