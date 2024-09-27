using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using Unity.VisualScripting;
using TMPro;

public class TutorialSceneManager : MonoBehaviour
{
    public static TutorialSceneManager instance;
    [SerializeField] GameObject player1,player2,player3;
    bool player1moved,player2moved,player3moved = false;
    bool player1threw, player2threw, player3threw = false;

    [SerializeField] AudioClip buttonClick;

    bool waterCollected = false;
    bool phase1, phase2, phase3, phase4 = false;
    bool zombieSpawned, waterSpawned = false;
    bool phase3Paused = true;
    bool timerhasntreduced = true;

    Rigidbody player1movedrb, player2movedrb, player3movedrb;

    MovementPlayer1 player1Code,player2Code,player3Code;
    [SerializeField] GameObject waterGen;
    [SerializeField] GameObject zombie;
    [SerializeField] Transform[] zombieSpawnPoint, waterSpawnPoint;
    [SerializeField] GameObject tutorialScreen, tutorialScreen2, tutorialScreen3, tutorialScreen4, tutorialScreen3Image, tutorialScreen2Image;

    List<GameObject> waters = new List<GameObject>();
    List<GameObject> zombieList = new List<GameObject>();

    [SerializeField] Button next;
    [SerializeField] GameObject tutorialImage;
    [SerializeField] GameObject TutorialOverMarrrr;

    [SerializeField] GameObject _3players1, _3players2, _2players1, _2players2, _3players, _2players;
    [SerializeField] GameObject _phase3_2, _phase3_1, textPhase3;

    private bool haventgolevel = true;
    private bool phase2paused = false;
    [SerializeField] AudioClip buttonSound;

    int timer = 15;
    [SerializeField] TMP_Text timerText;

    public AudioClip ZombieRise;

    public GameObject skipButton;


    // Start is called before the first frame update
    void Start()
    {
        tutorialScreen.SetActive(false);

       // next.onClick.AddListener(Next);
        tutorialScreen2.SetActive(false);
        tutorialScreen3.SetActive(false);
        tutorialScreen4.SetActive(false);

        player1movedrb = player1.GetComponent<Rigidbody>();
        player2movedrb = player2.GetComponent<Rigidbody>();
        player3movedrb = player3.GetComponent<Rigidbody>();

        player1Code = player1.GetComponent<MovementPlayer1>();
        player2Code = player2.GetComponent<MovementPlayer1>();
        player3Code = player3.GetComponent<MovementPlayer1>();


    }
    private void Awake()
    {
   //     GameManager.Instance.canBeHit = false;

    }

    public void StartTutorialImage()
    {
        SoundManager.Instance.Play(buttonClick);

        tutorialScreen.SetActive(true);
    }

    public void Next()
    {
        SoundManager.Instance.Play(buttonClick);
        tutorialScreen.SetActive(true);
        tutorialImage.SetActive(false);
    }

    public void NextPhase2()
    {
        SoundManager.Instance.Play(buttonClick);

        tutorialScreen2Image.SetActive(false);
        Time.timeScale = 1;
    }

    public void NextPhase31()
    {
        SoundManager.Instance.Play(buttonClick);

        _phase3_1.SetActive(false);
        _phase3_2.SetActive(true);
        textPhase3.SetActive(false);
        
    }

    public void NextPhase32()
    {
        SoundManager.Instance.Play(buttonClick);

        _phase3_1.SetActive(false);
        _phase3_2.SetActive(false);
        textPhase3.SetActive(true);

        Time.timeScale = 1;
        tutorialScreen3Image.SetActive(false);

    }
    // Update is called once per frame
    void Update()   
    {


        if (player1movedrb && player1Code!= null)
        {

            if (player1movedrb.velocity.magnitude > 4f)
            {
                player1moved = true;
            }
        }

        if (player2movedrb && player2Code != null)
        {
            if (player2movedrb.velocity.magnitude > 4f)
            {
                player2moved = true;
            }
        }


        if (player3movedrb && player3Code != null)
        {
            if (player3movedrb.velocity.magnitude > 4f)
            {
                player3moved = true;
            }
        }

        if (player3.activeSelf)
        {
            if (player1moved && player2moved && player3moved)
            {
                StartCoroutine(SpawnWater());
            }
        }
        else if (!player3.activeSelf)
        {
            if (player1moved && player2moved)
            {
                StartCoroutine(SpawnWater());
            }
        }
        

        if (phase2)
        {
            Invoke(nameof(CheckWater),3f);
        }

        if (phase3)
        {
            Invoke(nameof(WaterCheck),1f);
        }

        //if (phase4)
        //{
        //    timerText.SetText($"Be the last man standing, tutorial ends in {timer} ");
        //}

    }

    IEnumerator Timer()
    {
        while(timer >= 0)
        {
            yield return new WaitForSeconds(1);
            timer -= 1;
            timerText.SetText($"Be the last man standing! Tutorial ends in {timer} ");

        }

        yield break;
    }
       

    void GoLevelOne()
    {
        SceneManager.LoadSceneAsync(5);
    }

    private void Phase4()
    {
        phase4 = true;  
        tutorialScreen.SetActive(false);
        tutorialScreen2.SetActive(false);
        tutorialScreen3.SetActive(false);
        tutorialScreen4.SetActive(true);
        Invoke(nameof(TutorialEnder), 15f);
        if (timerhasntreduced)
        {
            timerhasntreduced = false;
            StartCoroutine(Timer());
        }
        //timer = 15;

        skipButton.SetActive(true);
        haventgolevel = false;
    }

    public void NextPhase3()
    {
        Time.timeScale = 1;
        tutorialScreen3Image.SetActive(false) ;
    }
    private void WaterCheck()
    {
        if (player1Code.waterCharge < 3)
        {
            player1threw = true;
        }

        if (player2Code.waterCharge < 3)
        {
            player2threw = true;
        }

        if (player3Code.waterCharge < 3)
        {
            player3threw = true;
        }

    }

    IEnumerator SpawnWater()
    {
        if (waterSpawned == false)
        {
            Debug.Log("WaterSpawn");
            waterSpawned = true;
            tutorialScreen.SetActive(false);
            tutorialScreen2.SetActive(true);
            tutorialScreen3.SetActive(false);
            tutorialScreen4.SetActive(false);

           
            phase2 = true;
            if (!phase2paused)
            {
                phase2paused = true;
                Time.timeScale = 0;

            }
            yield return new WaitForSeconds(1f);

            // Clear the list to avoid adding more waterGen objects if SpawnWater is called again
            waters.Clear();

            if (!player3.activeSelf)
            {
                for (int i = 0; i < waterSpawnPoint.Length - 1; i++)
                {
                    // Instantiate the waterGen prefab
                    GameObject water = Instantiate(waterGen, waterSpawnPoint[i].position, waterSpawnPoint[i].rotation);


                    // Add the instantiated waterGen to the waters list
                    waters.Add(water);
                }
            }
            else
            {
                for (int i = 0; i < waterSpawnPoint.Length; i++)
                {
                    // Instantiate the waterGen prefab
                    GameObject water = Instantiate(waterGen, waterSpawnPoint[i].position, waterSpawnPoint[i].rotation);


                    // Add the instantiated waterGen to the waters list
                    waters.Add(water);
                }
            }
           

            yield break;
        }
       
    }

    void CheckWater()
    {
        for (int i = 0; i < waters.Count; i++)
        {
            if (waters[i] == null)
            {
                waters.Remove(waters[i]);
            }
        }

        if (waters.Count == 0)
        {
            Debug.Log(waters.Count);
            StartCoroutine(SpawnZombies());
            phase3 = true;
            Invoke(nameof(Phase4), 10f);
            if (phase3Paused)
            {
                Time.timeScale = 0;
                phase3Paused = false;
            }
           
        }
    }

    IEnumerator SpawnZombies()
    {
        //Debug.Log("ZombieSpawn");

        if (zombieSpawned == false)
        {
            zombieSpawned = true;
            tutorialScreen.SetActive(false);
            tutorialScreen2.SetActive(false);
            tutorialScreen3.SetActive(true);
            tutorialScreen4.SetActive(false);
            
            yield return new WaitForSeconds(.5f);
            SoundManager.Instance.Play(ZombieRise);

            for (int i = 0; i < zombieSpawnPoint.Length; i++)
            {
                GameObject zombiesObject = Instantiate(zombie, zombieSpawnPoint[i].position, zombieSpawnPoint[i].rotation);

                zombieList.Add(zombiesObject);
            }
            yield break;
        }
    }

    public void ButtonSound()
    {
        SoundManager.Instance.Play(buttonSound);
    }
    public void TutorialEnder()
    {
        Time.timeScale = 0;
        TutorialOverMarrrr.SetActive(true);
    }

    public void MainMenuFromTutorial()
    {
        Time.timeScale = 1;
        SoundManager.Instance.Play(buttonClick);
        SceneManager.LoadScene("Main Menu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        SoundManager.Instance.Play(buttonClick);
        SceneManager.LoadScene(5);
    }

    public void ThreePlayers1()
    {
        _2players.SetActive(false);
        tutorialScreen.SetActive(true);
        _3players1.SetActive(true);
        _3players2.SetActive(false);
    }

    public void ThreePlayers2()
    {
        _3players1.SetActive(false);
        _3players2.SetActive(true);
    }

    public void TwoPlayers1()
    {
        _3players.SetActive(false);
        tutorialScreen.SetActive(true);
        _2players1.SetActive(true);
        _2players2.SetActive(false);
    }

    public void TwoPlayers2()
    {
        _2players1.SetActive(false);
        _2players2.SetActive(true);
    }
}


