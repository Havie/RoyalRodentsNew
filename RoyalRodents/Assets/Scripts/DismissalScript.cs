using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismissalScript : MonoBehaviour
{
    BuildableObject _bo;
    bWorkerScript _ws;

    // Start is called before the first frame update
    void Start()
    {
        Transform t = this.transform.parent;
        if(t)
            _bo = t.GetComponent<BuildableObject>();

        if(_bo)
        {
            _ws= _bo.GetComponentInChildren<bWorkerScript>();
        }
    }

    private void OnMouseDown()
    {
       // Debug.Log("!!!heardMouse Down in Dismissal!!");
        if(_ws)
        _ws.dismissRodent();
    }
    private void OnMouseEnter()
    {
        MVCController.Instance.CheckClicks(false);
    }
    private void OnMouseExit()
    {
        MVCController.Instance.CheckClicks(true);
    }
}
