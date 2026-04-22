using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Image gameOverImage;
    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        if (gameOverImage != null)
        {
            Color c = gameOverImage.color;
            gameOverImage.color = new Color(c.r, c.g, c.b, 0f);
            gameOverImage.gameObject.SetActive(false);
        }

        TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        if (gameOverImage != null)
        {
            Canvas parentCanvas = gameOverImage.canvas;
            if (parentCanvas != null)
            {
                parentCanvas.overrideSorting = true;
                parentCanvas.sortingOrder = 32767;
            }

            gameOverImage.gameObject.SetActive(true);
            StartCoroutine(FadeInImage(gameOverImage));
        }
        else
        {
            StartCoroutine(CreateAndFade());
        }
    }

    private System.Collections.IEnumerator FadeInImage(Image image)
    {
        Color c = image.color;
        c.a = 0f;
        image.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(timer / fadeDuration);
            image.color = c;
            yield return null;
        }

        c.a = 1f;
        image.color = c;
    }

    private System.Collections.IEnumerator CreateAndFade()
    {
        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("GameOverImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        RectTransform rect = imageObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        Image fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(timer / fadeDuration));
            yield return null;
        }
        fadeImage.color = Color.black;
    }
}

public class GameOverRunner : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2f;

    public void StartGameOver(float delay)
    {
        StartCoroutine(LoadAfterDelay(delay));
    }

    private IEnumerator LoadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Create fade canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        DontDestroyOnLoad(canvasObj);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        RectTransform rect = imageObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        UnityEngine.UI.Image fadeImage = imageObj.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        // Fade to black
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(timer / fadeDuration));
            yield return null;
        }

        // Now load the scene
        SceneManager.LoadScene("game-over");
        Destroy(gameObject);
    }
}