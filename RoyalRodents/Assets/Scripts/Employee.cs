using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Employee : MonoBehaviour
{
    public bool _onPlayer;
    private Vector3 _offSet;
    private Transform _Player;

    public GameObject _WorkerObj;
    public GameObject _PortraitOutline;
    public GameObject _RedX;

    private bool _Locked;
    private bool _Occupied;
    private Rodent _currentRodent;


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

        if(_onPlayer)
        {
            _Player = GameObject.FindGameObjectWithTag("Player").transform;
            //If the player moves right away it appears the offset gets messed up
            _offSet = this.transform.position - _Player.position;
        }

    }

    private void LateUpdate()
    {
        if(_onPlayer)
        {
            this.transform.position = _Player.position + _offSet;
        }
    }

    public void Assign(Rodent r)
    {
        if(!_Locked && !_Occupied)
        {
            //This script is on the portrait outline because its the visible clickable thing
            _PortraitOutline.GetComponent<bWorkerScript>().setWorker(r);
            _WorkerObj.GetComponent<SpriteRenderer>().sprite = r.GetPortrait();
            _Occupied = true;
            _currentRodent = r;
        }
    }

   public void Dismiss()
    {
        //Debug.Log("heard Dismiss In Employee");
        if (_Occupied)
        {
            if (_currentRodent)
                _currentRodent.setTarget(null);
            else
                Debug.LogWarning("Trying to dismiss a rodent thats not here?");

            // _PortraitOutline.GetComponent<bWorkerScript>().dismissRodent();
            _WorkerObj.GetComponent<SpriteRenderer>().sprite = null;
            _Occupied = false;
            _currentRodent = null;
            UIAssignmentMenu.Instance.ResetButtons();
        }
    }
    public void ShowRedX(bool cond)
    {
        //Debug.Log("Employee Show Red X:" + cond);

        if (cond)
        {
            _RedX.SetActive(true);
            MVCController.Instance.setLastRedX(this.gameObject);
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
}
