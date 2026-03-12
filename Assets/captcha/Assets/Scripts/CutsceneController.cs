using UnityEngine;
using TMPro;

public class CutsceneController : MonoBehaviour
{
    [Header("UI References :")]
    [SerializeField] private UnityEngine.UI.Image cutsceneImage;
    [SerializeField] private TextMeshProUGUI cutsceneText;
    [SerializeField] private UnityEngine.UI.Image backgroundPanel;

    [Header("Sprite Setup :")]
    [SerializeField] private Sprite[] sprites; // 30 images

    [Header("Text Setup :")]
    [SerializeField] private string[] image1Texts = new string[3] { "Text 1.1", "Text 1.2", "Text 1.3" };
    [SerializeField] private string[] image2Texts = new string[2] { "Text 2.1", "Text 2.2" };
    [SerializeField] private string[] image3Texts = new string[3] { "Text 3.1", "Text 3.2", "Text 3.3" };
    [SerializeField] private string[] image4Texts = new string[2] { "Text 4.1", "Text 4.2" };
    [SerializeField] private string[] image5Texts = new string[3] { "Text 5.1", "Text 5.2", "Text 5.3" };
    [SerializeField] private string[] image6Texts = new string[2] { "Text 6.1", "Text 6.2" };
    [SerializeField] private string[] image7Texts = new string[3] { "Text 7.1", "Text 7.2", "Text 7.3" };
    [SerializeField] private string[] image8Texts = new string[2] { "Text 8.1", "Text 8.2" };
    [SerializeField] private string[] image9Texts = new string[3] { "Text 9.1", "Text 9.2", "Text 9.3" };
    [SerializeField] private string[] image10Texts = new string[2] { "Text 10.1", "Text 10.2" };
    [SerializeField] private string[] image11Texts = new string[3] { "Text 11.1", "Text 11.2", "Text 11.3" };
    [SerializeField] private string[] image12Texts = new string[2] { "Text 12.1", "Text 12.2" };
    [SerializeField] private string[] image13Texts = new string[3] { "Text 13.1", "Text 13.2", "Text 13.3" };
    [SerializeField] private string[] image14Texts = new string[2] { "Text 14.1", "Text 14.2" };
    [SerializeField] private string[] image15Texts = new string[3] { "Text 15.1", "Text 15.2", "Text 15.3" };
    [SerializeField] private string[] image16Texts = new string[2] { "Text 16.1", "Text 16.2" };
    [SerializeField] private string[] image17Texts = new string[3] { "Text 17.1", "Text 17.2", "Text 17.3" };
    [SerializeField] private string[] image18Texts = new string[2] { "Text 18.1", "Text 18.2" };
    [SerializeField] private string[] image19Texts = new string[3] { "Text 19.1", "Text 19.2", "Text 19.3" };
    [SerializeField] private string[] image20Texts = new string[2] { "Text 20.1", "Text 20.2" };
    [SerializeField] private string[] image21Texts = new string[3] { "Text 21.1", "Text 21.2", "Text 21.3" };
    [SerializeField] private string[] image22Texts = new string[2] { "Text 22.1", "Text 22.2" };
    [SerializeField] private string[] image23Texts = new string[3] { "Text 23.1", "Text 23.2", "Text 23.3" };
    [SerializeField] private string[] image24Texts = new string[2] { "Text 24.1", "Text 24.2" };
    [SerializeField] private string[] image25Texts = new string[3] { "Text 25.1", "Text 25.2", "Text 25.3" };
    [SerializeField] private string[] image26Texts = new string[2] { "Text 26.1", "Text 26.2" };
    [SerializeField] private string[] image27Texts = new string[3] { "Text 27.1", "Text 27.2", "Text 27.3" };
    [SerializeField] private string[] image28Texts = new string[2] { "Text 28.1", "Text 28.2" };
    [SerializeField] private string[] image29Texts = new string[3] { "Text 29.1", "Text 29.2", "Text 29.3" };
    [SerializeField] private string[] image30Texts = new string[2] { "Text 30.1", "Text 30.2" };

    [Header("Game Objects :")]
    [SerializeField] private GameObject cutsceneCanvas;
    [SerializeField] private CarController carController;
    [SerializeField] private WarningMessage warningMessage;

    [Header("Text-Only Settings :")]
    [SerializeField] private bool[] textOnlySlides = new bool[30];
    [SerializeField] private Vector2 textCenterPosition = new Vector2(0, 0);

    [Header("Normal Image Settings :")]
    [SerializeField] private Vector2 normalImageSize = new Vector2(1920, 1080);

    [Header("Cover Image Settings :")]
    [SerializeField][Range(0.1f, 3f)] private float coverScale = 1.5f;
    [SerializeField] private bool[] coverSlides = new bool[30];

    [Header("Background Colors :")]
    [SerializeField] private Color mainBackgroundColor = Color.black;
    [SerializeField] private Color blueBackgroundColor = new Color(0.063f, 0.051f, 0.631f);
    [SerializeField] private bool[] blueSlides = new bool[30];

    private int currentImageIndex = 0;
    private int currentTextIndex = 0;
    private string[][] allTexts;
    private bool cutsceneActive = false;
    private Vector2 originalTextPosition;
    private RectTransform textRectTransform;
    private RectTransform imageRectTransform;
    private Vector2 originalImagePosition;
    private Vector2 originalImageSize;
    private bool storedOriginalImage = false;
    private Color originalBackgroundColor;

    private void Start()
    {
        if (backgroundPanel != null)
        {
            originalBackgroundColor = backgroundPanel.color;
            backgroundPanel.color = mainBackgroundColor;
        }

        if (cutsceneText != null)
        {
            textRectTransform = cutsceneText.GetComponent<RectTransform>();
            originalTextPosition = textRectTransform.anchoredPosition;
        }

        if (cutsceneImage != null)
        {
            imageRectTransform = cutsceneImage.GetComponent<RectTransform>();
            originalImagePosition = imageRectTransform.anchoredPosition;
            originalImageSize = imageRectTransform.sizeDelta;
            storedOriginalImage = true;
        }

        allTexts = new string[30][];
        allTexts[0] = image1Texts;
        allTexts[1] = image2Texts;
        allTexts[2] = image3Texts;
        allTexts[3] = image4Texts;
        allTexts[4] = image5Texts;
        allTexts[5] = image6Texts;
        allTexts[6] = image7Texts;
        allTexts[7] = image8Texts;
        allTexts[8] = image9Texts;
        allTexts[9] = image10Texts;
        allTexts[10] = image11Texts;
        allTexts[11] = image12Texts;
        allTexts[12] = image13Texts;
        allTexts[13] = image14Texts;
        allTexts[14] = image15Texts;
        allTexts[15] = image16Texts;
        allTexts[16] = image17Texts;
        allTexts[17] = image18Texts;
        allTexts[18] = image19Texts;
        allTexts[19] = image20Texts;
        allTexts[20] = image21Texts;
        allTexts[21] = image22Texts;
        allTexts[22] = image23Texts;
        allTexts[23] = image24Texts;
        allTexts[24] = image25Texts;
        allTexts[25] = image26Texts;
        allTexts[26] = image27Texts;
        allTexts[27] = image28Texts;
        allTexts[28] = image29Texts;
        allTexts[29] = image30Texts;

        StartCutscene();
    }

    private void StartCutscene()
    {
        cutsceneActive = true;
        currentImageIndex = 0;
        currentTextIndex = 0;

        if (cutsceneImage != null)
        {
            cutsceneImage.enabled = true;
        }

        if (imageRectTransform != null)
        {
            imageRectTransform.gameObject.SetActive(true);
            imageRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            imageRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            imageRectTransform.sizeDelta = normalImageSize;
            imageRectTransform.anchoredPosition = Vector2.zero;
        }

        if (carController != null)
        {
            carController.enabled = false;
        }

        ShowCurrentSlide();
    }

    private void ShowCurrentSlide()
    {
        bool isCoverSlide = (currentImageIndex < coverSlides.Length && coverSlides[currentImageIndex]);
        bool isTextOnly = (currentImageIndex < textOnlySlides.Length && textOnlySlides[currentImageIndex]);
        bool isBlueSlide = (currentImageIndex < blueSlides.Length && blueSlides[currentImageIndex]);

        if (backgroundPanel != null)
        {
            backgroundPanel.color = isBlueSlide ? blueBackgroundColor : mainBackgroundColor;
        }

        if (isCoverSlide)
        {
            if (cutsceneImage != null && imageRectTransform != null)
            {
                cutsceneImage.enabled = true;

                // Scale image by coverScale multiplier
                imageRectTransform.sizeDelta = originalImageSize * coverScale;
                imageRectTransform.anchoredPosition = Vector2.zero;
                imageRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                imageRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                if (currentImageIndex < sprites.Length && sprites[currentImageIndex] != null)
                {
                    cutsceneImage.sprite = sprites[currentImageIndex];
                }
            }
            // Hide text for cover slides
            if (cutsceneText != null)
            {
                cutsceneText.text = "";
            }
        }
        else if (isTextOnly)
        {
            if (cutsceneImage != null)
            {
                cutsceneImage.enabled = false;
            }
            if (textRectTransform != null)
            {
                textRectTransform.anchoredPosition = textCenterPosition;
            }
        }
        else
        {
            if (cutsceneImage != null && imageRectTransform != null)
            {
                cutsceneImage.enabled = true;
                imageRectTransform.sizeDelta = originalImageSize;
                imageRectTransform.anchoredPosition = originalImagePosition;
                imageRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                imageRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                if (currentImageIndex < sprites.Length && sprites[currentImageIndex] != null)
                {
                    cutsceneImage.sprite = sprites[currentImageIndex];
                }
            }
            if (textRectTransform != null)
            {
                textRectTransform.anchoredPosition = originalTextPosition;
            }
        }

        // Show text for non-cover slides
        if (!isCoverSlide)
        {
            if (currentImageIndex < allTexts.Length && allTexts[currentImageIndex] != null)
            {
                if (currentTextIndex < allTexts[currentImageIndex].Length)
                {
                    cutsceneText.text = allTexts[currentImageIndex][currentTextIndex];
                }
            }
        }
    }

    private void Update()
    {
        if (!cutsceneActive) return;

        if (Input.anyKeyDown)
        {
            AdvanceSlide();
        }
    }

    private void AdvanceSlide()
    {
        bool isFillScreen = (currentImageIndex == 0);

        if (isFillScreen)
        {
            currentImageIndex++;
            currentTextIndex = 0;

            if (currentImageIndex >= sprites.Length)
            {
                EndCutscene();
                return;
            }

            ShowCurrentSlide();
            return;
        }

        currentTextIndex++;

        if (currentImageIndex < allTexts.Length &&
            currentTextIndex >= allTexts[currentImageIndex].Length)
        {
            currentImageIndex++;
            currentTextIndex = 0;

            if (currentImageIndex >= sprites.Length)
            {
                EndCutscene();
                return;
            }
        }

        ShowCurrentSlide();
    }

    private void EndCutscene()
    {
        cutsceneActive = false;

        if (backgroundPanel != null)
        {
            backgroundPanel.color = originalBackgroundColor;
        }

        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(false);
        }

        if (carController != null)
        {
            carController.enabled = true;
        }

        if (warningMessage != null)
        {
            warningMessage.ShowWarning();
        }

        Debug.Log("Cutscene ended, game started!");
    }
}
