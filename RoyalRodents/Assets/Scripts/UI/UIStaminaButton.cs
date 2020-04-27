using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStaminaButton : MonoBehaviour
{
    private PlayerStats _PS;
    private int _RestoreAmount = 10;

    private bool clickTimer;

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
        if (clickTimer)
            return;

        StartCoroutine(clickDelay());
        if (ResourceManagerScript.Instance.GetResourceCount(ResourceManagerScript.ResourceType.Food) > 0 && _PS.getStamina() <= _PS.getStaminaMax() - _RestoreAmount)
        {
            ResourceManagerScript.Instance.incrementResource(ResourceManagerScript.ResourceType.Food, -1);
            _PS.IncrementStamina(_RestoreAmount);
        }
        // play sound 
        SoundManager.Instance.PlayClick();

    }

    private IEnumerator clickDelay()
    {
        clickTimer = true;
        yield return new WaitForSeconds(1);
        clickTimer = false;
    }
}
