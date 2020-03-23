using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDebuggPrints : MonoBehaviour
{
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
       // MVCController.Instance._debugger = this;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>()._debugger = this;
    }

    public void LogError(string s)
    {
        if (count < 20)
            this.GetComponent<TextMeshProUGUI>().text += "\n"+s;
        else
        {
            this.GetComponent<TextMeshProUGUI>().text = s;
            count = -1;
        }
        ++count;
    }
    public void Log(string s)
    {
        LogError(s);
    }
    public void LogWarning(string s)
    {
        LogError(s);
    }
  
}
