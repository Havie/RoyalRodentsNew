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
    [SerializeField]
    private Rodent _lastRodent;
    [SerializeField]
    private GameObject _dummyObj;
    public bool _isBuilding;

    public bool checkingClicks;

    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private UIAssignmentMenu _AssignmentMenu;
    private UIRecruitMenu _RecruitMenu;
    private List<BuildableObject> _lastRedX=new List<BuildableObject>();

    private bool _recruitDummy;

    private bool _printStatements;

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

        if (_dummyObj == null)
        {
            Transform t = this.transform.GetChild(0);
            if (t)
                _dummyObj = t.gameObject;
            else
                Debug.LogError("MVC controller cant find Dummy Object");
        }

        //Debug Mode:
        _printStatements = false;
    }



    public void SetUpAssignmentMenu(UIAssignmentMenu am)
    {
        _AssignmentMenu = am;
    }
    public void SetUpBuildMenu(UIBuildMenu bm)
    {
        _BuildMenu = bm;
    }
    public void SetUpRecruitMenu(UIRecruitMenu rm)
    {
        _RecruitMenu = rm;
    }

    /* * Called from "Approve Costs" in UIButtonCosts Script* */
    public void MVCBuildSomething(string type)
    {
        if (_lastClicked == null)
        {
            if(_printStatements)
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

    public void MVCUpgradeSomething()
    {
        if (_lastClicked == null)
        {
            if (_printStatements)
                Debug.LogError("Last clicked is null");
            return;
        }
        if (_lastClicked.GetComponent<BuildableObject>())
        {
            if (_printStatements)
                Debug.Log("Found Buildable Object to Upgrade");
            _lastClicked.GetComponent<BuildableObject>().UpgradeSomething();
            CheckClicks(true);
        }
    }

    public void MVCDemolishSomething()
    {
        if (_lastClicked == null)
        {
            if (_printStatements)
                Debug.LogError("Last clicked is null");
            return;
        }
        if (_lastClicked.GetComponent<BuildableObject>())
        {
            if (_printStatements)
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
        if (_printStatements)
            Debug.Log("Check Click!");
        if (!checkingClicks)
            return _dummyObj;

        if (_printStatements)
            Debug.Log("Passed");


        _recruitDummy = false;
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
            if (_printStatements)
                Debug.Log("Hit result:" + hit.collider.gameObject);
            if (_lastClicked == hit.collider.gameObject)
                return _lastClicked;

            if (_printStatements)
                Debug.Log("Enter");
            GameObject _TMPlastClicked = hit.collider.gameObject;


            if (_TMPlastClicked.GetComponent<Rodent>())
            {
                _lastRodent = _TMPlastClicked.GetComponent<Rodent>();
                if(_printStatements)
                    Debug.Log("Clicked a Rodent");

                if (_lastRodent.tag.Equals("NeutralRodent"))
                {
                    showRecruitMenu(true, MouseRaw, _lastRodent.getName());
                    _recruitDummy = true;
                }

                //Show a new Menu to Recruit and Feed this Thing


                // clicking a rodent should also close other menus, my if/ elses are fucked up below need serious restructuring 
            }

            
            else if (_TMPlastClicked.GetComponent<BuildableObject>())
            {
                if (_printStatements)
                    Debug.Log("Case0");
                // Debug.Log("Last Clicked is a building obj:" + lastClicked.name);
                BuildableObject buildObj = _TMPlastClicked.GetComponent<BuildableObject>();
                buildObj.imClicked();
                _isBuilding = true;


                //We have found an building Object that is not the last one clicked
                // check the state of the building clicked 
                if (buildObj.getState() != BuildableObject.BuildingState.Built)
                {
                    if (_DestroyMenu.isActive())
                        ShowDestroyMenu(false, MouseRaw, _TMPlastClicked, buildObj);

                    ShowBuildMenu(true, MouseRaw, _TMPlastClicked, buildObj);
                }
                else
                {
                    if (_BuildMenu.isActive())
                        ShowBuildMenu(false, MouseRaw, _TMPlastClicked, buildObj);
                    //Cant Demolish TownCenter
                    //Will need to find a solution to pull up Upgrade Button on its own
                    if(buildObj.getType()!=BuildableObject.BuildingType.TownCenter)
                        ShowDestroyMenu(true, MouseRaw, _TMPlastClicked, buildObj);

                }

                _AssignmentMenu.showMenu(false);
                showRedX(false);

                _lastClicked = _TMPlastClicked;
                return _lastClicked;
            }
            // If a Menu is active, and we click another object, we want to close the menu
            else if (_BuildMenu.isActive() || _DestroyMenu.isActive() || _RecruitMenu.isActive())
            {
                ShowBuildMenu(false, Vector3.zero, null, null);
                ShowDestroyMenu(false, Vector3.zero, null, null);
                _RecruitMenu.showMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;

                if (_printStatements)
                    Debug.Log("Case1");

                    return null;
            }
            else if(_TMPlastClicked.transform.parent)
            {
                if (_TMPlastClicked.transform.parent.gameObject == _lastClicked)
                {
                    //case that last clicked was assigned by the worker Portrait
                    if(_printStatements)
                         Debug.Log("Worker Portrait");
                    _isBuilding = true;
                    return _dummyObj;
                }
                if (_printStatements)
                    Debug.Log("Fall through Case");
            }

            else
            {
                if (_printStatements)
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
                ShowBuildMenu(false, Vector3.zero, null, null);
                _isBuilding = false;
                _lastClicked = null;

                if (_printStatements)
                    Debug.Log("Case2");
            }
            else if (_DestroyMenu.isActive())
            {
                ShowDestroyMenu(false, Vector3.zero, null, null);
                _isBuilding = false;
                _lastClicked = null;

                if (_printStatements)
                    Debug.Log("Case3");
            }
            else if(_AssignmentMenu.isActive())
            {
                _AssignmentMenu.showMenu(false);
                _isBuilding = false;
                _lastClicked = null;

                if (_printStatements)
                    Debug.Log("Case4");
            }
            else if (_RecruitMenu.isActive() && !_recruitDummy)
            {
                showRecruitMenu(false, Vector3.zero, null);
                _isBuilding = false;
                _lastClicked = null;

                if (_printStatements)
                    Debug.Log("Case5");
            }

        }
        if (_printStatements)
            Debug.Log("Fell Through MVC");

        showRedX(false);
        return null;
    }

    /**
     * Returns True if the object is not a building or Button
     */
    public bool checkIfAttackable(Vector3 MouseLoc)
    {
        GameObject go = checkClick(MouseLoc);
        if(_printStatements)
            Debug.Log("Attackable checked Go is::" + go);
        if (go)
            //May need to check later on that the building is the enemies
            //Could add a check here if go==dummyobj then to turn off last redX if I wanted to go back to single variable instead of List<>
            if (go.GetComponent<BuildableObject>() || go == _dummyObj)
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
        if (_printStatements)
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
        if (_printStatements)
            Debug.Log("heard rodent Assigned " + _lastClicked + " is last clicked");

        //Might want to do some other checks, like the building state?
        if (_lastClicked)
        {
            BuildableObject _Building = _lastClicked.GetComponent<BuildableObject>();
            if (_Building)
            {
                if (_printStatements)
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
    public void setLastRedX(BuildableObject redx)
    {
        if (_printStatements)
            Debug.Log("set redX");
        _lastRedX.Add(redx);
    }
    public void showRedX(bool cond)
    {
        if (_lastRedX.Count > 0)
            foreach (BuildableObject b in _lastRedX)
            { b.ShowRedX(cond); }
    }
    public void showRecruitMenu(bool cond, Vector3 loc, string name)
    {
        if (_RecruitMenu)
            _RecruitMenu.showMenu(cond, loc, name);
        else
            Debug.LogError("MVC has no RecruitMenu");
    }
    public void ShowBuildMenu(bool cond, Vector3 loc, GameObject go, BuildableObject building )
    {
        if(_BuildMenu)
            _BuildMenu.showMenu(cond, loc, go, building);
    }
    public void ShowDestroyMenu(bool cond, Vector3 loc, GameObject go, BuildableObject building)
    {
        if (_DestroyMenu)
            _DestroyMenu.showMenu(cond, loc, go, building);
    }
    public void Recruit()
    {
       // Debug.Log("Recruit: " + _lastRodent);
        showRecruitMenu(false, Vector3.zero, "");
        _lastRodent.tag = "PlayerRodent";
        _lastRodent.Recruit();
    }
}

