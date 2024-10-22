using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(LineRenderer))]
public class MediumEnemy : MonoBehaviour
{
    Waypoints waypoints;

    public float rotationSpeed = 1f;
    public float speed;
    public float health;
    public float enemyAttack = 2;
    public float knockbackDuration = 0.2f;
    public float updateSpeed = 0.1f;
    public float stoppingDistance = 1.0f;

    private NavMeshAgent agent;
    private List<GameObject> players = new List<GameObject>();
    private Transform closestPlayer;
    private bool wasAttacked;
    private bool isKnockedBack;
    private Rigidbody rb;
    private LineRenderer lineRenderer;

    private Transform[] points;
    public GameObject targetPlayer;
    private Transform targetLocation;
    private Vector3 lastTargetPosition;

    public bool CanHitWater = true;

    [SerializeField] TMP_Text chasingWho;

    public GameObject previousTarget;
    float distanceToWaypoint;

    public bool TargettedSamePlayer { get; private set; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        ConfigureLineRenderer();

        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");
        players.AddRange(foundPlayers);

        ChangeTarget();
        StartCoroutine(ChangePointRegularly());

    }

    void ConfigureLineRenderer()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void TargetPlayer()
    {
        if (players.Count == 0) return;

        GameObject newTarget = players[Random.Range(0, players.Count)];

        // Ensure the new target is different from the current targetPlayer
        while (newTarget == targetPlayer)
        {
            newTarget = players[Random.Range(0, players.Count)];
        }

        targetPlayer = newTarget;

        if (targetPlayer != null)
        {
            waypoints = targetPlayer.GetComponent<Waypoints>();
            points = waypoints?.pointsMedium;

            if (points != null && points.Length > 0)
            {
                int index = Random.Range(0, points.Length);
                targetLocation = points[index];
                lastTargetPosition = targetLocation.position;
                Debug.Log($"Target player: {targetPlayer.name}, Waypoint index: {index}");
                chasingWho.text = $"{targetPlayer.name}, {index}";
                if (targetPlayer.name == "Player 1")
                {
                    chasingWho.color = Color.blue;
                }
                else if (targetPlayer.name == "Player 2")
                {
                    chasingWho.color = Color.red;
                }
                else if (targetPlayer.name == "Player 3")
                {
                    chasingWho.color = Color.green;
                }
            }
            else
            {
                Debug.Log("No waypoints found for the target player.");
            }
        }
        else
        {
            Debug.Log("Target player not found.");
        }
    }


    void TargetSamePlayer()
    {
        if (players.Count == 0) return;

        if (targetPlayer != null)
        {
            waypoints = targetPlayer.GetComponent<Waypoints>();
            points = waypoints?.pointsMedium;

            if (points != null && points.Length > 0)
            {
                int index = Random.Range(0, points.Length);
                targetLocation = points[index];
                lastTargetPosition = targetLocation.position;
                Debug.Log($"Target player: {targetPlayer.name}, Waypoint index: {index}");
                chasingWho.text = $"{targetPlayer.name}, {index}";
                if (targetPlayer.name == "Player 1")
                {
                    chasingWho.color = Color.blue;
                }
                else if (targetPlayer.name == "Player 2")
                {
                    chasingWho.color = Color.red;
                }
                else if (targetPlayer.name == "Player 3")
                {
                    chasingWho.color = Color.green;
                }
            }
            else
            {
                Debug.Log("No waypoints found for the target player.");
            }
        }
        else
        {
            Debug.Log("Target player not found.");
        }
    }
    void Update()
    {
        if (!isKnockedBack && targetLocation != null)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, targetLocation.position);

            if (distanceToWaypoint > stoppingDistance)
            {
                agent.isStopped = false;
                if (agent.isOnNavMesh && (agent.destination != targetLocation.position))
                {
                    agent.SetDestination(targetLocation.position);
                }

            
                Vector3 velocity = agent.velocity;
                if (velocity.sqrMagnitude > 0.01f) 
                {
                    Quaternion lookRotation = Quaternion.LookRotation(velocity.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                }

                UpdateLineRenderer();

            }
            else
            {
                targetLocation = targetPlayer.transform;
                if (agent.destination != targetLocation.position)
                {
                    agent.SetDestination(targetLocation.position);
                }

                UpdateLineRenderer();
            }



            if (targetLocation == targetPlayer.transform)
            {
                if (distanceToWaypoint > .4f)
                {
                    Invoke(nameof(CheckIfStillFar), 3f);
                    TargettedSamePlayer = false;
                }
            }
        }
    }

    void CheckIfStillFar()
    {
        if (targetLocation == targetPlayer.transform)
        {
            if (distanceToWaypoint > .4f && !TargettedSamePlayer)
            {
                TargetSamePlayer();
                TargettedSamePlayer = true;
            }
        }
        
    }
    
    IEnumerator ChangePointRegularly()
    {
        while (targetLocation != targetPlayer.transform)
        {
            TargetSamePlayer();
            yield return new WaitForSeconds(4);
        }
    }

    void UpdateLineRenderer()
    {
        if (agent.path.corners.Length > 1)
        {
            lineRenderer.positionCount = agent.path.corners.Length;
            lineRenderer.SetPositions(agent.path.corners);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Water"))
        {
            Debug.Log("Got hit by holy water");
            Destroy(collision.gameObject);
            ChangeTarget();
        }
    }

    public void TakeDamage(float damageAmount, Vector3 knockbackDirection, float knockbackForce)
    {
        health -= damageAmount;
        isKnockedBack = true;

        if (rb != null)
        {
            StartCoroutine(Knockback(knockbackDirection, knockbackForce));
        }
    }

    public void OnPlayerDetected(Transform playerTransform, MovementPlayer1 playerStats)
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (!wasAttacked)
        {
            wasAttacked = true;
            TakeDamage(playerStats.playerAtk, -directionToPlayer, playerStats.playerKnockback);
            Invoke(nameof(CanTakeDamage), 0.2f);
        }
    }

    public void OnPlayerHitWater()
    {
        Debug.Log("Target another player Medium ");
        if (CanHitWater)
        {
            ChangeTarget();
            CanHitWater = false;
            Invoke(nameof(CanHitWaterCooldown), .2f);

        }

    }

    void CanHitWaterCooldown()
    {
        CanHitWater = true;
    }

    void CanTakeDamage()
    {
        wasAttacked = false;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator Knockback(Vector3 direction, float knockbackForce)
    {
        float elapsedTime = 0f;
        agent.enabled = false; // Disable NavMeshAgent during knockback

        rb.AddForce(direction * knockbackForce * 0.6f, ForceMode.Impulse);

        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero; // Stop any remaining velocity
        agent.enabled = true; // Re-enable NavMeshAgent

        if (health <= 0)
        {
            Die();
        }

        isKnockedBack = false;
    }

    public void ChangeTarget()
    {
        TargetPlayer();
    }
}
