using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
   public GameObject _loading;


    public void TurnOff()
    {
        this.gameObject.SetActive(false);
        _loading.SetActive(true);
    }
}
