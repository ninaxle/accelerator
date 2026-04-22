using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderHelper : MonoBehaviour
{
    public float StartDelay = 3f;

    private void Start()
    {
        Invoke(nameof(LoadScene), StartDelay);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("game-over");
        Destroy(gameObject);
    }
}