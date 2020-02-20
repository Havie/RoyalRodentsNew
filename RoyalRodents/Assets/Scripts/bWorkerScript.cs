﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bWorkerScript : MonoBehaviour
{

    UIAssignmentMenu _menu;
    private Rodent _worker;
    private BuildableObject bo;
    // Start is called before the first frame update
    void Start()
    {
        setUpMenu();
        bo = this.transform.parent.GetComponent<BuildableObject>();
    }
    private void setUpMenu()
    {
        _menu = MVCController.Instance.getAssignmentMenu();
    }
    public void setWorker(Rodent r)
    {
        _worker = r;
    }
    public bool isOccupied()
    {
        return (_worker != null);
    }
    private void OnMouseDown()
    {
        
        if (!isOccupied())
        {
            //tell the MVC Controller which Building has been clicked

            MVCController.Instance.setLastClicked(this.transform.parent.gameObject);

            // Need to Ask GameManager for a List of Player Rodents
            List<Rodent> _PlayerRodents = GameManager.Instance.getPlayerRodents();
            if (_menu)
                _menu.CreateButton(_PlayerRodents);
            else
            {
                Debug.LogWarning("No AssignmentMenu, attempting to re-setup");
                setUpMenu();
                _menu.CreateButton(_PlayerRodents);
            }
            if (bo)
            {
                bo.ShowRedX(false);
            }
        }
        else
        {
            Debug.Log("Occupied");
            //To-Do: Option to dismiss current worker 

            //Pull Up red X
            if(bo)
            {
                bo.ShowRedX(true);
            }
            //give rodent new target, tell building its unmanned
        }
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
