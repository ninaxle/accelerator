using UnityEngine;

public class CaptchaTrigger : MonoBehaviour
{
    [SerializeField] private string uiGameObjectName = "UI";
    [SerializeField] private CarController carController;
    
    private GameObject uiGameObject;
    private CaptchaUI captchaUIComponent;
    private bool captchaActive = false;
    private bool hasTriggered = false;

    private void Start()
    {
        uiGameObject = GameObject.Find(uiGameObjectName);
        
        if (uiGameObject != null)
        {
            uiGameObject.SetActive(false);
            
            Transform captchaChild = uiGameObject.transform.Find("CaptchaUI");
            if (captchaChild != null)
            {
                captchaUIComponent = captchaChild.GetComponent<CaptchaUI>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered && uiGameObject != null)
        {
            hasTriggered = true;
            captchaActive = true;
            
            carController.enabled = false;
            
            uiGameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (captchaActive && captchaUIComponent != null)
        {
            if (captchaUIComponent.IsSolved)
            {
                captchaActive = false;
                carController.enabled = true;
                
                uiGameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
}
