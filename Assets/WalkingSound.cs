using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSound : MonoBehaviour
{

    [SerializeField] GameObject[] players;
    [SerializeField] AudioSource walking;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (players[0].GetComponent<Rigidbody>().velocity.magnitude < .1f || players[1].GetComponent<Rigidbody>().velocity.magnitude < .1f || players[2].GetComponent<Rigidbody>().velocity.magnitude < .1f)
        {
            walking.Play();
        }

    }
}
