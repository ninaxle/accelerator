using UnityEngine;

public class CaptchaTrigger : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject;
    [SerializeField] private CaptchaUI captchaUIComponent;
    [SerializeField] private CarController carController;

    private bool captchaActive = false;
    private bool hasTriggered = false;

    private void Start()
    {
        if (uiGameObject != null)
        {
            uiGameObject.SetActive(false);
            Debug.Log("CaptchaTrigger: UI assigned and hidden");
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
        if (captchaActive && captchaUIComponent != null)
        {
            if (captchaUIComponent.IsSolved)
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
