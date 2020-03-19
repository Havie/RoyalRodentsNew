using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Employee : MonoBehaviour
{
    private Vector3 _offSet;
    private Transform _Player;

    public GameObject _WorkerObj;
    public GameObject _PortraitOutline;
    public GameObject _RedX;

    [SerializeField]
    private bool _Locked;
    private bool _Occupied;
    private Rodent _currentRodent;

    private int test = 0;
    public SpriteRenderer sp;

    void Start()
    {
        //Hack But Lazy
        if (_WorkerObj == null)
          _WorkerObj=  this.transform.GetComponentInChildren<eWorkerOBJ>().gameObject;
        if (_PortraitOutline == null)
            _PortraitOutline = this.transform.GetComponentInChildren<ePortraitOutline>().gameObject;
        if (_RedX == null)
            _RedX = this.transform.GetComponentInChildren<eRedX>().gameObject;

        ShowRedX(false);

        sSaveManager.Instance.GatherPortaits(this);
    }
    private void OnDestroy()
    {
        sSaveManager.Instance.RemovePortraits(this);
    }
    private void OnEnable()
    {
        LoadDataFix();
    }

     // A work around to this would be to make it subscribe to the event system
    // when it triggers, if a current rodent is employed, enable game object, and assign portrait
   // possible flip back off if it was previous on enabled
    public void LoadDataFix()
    {
        if(_currentRodent)
          _WorkerObj.GetComponent<SpriteRenderer>().sprite = _currentRodent.GetPortrait();

       
    }

    public void Assign(Rodent r)
    {
       // Debug.Log("ASsign in Employee" + r.name + test);
        if(!_Locked && !_Occupied)
        {
            //Debug.Log("Pass");
           // Debug.Log("Assign in Employee" +r.getName());
            //This script is on the portrait outline because its the visible clickable thing
            _PortraitOutline.GetComponent<bWorkerScript>().setWorker(r);
            _WorkerObj.GetComponent<SpriteRenderer>().sprite = r.GetPortrait();

            //Stupid Bugg... makes no sense , something seems to break if u assign and the gameobject is turned off
            // Debug.Log("Portraitnameis " + r.GetPortrait());
            //print(_WorkerObj.GetComponent<SpriteRenderer>());
            // print(_WorkerObj.GetComponent<SpriteRenderer>().sprite);
            // sp = _WorkerObj.GetComponent<SpriteRenderer>();

            _Occupied = true;
            _currentRodent = r;
			r.SetJob(this);

            //subscribe to the event system - unused now
            //EventSystem.Instance.rodentDead += Dismiss;
        }
    }

   public void Dismiss(Rodent r)
    {
        //Debug.Log("heard Dismiss In Employee");
        if (_Occupied)
        {
            if (_currentRodent && _currentRodent==r)
            {
                _currentRodent.setTarget(null);
                _PortraitOutline.GetComponent<bWorkerScript>().setWorker(null);
                _WorkerObj.GetComponent<SpriteRenderer>().sprite = null;
                print("dismiss");
                _Occupied = false;
                _currentRodent = null;
                ShowRedX(false);
                UIAssignmentMenu.Instance.ResetButtons();

                //unsubscribe - unused now
                //EventSystem.Instance.rodentDead -= Dismiss;
            }
        }
    }
    public void ShowRedX(bool cond)
    {
        //Debug.Log("Employee Show Red X:" + cond);

        if (cond)
        {
            _RedX.SetActive(true);
            MVCController.Instance.setLastRedX(this);
        }
        else
        {
            _RedX.SetActive(false);
            //Turn back on the collider possible hack
            if (_PortraitOutline)
                _PortraitOutline.GetComponent<bWorkerScript>().ToggleCollider(true);
        }
    }

    public void Lock(bool cond)
    {
        _Locked = cond;

        if (_Locked)
            _WorkerObj.GetComponent<eWorkerOBJ>().Locked(true);
        else
            _WorkerObj.GetComponent<eWorkerOBJ>().Locked(false);
    }
    public bool isLocked()
    {
        return _Locked;
    }
    public Rodent getCurrentRodent()
    {
        return _currentRodent;
    }
    public bool isOccupied()
    {
        return _Occupied;
    }

    public void showPortraitOutline(bool cond)
    {
        if (_PortraitOutline)
            _PortraitOutline.gameObject.SetActive(cond);
    }

    public void showWorkerPortrait(bool cond)
    {
      //  Debug.Log("Heard show worker port-" + cond);
        if (_WorkerObj)
            _WorkerObj.gameObject.SetActive(cond);
    }
}
