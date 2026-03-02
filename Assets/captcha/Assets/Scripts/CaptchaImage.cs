using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaptchaImage : MonoBehaviour
{
    [Header("UI References :")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Image Grid :")]
    [SerializeField] private Image[] imageSlots; // 16 image slots for 4x4 grid

    [Header("Sprite Sets :")]
    [SerializeField] private Sprite[] bikesSprites;
    // [SerializeField] private Sprite[] dataSprites;
    // [SerializeField] private Sprite[] dogSprites;
    // [SerializeField] private Sprite[] humanSprites;
    // [SerializeField] private Sprite[] peopleSprites;
    // [SerializeField] private Sprite[] streetSprites;

    [Header("Instructions :")]
    [SerializeField] private string bikesInstruction = "Select all the bikes";
    // [SerializeField] private string dataInstruction = "Select all the trees";
    // [SerializeField] private string dogInstruction = "Select all the dogs";
    // [SerializeField] private string humanInstruction = "Select the humans";
    // [SerializeField] private string peopleInstruction = "Select the people";
    // [SerializeField] private string streetInstruction = "Select the sidewalks";

    private Sprite[] currentSpriteSet;
    public bool IsSolved { get; private set; }

    private void Start()
    {
        IsSolved = false;

        // click listeners to all image slots
        for (int i = 0; i < imageSlots.Length; i++)
        {
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

    private void GenerateCaptcha()
    {
        // a list of only the sprite sets that are actually assigned
        System.Collections.Generic.List<int> availableSets = new System.Collections.Generic.List<int>();

        if (bikesSprites != null && bikesSprites.Length > 0) availableSets.Add(0);
        // if (dataSprites != null && dataSprites.Length > 0) availableSets.Add(1);
        // if (dogSprites != null && dogSprites.Length > 0) availableSets.Add(2);
        // if (humanSprites != null && humanSprites.Length > 0) availableSets.Add(3);
        // if (peopleSprites != null && peopleSprites.Length > 0) availableSets.Add(4);
        // if (streetSprites != null && streetSprites.Length > 0) availableSets.Add(5);

        // If no sets are assigned, show error
        if (availableSets.Count == 0)
        {
            Debug.LogError("No sprite sets assigned to CaptchaImage!");
            return;
        }

        // Randomly pick only from available sets
        int randomIndex = Random.Range(0, availableSets.Count);
        int randomSet = availableSets[randomIndex];

        switch (randomSet)
        {
            case 0: // bikes
                currentSpriteSet = bikesSprites;
                instructionText.text = bikesInstruction;
                break;
                // case 1: // data (trees)
                //     currentSpriteSet = dataSprites;
                //     instructionText.text = dataInstruction;
                //     break;
                // case 2: // dog
                //     currentSpriteSet = dogSprites;
                //     instructionText.text = dogInstruction;
                //     break;
                // case 3: // human
                //     currentSpriteSet = humanSprites;
                //     instructionText.text = humanInstruction;
                //     break;
                // case 4: // people
                //     currentSpriteSet = peopleSprites;
                //     instructionText.text = peopleInstruction;
                //     break;
                // case 5: // street (sidewalks)
                //     currentSpriteSet = streetSprites;
                //     instructionText.text = streetInstruction;
                //     break;
        }

        // Assign sprites to the 16 image slots
        for (int i = 0; i < imageSlots.Length; i++)
        {
            if (currentSpriteSet != null && i < currentSpriteSet.Length)
            {
                imageSlots[i].sprite = currentSpriteSet[i];
            }
        }

        // Hides error text
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }
    }

    private void OnImageClicked(int index)
    {
        if (IsSolved) return;

        if (imageSlots[index].sprite != null)
        {
            string spriteName = imageSlots[index].sprite.name.ToLower();

            // Check if the clicked image contains "correct" in the name
            if (spriteName.Contains("correct"))
            {
                // Correct!
                Debug.Log("Correct image clicked!");
                IsSolved = true;
                gameObject.SetActive(false);
            }
            else
            {
                // Wrong - show error
                Debug.Log("Wrong image clicked: " + spriteName);
                if (errorText != null)
                {
                    errorText.gameObject.SetActive(true);
                }
            }
        }
    }
}
