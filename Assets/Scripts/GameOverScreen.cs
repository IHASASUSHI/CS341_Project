using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public void Setup()
    {
        gameObject.SetActive(true);
        //FindObjectOfType<AudioManager>().PlaySound("Game Over");

    }

    public void RestartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}