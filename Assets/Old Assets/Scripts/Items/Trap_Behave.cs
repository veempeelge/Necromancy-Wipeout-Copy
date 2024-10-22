using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Trap_Behave : MonoBehaviour
{
    private Inv_Item inv;
    public GameObject itemButton;
    [SerializeField] AudioClip trapped;


    public bool isOn;
    private MovementPlayer1 mvP1;

    MeshFilter mesh;

    public void Start()
    {
        mesh = GetComponent<MeshFilter>();
        isOn = false;
    }

    private IEnumerator SetTrap()
    {
        GameObject tempCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Mesh capsuleMesh = tempCapsule.GetComponent<MeshFilter>().sharedMesh;

        Destroy(tempCapsule);

        if (capsuleMesh != null)
        {
            mesh.sharedMesh = capsuleMesh;
            yield return new WaitForSeconds(1);
            isOn = true;
        }
        else
        {
            Debug.LogError("Failed to create or find the Capsule mesh.");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            mvP1 = other.gameObject.GetComponent<MovementPlayer1>();

            if (isOn == false)
            {
                StartCoroutine(SetTrap());
            }
            else
            {
                StartCoroutine(RestrictMovement());
            }

 /*           if (gameObject.tag == "Trap")
            {
                Debug.Log("Player Trapped!");
            }
            else
            {
                inv = mvP1.GetComponent<Inv_Item>();

                for (int i = 0; i < inv.slots.Length; i++)
                {
                    if (inv.isFull[i] == false)
                    {
                        inv.hasTrap = true;
                        inv.hasWater = false;
                        inv.isFull[i] = true;
                        Instantiate(itemButton, inv.slots[i].transform, false);
                        Destroy(gameObject);
                        break;
                    }
                    if (inv.isFull[i] == true)
                    {
                        inv.DiscardItem(i);

                        inv.hasTrap = true;
                        inv.hasWater = false;
                        inv.isFull[i] = true;
                        Instantiate(itemButton, inv.slots[i].transform, false);
                        Destroy(gameObject);
                        break;
                    }
                }
            }*/
        }
    }

    private IEnumerator RestrictMovement()
    {
        mvP1.enabled = false;

        yield return new WaitForSeconds(3f);
        mvP1.enabled = true;
        Destroy(gameObject);
    }
}
