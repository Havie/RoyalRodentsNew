using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*Model-View-Controller
*Responsible for communication between game_world(Model) and UI(View)
* Keeps track of last clicked object.
* Is a Singleton/Instance 
*/
public class MVCController : MonoBehaviour
{
    private static MVCController _instance;

    [SerializeField]
    private GameObject _lastClicked;
    public bool _isBuilding;

    public bool checkingClicks;

    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private UIAssignmentMenu _AssignmentMenu;

    private bool _printStatement;

    public static MVCController Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<MVCController>();
            return _instance;
        }
    }


    void Start()
    {
        //Not doing any Null Checks here is bad practice
        GameObject o = GameObject.FindGameObjectWithTag("BuildMenu");
        _BuildMenu = o.GetComponent<UIBuildMenu>();
        o = GameObject.FindGameObjectWithTag("DestroyMenu");
        _DestroyMenu = o.GetComponent<UIBuildMenu>();
        checkingClicks = true;


        //Debugg Mode:
        _printStatement = false;
    }



    public void SetUpAssignmentMenu(UIAssignmentMenu am)
    {
        _AssignmentMenu = am;
    }
    public void SetUpBuildMenu(UIBuildMenu bm)
    {
        _BuildMenu = bm;
    }

    /** Called from "Approve Costs" in UIButtonCosts Script
     * */
    public void buildSomething(string type)
    {
        if (_lastClicked == null)
        {
            if(_printStatement)
                Debug.LogError("Last clicked is null");
            return;
        }
        //print("lastClicked: " + _lastClicked + " in BuildSomething");
        if (_lastClicked.GetComponent<BuildableObject>())
        {
            // Debug.Log("Found Buildable Object");
            _lastClicked.GetComponent<BuildableObject>().BuildSomething(type);
            CheckClicks(true);
        }

    }

    public void DemolishSomething()
    {
        if (_lastClicked == null)
        {
            if (_printStatement)
                Debug.LogError("Last clicked is null");
            return;
        }
        if (_lastClicked.GetComponent<BuildableObject>())
        {
            if (_printStatement)
                Debug.Log("Found Buildable Object");
            _lastClicked.GetComponent<BuildableObject>().DemolishSomething();
            CheckClicks(true);
        }

    }

    //unused currently but may need later
    public void CheckClicks(bool b)
    {
        checkingClicks = b;
    }
    /**This function is now called by the Player
    *Responsible for checking what was clicked, then notifying it if it needs to know
    */
    private GameObject checkClick(Vector3 MouseRaw)
    {
        if (_printStatement)
            Debug.Log("Check Click!");
        if (!checkingClicks)
            return null;

        if (_printStatement)
            Debug.Log("Passed");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(MouseRaw);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        //Old single way
        // LayerMask _LayerMask = (LayerMask.GetMask("Buildings"));

        // Gets the layer Mask Via Bitwise operations, then OR combines them.
        // This gets the "buildings" and "Player" Layer
        LayerMask _LayerMask = (1 << 8) | (1 << 9) | (1 << 5);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 19f, _LayerMask);


        if (hit.collider != null)
        {
            if (_printStatement)
                Debug.Log("Hit result:" + hit.collider.gameObject);
            if (_lastClicked == hit.collider.gameObject)
                return _lastClicked;

            if (_printStatement)
                Debug.Log("Enter");
            GameObject _TMPlastClicked = hit.collider.gameObject;

            if (_TMPlastClicked.GetComponent<BuildableObject>())
            {
                if (_printStatement)
                    Debug.Log("Case0");
                // Debug.Log("Last Clicked is a buildingobj:" + lastClicked.name);
                BuildableObject buildObj = _TMPlastClicked.GetComponent<BuildableObject>();
                buildObj.imClicked();
                _isBuilding = true;


                //We have found an building Object that is not the last one clicked
                // check the state of the building clicked 
                if (buildObj.getState() != BuildableObject.BuildingState.Built)
                {
                    if (_DestroyMenu.isActive())
                        _DestroyMenu.showMenu(false, MouseRaw, _TMPlastClicked);

                    _BuildMenu.showMenu(true, MouseRaw, _TMPlastClicked);
                }
                else
                {
                    if (_BuildMenu.isActive())
                        _BuildMenu.showMenu(false, MouseRaw, _TMPlastClicked);

                    _DestroyMenu.showMenu(true, MouseRaw, _TMPlastClicked);

                }

                // Does this belong here? Or better place?
                _AssignmentMenu.showMenu(false);

                _lastClicked = _TMPlastClicked;
                return _lastClicked;
            }
            // If a Menu is active, and we click another object, we want to close the menu
            else if (_BuildMenu.isActive() || _DestroyMenu.isActive())
            {
                _BuildMenu.showMenu(false, Vector3.zero, null);
                _DestroyMenu.showMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;
                if (_printStatement)
                    Debug.Log("Case1");

                    return null;
                // Does this belong here? Or better place?
               // _AssignmentMenu.showMenu(false);
            }
            else if(_TMPlastClicked.transform.parent)
            {
                if (_TMPlastClicked.transform.parent.gameObject == _lastClicked)
                {
                    //case that last clicked was assigned by the worker Portrait
                   // Debug.Log("Worker Portrait");
                    _isBuilding = true;
                    return null;
                }
                if (_printStatement)
                    Debug.Log("Fall through Case");
            }

            else
            {
                if (_printStatement)
                    Debug.LogError("else??");
                _AssignmentMenu.showMenu(false);
                _isBuilding = false;
                _lastClicked = null;
                return null;
            }

        }
       //else if (checkingClicks)// should only happen if we aren't hovering over a UI button
        { 

            //UI layer
            // If a Menu is active, and we click off of the object, we want to close the menu
            if (_BuildMenu.isActive())
            {
                _BuildMenu.showMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;
                if (_printStatement)
                    Debug.Log("Case2");
            }
            else if (_DestroyMenu.isActive())
            {
                _DestroyMenu.showMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;
                if (_printStatement)
                    Debug.Log("Case3");
            }
            else if(_AssignmentMenu.isActive())
            {
                _AssignmentMenu.showMenu(false);
                _isBuilding = false;
                _lastClicked = null;
                if (_printStatement)
                    Debug.Log("Case4");
            }

        }
        if (_printStatement)
            Debug.Log("Fell Through MVC");
        return null;
    }

    /**
     * Returns True if the object is not a building or Button
     */
    public bool checkIfAttackable(Vector3 MouseLoc)
    {
        GameObject go = checkClick(MouseLoc);
       // Debug.Log("Attackable checked Go is::" + go);
        if (go)
            //May need to check later on that the building is the enemies
            if (go.GetComponent<BuildableObject>() || go.GetComponent<Button>())
                return false;
            else
                return true;

        return true;
    }

    public GameObject getLastClicked()
    {
        return _lastClicked;
    }
    public void setLastClicked(GameObject o)
    {
        if (_printStatement)
            Debug.Log("setLast to" + o);
        _lastClicked = o;
    }
    public void clearLastClicked()
    {
        _lastClicked = null;
    }

    public UIAssignmentMenu getAssignmentMenu()
    {
        return _AssignmentMenu;
    }
    public void RodentAssigned(Rodent r)
    {
        if (_printStatement)
            Debug.Log("heard rodent Assigned " + _lastClicked + " is last clicked");

        //Might want to do some other checks, like the building state?
        if (_lastClicked)
        {
            BuildableObject _Building = _lastClicked.GetComponent<BuildableObject>();
            if (_Building)
            {
                if (_printStatement)
                    Debug.Log("enter obj");

                //Rodent Things
                r.setTarget(_lastClicked);
                _Building.AssignWorker(r);

                //To:Do update Rodent Status


                clearLastClicked();
                _AssignmentMenu.showMenu(false);
                CheckClicks(true);
            }
        }
    }
}

