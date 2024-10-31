using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEditor.Rendering;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public NavMeshSurface nav;

    int playersLeft;
    public bool player1alive, player2alive, player3alive;
    bool player1lastalive, player2lastalive, player3lastalive;


    [SerializeField] UnityEngine.UI.Button twoPlayers, threePlayers;

    [SerializeField] GameObject UIPlayerSelect, UIPlayerHP, UIGameOver;

    [SerializeField] GameObject player1obj, player2obj, player3obj;
    [SerializeField] GameObject player1Won, player2Won, player3Won;

    [SerializeField] GameObject[] player3UIs;

    [SerializeField] AudioClip gameplayMusic, playerselectionClick;
    public AudioClip buttonClick;
    [SerializeField] AudioClip p1Dead, p2Dead, p3Dead;


    [SerializeField] Transform hpBar2, weaponDurability2, item2;

    [SerializeField] GameObject player1deadText, player2deadText, player3deadText;

    [SerializeField] GameObject tutorialPage;

    public bool _2players = false;
    public bool _3players = false;


    public float
        defPlayerAtk,
        defPlayerAtkSpd,
        defPlayerRange,
        defPlayerAtkWidth,
        defPlayerKnockback,
        defSpeed,
        defAcc;
    private bool gameStart;
    private bool gameOver;

    public bool canBeHit = true;
    // Start is called before the first frame update
    void Start()
    {
        canBeHit = true;
        if (tutorialPage != null)
        {
            tutorialPage.SetActive(false);
        }
        gameStart = true;
        SoundManager.Instance.PlayMusic(gameplayMusic);
        UIPlayerHP.SetActive(false);
        UIPlayerSelect.SetActive(true);
        UIGameOver.SetActive(false);
        Time.timeScale = 0;
        twoPlayers.onClick.AddListener(_2Players);
        threePlayers.onClick.AddListener(_3Players);
        player1deadText.SetActive(false);
        player2deadText.SetActive(false);
        player3deadText.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _2Players();
                
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _3Players();
               
            }
        }

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ScoreManager.Instance.ResetScores();
                SceneManager.LoadScene("Main Menu");
            }
        }
    }

    void _2Players()
    {
        _2players = true;
        if (tutorialPage != null)
        {
            tutorialPage.SetActive(true);
        }
        SoundManager.Instance.Play(playerselectionClick);

        player1alive = true;
        player2alive = true;
        player3alive = false;

        playersLeft = 2;
        StartGame();
        player3obj.SetActive(false);
        for (int i = 0; i < player3UIs.Length; i++)
        {
            player3UIs[i].SetActive(false);
        }

        //weaponDurability2.position = new Vector3(1684.65f, weaponDurability2.position.y, weaponDurability2.position.z);
        //hpBar2.position = new Vector3(1648.833f, hpBar2.position.y, hpBar2.position.z);
        //item2.position = new Vector3(1591.637f, item2.position.y, item2.position.z);
        gameStart = false;
        Debug.Log("2players");
    }

    void _3Players()

    {
        _3players = true;
        if (tutorialPage != null)
        {
            tutorialPage.SetActive(true);
        }
        SoundManager.Instance.Play(playerselectionClick);

        player1alive = true;
        player2alive = true;
        player3alive = true;
        playersLeft = 3;
        StartGame();

        gameStart = false;
        Debug.Log("3players");

    }

    public void Player1Dead()
    {
        canBeHit = false;
        Invoke(nameof(CanBeHit), 1f);

        playersLeft--;
        player1alive = false;
        Debug.Log("Player1Dead");
        SoundManager.Instance.Play(p1Dead, 1.8f);
        if (playersLeft == 1)
        {

            if (player2alive)
            {
                //Player 2 Won
                ScoreManager.Instance.Player2Won();
                player2Won.SetActive(true);
            }

            if (player3alive)
            {
                //Player 3 Won
                ScoreManager.Instance.Player3Won();
                player3Won.SetActive(true);
            }

            StartCoroutine(GameOver());
            ScoreManager.Instance.Player1RunnerUp();
            if (player2alive)
            {
                
            }
        }

        player1deadText.SetActive(true);
            
        if (TutorialSceneManager.instance != null)
        {
            TutorialSceneManager.instance.TutorialEnder();
        }
    }

    public void Player2Dead()
    {
        canBeHit = false;
        Invoke(nameof(CanBeHit), 1f);

        playersLeft--;
        player2alive = false;
        Debug.Log("Player1Dead");
        SoundManager.Instance.Play(p2Dead, 1.8f);

        if (playersLeft == 1)
        {

            if (player1alive)
            {
                //Player 1 Won
                ScoreManager.Instance.Player1Won();
                player1Won.SetActive(true);
            }

            if (player3alive)
            {
                //Player 3 Won
                ScoreManager.Instance.Player3Won();
                player3Won.SetActive(true);
            }

            StartCoroutine(GameOver());
            ScoreManager.Instance.Player2RunnerUp();
        }

        player2deadText.SetActive(true);

        if (TutorialSceneManager.instance != null)
        {
            TutorialSceneManager.instance.TutorialEnder();
        }
    }

    public void Player3Dead()
    {
        canBeHit = false;
        Invoke(nameof(CanBeHit), 1f);
        playersLeft--;
        player3alive = false;
        Debug.Log("Player1Dead");
        player3deadText.SetActive(true);
        SoundManager.Instance.Play(p3Dead, 1.8f);

        if (playersLeft == 1)
        {

            if (player1alive)
            {
                //Player 1 Won
                ScoreManager.Instance.Player1Won();
                player1Won.SetActive(true);
            }

            if (player2alive)
            {
                //Player 2 Won
                ScoreManager.Instance.Player2Won();
                player2Won.SetActive(true);
            }

            ScoreManager.Instance.Player3RunnerUp();
            StartCoroutine(GameOver());
        }

        if (TutorialSceneManager.instance != null)
        {
            TutorialSceneManager.instance.TutorialEnder();
        }
    }

    void CanBeHit()
    {
        canBeHit = true;
    }

    void StartGame()
    {
        UIPlayerHP.SetActive(true);
        UIPlayerSelect.SetActive(false);
        Time.timeScale = 1;
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        gameOver = true;
        Debug.Log("GameOver");
        UIGameOver.SetActive(true);
        Time.timeScale = 0;

    }

    IEnumerator CamToWinner(CameraZoom cam, MovementPlayer1 mov)
    {
        yield return new WaitForSeconds(.3f);
        cam.PlayerDied(mov.gameObject.transform.position);
    }
}
