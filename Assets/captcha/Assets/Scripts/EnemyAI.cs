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

    [SerializeField] private float catchDistance = 5f;
    [SerializeField] private GameObject diveEnemyPrefab;
    [SerializeField] private Vector3 diveOffset;
    [SerializeField] private Quaternion diveRotation;
    [SerializeField] private float chaseStartDistance = 50f;
    [SerializeField] private bool waitForPlayer = true;
    [SerializeField] private float gameOverDelay = 3f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Car").transform;

        if (player != null)
        {
            playerCar = player.GetComponent<CarController>();
            trackStartPosition = player.position;
        }
    }

    private void Start()
    {
        if (player == null || playerCar == null)
        {
            Debug.LogError("EnemyAI: Player or PlayerCar not found!");
            return;
        }

        agent.speed = playerCar.MaxSpeed * 0.2f;

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
                Vector3 behindPoint = player.position - player.forward * 10f;
                agent.SetDestination(behindPoint);
                Debug.Log("Enemy started chasing!");
            }
            return;
        }

        if (!hasStartedChase) return;

        Vector3 behindTarget = player.position - player.forward * 10f;
        agent.SetDestination(behindTarget);

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

LoadGameOverSceneDelayed(gameOverDelay);

        Destroy(gameObject);

        GameObject diveEnemy = GameObject.Find("DiveEnemy");
        Debug.Log("DiveEnemy found: " + (diveEnemy != null));

        if (diveEnemy != null)
        {
            Quaternion rot = diveRotation.eulerAngles != Vector3.zero ? diveRotation : Quaternion.identity;
            GameObject spawnedDive = Instantiate(diveEnemy, player.position + diveOffset, rot);
            spawnedDive.SetActive(true);

            foreach (Renderer r in spawnedDive.GetComponentsInChildren<Renderer>())
                r.enabled = true;

            Animator anim = spawnedDive.GetComponent<Animator>();
            if (anim != null) anim.Play(0);

            foreach (Animator a in spawnedDive.GetComponentsInChildren<Animator>())
                a.Play(0);

            Debug.Log("DiveEnemy spawned and activated");
        }
    }

    private IEnumerator LoadGameOverSceneAfterDelay()
    {
        yield return new WaitForSeconds(gameOverDelay);
        SceneManager.LoadScene("game-over");
    }

    public static void LoadGameOverSceneDelayed(float delay)
    {
        GameObject helper = new GameObject("SceneLoader");
       DontDestroyOnLoad(helper);
        helper.AddComponent<SceneLoaderHelper>().StartDelay = delay;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}                                                  // ← closes class