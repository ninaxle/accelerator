using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnNoteCollected()
    {
        Time.timeScale = 0f;
        
        AudioListener listener = Camera.main?.GetComponent<AudioListener>();
        if (listener != null)
            listener.enabled = false;
    }
}