using UnityEngine;
using TMPro;

public class WarningMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float delay = 3f;

    public void ShowWarning()
    {
        if (warningText == null) return;

        warningText.gameObject.SetActive(true);
        Invoke(nameof(HideWarning), delay);
    }

    private void HideWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }
}
