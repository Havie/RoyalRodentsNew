﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bWorkerScript : MonoBehaviour
{
    public bool _onBuilding;
    public bool _onPlayer;
    UIAssignmentMenu _menu;
    private Rodent _worker;
    private BuildableObject bo;
    private Collider2D col;
    private PlayerStats ps;

    [SerializeField]
    private GameObject _owner;

    [SerializeField]
    private bool _isLocked = false;


    void Start()
    {
        setUpMenu();

        if (_onBuilding && _onPlayer)
        {
            Debug.LogWarning("This Worker script is set to be on both Player and Building, should only be one or the other");
        }
        figureOutOwner();
        if (_owner == null)
            Debug.LogError("Owner of bWorkerScript is null  :: " + this.transform.gameObject);

        if (_onBuilding && _owner)
            bo = _owner.GetComponent<BuildableObject>();
        else if (_onPlayer && _owner)
            ps = _owner.GetComponent<PlayerStats>();

        if (bo)
            col = this.GetComponent<CircleCollider2D>();
        if (ps)
            col = this.GetComponent<BoxCollider2D>();

    }
    private void setUpMenu()
    {
        //this seems unnecessary
            _menu = UIAssignmentMenu.Instance;
    }
    public void setWorker(Rodent r)
    {
        _worker = r;
    }
    public bool isOccupied()
    {
        return (_worker != null);
    }

    public void imClicked()
    {
        //Debug.LogWarning("MouseDownOnOWorker");

        if (!isOccupied())
        {
            //Debug.Log("Heard Not Occupied");
            //tell the MVC Controller which Building has been clicked
            //Becoming obsolete
            MVCController.Instance.setLastClicked(_owner);

            // Need to Ask GameManager for a List of Player Rodents
            List<Rodent> _PlayerRodents = GameManager.Instance.getPlayerRodents();
            if (_menu)
                _menu.CreateButtons(_PlayerRodents);
            else
            {
                //Debug.LogWarning("No AssignmentMenu, attempting to re-setup");
                setUpMenu();
                _menu.CreateButtons(_PlayerRodents);
            }
            if (bo)
            {
               // Debug.Log("We have a BO - show false");
                bo.ShowRedX(false);
            }
            else if (ps)
            {
                ps.ShowRedX(false);
            }


            //ShowMenu Manually here
            UIAssignmentMenu.Instance.showMenu(true, _owner);

            
        }
        else
        {
            // Option to dismiss current worker 
            //Debug.Log("Occupied");
            //Pull Up red X
            if (bo)
            {
              //  Debug.Log("We have a BO - show True");
                bo.ShowRedX(true);
                //Able to click the X
                ToggleCollider(false);
            }
            else if (ps)
            {
                ps.ShowRedX(true);
                //Able to click the X
                ToggleCollider(false);
            }
            //still activate the assignment menu for now
            UIAssignmentMenu.Instance.showMenu(true, _owner);
            // To-Do: in future might want to instead display info about the Rodent assigned?

        }
    }
    //unused, logic handled elsewhere in Employee now
    public void dismissRodent()
    {
       //Debug.Log("heard Dismiss");
        if (bo)
            bo.DismissWorker(_worker);
       else if (ps)
          ps.DismissWorker(_worker);
       _worker = null;
       MVCController.Instance.showRedX(false);
    }
    public void ToggleCollider(bool cond)
    {
        if (col!=null)
            col.enabled = cond;
        else
        {
            if(bo)
                col = this.GetComponent<CircleCollider2D>();
            if(ps)
                col= this.GetComponent<BoxCollider2D>();

            //IDK why this is blowing up - troubleshoot later
            //ToggleCollider(cond);
        }
    }
    private void figureOutOwner()
    {
        if (_onBuilding)
        {
            Transform parent = this.transform.parent;
            if (parent)
            {
                parent = parent.transform.parent;
                if (parent)
                {
                    parent = parent.transform.parent;
                    if (parent && parent.GetComponent<BuildableObject>())
                    {
                        _owner = parent.gameObject;
                    }
                }
            }
        }
        else if (_onPlayer)
        {
           // Debug.Log("Try to find player");
            Transform parent = GameObject.FindGameObjectWithTag("Player").transform;
            if (parent)
            {
                if (parent.GetComponent<PlayerStats>())
                {
                    _owner = parent.gameObject;
                }
            }
        }
    }

    public GameObject getOwner()
    {
        return _owner;
    }
}
