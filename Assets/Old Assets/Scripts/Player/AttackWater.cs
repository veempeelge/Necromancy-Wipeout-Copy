using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AttackWater : MonoBehaviour
{
    MovementPlayer1 playerStats;
    Enemy enemy;
    Inv_Item items;
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
    private bool canDecrease = true;
    [SerializeField] Image HitPlayerCooldown;
    float _hitPlayerCooldown = 5f;
    private float timer;
    private MovementPlayer1 playerStatsOtherEnemy;
    [SerializeField] ParticleSystem waterParticle;

    [SerializeField] Image waterInd;
    [SerializeField] AudioClip waterOut;

    void Start()
    {
        timer = 0;
        playerStats = GetComponentInParent<MovementPlayer1>();
        items = GetComponentInParent<Inv_Item>();
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = VisionConeMaterial;
        MeshFilter_ = gameObject.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        AttackAngle *= Mathf.Deg2Rad;
    }

    void Update()
    {
        // Ensure the vision cone is drawn only when there is water charge
        if (playerStats.waterCharge > 0)
        {
            AttackRange = 7f * timer / _hitPlayerCooldown;
            AttackAngle = 1f;
            DrawVisionCone();
        }

        if (timer < _hitPlayerCooldown)
        {
            timer += Time.deltaTime;
            this.transform.localScale = new Vector3(timer / _hitPlayerCooldown, timer / _hitPlayerCooldown, timer / _hitPlayerCooldown);

            waterInd.color = new Color(.5f, .7f, 1, .5f*timer/_hitPlayerCooldown);
            //HitPlayerCooldown.fillAmount = timer / _hitPlayerCooldown;
        }
        else
        {
            waterInd.color = new Color(.5f, .7f, 1, .5f);

        }


    }

    void DrawVisionCone()
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

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        if (!Physics.Raycast(transform.position + offset, RaycastDirection, out RaycastHit obstacleHit, Vector3.Distance(transform.position, hit.transform.position), VisionObstructingLayer))
                        {
                            enemy = hit.collider.gameObject.GetComponent<Enemy>();
                            if (enemy != null && enemy.targetPlayer == transform.parent.gameObject && playerStats.waterCharge > 0)
                            {
                                SoundManager.Instance.Play(waterOut,.3f);
                                playerStats.enabled = false;
                                playerStats.AttackAnim();
                                Invoke(nameof(WalkCooldown), .5f);
                                playerStats.DecreaseWaterCharge();
                                waterParticle.Play();
                                enemy.OnPlayerHitWater(playerStats.transform);
                                enemy.OnPlayerDetected(playerStats.transform, playerStats);
                                timer = 0;

                                break;
                            }
                        }
                    }

                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        if (!Physics.Raycast(transform.position + offset, RaycastDirection, out RaycastHit obstacleHit, Vector3.Distance(transform.position, hit.transform.position), VisionObstructingLayer))
                        {
                            playerStatsOtherEnemy = hit.collider.gameObject.GetComponent<MovementPlayer1>();
                            var player = GetComponentInParent<MovementPlayer1>();
                            if (playerStatsOtherEnemy != null && playerStats.waterCharge > 0)
                            {

                                

                                if (canDecrease)
                                {
                                    SoundManager.Instance.Play(waterOut, .3f);


                                    StartCoroutine(playerStatsOtherEnemy.HitByOtherPlayer(this.gameObject));
                                    playerStats.AttackAnim();

                                    canDecrease = false;
                                    player.DecreaseWaterCharge();
                                    Invoke(nameof(canDecreaseCooldown), 2f);

                                    CoolDownIndicator();
                                    waterParticle.Play();
                                    break;
                                }
                               
                               
                              
                              
                            }
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

    private void canDecreaseCooldown()
    {
        canDecrease = true;
    }

    void CoolDownIndicator()
    {
        timer = 0;
    }

    void WalkCooldown()
    {
        playerStats.enabled = true;
    }

    private void OnDisable()
    {
        timer = 0;
        HitPlayerCooldown.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        HitPlayerCooldown.gameObject.SetActive(true);

    }
}
