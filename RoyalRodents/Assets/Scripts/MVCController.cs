using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public GameObject _dummyObj;
    public bool _isBuilding;

    public bool checkingClicks;

    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private UIAssignmentMenu _AssignmentMenu;
    private UIRecruitMenu _RecruitMenu;
    private List<GameObject> _lastRedX = new List<GameObject>();

    private bool _recruitDummy;
    private bool _assignDummy;

    private BaseHitBox _lastColliderOFF;

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
            if (_printStatements)
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


    public void CheckClicks(bool b)
    {
        if (_printStatements)
            Debug.Log("Were Told to check clicks::" + b);
        checkingClicks = b;
    }

    IEnumerator ClickDelay()
    {
        CheckClicks(false);
        yield return new WaitForSeconds(0.5f);
        CheckClicks(true);
    }

    public void ShowDummy(bool cond, Vector3 loc)
    {
        _dummyObj.gameObject.SetActive(cond);
        _dummyObj.transform.position = loc;
    }
    public void rememberHitBox(BaseHitBox hitbox)
    {
        if (_lastColliderOFF)
            _lastColliderOFF.turnOnCollider(true);
        _lastColliderOFF = hitbox;
    }


    /**This function is now called by the Player
    *Responsible for checking what was clicked, then notifying it if it needs to know
    */
    public GameObject checkClick(Vector3 MouseRaw)
    {
        if (_printStatements)
            Debug.Log("Check Click!");

        //The following will detect UI Elements in the canvas
        CheckClicks(AlternateUITest(MouseRaw));

        if (!checkingClicks && !UIAssignmentMenu.Instance.isActive())
        {
            if (_RecruitMenu)
            {
                if (_RecruitMenu.isActive() && !_recruitDummy)
                {
                    showRecruitMenu(false, Vector3.zero, null, 0, 0);
                    _isBuilding = false;
                    _lastClicked = null;

                    if (_printStatements)
                        Debug.Log("Case00");
                }
            }
            if (_printStatements)
                Debug.Log("Auto Return Dummy OBJ because were not checking clicks");
            return _dummyObj;
        }



        if (_printStatements)
            Debug.Log("Passed");

        //used to keep track of if a menu needs to stay open
        _recruitDummy = false;
        _assignDummy = false;

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

           // if (_TMPlastClicked == _dummyObj)
               // return _dummyObj;


            if (_TMPlastClicked.transform.parent)
            {
                if (_printStatements)
                    Debug.LogWarning(_TMPlastClicked.transform.parent.gameObject + "   is parent clicked");

                if (_TMPlastClicked.transform.parent.GetComponent<Rodent>())
                {
                    _lastRodent = _TMPlastClicked.transform.parent.GetComponent<Rodent>();
                    if (_printStatements)
                        Debug.Log("Clicked a Rodent");

                    if (_lastRodent.getTeam() == 0)
                    {
                        // showRecruitMenu(true, MouseRaw, _lastRodent.getName(), _lastRodent.getRecruitmentCost(), _lastRodent.getPopulationCost());
                        _lastRodent.imClicked();
                        _recruitDummy = true;
                    }

                    else if (_lastRodent.getTeam() == 1)
                    {
                        // showKingGuardMenu(true, MouseRaw, _lastRodent.getName());
                        _lastRodent.imClicked();   // can probably combine now
                        _recruitDummy = true;
                    }


                }
               else if (_TMPlastClicked.transform.parent.GetComponent<BuildableObject>())
                {
                    if (_printStatements)
                        Debug.Log("Case0");
                    // Debug.Log("Last Clicked is a building obj:" + lastClicked.name);
                    BuildableObject buildObj = _TMPlastClicked.transform.parent.GetComponent<BuildableObject>();
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
                        if (buildObj.getType() != BuildableObject.BuildingType.TownCenter)
                            ShowDestroyMenu(true, MouseRaw, _TMPlastClicked, buildObj);

                    }

                    _AssignmentMenu.showMenu(false, null);
                    showRedX(false);
                    showRecruitMenu(false, Vector3.zero, "", 0, 0);

                    _lastClicked = _TMPlastClicked;
                    return _lastClicked;
                }
            }
            // check if it was a portrait  
            if (_TMPlastClicked.GetComponent<bWorkerScript>())
            {

                GameObject _owner = _TMPlastClicked.GetComponent<bWorkerScript>().getOwner();

                if (_owner)
                {
                    if (_owner.GetComponent<BuildableObject>())
                    {
                        if(_printStatements)
                            Debug.Log("Worker Portrait (building)");
                        _assignDummy = true;
                        TurnThingsoff();

                        _lastClicked = _owner;
                        return _lastClicked;
                    }
                    else if(_owner. GetComponent<PlayerStats>())
                    {
                        if (_printStatements)
                            Debug.Log("Worker Portrait (player)");
                        _assignDummy = true;
                        TurnThingsoff();

                        _lastClicked = _owner;
                        return _lastClicked;
                    }
                    Debug.Log("Owner Fallthru==" + _owner);
                }
                if (_printStatements)
                    Debug.Log("Fall through Case 00" + _TMPlastClicked);
            }

            if (_printStatements)
                Debug.Log("Fall through Case1");

            _isBuilding = false;
            return TurnThingsoff();
            //return null;
        }




        //UI layer
        if (UIAssignmentMenu.Instance.isActive())
        {
            if (_printStatements)
                Debug.Log("UI is On, Return Last clicked");

            // ??? _isBuilding = false;
            return null;
        }
        else
        {

            if (_printStatements)
                Debug.Log("Fall through Case2");

            _isBuilding = false;
            return TurnThingsoff();
        }
    }

    /**
     * Returns True if the object is not a building or Button
     */
    public bool checkIfAttackable(Vector3 MouseLoc)
    {
        GameObject go = checkClick(MouseLoc);
        if (_printStatements)
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
        _recruitDummy = false;
        StartCoroutine(ClickDelay());
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
                //Check if this building is occupied
                if (_Building.CheckOccupied())
                {
                    
                }
                else // free to assign 
                {
                    //Rodent Things , status update etc
                    //r.setTarget(_lastClicked);
                    _Building.AssignWorker(r);

                    clearLastClicked();

                    // Dont want menu to close so we can keep assigning in the mode
                    //_AssignmentMenu.showMenu(false);

                    //instead reset the buttons
                    UIAssignmentMenu.Instance.ResetButtons();

                    /* Keeping this off allows us to click once to pull up RedX
                     * Menu immediately after assigned
                     * Unknown if causes any other issues, onMouseExit from 
                     * portrait / bworkerscript should re enabled properly
                     * If having trouble, can try turning back on */
                    //CheckClicks(true);
                }
            }
            else
            {
                if (_printStatements)
                    Debug.Log("Assign to PLayer");
                PlayerStats Player = _lastClicked.GetComponent<PlayerStats>();
                if(Player)
                {
                    Player.AssignWorker(r);

                    //Need a check to see if he can be assigned
                    // r.setTarget(_lastClicked);
                    clearLastClicked();

                    UIAssignmentMenu.Instance.ResetButtons();

                }
            }
        }
    }
    public GameObject TurnThingsoff()
    {
        // If a Menu is active, and we click off of the object, we want to close the menu
        if (!_recruitDummy)
            showRecruitMenu(false, Vector3.zero, null, 0, 0);
        if (!_assignDummy)
            showAssignmenu(false);

        ShowBuildMenu(false, Vector3.zero, null, null);

        ShowDestroyMenu(false, Vector3.zero, null, null);

        showRedX(false);

        clearLastClicked();

        if(_lastColliderOFF)
            _lastColliderOFF.turnOnCollider(true);

        return null;
    }



    public void setLastRedX(GameObject redxHolder)
    {
        if (_printStatements)
            Debug.Log("set redX");
        _lastRedX.Add(redxHolder);
    }
    public void showRedX(bool cond)
    {
        if (_printStatements)
            Debug.Log("MVC::ShowRedX::" + cond);

        if (_lastRedX.Count > 0)
            foreach (GameObject g in _lastRedX)
            {
                if(g.GetComponent<BuildableObject>())
                    g.GetComponent<BuildableObject>().ShowRedX(cond);
                else if(g.GetComponent<PlayerStats>())
                     g.GetComponent<PlayerStats>().ShowRedX(cond);
            }
    }
    public void showAssignmenu(bool cond)
    {
        if (_AssignmentMenu && !_assignDummy)
            _AssignmentMenu.showMenu(cond, _lastClicked);  // TO-DO: NEED TO PHASE OUT?
    }
    public void showRecruitMenu(bool cond, Vector3 loc, string name, int foodCost, int popCost)
    {
        if (_RecruitMenu)
            _RecruitMenu.showMenu(cond, loc, name, foodCost, popCost);
    }
    public void showKingGuardMenu(bool cond, Vector3 loc, string name)
    {
        if (_RecruitMenu)
            _RecruitMenu.showKingGuardMenu(cond, loc, name);
    }
    public void ShowBuildMenu(bool cond, Vector3 loc, GameObject go, BuildableObject building)
    {
        if (_BuildMenu)
            _BuildMenu.showMenu(cond, loc, go, building);
    }
    public void ShowDestroyMenu(bool cond, Vector3 loc, GameObject go, BuildableObject building)
    {
        if (_DestroyMenu)
            _DestroyMenu.showMenu(cond, loc, go, building);
    }
    public void SetAssignmentDummy(bool cond)
    {
        _assignDummy = cond;
    }
    //Old functionality
    public void Recruit()
    {
        // Debug.Log("Recruit: " + _lastRodent);
        showRecruitMenu(false, Vector3.zero, "", 0, 0);
        _lastRodent.tag = "PlayerRodent";
        _lastRodent.Recruit();
        CheckClicks(true);
    }
    public void Recruit(Rodent r, UIRecruitMenu menu)
    {
       // Debug.Log("MVC Rodent Recruit: " + _lastRodent);
        menu.showMenu(false, Vector3.zero, "", 0, 0);
        r.tag = "PlayerRodent";
        r.Recruit();
        CheckClicks(true);
    }


    private bool AlternateUITest(Vector3 MouseRaw)
    {
        GraphicRaycaster gr = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GraphicRaycaster>();
        EventSystem es = GameManager.Instance.transform.GetComponentInChildren<EventSystem>();

        if (es == null)
            Debug.Log("NO EventSys");

        //Set up the new Pointer Event
        PointerEventData m_PointerEventData = new PointerEventData(es);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = MouseRaw;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();
        //RayCast it
        if (gr)
            gr.Raycast(m_PointerEventData, results);
        else
            Debug.Log("NO GraphicRaycaster");

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            Debug.LogError("GraphicCaster Hit " + result.gameObject.name);
            if(result.gameObject.GetComponent<Button>())
            {
                Debug.Log("Found a Button Setting clicks to false");

                //Might need to check certain buttons scripts to set assignmentDummy=true;


               return false;
            }
        }
       if(results.Count<=0)
            Debug.LogError("We tried to GraphicRaycast UI and failed @" + m_PointerEventData.position);




        m_PointerEventData.position = (MouseRaw);
        results.Clear();
        EventSystem.current.RaycastAll(m_PointerEventData, results);
        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                Debug.LogError("Alternate Hit " + result.gameObject.name);
                if (result.gameObject.GetComponent<Button>())
                {
                    Debug.Log("Found a Button Setting clicks to false");

                    //Might need to check certain buttons scripts to set assignmentDummy=true;


                    return false;
                }
            }
        }
        else
            Debug.LogError("We tried to ALLRaycast UI and failed @" + m_PointerEventData.position);

        return true;
    }
}

