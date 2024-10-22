using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyScriptBaru : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float speed = 3.5f;
    public float health;
    public float enemyAttack = 2;
    public float knockbackDuration = 0.2f;
    public float updateSpeed = 0.1f;
    public float stoppingDistance = 1.0f;

    private List<GameObject> players = new List<GameObject>();
    private Transform closestPlayer;
    private bool wasAttacked;
    private bool isKnockedBack;
    private Rigidbody rb;

    private Transform[] points;
    public GameObject targetPlayer;
    private int currentWaypointIndex = 0;

    public bool CanHitWater = true;

    [SerializeField] TMP_Text chasingWho;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");
        players.AddRange(foundPlayers);

        ChangeTarget();
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
            Enemy_Waypoints waypoints = targetPlayer.GetComponent<Enemy_Waypoints>();
            points = waypoints?.points;

            if (points != null && points.Length > 0)
            {
                currentWaypointIndex = 0; // Start at the first waypoint
                Debug.Log($"Target player: {targetPlayer.name}, Waypoint count: {points.Length}");
                chasingWho.text = $"{targetPlayer.name}";
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
        if (isKnockedBack || points == null || points.Length == 0)
            return;

        MoveTowardsWaypoint();
    }

    private void MoveTowardsWaypoint()
    {
        Transform targetWaypoint = points[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Vector3 move = direction * speed * Time.deltaTime;
        rb.MovePosition(transform.position + move);

        // Rotate towards the waypoint
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if the enemy is close enough to the waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < stoppingDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= points.Length)
            {
                currentWaypointIndex = 0; // Loop back to the first waypoint or you can handle differently
            }
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

        rb.AddForce(direction * knockbackForce * 0.6f, ForceMode.Impulse);

        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero; // Stop any remaining velocity

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
