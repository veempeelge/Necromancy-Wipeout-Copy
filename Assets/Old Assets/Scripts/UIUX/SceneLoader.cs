using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] AudioClip playMusic;

    private void Start()
    {
        SoundManager.Instance.PlayMusic(playMusic);
    }

    public void Menu()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScores();
        }
        SceneManager.LoadScene("Main Menu");
    }
}
