using System.Collections;
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
    private bool _isLocked=false;


    void Start()
    {
        setUpMenu();
       

        if(_onBuilding && _onPlayer)
        {
            Debug.LogWarning("This Worker script is set to be on both Player and Building, should only be one or the other");
        }
        figureOutOwner();
        if (_owner == null)
            Debug.LogError("Owner of bWorkerScript is null  :: " + this.transform.gameObject);
        if(_onBuilding && _owner)
            bo = _owner.GetComponent<BuildableObject>();
        else if(_onPlayer && _owner)
            ps= _owner.GetComponent<PlayerStats>();

        col = this.GetComponent<CircleCollider2D>();

    }
    private void setUpMenu()
    {
        _menu = MVCController.Instance.getAssignmentMenu();
        if (_menu == null)
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
    private void OnMouseDown()
    {
        
        if (!isOccupied())
        {
            //Debug.Log("Heard Not Occupied");
            //tell the MVC Controller which Building has been clicked

            MVCController.Instance.setLastClicked(_owner);

            // Need to Ask GameManager for a List of Player Rodents
            List<Rodent> _PlayerRodents = GameManager.Instance.getPlayerRodents();
            if (_menu)
                _menu.CreateButtons(_PlayerRodents);
            else
            {
                Debug.LogWarning("No AssignmentMenu, attempting to re-setup");
                setUpMenu();
                _menu.CreateButtons(_PlayerRodents);
            }
            if (bo)
            {
                bo.ShowRedX(false);
            }
            else if (ps)
            {
                ps.ShowRedX(false);
            }
            {
            }
        }
        else
        {
            // Option to dismiss current worker 
            //Debug.Log("Occupied");
            //Pull Up red X
            if (bo)
            {
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
            
        }
    }
    public void dismissRodent()
    {
        Debug.Log("heard Dismiss");
        if (bo)
            bo.DismissWorker(_worker);
        else if (ps)
            ps.DismissWorker(_worker);
        _worker = null;
        MVCController.Instance.showRedX(false);
    }
    public void ToggleCollider(bool cond)
    {
        if(col)
            col.enabled = cond;
        else
        {
            col = this.GetComponent<CircleCollider2D>();
            ToggleCollider(cond);
        }
    }
    private void figureOutOwner()
    {
        if(_onBuilding)
        {
            Transform parent = this.transform.parent;
            if(parent)
            {
                parent = parent.transform.parent;
                if(parent && parent.GetComponent<BuildableObject>())
                {
                    _owner = parent.gameObject;
                }
            }
        }
        else if (_onPlayer)
        {
            Transform parent = this.transform.parent;
            if (parent)
            {
                parent = parent.transform.parent;
                if (parent && parent.GetComponent<PlayerStats>())
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

    private void OnMouseEnter()
    {
        MVCController.Instance.CheckClicks(false);
    }
    private void OnMouseExit()
    {
        MVCController.Instance.CheckClicks(true);
    }
}
