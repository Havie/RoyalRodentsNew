using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDebuggMobile : MonoBehaviour
{

    private void Start()
    {
        setMode(GameManager.Instance.getMobileMode());
    }

    public void setMode(bool cond)
    {
       // Debug.Log("called" + cond);
        this.transform.GetComponent<TextMeshProUGUI>().text += cond;
    }
}
