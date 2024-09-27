using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class MovementPlayer1 : MonoBehaviour
{
    public static MovementPlayer1 instance;

    //[SerializeField] GameObject UIPlayerIsDead;

    private CameraZoom cameraZoom;

    public Attack attack;
    public AttackWater attackWater;
    public Inv_Item item;

    public HPBar hpBar;

    public GameManager gameManager;
    public float moveSpeed;
    public float maxAcceleration;
    public float rotationSpeed = 700f;

    public string horizontalAxis;
    public string verticalAxis;
    public string attackButton;

    [SerializeField] private Rigidbody rb;
    private Vector3 movement;

    public float MaxHP = 4;
    public float currentHP;

    public GameObject attackIndicatorPrefab;
    private bool canAttack = true;
    public bool player1, player2, player3;
    public float attackAreaMultiplier = 1.5f;

    [Header("PlayerStats")]

    public float playerAtk;
    public float playerAtkSpd;
    public float playerRange;
    public float weaponDurability;
    public float weaponCurrentDurability;
    public float playerAtkWidth;
    public float playerKnockback;

    public Vector3 knockbackDirection;
    public GameObject hitIndicator;
    public GameObject waterHitIndicatorPrefab;
    private bool IsDecreased;
    private bool usingWeapon = false;

    [SerializeField] GameObject wpDurabilityBar;
    private float defaultSpeed;
    private bool isImmune;

    [Header("Audio")]
    [SerializeField] AudioClip attackAir;
    [SerializeField] AudioClip gotItem;
    [SerializeField] AudioClip ManDeath;
    [SerializeField] AudioClip WomanDeath;

    public int waterCharge;
    private bool waterDecreased;
    private Inv_Item inv;
    public Item_Slot slot;

    private bool isWaterAttacking = false;
    private bool canhitbyotherplayer = true;

    [SerializeField] Animator anim;
    [SerializeField] GameObject cameras;

    [SerializeField] GameObject deadBodyObject;

    [SerializeField] SpriteRenderer tutorialUI;
    [SerializeField] Sprite arrows, ijkl;

    [SerializeField] AudioClip hitWater;

    [SerializeField] AudioSource walking;

    [SerializeField] GameObject waterIndicator;

    [SerializeField] ParticleSystem stunParticle;

    public ParticleSystem gotWaterParticle;



    private void Awake()
    {
        waterIndicator.SetActive(false);
        waterCharge = 0;
    }
    void Start()
    {
        cameraZoom = Camera.main.GetComponent<CameraZoom>();

        item = GetComponent<Inv_Item>();
        attackWater = GetComponentInChildren<AttackWater>();
        //waterCharge = slot.count;
        moveSpeed = gameManager.defSpeed;
        maxAcceleration = gameManager.defAcc;
        playerAtk = gameManager.defPlayerAtk;
        playerAtkSpd = gameManager.defPlayerAtkSpd;
        playerRange = gameManager.defPlayerRange;
        playerAtkWidth = gameManager.defPlayerAtkWidth;
        playerKnockback = gameManager.defPlayerKnockback;

        wpDurabilityBar.SetActive(false);
        defaultSpeed = moveSpeed;

        currentHP = MaxHP;
        rb = GetComponent<Rigidbody>();

        if (waterCharge == 0)
        {
            waterHitIndicatorPrefab.SetActive(false);
        }

       
    }



    void Update()
    {
        float moveX = Input.GetAxis(horizontalAxis);
        float moveZ = Input.GetAxis(verticalAxis);
        float threshold = 0.1f;

        if (tutorialUI.enabled)
        {
            if (player2 && gameManager._3players)
            {
                horizontalAxis = "Horizontal2";
                verticalAxis = "Vertical2";
                tutorialUI.sprite = arrows;
            }

            if (player2 && gameManager._2players)
            {
                horizontalAxis = "Horizontal3";
                verticalAxis = "Vertical3";
                tutorialUI.sprite = ijkl;

            }

            if (player3 && gameManager._3players)
            {
                horizontalAxis = "Horizontal3";
                verticalAxis = "Vertical3";
                tutorialUI.sprite = arrows;

            }
        }

        if (tutorialUI.enabled)
        {
            if (Mathf.Abs(moveX) > threshold || Mathf.Abs(moveZ) > threshold)
            {
                tutorialUI.enabled = false; ;
            }
        }

        

        movement = new Vector3(moveX, 0, moveZ);
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (rb.velocity.y < -.1f)
        {
            rb.velocity += Physics.gravity * Time.fixedDeltaTime;
        }

        
       


        if (Input.GetButton(horizontalAxis) || Input.GetButton(verticalAxis))
        {
            PlayWalk();
            anim.SetFloat("IsRunning", rb.velocity.magnitude);
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);

        }

        RotatePlayer();
    }

    void PlayWalk()
    {
        if (!walking.isPlaying)
        {
            walking.Play();
        }

    }

    public void StartWaterCoroutine()
    {
        SprayWater();
        Debug.Log("StartWaterCoroutine called");
        if (!isWaterAttacking)
        {
            StartCoroutine(WaterAttack());
        }
    }

    void MovePlayer()
    {
        Vector3 normalizedMovement = movement.normalized;

        rb.AddForce(normalizedMovement * moveSpeed * 5, ForceMode.Acceleration);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxAcceleration);
    }

    public IEnumerator HitByOtherPlayer(GameObject otherPlayer)
    {
        yield return new WaitForSeconds(.23f);
        Vector3 direction = (otherPlayer.transform.position - transform.position).normalized;

        if (canhitbyotherplayer)
        {
            stunParticle.Play();
            anim.SetTrigger("Choke");

            rb.AddForce( -direction * 10, ForceMode.Impulse);        anim.SetTrigger("Choke");

            canhitbyotherplayer = false;
            this.enabled = false;
            StartCoroutine(EnableMovement());

            SoundManager.Instance.Play(hitWater, 1.5f);
        }
        yield break;
        
    }

    IEnumerator EnableMovement()
    {
        yield return new WaitForSeconds(1f);
        this.enabled = true;
        anim.SetTrigger("StopChoke");
        yield return new WaitForSeconds(1f);
        canhitbyotherplayer = true;

    }
    void RotatePlayer()
    {
        if (movement != Vector3.zero)
        {

            Quaternion targetRotation = Quaternion.LookRotation(movement);


            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

   public void TakeDamage(float damage)
   {
        if (gameManager.canBeHit)
        {
            currentHP -= damage;
            hpBar.UpdateBar(currentHP);
            //Debug.Log("Got Hit, HP Remaining = " + currentHP);
            

            if (currentHP <= 0)
            {
                Die();
            }
        }

        SoundManager.Instance.Play(hitWater,1.5f);
        StartCoroutine(KnockEnemy());
        anim.SetTrigger("Choke");
        stunParticle.Play();

    }

    void Die()
    {
        //anim.SetTrigger("Ded");
        //StartCoroutine(DestroyObject());

        Destroy(gameObject);
        Instantiate(deadBodyObject, transform.position, Quaternion.identity);

        if (player1)
        {
            gameManager.Player1Dead();
            //UI player 1
            cameraZoom.PlayerDied(cameras.transform.position);
            //UIPlayerIsDead.SetActive(true);
        }

        if (player2)
        {
            gameManager.Player2Dead();
            //UI player 2
            cameraZoom.PlayerDied(cameras.transform.position);
            //UIPlayerIsDead.SetActive(true);
        }

        if (player3)
        {
            gameManager.Player3Dead();
            //UI player 3
            cameraZoom.PlayerDied(cameras.transform.position);
            //UIPlayerIsDead.SetActive(true);
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(.3f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(.3f);
        Destroy(gameObject);
       
    }

    public void ChangeStats(float atk, float atkspd, float range, float atkradius, float durability, float knockback)
    {
        usingWeapon = true;
        playerAtk = atk;
        playerAtkSpd = atkspd;
        playerAtkWidth = atkradius;
        playerRange = range;
        weaponDurability = durability;
        playerKnockback = knockback;

        weaponDurability = durability;
        weaponCurrentDurability = durability;
        hpBar.UpdateDurabilityBar(weaponDurability, weaponCurrentDurability);
        wpDurabilityBar.SetActive(true);
        SoundManager.Instance.Play(gotItem);

    }

    IEnumerator AutoAttack()
    {
        while (weaponDurability > 0)
        {
            if (canAttack && !isImmune)
            {
                yield return new WaitForSeconds(1f / playerAtkSpd);
                DurabilityCheck();
              //  AttackEnemy();
                yield return new WaitForSeconds(.5f);
                hitIndicator.SetActive(false);
                attack.isAttacking = false;
            }
            else
            {
                yield return null;
            }
        }
        Debug.Log("Weapon is broken!");
        wpDurabilityBar.SetActive(false);
    }

    IEnumerator KnockEnemy()
    {
            this.enabled = false;
            yield return new WaitForSeconds(.5f);
            anim.SetTrigger("StopChoke");
            this.enabled = true;
            //anim.SetTrigger("Attack");
            attack.isAttacking = true;
            hitIndicator.SetActive(true);
            yield return new WaitForSeconds(.1f);
            hitIndicator.SetActive(false);
            attack.isAttacking = false;
            //yield return null;
    }
    private IEnumerator WaterAttack()
    {
        while (slot.count > 0)
        {
            isWaterAttacking = true;
            yield return new WaitForSeconds(1);
            SprayWater();
            yield return new WaitForSeconds(.1f);
            attackWater.isAttacking = false;
        }

        for (int i = 0; i < item.slots.Length; i++)
        {
            item.DiscardItem(i);
        }

        waterHitIndicatorPrefab.SetActive(false);
        isWaterAttacking = false;
    }


    void ResetStats()
    {
        weaponDurability = 10f;
        playerAtk = gameManager.defPlayerAtk;
        playerAtkSpd = gameManager.defPlayerAtkSpd;
        playerRange = gameManager.defPlayerRange;
        playerAtkWidth = gameManager.defPlayerAtkWidth;
        playerKnockback = gameManager.defPlayerKnockback;
    }

    void DurabilityCheck()
    {
        if (weaponCurrentDurability <= 0)
        {
            ResetStats();
            wpDurabilityBar.SetActive(false);
        }
        else
        {
            return;
        }
    }
    void AttackEnemy()
    {
        attack.isAttacking = true;
        hitIndicator.SetActive(true);
        SoundManager.Instance.Play(attackAir);
    }

    void SprayWater()
    {
        attackWater.isAttacking = true;
        waterIndicator.SetActive(true);

        waterHitIndicatorPrefab.SetActive(true);
        //SoundManager Spraying Water
    }

    public void DecreaseDurability()
    {
        if (!IsDecreased && usingWeapon)
        {
            IsDecreased = true;
            weaponCurrentDurability -= 1f;
            Debug.Log(weaponDurability);
            Invoke(nameof(DecreaseOnce), .2f);
            hpBar.UpdateDurabilityBar(weaponDurability, weaponCurrentDurability);
        }

    }

    public void DecreaseWaterCharge()
    {
        if (!waterDecreased)
        {
            if (waterCharge >= 0)
            {
               
                slot.count--;
                slot.RefreshCount();
                waterDecreased = true;
                waterCharge--;
                Invoke(nameof(WaterDecreasedOnce), .2f);
                Debug.Log("Decreased Water " + waterCharge);

              //  hpBar.UpdateWater(3, slot.count);
            }


        }
    }

    public void Victory()
    {
        anim.SetTrigger("Victory");
    }

   
    public void AttackAnim()
    {
        anim.SetTrigger("Attack");
    }

    void WaterDecreasedOnce()
    {
        waterDecreased = false;
    }
    void DecreaseOnce()
    {
        IsDecreased = false;
    }

   void PowerUpHP(float amount)
    {
        currentHP += amount;
    }

    void PowerUpSpeed(float amount, float duration)
    {
        moveSpeed += amount;
        Invoke(nameof(SpeedReset), duration);
    }

    void Immune(float duration)
    {
        isImmune = true;
        Invoke(nameof(ImmuneReset), duration);
    }



    void SpeedReset()
    {
        moveSpeed = defaultSpeed;
    }

    void ImmuneReset()
    {
        isImmune = false;
    }

}
