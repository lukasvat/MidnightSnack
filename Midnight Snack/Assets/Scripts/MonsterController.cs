using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterController : MonoBehaviour
{
    public Transform playerTarget; // Tells monster what to chase
    public float updatePathInterval = 0.2f; // How often to update path to player

    private NavMeshAgent agent;
    private float pathTimer;

    public Transform playerCamera;
    public float teleportDistance = 0.5f;
    public float scareHoldTime = 2f;
    private AudioSource monsterAudioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Awake()
    {
        monsterAudioSource = GetComponent<AudioSource>();
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

    public IEnumerator JumpScareSequence()
    {
        // Disable monster, calculate position and rotation for jumpscare
        float currentMonsterHeight = transform.position.y;
        if (agent != null) agent.enabled = false; 
        Vector3 forwardDirection = playerCamera.forward;
        Vector3 playerBodyPosition = playerCamera.parent.position; 
        Vector3 targetPosition = playerBodyPosition + forwardDirection * teleportDistance;
        targetPosition.y = currentMonsterHeight; 
        Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
        Quaternion finalRotation = targetRotation * Quaternion.Euler(0, 180, 0);

        // Teleport and Rotate the monster
        if (agent != null)
        {
            agent.Warp(targetPosition);
        }
        else
        {
            transform.position = targetPosition; 
        }
        transform.rotation = finalRotation;

        // Trigger attack animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("ScareAttack"); 
        }

        yield return new WaitForSecondsRealtime(scareHoldTime); 

        GameManager.Instance.ShowGameOverUI();
    }

    public void PlayMonsterRoar()
    {
        monsterAudioSource.Play();
    }
}