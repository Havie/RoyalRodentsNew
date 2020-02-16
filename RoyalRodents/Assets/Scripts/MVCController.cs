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


    public GameObject _lastClicked;
    public bool _isBuilding;

    public UIBuildMenu _BuildMenu;

    public bool checkingClicks;

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
        GameObject o = GameObject.FindGameObjectWithTag("BuildMenu");
        _BuildMenu = o.GetComponent<UIBuildMenu>();
        checkingClicks = true;
    }



    public void Update()
    {

    }

    /** Called from "Approve Costs" in UIButtonCosts Script
     * */
    public void buildSomething(string type)
    {
        if (_lastClicked == null)
        {
            Debug.Log("Last clicked is null");
            return;
        }
        //print("lastClicked: " + _lastClicked + " in BuildSomething");
        if (_lastClicked.GetComponent<BuildableObject>())
        {
            // Debug.Log("Found Buildable Object");
            _lastClicked.GetComponent<BuildableObject>().BuildSomething(type);
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

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(MouseRaw);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        //Old single way
        // LayerMask _LayerMask = (LayerMask.GetMask("Buildings"));

        // Gets the layer Mask Via Bitwise operations, then OR combines them.
        // This gets the "buildings" and "Player" Layer
        LayerMask _LayerMask = (1 << 8) | (1<<9);

        RaycastHit2D hit=  Physics2D.Raycast(mousePos, Vector2.zero, 19f, _LayerMask);


        if (hit.collider != null)
        {
            Debug.Log("Hit result:" + hit.collider.gameObject);
            if (_lastClicked == hit.collider.gameObject)
                return _lastClicked;


            _lastClicked = hit.collider.gameObject;

            if (_lastClicked.GetComponent<BuildableObject>())
            {
                // Debug.Log("Last Clicked is a buildingobj:" + lastClicked.name);
                BuildableObject buildObj = _lastClicked.GetComponent<BuildableObject>();
                buildObj.imClicked();
                _isBuilding = true;

                //There is a disconnect here, for clarity sake this script should be the only one responisible for telling the UI what to do,
                // However we will have to check the state of the building clicked from this script then instead
                if(!UIBuildMenu.isActiveStatic() && buildObj.getState()!= BuildableObject.BuildingState.Built)
                    _BuildMenu.showMenu(true, MouseRaw);
                return _lastClicked;
            }
            else if (_lastClicked.GetComponent<Button>())
            {
                Debug.LogError("Its a button");
            }
            // If Menu is active, and we click another object, we want to close the menu
            else if (UIBuildMenu.isActiveStatic())
            {
                _BuildMenu.showMenu(false, Vector3.zero);
                _isBuilding = false;
                _lastClicked = null;
                return null;
            }
            else
            {
                _isBuilding = false;
                _lastClicked = null;
                return null;
            }

        }
        // If Menu is active, and we click off of the object, we want to close the menu
        else if (UIBuildMenu.isActiveStatic())
        {
            _BuildMenu.showMenu(false, Vector3.zero);
            _isBuilding = false;
            _lastClicked = null;
        }
        return null;
    }

    /**
     * Returns True if the object is not a building or Button
     */
    public bool checkIfAttackable(Vector3 MouseLoc)
    {
        GameObject go = checkClick(MouseLoc);
        if (go)
            //May need to check later on that the building is the enemies
            if (go.GetComponent<BuildableObject>() || go.GetComponent<Button>())
                return false;
            else
                return true;

        return true;
    }
}

