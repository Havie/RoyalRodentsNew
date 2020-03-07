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

    public bool IgnoreClicks;

    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private UIAssignmentMenu _AssignmentMenu;
    private UIRecruitMenu _RecruitMenu;
    private List<Employee> _lastRedX = new List<Employee>();

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
        IgnoreClicks = true;

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
        IgnoreClicks = b;
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
    /** This function is now called by the Player in PlayerMovement
    *   Responsible for checking what was clicked
    */
    public GameObject checkClick(Vector3 MouseRaw)
    {
        if (_printStatements)
            Debug.Log("Check Click!");

        // Will detect UI Elements in the canvas
        CheckClicks(AlternateUITest(MouseRaw));
        // Chance we dont want to do anything
        if (!IgnoreClicks && !UIAssignmentMenu.Instance.isActive())
        {
            if (_RecruitMenu) // if the menus open, conditions to close it becuz we clicked off it
            {
                if (_RecruitMenu.isActive() && !_recruitDummy)
                {
                    showRecruitMenu(false, Vector3.zero, null, 0, 0);
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
        //perform a ray cast on player and buildings layer 
        RaycastHit2D hit=  RayCastPlayerAndBuildings(MouseRaw);
        //We found something
        if (hit.collider != null)
        {
            GameObject _TMPlastClicked = InspectHit(hit);

            //Alot of colliders are on the children, Need to check parent to get proper scripts
            if (_TMPlastClicked.transform.parent)
            {
                if (_printStatements)
                    Debug.LogWarning(_TMPlastClicked.transform.parent.gameObject + "   is parent clicked");


                //If Click Player Do a new RayCast here to avoid player/player Layer, so we can click through the player and ago radius - worried what will happen if we have multiple agro radius
                if (_TMPlastClicked.transform.parent.GetComponentInChildren<PlayerMovement>() || _TMPlastClicked.transform.GetComponent<AttackRadius>())
                {
                    if(_printStatements)
                         Debug.LogWarning("Found a warning click");
                    hit = RayCastBehindPlayer(MouseRaw);
                    if (hit.collider != null)
                    {
                        _TMPlastClicked = InspectHit(hit);
                    }

                }
                //Clicked the Portrait Employee Object - collider isn't on child, but it has a parent so its safe to do this in here
                if(CheckWorkerObject(_TMPlastClicked))
                {
                    GameObject go = FoundWorkerObj(_TMPlastClicked);
                    if (go)
                        return go;
                }
                // We clicked something tangible, not an agro collider 
                if (!_TMPlastClicked.transform.GetComponent<AttackRadius>())
                {
                    //prevents us from opening a menu accidentally
                    if (UIAssignmentMenu.Instance.isActive())
                        return null;

                    if (CheckRodent(_TMPlastClicked))
                    {
                        return FoundRodent(_TMPlastClicked);
                    }
                    else if (CheckSpawnVolume(_TMPlastClicked))
                    {
                        return FoundSpawnVolume(_TMPlastClicked);
                    }
                    else if(CheckBuilding(_TMPlastClicked))
                    {
                        return FoundBuilding(_TMPlastClicked);
                    }
                }
            }
            //we fell through the list of available objects, turn menus off
            if (_printStatements)
                Debug.Log("Fall through Case1");

            return TurnThingsoff();
        }

        //We clicked nothing, but if the assignment menu is active, do nothing
        if (UIAssignmentMenu.Instance.isActive())
        {
            if (_printStatements)
                Debug.Log("UI is On, Return Last clicked");
            return null;
        }
        else
        {
            //we clicked absolutely nothing, turn everything off 
            if (_printStatements)
                Debug.Log("Fall through Case2");
            return TurnThingsoff();
        }
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
                    //To-Do:
                    //play negative sound?
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
    public void setLastRedX(Employee redxHolder)
    {
       if (_printStatements)
            Debug.Log("set redX in MVC ::" + redxHolder);
        _lastRedX.Add(redxHolder);
    }
    public void showRedX(bool cond)
    {
        if (_printStatements)
            Debug.Log("MVC::ShowRedX::" + cond);

        if (_lastRedX.Count > 0)
            foreach (Employee e in _lastRedX)
            {
                 if (e.GetComponent<Employee>())
                     e.GetComponent<Employee>().ShowRedX(cond);
        }
    }
    public void showAssignmenu(bool cond)
    {
       // Debug.Log("ShowAssignMenu in MVC " + cond);
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
    public void Recruit(Rodent r, UIRecruitMenu menu)
    {
       // Debug.Log("MVC Rodent Recruit: " + _lastRodent);
        menu.showMenu(false, Vector3.zero, "", 0, 0);
        r.tag = "PlayerRodent"; //obsolete now
        r.Recruit();
        CheckClicks(true);
    }


    private bool AlternateUITest(Vector3 MouseRaw)
    {
        GraphicRaycaster gr = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GraphicRaycaster>();
        EventSystem es = GameManager.Instance.transform.GetComponentInChildren<EventSystem>();

        if (es == null)
            Debug.LogError("NO EventSys");

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
            Debug.LogError("NO GraphicRaycaster");

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            if(_printStatements)
                Debug.LogError("GraphicCaster Hit " + result.gameObject.name);
            if(result.gameObject.GetComponent<Button>())
            {
                if (_printStatements)
                    Debug.Log("Found a Button Setting clicks to false");

                if (result.gameObject.GetComponent<UIDraggableButton>())
                {

                    if (!result.gameObject.GetComponent<UIDraggableButton>().isSelected())
                    {
                        result.gameObject.GetComponent<UIDraggableButton>().imClicked();
                    }

                }
                else if (result.gameObject.GetComponent<UIAssignmentVFX>())
                {
                    result.gameObject.GetComponent<UIAssignmentVFX>().imClicked();
                }
                else if (result.gameObject.GetComponent<UIStaminaButton>())
                {
                    result.gameObject.GetComponent<UIStaminaButton>().imClicked();
                }
                else if (result.gameObject.GetComponent<UIAssignmentMovement>())
                {
                    result.gameObject.GetComponent<UIAssignmentMovement>().imClicked();
                }

                //Might need to check certain buttons scripts to set assignmentDummy=true;
                return false;
            }
            
        }
       if(results.Count<=0 && (_printStatements))
            Debug.LogWarning("We tried to GraphicRaycast UI and failed @" + m_PointerEventData.position);




        m_PointerEventData.position = (MouseRaw);
        results.Clear();
        EventSystem.current.RaycastAll(m_PointerEventData, results);
        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                if (_printStatements)
                    Debug.LogError("Alternate Hit " + result.gameObject.name);
                if (result.gameObject.GetComponent<Button>())
                {
                    if (_printStatements)
                        Debug.Log("Found a Button Setting clicks to false");

                    //Might need to check certain buttons scripts to set assignmentDummy=true;


                    return false;
                }
                else if(result.gameObject.GetComponent<bWorkerScript>())
                {
                    Debug.Log("Found B Worker Script");
                    result.gameObject.GetComponent<bWorkerScript>().imClicked();
                }
            }
        }
        else if (_printStatements)
            Debug.LogWarning("We tried to ALLRaycast UI and failed @" + m_PointerEventData.position);

        return true;
    }
    private RaycastHit2D RayCastPlayerAndBuildings(Vector3 MouseRaw)
    {
        if (_printStatements)
            Debug.Log("RayCastPlayerAndBuildings");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(MouseRaw);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        // Gets the layer Mask Via Bitwise operations
        // This gets the "player" and "buildings" layer, and fails at the UI layer
        LayerMask _LayerMask = (1 << 8) | (1 << 9) | (1 << 5);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 19f, _LayerMask);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 19f, _LayerMask);

        //Drawing a Ray doesn't work?
        //Debug.DrawRay(_startPos, _ourDir, Color.red);
        if (hit.collider!=null && _printStatements)
             Debug.Log("Initial Hit Found:" + hit.collider.gameObject);

        //Might be possible to write something here that ignores agro spheres and somehow
        //gets a better more valuable hit from hits

        if (hits.Length > 1)
        {
            Debug.LogWarning("Possible to Hit more than 1 thing??");
            foreach (var h in hits)
            {
                 Debug.Log("Found" + h.collider.gameObject);
            }
        }

        return hit;
    }
    private RaycastHit2D RayCastBehindPlayer(Vector3 MouseRaw)
    {
        if (_printStatements)
            Debug.Log("RayCastBehindPlayer");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(MouseRaw);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);


        //Old single way
        // LayerMask _LayerMask = (LayerMask.GetMask("Buildings"));

        // Gets the layer Mask Via Bitwise operations
        // This gets the "buildings"
        LayerMask _LayerMask = (1 << 9);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 19f, _LayerMask);

        //Debug.Log("Hit Dis:" + hit.distance);

        if (hit.collider != null)
        {
            if(_printStatements)
                Debug.Log("Secondary Hit Found:" + hit.collider.gameObject);
        }

        return hit;
    }
    private GameObject InspectHit(RaycastHit2D hit)
    {
        if (_printStatements)
            Debug.Log("Hit result:" + hit.collider.gameObject);
        if (_lastClicked == hit.collider.gameObject)
            return _lastClicked;

        if (_printStatements)
            Debug.Log("Enter");

       return hit.collider.gameObject;
    }
    private bool CheckBuilding(GameObject _TMPlastClicked)
    {
        return (_TMPlastClicked.transform.parent.GetComponent<BuildableObject>());
            
    }
    private bool CheckRodent(GameObject _TMPlastClicked)
    {
        return (_TMPlastClicked.transform.parent.GetComponent<Rodent>());

    }
    private bool CheckSpawnVolume(GameObject _TMPlastClicked)
    {
        return (_TMPlastClicked.transform.parent.GetComponent<SpawnVolume>());

    }
    private bool CheckWorkerObject(GameObject _TMPlastClicked)
    {
        return (_TMPlastClicked.transform.GetComponentInChildren<bWorkerScript>());

    }
    private GameObject FoundBuilding( GameObject _TMPlastClicked)
    {
            if (_printStatements)
                Debug.Log("Clicked a Building");
            // Debug.Log("Last Clicked is a building obj:" + lastClicked.name);
            BuildableObject buildObj = _TMPlastClicked.transform.parent.GetComponent<BuildableObject>();
            buildObj.imClicked();

            _AssignmentMenu.showMenu(false, null);
            showRedX(false);
            showRecruitMenu(false, Vector3.zero, "", 0, 0);

            _lastClicked = _TMPlastClicked;
            return _lastClicked;
    }
    private GameObject FoundRodent(GameObject _TMPlastClicked)
    {
        _lastRodent = _TMPlastClicked.transform.parent.GetComponent<Rodent>();
        if (_printStatements)
            Debug.Log("Clicked a Rodent");

        if (_lastRodent.getTeam() == 0)
        {
            _lastRodent.imClicked();
            _recruitDummy = true;
        }

        else if (_lastRodent.getTeam() == 1)
        {
            _lastRodent.imClicked();   
            _recruitDummy = true;
        }

        return _lastRodent.gameObject;
    }
    private GameObject FoundSpawnVolume(GameObject _TMPlastClicked)
    {
        _lastRodent = _TMPlastClicked.GetComponent<Rodent>();
        if (_printStatements)
            Debug.Log("Clicked a Rodent through spawn volume");

        if (_lastRodent.getTeam() == 0)
        {
            _lastRodent.imClicked();
            _recruitDummy = true;
        }

        else if (_lastRodent.getTeam() == 1)
        {
            _lastRodent.imClicked(); 
            _recruitDummy = true;
        }

        return _lastRodent.gameObject;
    }
    private GameObject FoundWorkerObj(GameObject _TMPlastClicked)
    {
        GameObject _owner = _TMPlastClicked.GetComponent<bWorkerScript>().getOwner();

        if (_owner)
        {
            if (_owner.GetComponent<BuildableObject>())
            {
                if (_printStatements)
                    Debug.Log("Clicked Worker Portrait (building)");
                _assignDummy = true;
                TurnThingsoff();

                _lastClicked = _owner;
                return _lastClicked;
            }
            else if (_owner.GetComponent<PlayerStats>())
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

        return null;
    }
}

