using UnityEngine;
using System.Reflection;

public class CaptchaTrigger : MonoBehaviour
{
    [SerializeField] public GameObject uiGameObject;
    [SerializeField] public CarController carController;

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
            if (uiGameObject != null && uiGameObject.activeSelf)
            {
                return;
            }

            hasTriggered = true;
            captchaActive = true;
            if (carController != null) carController.enabled = false;
            if (uiGameObject != null) uiGameObject.SetActive(true);
        }
    }

    private void CompleteCaptcha()
    {
        if (captchaComponent != null)
        {
            var type = captchaComponent.GetType();
            PropertyInfo prop = type.GetProperty("IsSolved");
            if (prop != null) prop.SetValue(captchaComponent, false, null);
            else
            {
                FieldInfo field = type.GetField("IsSolved");
                if (field != null) field.SetValue(captchaComponent, false);
            }

            MethodInfo generateMethod = type.GetMethod("GenerateCaptcha");
            if (generateMethod != null) generateMethod.Invoke(captchaComponent, null);
        }

        if (carController != null) carController.enabled = true;
        if (uiGameObject != null) uiGameObject.SetActive(false);

        Debug.Log("Success! Destroying Trigger.");
        Destroy(gameObject);
    }
}