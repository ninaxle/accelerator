using UnityEngine;

public class CaptchaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject captchaTemplate;
    [SerializeField] private int captchaCount = 10;
    [SerializeField] private float minZ = 50f;
    [SerializeField] private float maxZ = 500f;
    [SerializeField] private float roadWidth = 15f;

    [SerializeField] private GameObject uiGameObject;
    [SerializeField] private CarController carController;

    void Start()
    {
        if (captchaTemplate == null)
        {
            Debug.LogError("CaptchaSpawner: No template assigned!");
            return;
        }

        // Find UI object if not assigned
        if (uiGameObject == null)
        {
            CaptchaUI captchaUI = FindObjectOfType<CaptchaUI>();
            if (captchaUI != null)
                uiGameObject = captchaUI.gameObject;
            else
            {
                CaptchaImage captchaImage = FindObjectOfType<CaptchaImage>();
                if (captchaImage != null)
                    uiGameObject = captchaImage.gameObject;
            }
        }

        // Find CarController if not assigned
        if (carController == null)
        {
            carController = FindObjectOfType<CarController>();
        }

        Debug.Log("CaptchaSpawner: Spawning " + captchaCount + " captchas!");

        for (int i = 0; i < captchaCount; i++)
        {
            float z = Mathf.Lerp(minZ, maxZ, (float)i / captchaCount);
            float x = Random.Range(-roadWidth, roadWidth);

            GameObject spawned = Instantiate(captchaTemplate, new Vector3(x, 0.5f, z), Quaternion.identity);

            // Set references on the spawned captcha
            CaptchaTrigger trigger = spawned.GetComponent<CaptchaTrigger>();
            if (trigger != null)
            {
                trigger.uiGameObject = uiGameObject;
                trigger.carController = carController;
            }
        }
    }
}
