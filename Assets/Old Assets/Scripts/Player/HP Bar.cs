using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    
    [SerializeField] Image HPCurrentBar, DurabilityCurrentBar;
    [SerializeField] float HPFull;
    [SerializeField] float HPCurrent;
    float durabilityFull;
    float durabilityCurrent;
    [SerializeField] MovementPlayer1 player1;
    [SerializeField] Camera cam;

    int currentWater;
    int maxWater;
    [SerializeField] Image waterCurrent;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindAnyObjectByType<Camera>();   
        HPFull = player1.MaxHP;
        HPCurrent = player1.currentHP;

        durabilityFull = player1.weaponDurability;
        durabilityCurrent = player1.weaponCurrentDurability;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cam.transform.rotation;
    }

    public void UpdateBar(float updatedHealth)
    {
        HPCurrent = updatedHealth;
        HPCurrentBar.fillAmount = HPCurrent / HPFull;
    }

    public void UpdateDurabilityBar(float updatedFullDurability, float updatedCurrentDurability)
    {
        durabilityCurrent = updatedCurrentDurability;
        durabilityFull = updatedFullDurability;
        DurabilityCurrentBar.fillAmount = durabilityCurrent / durabilityFull;
    }

    public void UpdateWater(int updatedFullWater, int updatedCurrentWater)
    {
        currentWater = updatedCurrentWater;
        maxWater = updatedFullWater;
        waterCurrent.fillAmount = updatedCurrentWater / updatedFullWater;   
    }
}
