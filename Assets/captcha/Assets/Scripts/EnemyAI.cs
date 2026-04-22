using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private CarController playerCar;
    private bool hasTriggeredJumpscare;

    [SerializeField] private float catchDistance = 5f;
    [SerializeField] private GameObject diveEnemyPrefab;
    [SerializeField] private Image gameOverPanel;
    [SerializeField] private float fadeDuration = 2f;

    [SerializeField] private Vector3 diveOffset;
    [SerializeField] private Quaternion diveRotation;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Car").transform;

        if (player != null)
        {
            playerCar = player.GetComponent<CarController>();
        }
    }

    private void Start()
    {
        if (player == null || playerCar == null)
        {
            Debug.LogError("EnemyAI: Player or PlayerCar not found!");
            return;
        }

        agent.speed = playerCar.MaxSpeed * 0.5f;

        Vector3 behindPlayer = player.position - player.forward * 50f;
        behindPlayer.y = player.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(behindPlayer, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.Warp(hit.position);
        }
        else
        {
            transform.position = behindPlayer;
        }

        agent.SetDestination(player.position);
    }

    private void Update()
    {
        if (hasTriggeredJumpscare) return;

        if (player == null) return;

        agent.SetDestination(player.position);

        if (agent.remainingDistance <= catchDistance)
        {
            TriggerCatchSequence();
        }
    }

    private void TriggerCatchSequence()
    {
        hasTriggeredJumpscare = true;
        agent.isStopped = true;

        if (playerCar != null)
        {
            playerCar.enabled = false;
        }

        Rigidbody carRb = player.GetComponent<Rigidbody>();
        if (carRb != null)
        {
            carRb.linearVelocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
            carRb.isKinematic = true;
        }

        Destroy(gameObject);

        GameObject diveEnemy = GameObject.Find("DiveEnemy");
        Debug.Log("DiveEnemy found: " + (diveEnemy != null));
        if (diveEnemy != null)
        {
            Debug.Log("Spawning DiveEnemy at: " + (player.position + diveOffset));
            Quaternion rot = diveRotation.eulerAngles != Vector3.zero ? diveRotation : Quaternion.identity;
            GameObject spawnedDive = Instantiate(diveEnemy, player.position + diveOffset, rot);
            spawnedDive.SetActive(true);
            foreach (Renderer r in spawnedDive.GetComponentsInChildren<Renderer>())
            {
                r.enabled = true;
            }
            Animator anim = spawnedDive.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play(0);
            }
            foreach (Animator a in spawnedDive.GetComponentsInChildren<Animator>())
            {
                a.Play(0);
            }
            Debug.Log("DiveEnemy spawned and activated");
        }

        StartCoroutine(ScreenFadeToBlack());
    }

    private System.Collections.IEnumerator ScreenFadeToBlack()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            Color panelColor = gameOverPanel.color;
            panelColor.a = 0f;
            gameOverPanel.color = panelColor;

            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                panelColor.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                gameOverPanel.color = panelColor;
                yield return null;
            }

            panelColor.a = 1f;
            gameOverPanel.color = panelColor;
        }
        else
        {
            GameObject fadeObj = new GameObject("FadeOverlay");
            fadeObj.transform.SetParent(Camera.main.transform);
            fadeObj.transform.localPosition = new Vector3(0f, 0f, 1f);
            fadeObj.transform.localScale = new Vector3(100f, 100f, 1f);
            fadeObj.transform.localRotation = Quaternion.identity;

            fadeObj.AddComponent<MeshFilter>().mesh = CreateQuadMesh();
            MeshRenderer mr = fadeObj.AddComponent<MeshRenderer>();
            Material fadeMat = CreateBlackMaterial();
            mr.material = fadeMat;

            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                Color c = new Color(0f, 0f, 0f, alpha);
                mr.material.SetColor("_Color", c);
                yield return null;
            }

            mr.material.SetColor("_Color", Color.black);
        }

        Debug.Log("GAME OVER");
    }

    private Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(-0.5f, -0.5f, 0f),
            new Vector3(0.5f, -0.5f, 0f),
            new Vector3(0.5f, 0.5f, 0f),
            new Vector3(-0.5f, 0.5f, 0f)
        };
        mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        mesh.uv = new Vector2[] {
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(1f, 1f), new Vector2(0f, 1f)
        };
        return mesh;
    }

    private Material CreateBlackMaterial()
    {
        Shader shader = Shader.Find("UI/Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Transparent");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        Material mat = new Material(shader);
        mat.SetColor("_Color", Color.black);
        return mat;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}