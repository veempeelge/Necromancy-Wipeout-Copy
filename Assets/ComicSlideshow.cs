using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ComicSlideshow : MonoBehaviour
{
    public Image[] pages; // Assign your comic pages in the inspector
    public Button nextButton;
    public Button previousButton;
    private int currentPageIndex = 0;
    public float slideDuration = 0.5f; // Duration of slide transitions
    public int nextSceneName; // Name of the scene to load after the slideshow
    public int previousSceneName; // Name of the scene to load when going back from the first image

    void Start()
    {
        // Initialize the slideshow
        foreach (Image page in pages)
        {
            page.gameObject.SetActive(false);
        }
        pages[currentPageIndex].gameObject.SetActive(true);

        nextButton.onClick.AddListener(NextPage);
        previousButton.onClick.AddListener(PreviousPage);

        UpdateButtons();
    }

    void NextPage()
    {
        if (currentPageIndex < pages.Length)
        {
            StartCoroutine(SlideTransition(1)); // Slide forward
        }
        else
        {
            // Load the next scene when all slides have been viewed
            SceneManager.LoadScene(nextSceneName);
            Debug.Log("NextScene");
        }
    }

    void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            StartCoroutine(SlideTransition(-1)); // Slide backward
        }
        else
        {
            // Load the previous scene when going back from the first image
            SceneManager.LoadScene(previousSceneName);
        }
    }

    IEnumerator SlideTransition(int direction)
    {
        Image currentPage = pages[currentPageIndex];
        Image nextPage = pages[currentPageIndex + direction];

        RectTransform currentRect = currentPage.GetComponent<RectTransform>();
        RectTransform nextRect = nextPage.GetComponent<RectTransform>();

        // Set up next page for sliding
        nextPage.gameObject.SetActive(true);
        nextRect.anchoredPosition = new Vector2(direction * Screen.width, nextRect.anchoredPosition.y);

        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            currentRect.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(-direction * Screen.width*2, 0), t);
            nextRect.anchoredPosition = Vector2.Lerp(new Vector2(direction * Screen.width * 2, 0), Vector2.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final positions are set
        currentRect.anchoredPosition = new Vector2(-direction * Screen.width * 2, 0);
        nextRect.anchoredPosition = Vector2.zero;

        // Deactivate the previous page
        currentPage.gameObject.SetActive(false);
        currentPageIndex += direction;

        UpdateButtons();
    }

    void UpdateButtons()
    {
        // Disable the "Previous" button if on the first page, otherwise enable it
        previousButton.interactable = currentPageIndex > 0;
        // Disable the "Next" button if on the last page, otherwise enable it
        nextButton.interactable = currentPageIndex < pages.Length - 1;
    }
}
