using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Button nextPage1;
    [SerializeField] Button nextPage2;

    [SerializeField] GameObject page1;
    [SerializeField] GameObject page2;
    [SerializeField] GameObject tutorialPage;

    // Start is called before the first frame update
    void Start()
    {
        nextPage1.onClick.AddListener(Page1Button);
        nextPage2.onClick.AddListener(Page1Button);

        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public  void Page1Button()
    {
        page1.SetActive(false);
    }

   public  void Page2Button()
    {
        tutorialPage.SetActive(false);
        page2.SetActive(false);
        Time.timeScale = 1;
    }
}
