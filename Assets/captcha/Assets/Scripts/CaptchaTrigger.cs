using UnityEngine;
using System.Reflection;
using System.Collections;

public class CaptchaTrigger : MonoBehaviour
{
    [SerializeField] public GameObject uiGameObject;
    [SerializeField] public CarController carController;

    private bool captchaActive = false;
    private bool hasTriggered = false;
    private MonoBehaviour captchaComponent;
    private float enemyFreezeTime = 10f;
    private Coroutine enemyUnfreezeCoroutine;

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

            FreezeEnemies();
            enemyUnfreezeCoroutine = StartCoroutine(UnfreezeEnemiesAfterDelay());
        }
    }

    private void FreezeEnemies()
    {
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        foreach (EnemyAi enemy in enemies)
        {
            var agentField = typeof(EnemyAi).GetField("agent", BindingFlags.NonPublic | BindingFlags.Instance);
            if (agentField != null)
            {
                UnityEngine.AI.NavMeshAgent agent = agentField.GetValue(enemy) as UnityEngine.AI.NavMeshAgent;
                if (agent != null) agent.isStopped = true;
            }
        }
    }

    private IEnumerator UnfreezeEnemiesAfterDelay()
    {
        yield return new WaitForSeconds(enemyFreezeTime);
        UnfreezeEnemies();
    }

    private void UnfreezeEnemies()
    {
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        foreach (EnemyAi enemy in enemies)
        {
            var agentField = typeof(EnemyAi).GetField("agent", BindingFlags.NonPublic | BindingFlags.Instance);
            if (agentField != null)
            {
                UnityEngine.AI.NavMeshAgent agent = agentField.GetValue(enemy) as UnityEngine.AI.NavMeshAgent;
                if (agent != null)
                {
                    agent.isStopped = false;
                    var chaseStartField = typeof(EnemyAi).GetField("chaseStartDistance", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (chaseStartField != null)
                    {
                        float chaseStartDistance = (float)chaseStartField.GetValue(enemy);
                        if (chaseStartDistance > 0)
                        {
                            var hasStartedChaseField = typeof(EnemyAi).GetField("hasStartedChase", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (hasStartedChaseField != null) hasStartedChaseField.SetValue(enemy, true);
                        }
                    }
                }
            }
        }
    }

    private void CompleteCaptcha()
    {
        if (enemyUnfreezeCoroutine != null)
        {
            StopCoroutine(enemyUnfreezeCoroutine);
            enemyUnfreezeCoroutine = null;
        }

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

        UnfreezeEnemies();

        Debug.Log("Success! Destroying Trigger.");
        Destroy(gameObject);
    }
}