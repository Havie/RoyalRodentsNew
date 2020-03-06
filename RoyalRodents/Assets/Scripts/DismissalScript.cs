using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismissalScript : MonoBehaviour
{
    [SerializeField]
     private BuildableObject _bo; // I'm not even sure this is ever used ffs..
    [SerializeField]
    private  bWorkerScript _ws;

    public bool _onBuilding;
    public bool _onPlayer;

    // Start is called before the first frame update
    void Start()
    {
        Transform t = this.transform.parent;
        if (t)
        {
            _ws = t.GetComponentInChildren<bWorkerScript>();
            if (_ws)
            {
                GameObject go = _ws.getOwner();
                if (go)
                    _bo = go.GetComponent<BuildableObject>();
                else
                {
                    // bWorker hasn't finished its start setup, need delay
                    StartCoroutine(ReSetup());
                }
                FigureOutMode();
            }
        }

    }

    private void OnMouseDown()
    {
        //Debug.Log("!!!heardMouse Down in Dismissal!!");
        if (_ws)
            _ws.dismissRodent();
        else
        {
            StartCoroutine(ReSetup());
            Debug.Log("Dismal messed up");
          
        }
    }
    private void OnMouseEnter()
    {
       // MVCController.Instance.CheckClicks(false);
    }
    private void OnMouseExit()
    {
        //MVCController.Instance.CheckClicks(true);
    }
    //if bo is never used, this is irrelevant.. 
    private void FigureOutMode()
    {
        if(_ws)
        {
            if (_ws.getOwner() == null)
                return;
            if (_ws.getOwner().GetComponent<BuildableObject>())
                _onBuilding = true;
            else if (_ws.getOwner().GetComponent<PlayerStats>())
                _onPlayer = true;
        }
    }

    IEnumerator ReSetup()
    {
        yield return new WaitForSeconds(1f);
        if (_ws)
        {
            GameObject go = _ws.getOwner();
            if (go)
                _bo = go.GetComponent<BuildableObject>();
            else
                Debug.LogError("Still Cant find the damn Owner");
        }
        FigureOutMode();
    }
}
