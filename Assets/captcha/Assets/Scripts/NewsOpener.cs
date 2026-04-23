
using UnityEngine;
using TMPro;

public class NewsOpener : MonoBehaviour
{
    [SerializeField] private GameObject newsPopPanel;
    [SerializeField] private GameObject pressEMessage;
    [SerializeField] private float triggerDistance = 30f;
    [SerializeField] private Transform player;

    private bool isPopupOpen = false;
    private bool justOpened = false;

    void Start()
    {
        if (player == null)
        {
            CarController[] cars = FindObjectsOfType<CarController>();
            if (cars.Length > 0)
                player = cars[0].transform;
        }

        pressEMessage.SetActive(false);
        newsPopPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isNear = distance <= triggerDistance;

        if (isPopupOpen)
        {
            if (justOpened)
                justOpened = false;
            else if (Input.GetKeyDown(KeyCode.E))
                ClosePopup();
        }
        else
        {
            if (isNear)
            {
                pressEMessage.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                    OpenPopup();
            }
            else
            {
                pressEMessage.SetActive(false);
            }
        }
    }

    void OpenPopup()
    {
        isPopupOpen = true;
        justOpened = true;
        pressEMessage.SetActive(false);
        newsPopPanel.SetActive(true);

        if (player != null)
        {
            CarController car = player.GetComponent<CarController>();
            if (car != null) car.enabled = false;
        }
        
        if (GameManager.Instance != null)
            GameManager.Instance.OnNoteCollected();
    }

    void ClosePopup()
    {
        isPopupOpen = false;
        newsPopPanel.SetActive(false);

        if (player != null)
        {
            CarController car = player.GetComponent<CarController>();
            if (car != null) car.enabled = true;
        }
    }
}
