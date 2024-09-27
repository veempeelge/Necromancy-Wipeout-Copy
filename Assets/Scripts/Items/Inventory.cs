using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;

    public void DiscardItem(int slotIndex)
    {
        if (isFull[slotIndex])
        {
            foreach (Transform child in slots[slotIndex].transform)
            {
                Destroy(child.gameObject);
            }

            isFull[slotIndex] = false; 
        }
    }
}
