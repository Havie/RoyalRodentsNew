using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildableObject : MonoBehaviour, IDamageable<float>, DayNight
{
    #region sprites
    [SerializeField] private Sprite _sStatedefault;
    [SerializeField] private Sprite _sStateHighlight;
    [SerializeField] private Sprite _sStateConstruction;
    [SerializeField] private Sprite _sStateDamaged;
    [SerializeField] private Sprite _sStateDestroyed;
    [SerializeField] private Sprite _sOnHover;
    [SerializeField] private Sprite _sNotification;
    [SerializeField] private Sprite _sBuildingHammer;
    #endregion

    [SerializeField] private GameObject _NotificationObject;

    private SpriteRenderer _sr;
    private SpriteRenderer _srNotify;

    public enum BuildingState { Available, Idle, Building, Built };
    public enum BuildingType { House, Farm, Outpost, Banner, TownCenter, Vacant, GarbageCan, WoodPile, StonePile }
    [SerializeField] private BuildingState eState;
    [SerializeField] private BuildingType eType;
    private int _level = 0;

    [SerializeField]
    private float _hitpoints = 0;
    [SerializeField]
    private float _hitpointsMax = 5;

    [SerializeField]
    private int _construction = 0;
    [SerializeField]
    private int _constructionMax = 100;
    private GameObject _ConstructionBarObj;
    private HealthBar _ConstructionBar;

    // NEW
    public Employee[] _Workers = new Employee[1];

    [SerializeField]
    private int _Team = 0; // 0 is neutral, 1 is player, 2 is enemy
    [SerializeField]
    private int _ID = 0;

    #region otherClasses
    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private MVCController _controller;
    private CameraController _cameraController;
    #endregion

    #region InterfaceStuff
    [SerializeField] private Animator _animator;
    private HealthBar _HealthBar;
    [SerializeField] private GameObject _HealthBarObj;
    public void Damage(float damageTaken)
    {
        if (_hitpoints - damageTaken > 0)
            _hitpoints -= damageTaken;
        else
            _hitpoints = 0;

        UpdateHealthBar();
    }
    public void SetUpHealthBar(GameObject go)
    {
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("HealthBarCanvas");
        if (_HealthBarObj)
        {
            //which comes first the chicken or the egg...
            _HealthBarObj = Instantiate(go);
            _HealthBarObj.gameObject.transform.SetParent(this.transform);
            _HealthBar = _HealthBarObj.GetComponentInChildren<HealthBar>();
            if (!_HealthBar)
                Debug.LogError("Cant Find Health bar");
            _HealthBarObj.transform.SetParent(this.transform);
            _HealthBarObj.transform.localPosition = new Vector3(0, 0.75f, 0);
        }
        else
            Debug.LogError("Cant Find Health bar Prefab");
    }
    public void UpdateHealthBar()
    {
        if (_HealthBar)
            _HealthBar.SetFillAmount(_hitpoints / _hitpointsMax);
    }
    public void SetUpConstructionBar(GameObject go)
    {
        if (_ConstructionBarObj != null)
        {
            _ConstructionBarObj = Instantiate(go);
            _ConstructionBarObj.gameObject.transform.SetParent(this.transform);
            _ConstructionBar = _ConstructionBarObj.GetComponentInChildren<HealthBar>();
            if (!_ConstructionBar)
                Debug.LogError("Cant Find Construction bar");
            _ConstructionBarObj.transform.SetParent(this.transform);
            _ConstructionBarObj.transform.localPosition = new Vector3(0, 0.55f, 0);
        }
        else
            Debug.LogError("Cant Find Construction bar Prefab");

        UpdateConstructionBar();
    }
    public void UpdateConstructionBar()
    {
        if (_ConstructionBar)
            _ConstructionBar.SetFillAmount((float)_construction / _constructionMax);
    }

    public void SetUpDayNight()
    {
        if (this.transform.gameObject.GetComponent<Register2DDN>() == null)
            this.transform.gameObject.AddComponent<Register2DDN>();
    }
    #endregion

    public void LoadData(int ID, int type, int state, int lvl, float hp, float hpmax)
    {
        if (_ID != ID)
            Debug.LogWarning("Building IDs dont match up!..Save Data Corrupted");

        //_hitpoints = hp;
        // _hitpointsMax = hpmax;
        _level = lvl;

        eState = (BuildingState)state;
        eType = (BuildingType)type;
        LoadComponents();
        UpdateState();
        UpdateHealthBar();
        _HealthBar.gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        _sr = this.transform.GetComponent<SpriteRenderer>();
        _sStatedefault = Resources.Load<Sprite>("Buildings/DirtMound/dirt_mound_final");


        //SetUp the NotifyObj
        _srNotify = _NotificationObject.transform.GetComponent<SpriteRenderer>();
        _srNotify.sprite = _sNotification;


        if (eType != BuildingType.TownCenter && eType != BuildingType.GarbageCan && eType != BuildingType.WoodPile && eType != BuildingType.StonePile)
        {
            eState = BuildingState.Available;
            eType = BuildingType.Vacant;
        }
        if (eType == BuildingType.Vacant)
            _sr.sprite = _sStatedefault;
        _animator = GetComponentInChildren<Animator>();


        //Other classes
        GameObject o = GameObject.FindGameObjectWithTag("BuildMenu");
        _BuildMenu = o.GetComponent<UIBuildMenu>();
        o = GameObject.FindGameObjectWithTag("DestroyMenu");
        _DestroyMenu = o.GetComponent<UIBuildMenu>();
        _cameraController = Camera.main.GetComponent<CameraController>();


        //little unnecessary
        _controller = MVCController.Instance;

        //Health Bar Prefab
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("UI/HealthBarCanvas");
        SetUpHealthBar(_HealthBarObj.gameObject);

        //Construction Bar Prefab
        if (_ConstructionBarObj == null)
            _ConstructionBarObj = Resources.Load<GameObject>("UI/ConstructionBarCanvas");
        SetUpConstructionBar(_ConstructionBarObj);

        if (_Team < 3)
            setTeam(500); // default value for destroyed state
        UpdateState();
        setUpWorkers();
        UpdateHealthBar();

        //Feel like these could load in a different order on start
        _ID = GameManager.Instance.getBuildingIndex();
        // Debug.Log(this.gameObject + " ID is: " + _ID);
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Damage(5);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            setOutlineAvailable();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            setOutlineSelected();
        }
    }
    public void setUpWorkers()
    {
        if (_Workers.Length != 0)
        {
            //How to check if is initialized?
            for (int i = 0; i < _Workers.Length; ++i)
            {
                if (i == 0)
                    _Workers[0].GetComponent<Employee>().Lock(false);
                else
                    _Workers[i].GetComponent<Employee>().Lock(true);
            }


            ShowWorkers(false);
        }
        else
            Debug.LogWarning("Building has No Workers");
    }
    private void UpdateState()
    {
        //Debug.Log("UpdateState =" + eState);
        switch (eState)
        {
            case BuildingState.Available:
                {
                    _srNotify.sprite = _sNotification;
                    _srNotify.enabled = true;
                    ShowWorkers(false);
                    _animator.SetBool("Notify", true);
                    _animator.SetBool("Building", false);
                    break;
                }
            case BuildingState.Building:
                {
                    _srNotify.sprite = _sBuildingHammer;
                    _srNotify.enabled = true;
                    //need special case for Outpost
                    ShowWorkers(true); //_srWorker.enabled = true;
                    _animator.SetBool("Building", true);
                    break;
                }
            case BuildingState.Idle:
                {
                    _srNotify.enabled = false;
                    if (eType != BuildingType.TownCenter && eType != BuildingType.House && eType != BuildingType.Outpost)
                        ShowWorkers(true);
                    else
                        ShowWorkers(false);
                    _animator.SetBool("Notify", false);
                    _animator.SetBool("Building", false);
                    break;
                }
            case BuildingState.Built:
                {
                    _srNotify.enabled = false;
                    if (eType != BuildingType.TownCenter && eType != BuildingType.House && eType != BuildingType.Outpost)
                        ShowWorkers(true);
                    else
                        ShowWorkers(false);
                    _animator.SetBool("Notify", false);
                    _animator.SetBool("Building", false);
                    break;
                }

        }

    }


    //Getters
    public BuildingState getState() => eState;
    public BuildingType getType() => eType;
    public int getLevel() => _level;
    public int getTeam() => _Team;
    public int getID() => _ID;
    public float getHP() => _hitpoints;
    public float getHPMax() => _hitpointsMax;
    /**Sets the ID for the team
    * 0 = neutral
    * 1 = player
    * 2 = enemy
    * Also handles updating the Animator based on Type*/
    public void setTeam(int id)
    {
        if (id > -1 && id < 3)
            _Team = id;
        else if (id == 500) // dummy setting for dirt mount
            _Team = 500;
    }


    // used to be from MVC controller to let the building know its been clicked
    public void imClicked()
    {

        // Debug.Log("Building is Clicked state is" + eState);
        if (eState == BuildingState.Built)
        {
            //Create a new menu interaction on a built object, downgrade? Demolish? Show resource output etc. Needs Something
            if (eType == BuildingType.GarbageCan || eType == BuildingType.WoodPile || eType == BuildingType.StonePile)
            {
                if (getEmployeeCount() == 0) // if someone is working here, player cant gather
                {
                    //Debug.Log("I am gathering!");
                    Searchable s = GetComponent<Searchable>();
                    if (s)
                    {
                        // s.GatherAction(20);
                        s.ImClicked(); // should use ImClicked instead of GatherAction and encapsulate gather action into searchables functionality
                    }
                }
            }
            else if(eType == BuildingType.Outpost && _cameraController.getOverrideMode())
            {
                bOutpost outpost = GetComponent<bOutpost>();
                if (outpost.getSelected())
                {
                    setOutlineSelected();
                    UITroopSelection.Instance.addTroops(getEmployeeCount());
                }
                else
                {
                    setOutlineAvailable();
                    UITroopSelection.Instance.addTroops(0-getEmployeeCount());
                }
            }
            else
            {
                StartCoroutine(ClickDelay(true, _DestroyMenu));
                StartCoroutine(ClickDelay(false, _BuildMenu));
            }
        }
        else if (eState == BuildingState.Available || eState == BuildingState.Idle)
        {
            // Turns off the "notification exclamation mark" as the player is now aware of obj
            eState = BuildingState.Idle;

            StartCoroutine(ClickDelay(true, _BuildMenu));
            StartCoroutine(ClickDelay(false, _DestroyMenu));

            //Disconnect here, MVC controller is now responsible for talking to UI
        }
        else if (eState == BuildingState.Building)
        {
            IncrementConstruction(1);
            //Default
            //Debug.LogWarning("Does this Happen?");
            //eState = BuildingState.Idle;
            // StartCoroutine(ClickDelay(true, _DestroyMenu));
            //SetConstructionMax(5);
        }

        if(!_cameraController.getOverrideMode())
            UpdateState();
    }

    // Called from MVC controller to Build or Upgrade a building
    public void BuildSomething(string type)
    {
        // Debug.Log("Time to Build Something type=" + type);
        switch (type)
        {
            case ("house"):
                this.gameObject.AddComponent<bHouse>();
                eType = BuildingType.House;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                // Debug.Log("Made a house");
                break;
            case ("farm"):
                this.gameObject.AddComponent<bFarm>();
                eType = BuildingType.Farm;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                // Debug.Log("Made a Farm");
                break;
            case ("banner"):
                this.gameObject.AddComponent<bBanner>();
                eType = BuildingType.Banner;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                // Debug.Log("Made a Banner");
                break;
            case ("outpost"):
                this.gameObject.AddComponent<bOutpost>();
                eType = BuildingType.Outpost;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                // Debug.Log("Made an Outpost");
                break;
            case ("towncenter"):
                this.gameObject.AddComponent<bTownCenter>();
                eType = BuildingType.TownCenter;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                // Debug.Log("Made a TownCenter");
                break;
            case ("garbagecan"):
                this.gameObject.AddComponent<bGarbageCan>();
                eType = BuildingType.GarbageCan;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                break;
            case ("woodpile"):
                this.gameObject.AddComponent<bWoodPile>();
                eType = BuildingType.WoodPile;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                break;
            case ("stonepile"):
                this.gameObject.AddComponent<bStonePile>();
                eType = BuildingType.StonePile;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                break;

            case null:
                break;
        }
        UpdateState();
        _BuildMenu.showMenu(false, Vector3.zero,null, this);
        //StartCoroutine(BuildCoroutine());
        SetConstructionMax(5);
    }

    public void UpgradeSomething()
    {
        //We need to Upgrade this but NOT kick the worker rodent off 
        eState = BuildingState.Building;
        _sr.sprite = _sStateConstruction;
        _level++;
        UpdateState();
        _DestroyMenu.showMenu(false, Vector3.zero, null, this);
        //StartCoroutine(BuildCoroutine());
    }

    // Called from MVC controller
    public void DemolishSomething()
    {
        // Debug.Log("Time to Destroy Something" );
        switch (eType)
        {
            case (BuildingType.House):
                bHouse house = this.GetComponent<bHouse>();
                house.DemolishAction(_level);
                Destroy(house);
                eType = BuildingType.Vacant;
                eState = BuildingState.Available;
                _sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a house");
                break;
            case (BuildingType.Farm):
                bFarm farm = this.GetComponent<bFarm>();
                farm.DemolishAction();
                Destroy(farm);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a Farm");
                break;
            case (BuildingType.Banner):
                bBanner banner = this.GetComponent<bBanner>();
                Destroy(banner);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a Banner");
                break;
            case (BuildingType.Outpost):
                bOutpost outpost = this.GetComponent<bOutpost>();
                outpost.DemolishAction();
                Destroy(outpost);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;

                //Need to Reset Worker Object to Base
                ResetWorkers();
                // Debug.Log("Destroyed an Outpost");
                break;
            case (BuildingType.TownCenter):
                bTownCenter btc = this.GetComponent<bTownCenter>();
                Destroy(btc);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a TownCenter");
                break;
            case (BuildingType.GarbageCan):
                bGarbageCan garbagecan = this.GetComponent<bGarbageCan>();
                Destroy(garbagecan);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                break;
            case (BuildingType.WoodPile):
                bWoodPile wp = this.GetComponent<bWoodPile>();
                Destroy(wp);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                break;
            case (BuildingType.StonePile):
                bStonePile sp = this.GetComponent<bStonePile>();
                Destroy(sp);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                break;
        }

        UpdateState();
        _DestroyMenu.showMenu(false, Vector3.zero, null, this);
        StartCoroutine(DemolishCoroutine());
    }

    public void IncrementConstruction(int amnt)
    {
        //increment construction safely
        if (amnt > 0)
        {
            if (_construction + amnt <= _constructionMax)
                _construction += amnt;
            else
                _construction = _constructionMax;
        }

        //check if construction is full, then complete building
        if (_construction >= _constructionMax)
        {
            BuildComplete();
            //Do this here so when we load from save things dont get wonky
            eState = BuildingState.Built;

            //To:Do Update to kick builder rat off worker_obj
        }

        //update Gather Bar
        UpdateConstructionBar();
    }
    public void SetConstructionMax(int amnt)
    {
        _constructionMax = amnt;
        UpdateConstructionBar();
    }

    //Temporary way to delay construction
    IEnumerator BuildCoroutine()
    {
        yield return new WaitForSeconds(5f);
        BuildComplete();
        //Do this here so when we load from save things dont get wonky
        eState = BuildingState.Built;

        //To:Do Update to kick builder rat off worker_obj
    }
    IEnumerator DemolishCoroutine()
    {
        yield return new WaitForSeconds(5f);
        DemolishComplete();
    }

    //Upon completion let the correct script know to assign the new Sprite, and update our HP/Type.
    public void BuildComplete()
    {
        if (eType == BuildingType.House)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bHouse>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;

        }
        else if (eType == BuildingType.Farm)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bFarm>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;
        }
        else if (eType == BuildingType.Banner)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bBanner>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;
        }
        else if (eType == BuildingType.Outpost)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bOutpost>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;
            // Tell someone this is an outpost and Needs to have it Employees Shown On "Assignment Mode Toggle"
            UIAssignmentMenu.Instance.SetOutpostWorkers(_Workers);
            GameManager.Instance.PlayerOutpostCreated(this);
        }
        else if (eType == BuildingType.TownCenter)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bTownCenter>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;
        }
        else if (eType == BuildingType.GarbageCan)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bGarbageCan>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;
        }
        else if (eType == BuildingType.WoodPile)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bWoodPile>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;
        }
        else if (eType == BuildingType.StonePile)
        {
            float oldMax = _hitpointsMax;
            _hitpointsMax += this.GetComponent<bStonePile>().BuildingComplete(_level);
            float difference = _hitpointsMax - oldMax;
            _hitpoints += difference;
            _construction = 0;
        }
        UpdateState();
        //Debug.Log("Built a level " + _level + " structure");

        //If the state is dirt mount, set it to player team - will have to figure out
        // how to over ride for enemy / natural resources later on
        if (_Team == 500)
            setTeam(1);

        //Resets it so we can click again without clicking off first
        if (_controller.getLastClicked() == this.gameObject)
            _controller.clearLastClicked();
    }
    public void DemolishComplete()
    {
        eState = BuildingState.Available;
        _sr.sprite = _sStatedefault;
        _construction = 0;
        _constructionMax = 100;
        UpdateConstructionBar();
        _hitpointsMax = 5;
        _hitpoints = 0;
        if (_controller.getLastClicked() == this.gameObject)
            _controller.clearLastClicked();

        ShowRedX(false);

        //To-Do : Kick the worker rodent off

        //if we have returned to a dirt mount, reset team to default
        _level = 0;
        setTeam(500);

        // if outpost destroyed:
        //tell UI menu RemoveOutpostWorkers
        // tell game manager PlayerOutpostDestroyed

    }
    private void LoadComponents()
    {  //Debug.Log("LoadingCompnent type=" + eType);
        switch (eType)
        {
            case (BuildingType.House):
                if (this.GetComponent<bHouse>() == null)
                {
                    this.gameObject.AddComponent<bHouse>();
                    BuildComplete();
                }
                break;
            case (BuildingType.Farm):
                if (this.GetComponent<bFarm>() == null)
                {
                    this.gameObject.AddComponent<bFarm>();
                    BuildComplete();
                }
                break;
            case (BuildingType.Banner):
                if (this.GetComponent<bBanner>() == null)
                {
                    this.gameObject.AddComponent<bBanner>();
                    BuildComplete();
                }
                break;
            case (BuildingType.Outpost):
                if (this.GetComponent<bOutpost>() == null)
                {
                    this.gameObject.AddComponent<bOutpost>();
                    BuildComplete();
                }
                break;
            case (BuildingType.TownCenter):
                if (this.GetComponent<bTownCenter>() == null)
                {
                    this.gameObject.AddComponent<bTownCenter>();
                    BuildComplete();
                }
                break;
            case (BuildingType.GarbageCan):
                if (this.GetComponent<bGarbageCan>() == null)
                {
                    this.gameObject.AddComponent<bGarbageCan>();
                    BuildComplete();
                }
                break;
            case (BuildingType.WoodPile):
                if (this.GetComponent<bWoodPile>() == null)
                {
                    this.gameObject.AddComponent<bWoodPile>();
                    BuildComplete();
                }
                break;
            case (BuildingType.StonePile):
                if (this.GetComponent<bStonePile>() == null)
                {
                    this.gameObject.AddComponent<bStonePile>();
                    BuildComplete();
                }
                break;
        }
    }

    /** Used to undo the Outpost Structure */
    public void ResetWorkers()
    {
        UIAssignmentMenu.Instance.SetOutpostWorkers(null);
        GameObject _Worker1Prefab = Resources.Load<GameObject>("UI/Workers1");
        _Worker1Prefab = Instantiate(_Worker1Prefab);
        _Worker1Prefab.transform.SetParent(this.transform);
        _Worker1Prefab.transform.localPosition = new Vector3(0, 0, 0);
        _Worker1Prefab.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);

        //Hack Lazy
        Employee[] workers = _Worker1Prefab.GetComponent<eWorkers>().getWorkers();
        this.transform.GetComponent<BuildableObject>().ChangeWorkers(workers);
    }
    //Temp hack/work around for GameManager to create your town center on launch, must be updated later on
    public void SetType(string type)
    {
        // Debug.Log("Heard set Type");
        switch (type)
        {
            case ("TownCenter"):
                {
                    eType = BuildingType.TownCenter;
                    _hitpoints = bTownCenter.getHPStats();
                    _hitpointsMax = bTownCenter.getHPStats();
                    UpdateHealthBar();
                    break;
                }
            case ("GarbageCan"):
                {
                    eType = BuildingType.GarbageCan;
                    _hitpoints = bGarbageCan.getHPStats();
                    _hitpointsMax = bGarbageCan.getHPStats();
                    UpdateHealthBar();
                    break;
                }
            case ("WoodPile"):
                {
                    eType = BuildingType.WoodPile;
                    _hitpoints = bWoodPile.getHPStats();
                    _hitpointsMax = bWoodPile.getHPStats();
                    UpdateHealthBar();
                    break;
                }
            case ("StonePile"):
                {
                    eType = BuildingType.StonePile;
                    _hitpoints = bStonePile.getHPStats();
                    _hitpointsMax = bStonePile.getHPStats();
                    UpdateHealthBar();
                    break;
                }
        }

        eState = BuildingState.Built;
    }

    public void SetLevel(int lvl)
    {
        _level = lvl;
    }

    //unused Atm, was used in MVC but commented out i believe
    public bool CheckOccupied()
    {
        //Not Tested
        int _index = findAvailableSlot();
        if (_index != -1)
            return true;

        return false;
    }

    //Absolute nonsense i have to do this otherwise the same click insta clicks a button on the menu opened
    IEnumerator ClickDelay(bool cond, UIBuildMenu menu)
    {
        yield return new WaitForSeconds(0.05f);
        // To-Do: update for touch
        Debug.Log("Will need to get click location from somewhere for Mobile");
        Vector3 Location = Input.mousePosition;

        menu.showMenu(cond, Location, this.transform.gameObject, this);

    }

    public void ShowWorkers(bool cond)
    {
        foreach (Employee e in _Workers)
        {
            e.transform.gameObject.SetActive(cond);
        }
    }
    private int findAvailableSlot()
    {
        int _count = 0;

        foreach (Employee e in _Workers)
        {
            if (!e.isOccupied() && !e.isLocked())
            {
                // Debug.Log("Returned index= " + _count);
                return _count;
            }
            ++_count;

        }

        return -1;
    }
    public void AssignWorker(Rodent r)
    {
        // Debug.Log("AssignWorker!" + r.getName() + "to " + this.gameObject);

        int index = findAvailableSlot();
        if (index > -1)         //This is kind of a hack
        {
            _Workers[index].Assign(r);
            r.setTarget(this.gameObject);
        }
        //  else
        //  Debug.Log("no Empty");

    }
    public void DismissWorker(Rodent r)
    {
        foreach (Employee e in _Workers)
        {
            if (e.isOccupied())
            {
                if (e.getCurrentRodent() == r)
                {
                    //Debug.Log("We found the right Employee");
                    e.Dismiss(r);
                    break;
                }
            }
        }
    }
    public void ShowRedX(bool cond)
    {
        //  Debug.Log("Told to show RedX in Building");

        //Tell any occupied Employees to show x or tell all to not show it
        foreach (Employee e in _Workers)
        {
            if (e)
            {

                if (e.isOccupied() && cond == true)
                {
                    e.ShowRedX(true);
                }
                else
                    e.ShowRedX(false);

            }
        }

    }
    public void ChangeWorkers(Employee[] workers)
    {
        //Delete old workers no matter what?
        //When this is called there Shouldnt be anyone working here?
        //No need to handle dismissals etc
        //Destroying Parent, destroys children
        foreach (Employee e in _Workers)
            MVCController.Instance.RemoveRedX(e);
        Destroy(_Workers[0].transform.parent.gameObject);
        _Workers = null;
        _Workers = workers;
        UpdateState();
    }
    public void UnlockWorkers(int number)
    {
        int _count = 0;
        foreach (Employee e in _Workers)
        {
            if (e.isLocked() && _count < number)
            {
                e.Lock(false);
                ++_count;
            }
        }
    }
    public int getEmployeeCount()
    {
        int _count = 0;

        foreach (Employee e in _Workers)
        {
            if (e.isOccupied())
            {
                // Debug.Log("Returned index= " + _count);
                ++_count;
            }

        }

        return _count;
    }
    public List<GameObject> getEmployees()
    {
        List<GameObject> employees = new List<GameObject>();
        foreach (Employee e in _Workers)
        {
            if (e.isOccupied())
            {
                Rodent r = e.getCurrentRodent();
                if (r)
                {
                    employees.Add(r.gameObject);
                }
                else
                    Debug.LogError("No Rodent found when should be employee?");
            }

        }

        return employees;
    }
    public void setOutlineAvailable()
    {
        if (eType == BuildingType.Outpost)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                bOutpost outpost = GetComponent<bOutpost>();
                var s = outpost.getAvailable(_level);
                sr.sprite = s;
                outpost.setSelected(true);
            }
        }
    }

    public void setOutlineSelected()
    {
        if (eType == BuildingType.Outpost)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                bOutpost outpost = GetComponent<bOutpost>();
                var s = outpost.getSelected(_level);
                sr.sprite = s;
                outpost.setSelected(false);
            }
        }
    }
    public bool checkSelected()
    {
        if (eType == BuildingType.Outpost)
        {
            bOutpost outpost = GetComponent<bOutpost>();
            return ! outpost.getSelected();  // logic is backwards somewhere w the negation sign but this works
        }
        else
            Debug.LogWarning("its not an outpost?");
        return false;
    }

}




