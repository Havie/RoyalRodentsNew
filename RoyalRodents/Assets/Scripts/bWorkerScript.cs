using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bWorkerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
       // Debug.Log("WorkScriptActive");

        // Need to Ask GameManager for a List of Player Rodents
        List<Rodent> _PlayerRodents=GameManager.Instance.getPlayerRodents();

        // Look for Available Rodents
        foreach(Rodent r in _PlayerRodents)
        {
            if(r.GetRodentStatus() == Rodent.eStatus.Available)
            {
                //do something / put into a Button
                Debug.Log(r.getName() + "  is Available");
            }
        }
    }
}
