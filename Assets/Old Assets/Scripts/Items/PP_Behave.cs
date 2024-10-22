using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP_Behave : MonoBehaviour
{
    private Inventory inv;
    public GameObject itemButton;

    private void Start()
    {
        StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            MovementPlayer1 mvP1 = other.gameObject.GetComponent<MovementPlayer1>();
            inv = mvP1.GetComponent<Inventory>();

            for (int i = 0; i < inv.slots.Length; i++)
            {
                if (inv.isFull[i] == false)
                {
                    inv.isFull[i] = true;
                    Instantiate(itemButton, inv.slots[i].transform, false);
                    if (gameObject.tag == "RollPin")
                    {
                        mvP1.ChangeStats(3, 2, 3, 2, 6, 2);
                    }
                    else if (gameObject.tag == "MeatHam")
                    {
                        mvP1.ChangeStats(5, 1, 3, 2, 10, 3);
                    }
                    else if (gameObject.tag == "BSpoon")
                    {
                        mvP1.ChangeStats(2, 3, 3, 3, 7, 3);
                    }
                    else if (gameObject.tag == "Book")
                    {
                        mvP1.ChangeStats(4, 4, 3, 3, 5, 2);
                    }

                    Destroy(gameObject);
                    break;
                }
                if (inv.isFull[i] == true)
                {
                    inv.DiscardItem(i);

                    // Add the new item
                    inv.isFull[i] = true;
                    Instantiate(itemButton, inv.slots[i].transform, false);
                    if (gameObject.tag == "RollPin")
                    {
                        mvP1.ChangeStats(3, 2, 5, 2, 6, 5);
                    }
                    else if (gameObject.tag == "MeatHam")
                    {
                        mvP1.ChangeStats(5, 1, 6, 2, 10, 3);
                    }
                    else if (gameObject.tag == "BSpoon")
                    {
                        mvP1.ChangeStats(2, 3, 5, 3, 7, 9);
                    }
                    else if (gameObject.tag == "Book")
                    {
                        mvP1.ChangeStats(4, 4, 5, 5, 5, 9);
                    }
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}
