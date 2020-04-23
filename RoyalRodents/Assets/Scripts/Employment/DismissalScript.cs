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
            //Debug.Log("Found parent");
            _ws = t.GetComponentInChildren<bWorkerScript>();
            if (_ws)
            {
               // Debug.Log("Found WS");
                GameObject go = _ws.getOwner();
                if (go)
                    _bo = go.GetComponent<BuildableObject>();
                else
                {
                    // bWorker hasn't finished its start setup, need delay
                    StartCoroutine(ReSetup());
                   // Debug.Log("ReSetup");
                }
                FigureOutMode();
            }
            else
            {
                //Debug.Log("Cant find ws in children so Resetup2");
                StartCoroutine(ReSetup2());
            }
        }

    }


    public void imClicked()
    {
       // Debug.Log("!!!heardMouse Down in Dismissal!!");
        SoundManager.Instance.PlayClick();
        if (_ws)
            _ws.dismissRodent();
        else
        {
            StartCoroutine(ReSetup());
            //Debug.Log("Dismal messed up");
          
        }
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
            Debug.Log("ReSetup Found WS");
            GameObject go = _ws.getOwner();
            if (go)
                _bo = go.GetComponent<BuildableObject>();
            else
                Debug.LogError("Still Cant find the damn Owner");
        }
        FigureOutMode();
    }

    IEnumerator ReSetup2()
    {
        yield return new WaitForSeconds(1f);
        Transform t = this.transform.parent;
        if (t)
        {
           // Debug.Log("Found parent");
            _ws = t.GetComponentInChildren<bWorkerScript>();
            if (_ws)
            {
               // Debug.Log("Found WS");
                GameObject go = _ws.getOwner();
                if (go)
                    _bo = go.GetComponent<BuildableObject>();
                else
                {
                    // bWorker hasn't finished its start setup, need delay
                    StartCoroutine(ReSetup());
                    //Debug.Log("ReSetup");
                }
                FigureOutMode();
            }

        }
    }
}
