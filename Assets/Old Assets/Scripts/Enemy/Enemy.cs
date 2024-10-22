using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using FirstGearGames.SmoothCameraShaker;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent), typeof(LineRenderer))]
public class Enemy : MonoBehaviour
{
    public ShakeData EnemyShake;
    private Waypoints waypoints;

    public float rotationSpeed = 1f;
    public float speed;
    public float health;
    public float enemyAttack = 2;
    public float knockbackDuration = 0.2f;
    public float stoppingDistance = 1.0f;

    private NavMeshAgent agent;
    private List<GameObject> players = new List<GameObject>();
    private Transform targetLocation;
    private Vector3 lastTargetPosition;

    private bool wasAttacked;
    private bool isKnockedBack;

    private Rigidbody rb;
    private LineRenderer lineRenderer;

    private Transform[] points;
    public GameObject targetPlayer;

    public bool CanHitWater = true;

    [SerializeField] private TMP_Text chasingWho;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip hitWater, hit;
    [SerializeField] private AudioClip spawn;

    [SerializeField] private Image waterIndicatorCone;
    [SerializeField] private ParticleSystem stunParticle;

    public bool TargettedSamePlayer = true;
    public bool hardEnemy;
    public bool mediumEnemy;
    public bool easyEnemy;

    private float distanceToWaypoint;

    void Start()
    {
        animator.SetTrigger("Spawn");

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;

        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        ConfigureLineRenderer();

        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        DisableAgentTemporarily();

        ChangeTarget();
    }

    private void DisableAgentTemporarily()
    {
        agent.speed = 0;
        agent.angularSpeed = 0;
        agent.acceleration = 0;
        Invoke(nameof(EnableAgent), 1f);
    }

    private void EnableAgent()
    {
        agent.speed = 8;
        agent.angularSpeed = 120;
        agent.acceleration = 16;
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

    void Update()
    {
        if (!isKnockedBack)
        {
            UpdateTargetAndMovement();
        }
    }

    private void UpdateTargetAndMovement()
    {
        if (targetPlayer == null) ChangeTarget();

        if (targetLocation != null)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, targetLocation.position);

            if (distanceToWaypoint > stoppingDistance)
            {
                MoveTowardsTarget();
            }
            else
            {
                SetTargetToPlayer();
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (agent.isOnNavMesh && agent.destination != targetLocation.position)
        {
            agent.SetDestination(targetLocation.position);
            UpdateLineRenderer();
        }

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void SetTargetToPlayer()
    {
        targetLocation = targetPlayer.transform;
        if (agent.destination != targetLocation.position)
        {
            agent.SetDestination(targetLocation.position);
        }
        UpdateLineRenderer();

        if (distanceToWaypoint > 1f && TargettedSamePlayer)
        {
            Invoke(nameof(CheckIfStillFar), 3f);
            TargettedSamePlayer = false;
        }
    }

    private void CheckIfStillFar()
    {
        if (targetPlayer != null && targetLocation == targetPlayer.transform && distanceToWaypoint > 2f)
        {
            TargetSamePlayer();
            TargettedSamePlayer = true;
        }
    }

    private void TargetPlayer()
    {
        if (players.Count == 0) return;

        GameObject newTarget = players[Random.Range(0, players.Count)];

        // Ensure the new target is different from the current targetPlayer
        while (newTarget == targetPlayer)
        {
            newTarget = players[Random.Range(0, players.Count)];
        }

        SetTargetPlayer(newTarget);
    }

    private void TargetSamePlayer()
    {
        if (players.Count == 0 || targetPlayer == null) return;
        SetTargetPlayer(targetPlayer);
    }

    private void SetTargetPlayer(GameObject newTarget)
    {
        targetPlayer = newTarget;

        if (targetPlayer != null)
        {
            waypoints = targetPlayer.GetComponent<Waypoints>();
            points = easyEnemy ? waypoints?.points : mediumEnemy ? waypoints?.pointsMedium : hardEnemy ? waypoints?.pointsHard : null;

            if (points != null && points.Length > 0)
            {
                int index = Random.Range(0, points.Length);
                targetLocation = points[index];
                lastTargetPosition = targetLocation.position;
                UpdateUI(index);
            }
            else
            {
                Debug.Log("No waypoints found for the target player.");
            }
        }
    }

    private void UpdateUI(int index)
    {
        chasingWho.text = $"{targetPlayer.name}, {index}";

        switch (targetPlayer.name)
        {
            case "Player 1":
                waterIndicatorCone.color = new Color(0, 1, 1, .5f);
                chasingWho.color = Color.blue;
                break;
            case "Player 2":
                waterIndicatorCone.color = new Color(1, 0, 0.7330103f, .5f);
                chasingWho.color = Color.red;
                break;
            case "Player 3":
                waterIndicatorCone.color = new Color(1, 1, 0, .5f);
                chasingWho.color = Color.green;
                break;
            default:
                waterIndicatorCone.color = new Color(1, 1, 1, .5f);
                chasingWho.color = Color.white;
                break;
        }
    }

    private void UpdateLineRenderer()
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

    public IEnumerator TakeDamage(float damageAmount, Vector3 knockbackDirection, float knockbackForce)
    {
        yield return new WaitForSeconds(0.23f);
        health -= damageAmount;
        isKnockedBack = true;
        animator.SetTrigger("Knockback");

        if (rb != null)
        {
            StartCoroutine(Knockback(knockbackDirection, knockbackForce));
        }
    }

    private IEnumerator Knockback(Vector3 direction, float knockbackForce)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;

        // Calculate the target position, but keep the Y position unchanged
        Vector3 targetPosition = new Vector3(
            originalPosition.x + direction.x * knockbackForce,
            originalPosition.y, // Preserve Y position
            originalPosition.z + direction.z * knockbackForce
        );

        agent.enabled = false; // Disable NavMeshAgent during knockback

        while (elapsedTime < knockbackDuration)
        {
            // Smoothly move the enemy towards the knockback target position
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / knockbackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the enemy ends up exactly at the target position
        transform.position = targetPosition;

        agent.enabled = true; // Re-enable NavMeshAgent

        if (health <= 0)
        {
            Die();
        }
        else
        {
            SoundManager.Instance.Play(hitWater);
            animator.SetTrigger("Pain");
            isKnockedBack = false;
        }
    }


    public void OnPlayerDetected(Transform playerTransform, MovementPlayer1 playerStats)
    {
        if (!wasAttacked)
        {
            wasAttacked = true;
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            StartCoroutine(TakeDamage(playerStats.playerAtk, -directionToPlayer, playerStats.playerKnockback));
            Invoke(nameof(CanTakeDamage), 0.2f);
        }
    }

    public void OnPlayerHitWater(Transform player)
    {
        if (CanHitWater)
        {
            stunParticle.Play();
            SoundManager.Instance.Play(hitWater, 1.2f);
            //SoundManager.Instance.Play(hit);

            CanHitWater = false;
            Invoke(nameof(CanHitWaterCooldown), 2f);
            ChangeTarget();
            animator.SetTrigger("Knockback");
            enabled = false;
        }
    }

    private void CanHitWaterCooldown()
    {
        CanHitWater = true;
        enabled = true;
    }

    private void CanTakeDamage()
    {
        wasAttacked = false;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void Grab()
    {
        animator.SetTrigger("Grab");
        CameraShakerHandler.Shake(EnemyShake);
    }

    public void ChangeTarget()
    {
        TargetPlayer();
    }
}