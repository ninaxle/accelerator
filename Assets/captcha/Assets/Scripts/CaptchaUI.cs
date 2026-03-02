using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaptchaUI : MonoBehaviour
{
   [Header("UI References :")]
   [SerializeField] private Image uiCodeImage;
   [SerializeField] private TMP_InputField uiCodeInput;
   [SerializeField] private TextMeshProUGUI uiErrorsText;
   [SerializeField] private Button uiRefreshButton;
   [SerializeField] private Button uiSubmitButton;

   [Header("Captcha Generator :")]
   [SerializeField] private CaptchaGenerator captchaGenerator;

   private Captcha currentCaptcha;
   public bool IsSolved { get; private set; }

   private void Start()
   {
      IsSolved = false;

      GenerateCaptcha();

      //Buttons:
      uiRefreshButton.onClick.AddListener(GenerateCaptcha);
      uiSubmitButton.onClick.AddListener(Submit);
   }

   private void GenerateCaptcha()
   {
      currentCaptcha = captchaGenerator.Generate();

      //Change UI:
      uiCodeImage.sprite = currentCaptcha.Image;
      uiErrorsText.gameObject.SetActive(false);
   }

   private void Submit()
   {
      string enteredCode = uiCodeInput.text;

      if (captchaGenerator.IsCodeValid(enteredCode, currentCaptcha))
      {
         //valid
         uiErrorsText.gameObject.SetActive(false);
         Debug.Log("<color=green>Valid Code </color>");
         IsSolved = true;
      }
      else
      {
         //invalid
         uiErrorsText.gameObject.SetActive(true);
         Debug.Log("<color=red>Invalid Code </color>");
      }
   }
}
