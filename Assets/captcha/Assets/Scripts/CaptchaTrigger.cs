using UnityEngine;

public class CaptchaTrigger : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject;
    [SerializeField] private CarController carController;

    private bool captchaActive = false;
    private bool hasTriggered = false;
    private MonoBehaviour captchaComponent;

    private void Start()
    {
        if (uiGameObject != null)
        {
            uiGameObject.SetActive(false);
            Debug.Log("CaptchaTrigger: UI assigned and hidden");
        }

        // Try to find either CaptchaUI or CaptchaImage component on the UI object
        if (uiGameObject != null)
        {
            captchaComponent = uiGameObject.GetComponent<CaptchaUI>();
            if (captchaComponent == null)
            {
                captchaComponent = uiGameObject.GetComponent<CaptchaImage>();
            }

            if (captchaComponent != null)
            {
                Debug.Log("CaptchaTrigger: Found captcha component: " + captchaComponent.GetType().Name);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CaptchaTrigger: Hit by: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            captchaActive = true;

            if (carController != null)
            {
                carController.enabled = false;
                Debug.Log("CaptchaTrigger: Car disabled");
            }

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
                Debug.Log("CaptchaTrigger: UI showing");
            }
        }
    }

    private void Update()
    {
        if (captchaActive && captchaComponent != null)
        {
            // Check if IsSolved is true (works for both CaptchaUI and CaptchaImage)
            if (captchaComponent.GetType().GetProperty("IsSolved") != null)
            {
                bool isSolved = (bool)captchaComponent.GetType().GetProperty("IsSolved").GetValue(captchaComponent, null);

                if (isSolved)
                {
                    captchaActive = false;

                    if (carController != null)
                    {
                        carController.enabled = true;
                    }

                    if (uiGameObject != null)
                    {
                        uiGameObject.SetActive(false);
                    }

                    // Destroy all colliders on object and children
                    foreach (Collider col in GetComponentsInChildren<Collider>())
                    {
                        Destroy(col);
                    }
                    Destroy(gameObject); // destroy the cube
                }
            }
        }
    }
}
