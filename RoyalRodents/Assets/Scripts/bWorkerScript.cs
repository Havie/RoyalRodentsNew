using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bWorkerScript : MonoBehaviour
{

    UIAssignmentMenu _menu;
    // Start is called before the first frame update
    void Start()
    {
        _menu = MVCController.Instance.getAssignmentMenu();
    }

    private void OnMouseDown()
    {
        //tell the MVC Controller which Building has been clicked
       
        MVCController.Instance.setLastClicked(this.transform.parent.gameObject);

        // Need to Ask GameManager for a List of Player Rodents
        List<Rodent> _PlayerRodents=GameManager.Instance.getPlayerRodents();

        // Look for Available Rodents
        foreach(Rodent r in _PlayerRodents)
        {
            if(r.GetRodentStatus() == Rodent.eStatus.Available)
            {
                //do something / put into a Button
                Debug.Log(r.getName() + "  is Available");
                if(_menu)
                {
                    _menu.CreateButton(r);
                }
            }
        }
    }
}
