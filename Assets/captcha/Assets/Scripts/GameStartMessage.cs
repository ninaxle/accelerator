using UnityEngine;

public class GameStartMessage : MonoBehaviour
{
    [SerializeField] private GameObject instructionUI;
    [SerializeField] private GameObject warningUI;
    [SerializeField] private CarController carController;
    [SerializeField] private float delay = 3f;

    void Start()
    {
        if (carController != null)
            carController.enabled = false;

        ShowUI();
        Invoke("EndDelay", delay);
    }

    void ShowUI()
    {
        if (instructionUI != null)
            instructionUI.SetActive(true);

        if (warningUI != null)
            warningUI.SetActive(true);
    }

    void HideUI()
    {
        if (instructionUI != null)
            instructionUI.SetActive(false);

        if (warningUI != null)
            warningUI.SetActive(false);
    }

    void EndDelay()
    {
        HideUI();

        if (carController != null)
            carController.enabled = true;
    }
}
