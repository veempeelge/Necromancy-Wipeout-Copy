using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatGen_Behave : MonoBehaviour
{
    private Inv_Item inv;
    [SerializeField] Item_Slot slot;
    public GameObject itemButton;
    [SerializeField] AudioClip waterGet;

    private void Start()
    {
        //StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.Play(waterGet);

            MovementPlayer1 mvP1 = other.gameObject.GetComponent<MovementPlayer1>();
            inv = mvP1.GetComponent<Inv_Item>();
            slot = inv.slot;

            for (int i = 0; i < inv.slots.Length; i++)
            {
                if (inv.isFull[i] == false)
                {
                    mvP1.waterCharge = 3;
                    mvP1.gotWaterParticle.Play();
                    inv.hasWater = true;
                    inv.hasTrap = false;
                    inv.isFull[i] = true;
                    Instantiate(itemButton, inv.slots[i].transform, false);
                    slot.count = 3;
                    slot.RefreshCount();
                    mvP1.StartWaterCoroutine();
                    Destroy(gameObject);

                    break;
                    
                }
                if (inv.isFull[i] == true)
                {
                    mvP1.waterCharge = 3;
                    mvP1.gotWaterParticle.Play();

                    inv.DiscardItem(i);
                    inv.hasWater = true;
                    inv.hasTrap = false;
                    inv.isFull[i] = true;
                    Instantiate(itemButton, inv.slots[i].transform, false);
                    slot.count = 3;
                    slot.RefreshCount();
                    mvP1.StartWaterCoroutine();
                    Destroy(gameObject);

                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.Play(waterGet);

            MovementPlayer1 mvP1 = other.gameObject.GetComponent<MovementPlayer1>();
            inv = mvP1.GetComponent<Inv_Item>();
            slot = inv.slot;

            for (int i = 0; i < inv.slots.Length; i++)
            {
                if (inv.isFull[i] == false)
                {
                    mvP1.waterCharge = 3;
                    mvP1.gotWaterParticle.Play();

                    inv.hasWater = true;
                    inv.hasTrap = false;
                    inv.isFull[i] = true;
                    Instantiate(itemButton, inv.slots[i].transform, false);
                    slot.count = 3;
                    slot.RefreshCount();
                    mvP1.StartWaterCoroutine();
                    Destroy(gameObject);

                    break;

                }
                if (inv.isFull[i] == true)
                {
                    mvP1.waterCharge = 3;
                    mvP1.gotWaterParticle.Play();

                    inv.DiscardItem(i);
                    inv.hasWater = true;
                    inv.hasTrap = false;
                    inv.isFull[i] = true;
                    Instantiate(itemButton, inv.slots[i].transform, false);
                    slot.count = 3;
                    slot.RefreshCount();
                    mvP1.StartWaterCoroutine();
                    Destroy(gameObject);

                    break;
                }
            }
        }
    }
}
