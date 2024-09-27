using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Inv_Item : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;
    public bool hasTrap;
    public bool hasWater;
    public GameObject itemButton;

    public int playerNumber;
    private string fireButton;
    public GameObject TrapPrefab;
    public float spawnDistance;

    private MovementPlayer1 mvP1;
    private Transform PTransform;

    public GameObject WaterPrefab;
    public Transform shootPoint;
    public float ShootSpeed = 10f;

    private bool isMovementRestricted = false;
    public Item_Slot slot;
    public int initialWaterCount;

    private void Start()
    {
        mvP1 = GetComponent<MovementPlayer1>();
        PTransform = GetComponent<Transform>();
        fireButton = "Fire" + playerNumber;

        for (int i = 0; i < slots.Length; i++)
        {
            Item_Slot slot = slots[i].GetComponent<Item_Slot>();

            
            slot.count = initialWaterCount;
            if (slot.count > 0)
            {
                Instantiate(itemButton, slots[i].transform, false);
                hasWater = true;
                slot.RefreshCount();
            }
            else
            {
                hasWater = false;
                slot.RefreshCount();
            }
           
        }
    }

    private void Update()
    {
        //if (Input.GetButtonDown(fireButton))
        //{
        //    UseItem();
        //}
    }

    public void DiscardItem(int slotIndex)
    {
        if (isFull[slotIndex])
        {
            foreach (Transform child in slots[slotIndex].transform)
            {
                hasTrap = false;
                hasWater = false;
                Destroy(child.gameObject);
            }

            isFull[slotIndex] = false;
        }
    }

    private void UseItem()
    {

        for (int i = 0; i < slots.Length; i++)
        {
            Item_Slot slot = slots[i].GetComponent<Item_Slot>();
            if (slot == null)
            {
                Debug.LogWarning("Slot not found!");
            }
            if (hasTrap)
            {
                Vector3 spawnPosition = PTransform.position + (-PTransform.forward * spawnDistance);
                Debug.Log("Spawn Position: " + spawnPosition);
                Instantiate(TrapPrefab, spawnPosition, PTransform.rotation);

                Debug.Log("Player " + playerNumber + " used a trap!");
                DiscardItem(i);

            }
            if (hasWater)
            {
                slot.count -= 1;
                slot.RefreshCount();
                GameObject water = Instantiate(WaterPrefab, shootPoint.position, shootPoint.rotation);
                Rigidbody rb = water.GetComponent<Rigidbody>();
                rb.velocity = shootPoint.forward * ShootSpeed;

                Debug.Log("Player " + playerNumber + " sprayed water!");

                if (slot.count < 1)
                {
                    DiscardItem(i);
                }
            }
            else
            {
                Debug.Log("Player " + playerNumber + " does not have item");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            StartCoroutine(RestrictMovement(other.gameObject));
        }
    }

    private IEnumerator RestrictMovement(GameObject trap)
    {
        isMovementRestricted = true;
        mvP1.enabled = false;

        yield return new WaitForSeconds(3f);
            mvP1.enabled = true;
            isMovementRestricted = false;
            Destroy(trap);
    }

}
