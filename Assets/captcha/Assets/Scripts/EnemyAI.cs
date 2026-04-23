using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyAi : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private CarController playerCar;
    private bool hasTriggeredJumpscare;
    private bool hasStartedChase;
    private Vector3 trackStartPosition;
    private AudioSource audioSource;
    private AudioClip closerClip;
    private float distanceThreshold = 10f;
    private int lastThresholdIndex;
    private float initialChaseDistance;

    [SerializeField] private float catchDistance = 5f;
    [SerializeField] private GameObject diveEnemyPrefab;
    [SerializeField] private Vector3 diveOffset;
    [SerializeField] private Quaternion diveRotation;
    [SerializeField] private float chaseStartDistance = 50f;
    [SerializeField] private bool waitForPlayer = true;
    [SerializeField] private float gameOverDelay = 3f;
    [SerializeField] private float enemyMinVolume = 0.02f;
    [SerializeField] private float enemyMaxVolume = 0.1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Car").transform;

        if (player != null)
        {
            playerCar = player.GetComponent<CarController>();
            trackStartPosition = player.position;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 100f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    private void Start()
    {
        if (player == null || playerCar == null)
        {
            Debug.LogError("EnemyAI: Player or PlayerCar not found!");
            return;
        }

        agent.speed = playerCar.MaxSpeed * 0.2f;
        float maxEnemySpeed = playerCar.MaxSpeed * 0.9f;
        agent.speed = Mathf.Min(agent.speed, maxEnemySpeed);

        Vector3 behindPlayer = player.position - player.forward * 200f;
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

        agent.isStopped = true;
    }

    private void Update()
    {
        if (hasTriggeredJumpscare) return;
        if (player == null) return;

        if (waitForPlayer && !hasStartedChase)
        {
            float playerDistance = Vector3.Distance(player.position, trackStartPosition);
            if (playerDistance >= chaseStartDistance)
            {
                hasStartedChase = true;
                agent.isStopped = false;
                Vector3 behindPoint = player.position - player.forward * 30f;
                NavMeshHit startHit;
                if (NavMesh.SamplePosition(behindPoint, out startHit, 5f, NavMesh.AllAreas))
                {
                    agent.SetDestination(startHit.position);
                }
                initialChaseDistance = Vector3.Distance(transform.position, player.position);
                lastThresholdIndex = 0;

#if UNITY_EDITOR
                if (closerClip == null)
                {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets("closer t:AudioClip");
                    if (guids.Length > 0)
                        closerClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                            UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
                }
#endif

                Debug.Log("Enemy started chasing!");
            }
            return;
        }

        if (!hasStartedChase) return;

        Vector3 behindTarget = player.position - player.forward * 30f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(behindTarget, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        float currentDistance = agent.remainingDistance;
        int currentThresholdIndex = Mathf.FloorToInt((initialChaseDistance - currentDistance) / distanceThreshold);

        if (currentThresholdIndex > lastThresholdIndex)
        {
            lastThresholdIndex = currentThresholdIndex;

            float distanceRatio = 1f - Mathf.Clamp01(currentDistance / initialChaseDistance);
            float volume = Mathf.Lerp(enemyMinVolume, enemyMaxVolume, distanceRatio);

            if (closerClip != null)
                audioSource.PlayOneShot(closerClip, volume);
        }

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
            playerCar.enabled = false;

        Rigidbody carRb = player.GetComponent<Rigidbody>();
        if (carRb != null)
        {
            carRb.linearVelocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
            carRb.isKinematic = true;
        }

        GameObject diveEnemy = GameObject.Find("DiveEnemy");
        if (diveEnemy != null)
        {
            Quaternion rot = diveRotation.eulerAngles != Vector3.zero ? diveRotation : Quaternion.identity;
            GameObject spawnedDive = Instantiate(diveEnemy, player.position + diveOffset, rot);
            spawnedDive.SetActive(true);

            foreach (Renderer r in spawnedDive.GetComponentsInChildren<Renderer>())
                r.enabled = true;

            foreach (Animator a in spawnedDive.GetComponentsInChildren<Animator>())
                a.Play(0);
        }

        // Create a temporary runner that survives the Destroy, then hand off the coroutine
        GameObject runner = new GameObject("GameOverRunner");
        DontDestroyOnLoad(runner);
        runner.AddComponent<GameOverRunner>().StartGameOver(gameOverDelay);

        Destroy(gameObject); // ✅ safe now — runner handles the rest
    }                                      // ← closes TriggerCatchSequence

    private IEnumerator LoadGameOverSceneAfterDelay()
    {
        yield return new WaitForSeconds(gameOverDelay);
        SceneManager.LoadScene("game-over");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}                                                  // ← closes class