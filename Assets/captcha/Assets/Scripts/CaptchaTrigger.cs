using UnityEngine;
using System.Reflection;

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

            // Try finding the main object first
            captchaComponent = uiGameObject.GetComponent<CaptchaUI>();
            if (captchaComponent == null) captchaComponent = uiGameObject.GetComponent<CaptchaImage>();

            // If still null, look in the children objects
            if (captchaComponent == null)
            {
                captchaComponent = uiGameObject.GetComponentInChildren<CaptchaUI>();
                if (captchaComponent == null) captchaComponent = uiGameObject.GetComponentInChildren<CaptchaImage>();
            }

            if (captchaComponent != null)
                Debug.Log($"<color=green>SUCCESS:</color> Found {captchaComponent.GetType().Name}!");
            else
                Debug.LogError("STILL ERROR: I searched the UI and all its children but found NO Captcha script.");
        }
    }

    private void Update()
    {
        if (captchaActive && captchaComponent != null)
        {
            if (CheckIfSolved())
            {
                CompleteCaptcha();
            }
        }
    }

    private bool CheckIfSolved()
    {
        var type = captchaComponent.GetType();
        PropertyInfo prop = type.GetProperty("IsSolved");
        if (prop != null) return (bool)prop.GetValue(captchaComponent, null);

        FieldInfo field = type.GetField("IsSolved");
        if (field != null) return (bool)field.GetValue(captchaComponent);

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            captchaActive = true;
            if (carController != null) carController.enabled = false;
            if (uiGameObject != null) uiGameObject.SetActive(true);
        }
    }

    private void CompleteCaptcha()
    {
        if (carController != null) carController.enabled = true;
        if (uiGameObject != null) uiGameObject.SetActive(false);

        Debug.Log("Success! Destroying Trigger.");
        Destroy(gameObject);
    }
}