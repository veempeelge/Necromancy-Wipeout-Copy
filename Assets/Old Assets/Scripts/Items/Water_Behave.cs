using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Behave : MonoBehaviour
{
    private bool canCollide = false;
    public float delayTime = 0.1f;

    private void Start()
    {
        StartCoroutine(EnableCollisionAfterDelay());
        StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    private IEnumerator EnableCollisionAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        canCollide = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!canCollide)
        {
            return;
        }
        else
        {
           //Debug.Log("Collided with: " + other.gameObject.name);
           // Destroy(gameObject);
        }
    }
}
