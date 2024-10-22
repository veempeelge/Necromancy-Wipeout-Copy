using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] MovementPlayer1 playerStats;
    public Material VisionConeMaterial;
    public float AttackRange;
    public float AttackAngle;
    public LayerMask VisionObstructingLayer; // Layer with objects that obstruct the enemy view, like walls, for example
    public LayerMask EnemyLayer; // Layer for enemies
    public int VisionConeResolution = 120; // The vision cone will be made up of triangles, the higher this value is the prettier the vision cone will be
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    public bool isAttacking;

    [SerializeField] AudioClip[] attackHit;

    void Start()
    {
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = VisionConeMaterial;
        MeshFilter_ = gameObject.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        AttackAngle *= Mathf.Deg2Rad;
    }

    void Update()
    {
        AttackRange = playerStats.playerRange;
        AttackAngle = playerStats.playerAtkWidth;
        DrawVisionCone();
    }

    void DrawVisionCone() // This method creates the vision cone mesh
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -AttackAngle / 2;
        float angleIncrement = AttackAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            Vector3 offset = Vector3.zero;

            RaycastHit visionHit;
            bool obstructed = Physics.Raycast(transform.position + offset, RaycastDirection, out visionHit, AttackRange, VisionObstructingLayer);

            if (obstructed)
            {
                Vertices[i + 1] = VertForward * visionHit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * AttackRange;

                
                RaycastHit[] hits = Physics.RaycastAll(transform.position + offset, RaycastDirection, AttackRange);

                bool enemyDetected = false;
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        if (!Physics.Raycast(transform.position + offset, RaycastDirection, out RaycastHit obstacleHit, Vector3.Distance(transform.position, hit.transform.position), VisionObstructingLayer))
                        {
                            SoundManager.Instance.Play(attackHit[Random.Range(0,attackHit.Length)]);
                            playerStats.DecreaseDurability();


                            hit.collider.gameObject.GetComponent<Enemy>().OnPlayerDetected(playerStats.transform, playerStats);

                            //  Debug.Log("HIT" + hit.collider.transform.name);
                            enemyDetected = true;
                            break;  
                        }
                    }
                }
            }

            Currentangle += angleIncrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }

}
