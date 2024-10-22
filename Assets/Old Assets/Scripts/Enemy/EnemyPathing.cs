using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    public Transform[] targets; // Array of potential targets
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Select a target
        Transform selectedTarget = SelectTarget();

        // Decide path here: nearest or farthest
        if (ShouldTakeFarthestPath())
        {
            TakeFarthestPath(selectedTarget);
        }
        else
        {
            TakeNearestPath(selectedTarget);
        }
    }

    private Transform SelectTarget()
    {
        // Implement your logic here to select a target
        // For simplicity, we'll randomly choose one of the targets
        return targets[Random.Range(0, targets.Length)];
    }

    private bool ShouldTakeFarthestPath()
    {
        // Implement your logic here to decide if the enemy should take the farthest path
        return Random.value > 0.5f;
    }

    private void TakeNearestPath(Transform target)
    {
        agent.SetDestination(target.position);
    }

    private void TakeFarthestPath(Transform target)
    {
        // Implementing the farthest path can be complex, as Unity NavMesh does not provide a direct way to get the farthest path.
        // One way to approximate it is by setting waypoints or destinations that are significantly longer to reach the target.
        // Here is a simple example using random points within a range.

        Vector3 farthestPoint = GetFarthestPoint(target.position);
        agent.SetDestination(farthestPoint);
    }

    private Vector3 GetFarthestPoint(Vector3 destination)
    {
        Vector3 farthestPoint = Vector3.zero;
        float maxDistance = 0f;

        // Generate random points and choose the farthest valid one
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 20f; // Adjust the multiplier based on your needs
            randomDirection += destination;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 20f, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(hit.position, destination);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = hit.position;
                }
            }
        }

        return farthestPoint;
    }
}
