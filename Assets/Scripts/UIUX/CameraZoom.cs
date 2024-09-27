using UnityEngine;
using UnityEngine.UI;

public class CameraZoom : MonoBehaviour
{
    public GameObject[] cameras;
    public float zoomSpeed = 5f;
    public float slowMoFactor = 0.1f;
    public float zoomAmount = 10f;
    public float duration = .5f;

    private bool isDead = false;
    private float originalFOV;
    private Vector3 originalPosition;
    private float timer = 0f;
    private Vector3 deadPlayer;
    [SerializeField] GameObject[] PlayerUIs;


    [SerializeField] Image _whatIsThisLevel;
    [SerializeField] GameObject Tiebreaker;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            if (ScoreManager.Instance.roundCount == 4)
            {
                Tiebreaker.SetActive(true);
            }
        }
        else
        {
            Tiebreaker.SetActive(false);

        }


        if (Camera.main != null)
        {
            originalFOV = Camera.main.fieldOfView;
            originalPosition = Camera.main.transform.position;
        }
    }

    void Update()
    {
        if (isDead)
        {
            if (gameObject != null)
            {
                timer += Time.deltaTime;
                if (timer <= duration)
                {
                    if (Camera.main != null)
                    {
                        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, originalFOV - zoomAmount, Time.deltaTime * zoomSpeed);
                        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, deadPlayer, Time.deltaTime * zoomSpeed);
                    }

                    for (int i = 0; i < PlayerUIs.Length; i++)
                    {
                        if (PlayerUIs[i] != null)
                        {
                            PlayerUIs[i].SetActive(false);
                        }
                    }
                }
                else
                {
                    Time.timeScale = 1f;
                    isDead = false;
                   // GameManager.Instance.canBeHit = true;
                    if (Camera.main != null)
                    {
                        Camera.main.fieldOfView = originalFOV;
                        Camera.main.transform.position = originalPosition;
                    }

                    timer = 0f;
                    for (int i = 0; i < PlayerUIs.Length; i++)
                    {
                        if (PlayerUIs[i] != null)
                        {
                            PlayerUIs[i].SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void PlayerDied(Vector3 position)
    {
        deadPlayer = position + new Vector3(0, 0, -10);
        isDead = true;
        timer = 0f;
        Time.timeScale = slowMoFactor;
    }
}
