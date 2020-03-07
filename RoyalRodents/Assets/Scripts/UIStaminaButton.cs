using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStaminaButton : MonoBehaviour
{
    private PlayerStats _PS;
    private int _RestoreAmount = 10;

    public void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
        {
            _PS = player.GetComponent<PlayerStats>();
            if (_PS == null)
                Debug.LogWarning("Stamina Bar cant find Player Stats");
        }
    }

    public void imClicked()
    {
        if (ResourceManagerScript.Instance.Food > 0)
        {
            ResourceManagerScript.Instance.incrementFood(-1);
            _PS.IncrementStamina(_RestoreAmount);
        }
        //To:Do play sound 

    }
}
