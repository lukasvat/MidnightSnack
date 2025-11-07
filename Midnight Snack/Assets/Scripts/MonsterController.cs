using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public Transform playerTarget; // Tells monster what to chase
    public float updatePathInterval = 0.2f; // How often to update path to player

    private NavMeshAgent agent;
    private float pathTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        pathTimer += Time.deltaTime;
        if (pathTimer > updatePathInterval)
        {
            agent.SetDestination(playerTarget.position);
            pathTimer = 0f;
        }
    }
}