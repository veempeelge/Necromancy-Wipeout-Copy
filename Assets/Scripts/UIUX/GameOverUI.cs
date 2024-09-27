using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] AudioClip buttonClick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene("Result");

    }

    public void MainMenu()
    {
        ScoreManager.Instance.ResetScores();

        SoundManager.Instance.Play(buttonClick);

        SceneManager.LoadScene("Main Menu");
    }
}
