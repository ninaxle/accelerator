using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Image gameOverImage;
    [SerializeField] private float fadeDuration = 2f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(false);
        }
    }

    public void TriggerGameOver()
    {
        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(true);

            Canvas parentCanvas = gameOverImage.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                parentCanvas.sortingOrder = 32767;
            }

            CanvasGroup parentGroup = gameOverImage.GetComponentInParent<CanvasGroup>();
            if (parentGroup == null && parentCanvas != null)
            {
                parentGroup = parentCanvas.gameObject.AddComponent<CanvasGroup>();
            }

            if (parentGroup != null)
            {
                parentGroup.alpha = 0f;
                StartCoroutine(FadeIn(parentGroup));
            }
        }
    }

    private System.Collections.IEnumerator FadeIn(CanvasGroup group)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            group.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        group.alpha = 1f;
    }
}
}