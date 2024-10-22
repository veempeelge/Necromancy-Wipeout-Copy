using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{
   void DisableDiedObject()
    {
        gameObject.SetActive(false);
    }
}
