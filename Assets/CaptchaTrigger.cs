using UnityEngine;

public class CaptchaTrigger : MonoBehaviour
{
    [SerializeField] private CaptchaUI captchaUI;
    [SerializeField] private CarController carController;
    private bool captchaActive = false;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            captchaActive = true;
            
            carController.enabled = false;
            
            captchaUI.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (captchaActive && captchaUI.IsSolved)
        {
            captchaActive = false;
            carController.enabled = true;
            
            gameObject.SetActive(false);
        }
    }
}
