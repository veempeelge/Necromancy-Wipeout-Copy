using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAiScript : MonoBehaviour
{
    public static Enemy instance;
    public float rotationSpeed = 1f;
    public float Speed;
    public float health;
    public float enemyAttack = 2;
    public float knockbackDuration = 0.2f;
    public Vector3 parameter = new Vector3(1, 0, 1);
    public float UpdateSpeed = 0.1f;

    private NavMeshAgent Agent;
    private List<GameObject> Players = new List<GameObject>();
    private Transform closestPlayer;
    private bool wasAttacked;

    void Start()
    {
        // Get the NavMeshAgent component
        Agent = GetComponent<NavMeshAgent>();

        // Find all GameObjects with the tag "Player"
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

        // Add each found player to the list
        foreach (GameObject player in foundPlayers)
        {
            Players.Add(player);
        }

        // Start the coroutine to follow the closest player
        StartCoroutine(FollowClosestPlayer());
    }

    void Update()
    {
        if (closestPlayer != null)
        {
            // Smooth rotation towards the player
            Vector3 direction = (closestPlayer.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Transform FindClosestPlayer()
    {
        Transform nearestPlayer = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject player in Players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player.transform;
            }
        }

        return nearestPlayer;
    }

    private IEnumerator FollowClosestPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateSpeed);

        while (true)
        {
            closestPlayer = FindClosestPlayer();

            if (closestPlayer != null)
            {
                Agent.SetDestination(closestPlayer.position);
            }

            yield return wait;
        }
    }

    public void TakeDamage(float damageAmount, Vector3 knockbackDirection, float knockbackForce)
    {
        health -= damageAmount;
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 multiplier = new Vector3(0, -knockbackDirection.y, 0);

        if (rb != null)
        {
            StartCoroutine(Knockback(-knockbackDirection + multiplier, knockbackForce));
        }
    }

    public void OnPlayerDetected(Transform playerTransform, MovementPlayer1 playerStats)
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (!wasAttacked)
        {
            wasAttacked = true;
            TakeDamage(playerStats.playerAtk, directionToPlayer, playerStats.playerKnockback);
            Invoke(nameof(CanTakeDamage), 0.2f);
        }
    }

    void CanTakeDamage()
    {
        wasAttacked = false;
    }

    private void Die()
    {
        // Handle enemy death
        Destroy(gameObject);
    }

    private IEnumerator Knockback(Vector3 direction, float knockbackForce)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction * knockbackForce;

        while (elapsedTime < knockbackDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / knockbackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (health <= 0)
        {
            yield return new WaitForSeconds(0.1f);
            Die();
        }

        transform.position = targetPosition; // Ensure the final position is set
    }
}