using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UITroopSelection : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI _amounts;

    // Start is called before the first frame update
    void Start()
    {
        _amounts= GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowSelection(bool cond)
    {
        this.gameObject.SetActive(cond);
    }
}
