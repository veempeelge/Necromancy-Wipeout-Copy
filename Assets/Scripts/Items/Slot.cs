using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private Inventory inv;
    public int i;
    [SerializeField] MovementPlayer1 player1;

    // Start is called before the first frame update
    private void Start()
    {
        inv = player1.GetComponent<Inventory>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.childCount <= 0)
        {
            inv.isFull[i] = false;
        }
    }
}
