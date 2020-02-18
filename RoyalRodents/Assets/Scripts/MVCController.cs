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
            Debug.LogError("Last clicked is null");
            return;
        }
        //print("lastClicked: " + _lastClicked + " in BuildSomething");
        if (_lastClicked.GetComponent<BuildableObject>())
        {
            // Debug.Log("Found Buildable Object");
            _lastClicked.GetComponent<BuildableObject>().BuildSomething(type);
        }

    }

    public void DemolishSomething()
    {
        if (_lastClicked == null)
        {
            Debug.LogError("Last clicked is null");
            return;
        }
        if (_lastClicked.GetComponent<BuildableObject>())
        {
            // Debug.Log("Found Buildable Object");
            _lastClicked.GetComponent<BuildableObject>().DemolishSomething();
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
        LayerMask _LayerMask = (1 << 8) | (1 << 9) | (1 << 5);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 19f, _LayerMask);


        if (hit.collider != null)
        {
           // Debug.Log("Hit result:" + hit.collider.gameObject);
            if (_lastClicked == hit.collider.gameObject)
                return _lastClicked;

            // Debug.Log("Enter");
            GameObject _TMPlastClicked = hit.collider.gameObject;

            if (_TMPlastClicked.GetComponent<BuildableObject>())
            {
               // Debug.Log("Case0");
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


                _lastClicked = _TMPlastClicked;
                return _lastClicked;
            }
            else if (_TMPlastClicked.GetComponent<Button>())
            {
                Debug.LogError("Its a button");
            }
            // If a Menu is active, and we click another object, we want to close the menu
            else if (_BuildMenu.isActive() || _DestroyMenu.isActive())
            {
                _BuildMenu.showMenu(false, Vector3.zero, null);
                _DestroyMenu.showMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;
               // Debug.Log("Case1");
            }
            else
            {
                //Debug.LogError("else??");
                _isBuilding = false;
                _lastClicked = null;
                return null;
            }

        }
       else if (checkingClicks)// should only happen if we arent hovering over a UI button
        { 

            //UI layer
            // If a Menu is active, and we click off of the object, we want to close the menu
            if (_BuildMenu.isActive())
            {
                _BuildMenu.showMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;
                Debug.Log("Case2");
            }
            else if (_DestroyMenu.isActive())
            {
                _DestroyMenu.showMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;
                Debug.Log("Case3");
            }


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

    public GameObject getLastClicked()
    {
        return _lastClicked;
    }
    public void clearLastClicked()
    {
        _lastClicked = null;
    }
}

