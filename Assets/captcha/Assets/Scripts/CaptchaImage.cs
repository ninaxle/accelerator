using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CaptchaImage : MonoBehaviour
{
    [Header("UI References :")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Image Grid :")]
    [SerializeField] private Image[] imageSlots;

    [Header("Sprite Sets :")]
    [SerializeField] private Sprite[] bikesSprites;
    [SerializeField] private Sprite[] dataSprites;
    [SerializeField] private Sprite[] dogSprites;
    [SerializeField] private Sprite[] humanSprites;
    [SerializeField] private Sprite[] peopleSprites;
    [SerializeField] private Sprite[] streetSprites;

    [Header("Instructions :")]
    [SerializeField] private string bikesInstruction = "Select all the bikes";
    [SerializeField] private string dataInstruction = "Select all the trees";
    [SerializeField] private string dogInstruction = "Select all the dogs";
    [SerializeField] private string humanInstruction = "Select the humans";
    [SerializeField] private string peopleInstruction = "Select the people";
    [SerializeField] private string streetInstruction = "Select the sidewalks";

    [Header("Settings :")]
    [SerializeField] private Color selectedColor = new Color(0, 0, 1, 0.25f);

    private Sprite[] currentSpriteSet;
    private bool[] selectedSlots;
    private Color[] originalColors;
    public bool IsSolved { get; private set; }

    private void Start()
    {
        IsSolved = false;

        // Initialize selection tracking
        selectedSlots = new bool[imageSlots.Length];
        originalColors = new Color[imageSlots.Length];

        // Add click listeners and store original colors
        for (int i = 0; i < imageSlots.Length; i++)
        {
            originalColors[i] = imageSlots[i].color;

            int index = i;
            Button btn = imageSlots[i].GetComponent<Button>();
            if (btn == null)
            {
                btn = imageSlots[i].gameObject.AddComponent<Button>();
            }
            btn.onClick.AddListener(() => OnImageClicked(index));
        }

        GenerateCaptcha();
    }

    public void GenerateCaptcha()
    {
        // Reset selections
        for (int i = 0; i < selectedSlots.Length; i++)
        {
            selectedSlots[i] = false;
            imageSlots[i].color = originalColors[i];
        }

        // Build list of available sets
        List<int> availableSets = new List<int>();
        if (bikesSprites != null && bikesSprites.Length > 0) availableSets.Add(0);
        if (dataSprites != null && dataSprites.Length > 0) availableSets.Add(1);
        if (dogSprites != null && dogSprites.Length > 0) availableSets.Add(2);
        if (humanSprites != null && humanSprites.Length > 0) availableSets.Add(3);
        if (peopleSprites != null && peopleSprites.Length > 0) availableSets.Add(4);
        if (streetSprites != null && streetSprites.Length > 0) availableSets.Add(5);

        if (availableSets.Count == 0)
        {
            Debug.LogError("No sprite sets assigned to CaptchaImage!");
            return;
        }

        // Randomly pick from available sets
        int randomIndex = Random.Range(0, availableSets.Count);
        int randomSet = availableSets[randomIndex];

        switch (randomSet)
        {
            case 0:
                currentSpriteSet = bikesSprites;
                instructionText.text = bikesInstruction;
                break;
            case 1:
                currentSpriteSet = dataSprites;
                instructionText.text = dataInstruction;
                break;
            case 2:
                currentSpriteSet = dogSprites;
                instructionText.text = dogInstruction;
                break;
            case 3:
                currentSpriteSet = humanSprites;
                instructionText.text = humanInstruction;
                break;
            case 4:
                currentSpriteSet = peopleSprites;
                instructionText.text = peopleInstruction;
                break;
            case 5:
                currentSpriteSet = streetSprites;
                instructionText.text = streetInstruction;
                break;
        }

        // Assign sprites to image slots
        for (int i = 0; i < imageSlots.Length; i++)
        {
            if (currentSpriteSet != null && i < currentSpriteSet.Length)
            {
                imageSlots[i].sprite = currentSpriteSet[i];
            }
        }

        // Hide error text
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }
    }

    private void OnImageClicked(int index)
    {
        if (IsSolved) return;

        // Toggle selection
        selectedSlots[index] = !selectedSlots[index];

        // Apply color
        if (selectedSlots[index])
        {
            imageSlots[index].color = selectedColor;
        }
        else
        {
            imageSlots[index].color = originalColors[index];
        }

        // Check selection after every click
        CheckSelection();
    }

    private void CheckSelection()
    {
        int correctSelected = 0;
        int wrongSelected = 0;
        int totalCorrect = 0;

        // Count correct and wrong in the current set
        for (int i = 0; i < currentSpriteSet.Length; i++)
        {
            string spriteName = currentSpriteSet[i].name.ToLower();
            if (spriteName.Contains("correct"))
            {
                totalCorrect++;
                if (selectedSlots[i])
                {
                    correctSelected++;
                }
            }
            else if (spriteName.Contains("error"))
            {
                if (selectedSlots[i])
                {
                    wrongSelected++;
                }
            }
        }

        // Check if all correct are selected
        if (correctSelected == totalCorrect && wrongSelected == 0)
        {
            // All correct selected, no wrong - PASS!
            IsSolved = true;
        }
        // Check if 3 or more wrong are selected
        else if (wrongSelected >= 3)
        {
            if (errorText != null)
            {
                errorText.gameObject.SetActive(true);
            }
        }
        else
        {
            // Less than 3 wrong - hide error
            if (errorText != null)
            {
                errorText.gameObject.SetActive(false);
            }
        }
    }
}
