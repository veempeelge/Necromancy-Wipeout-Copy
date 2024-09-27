using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
   
public class MainMenu : MonoBehaviour
{

    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip buttonClick;

    private void Start()
    {
        if(mainMenuMusic != null)
        {
            SoundManager.Instance.PlayMusic(mainMenuMusic);

        }
        else
        {
            return;
        }
    }

    public void Level1()
    {
        SoundManager.Instance.Play(buttonClick);
        SceneManager.LoadScene("PRO1.5_TestLvl.1 Populate");
    }

    public void Menu()
    {
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene("Main Menu");
    }

    public void Credits()
    {
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene("Credits");
    }

    public void Lvl1()
    {
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene(5);
    }

    public void Lvl2()
    {
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene(6);
    }

    public void Lvl3()
    {
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene(7);
    }

    public void LvlPicker()
    {
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene("LevelPicker");
    }

    public void TutorialMode()
    {
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene(3);
    }

    public void Quit()
    {
        SoundManager.Instance.Play(buttonClick);

        Application.Quit();
    }
}
