using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAi : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private CarController playerCar;
    private bool hasTriggeredJumpscare;

    [SerializeField] private float catchDistance = 5f;

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

        agent.speed = playerCar.MaxSpeed * 1f;

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
            TriggerJumpscare();
        }
    }

    private void TriggerJumpscare()
    {
        hasTriggeredJumpscare = true;
        agent.isStopped = true;
        Time.timeScale = 0f;
        Debug.Log("JUMPSCARE - GAME OVER");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}