using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildableObject : MonoBehaviour, IDamageable<float>, DayNight
{
    [SerializeField] private Sprite _sStatedefault;
    [SerializeField] private Sprite _sStateHighlight;
    [SerializeField] private Sprite _sStateConstruction;
    [SerializeField] private Sprite _sStateDamaged;
    [SerializeField] private Sprite _sStateDestroyed;
    [SerializeField] private Sprite _sOnHover;
    [SerializeField] private Sprite _sNotification;
    [SerializeField] private Sprite _sBuildingHammer;

    [SerializeField] private GameObject _NotificationObject;


    [SerializeField] private Animator _animator;
    [SerializeField] private HealthBar _HealthBar;
    

    [SerializeField]
    private BuildingState eState;

    [SerializeField]
    private BuildingType eType;

    private int _level = 0;

    // NEW
    public Employee[] _Workers = new Employee[1];

    private SpriteRenderer _sr;
    private SpriteRenderer _srNotify;

    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private MVCController _controller;

    [SerializeField]
    private float _hitpoints = 0;
    private float _hitpointsMax = 0;

    public enum BuildingState { Available, Idle, Building, Built };
    public enum BuildingType { House, Farm, Outpost, Banner, TownCenter, Vacant}

    [SerializeField]
    private int _Team = 0; // 0 is neutral, 1 is player, 2 is enemy


    /**Begin Interface stuff*/
    public void Damage(float damageTaken)
    {
        if (_hitpoints - damageTaken > 0)
            _hitpoints -= damageTaken;
        else
            _hitpoints = 0;
    }
    public void SetUpHealthBar(GameObject go)
    {
        _HealthBar = go.GetComponent<HealthBar>();
    }

    public void UpdateHealthBar()
    {
        if (_HealthBar)
            _HealthBar.SetFillAmount(_hitpoints / _hitpointsMax);

        if (_hitpoints == 0)
            _HealthBar.gameObject.SetActive(false);
    }
    public void SetUpDayNight()
    {
        if (this.transform.gameObject.GetComponent<Register2DDN>() == null)
            this.transform.gameObject.AddComponent<Register2DDN>();
    }
    /** End interface stuff*/


    // Start is called before the first frame update
    void Start()
    {
        _sr = this.transform.GetComponent<SpriteRenderer>();
        _sStatedefault= Resources.Load<Sprite>("Buildings/DirtMound/dirt_mound_final");
        if (eType != BuildingType.TownCenter)
            _sr.sprite = _sStatedefault;

        //SetUp the NotifyObj
        _srNotify = _NotificationObject.transform.GetComponent<SpriteRenderer>();
        _srNotify.sprite = _sNotification;


        if (eType != BuildingType.TownCenter)
        {
            eState = BuildingState.Available;
            eType = BuildingType.Vacant;
        }
        _animator = GetComponentInChildren<Animator>();
        


        GameObject o=GameObject.FindGameObjectWithTag("BuildMenu");
        _BuildMenu = o.GetComponent<UIBuildMenu>();
        o = GameObject.FindGameObjectWithTag("DestroyMenu");
        _DestroyMenu = o.GetComponent<UIBuildMenu>();

        //little unnecessary
        _controller = MVCController.Instance;


        UpdateState();
        SetUpTeam();
        setUpWorkers();
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
                    if (eType != BuildingType.TownCenter && eType != BuildingType.House && eType!= BuildingType.Outpost)
                        ShowWorkers(true);
                    else
                        ShowWorkers(false);
                    _animator.SetBool("Notify", false);
                    _animator.SetBool("Building", false);
                    break;
                }
           
        }

    }
    private void SetUpTeam()
    {
        //To-Do:
        //method will need a way to find out which team this building should be on
        //either via tag, starting team inspector, or world map/state loaded from game manager

        //for now buildings are players
        setTeam(1);
    }


    //Getters
    public BuildingState getState()
    {
        return eState;
    }
    public BuildingType getType()
    {
        return eType;
    }
    public int getLevel()
    {
        return _level;
    }
    /**Sets the ID for the team
    * 0 = neutral
    * 1 = player
    * 2 = enemy
    * Also handles updating the Animator based on Type*/
    public void setTeam(int id)
    {
        if (id > -1 && id < 3)
            _Team = id;

    }
    public int getTeam()
    {
        return _Team;
    }

    // used to be from MVC controller to let the building know its been clicked
    public void imClicked()
    {

      //  Debug.Log("Building is Clicked state is" + eState);
        if (eState == BuildingState.Built)
        {
            //Create a new menu interaction on a built object, downgrade? Demolish? Show resource output etc. Needs Something
            StartCoroutine(ClickDelay(true, _DestroyMenu));
            StartCoroutine(ClickDelay(false, _BuildMenu));
        }
       else if (eState == BuildingState.Available || eState == BuildingState.Idle)
        {
            // Turns off the "notification exclamation mark" as the player is now aware of obj
            eState = BuildingState.Idle;

           StartCoroutine(ClickDelay(true, _BuildMenu));
            StartCoroutine(ClickDelay(false, _DestroyMenu));

            //Disconnect here, MVC controller is now responsible for talking to UI
        }
        else
        {
            //Default
            Debug.LogWarning("Does this Happen?");
            eState = BuildingState.Idle;
           // StartCoroutine(ClickDelay(true, _DestroyMenu));
        }

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

            case null:
                break;
        }
        UpdateState();
        _BuildMenu.showMenu(false, Vector3.zero,null, this);
        StartCoroutine(BuildCoroutine());
    }

    public void UpgradeSomething()
    {
        //We need to Upgrade this but NOT kick the worker rodent off 
        eState = BuildingState.Building;
        _sr.sprite = _sStateConstruction;
        _level++;
        UpdateState();
        _DestroyMenu.showMenu(false, Vector3.zero, null, this);
        StartCoroutine(BuildCoroutine());
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
                Destroy(outpost);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
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

        }
        UpdateState();
        _DestroyMenu.showMenu(false, Vector3.zero, null, this);
        StartCoroutine(DemolishCoroutine());
    }

    //Temporary way to delay construction
    IEnumerator BuildCoroutine()
    {
        yield return new WaitForSeconds(5f);
        BuildComplete();

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
       eState = BuildingState.Built;
       if(eType == BuildingType.House)
       {
            _hitpoints += this.GetComponent<bHouse>().BuildingComplete(_level);
       }
       else if (eType == BuildingType.Farm)
        {
            _hitpoints += this.GetComponent<bFarm>().BuildingComplete(_level);
        }
       else if (eType == BuildingType.Banner)
        {
            _hitpoints += this.GetComponent<bBanner>().BuildingComplete(_level);
        }
       else if (eType == BuildingType.Outpost)
        {
            _hitpoints += this.GetComponent<bOutpost>().BuildingComplete(_level);
            //To-Do: Tell someone this is an outpost and Needs to have it Employees Shown On "Assignment Mode Toggle"
        }
       else if (eType == BuildingType.TownCenter)
        {
            _hitpoints += this.GetComponent<bTownCenter>().BuildingComplete(_level);
        }
        UpdateState();
        //Debug.Log("Built a level " + _level + " structure");

        //Resets it so we can click again without clicking off first
        if (_controller.getLastClicked()==this.gameObject)
            _controller.clearLastClicked();
    }
    public void DemolishComplete()
    {
        eState = BuildingState.Available;
        _sr.sprite = _sStatedefault;
        if (_controller.getLastClicked() == this.gameObject)
            _controller.clearLastClicked();

        ShowRedX(false);
        //To-Do : Kick the worker rodent off
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
                    break;
                }
        }

        eState = BuildingState.Built;
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
       // Debug.Log("Will need to get click location from somewhere for Mobile");
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
                    Debug.Log("Returned index= " + _count);
                    return _count;
                }
                ++_count;

        }

        return -1;
    }
    public void AssignWorker(Rodent r)
    {
        Debug.Log("AssignWorker!" + r.getName());

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
        foreach(Employee e in _Workers)
            MVCController.Instance.RemoveRedX(e);
        Destroy(_Workers[0].transform.parent.gameObject);
        _Workers = null;
        _Workers = workers;
        UpdateState();
    }
    public void UnlockWorkers(int number)
    {
        int _count = 0;
        foreach(Employee e in _Workers)
        {
            if (e.isLocked() && _count < number)
            {
                e.Lock(false);
                ++_count;
            }
        }
    }
}




//ALL OF THIS IS TEST For tracking mouse clicks //ignore for now
/* GameObject o = GameObject.FindGameObjectWithTag("Canvas");
 RectTransform CanvasRect = o.GetComponent<RectTransform>();
 Vector2 WorldObject_ScreenPosition = new Vector2(
 ((mousePos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
 ((mousePos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

 Vector2 localpoint;
 RectTransform rectTransform = _BuildMenu.getRect();
 Canvas canvas = o.GetComponent<Canvas>();
 RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvas.worldCamera, out localpoint);
 Vector2 normalizedPoint = Rect.PointToNormalized(rectTransform.rect, localpoint);
 Debug.Log("Normalized :  " +normalizedPoint);


 Vector2 pos;
 RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
 Vector2 newPos2D_Cav = canvas.transform.TransformPoint(pos);

 Debug.Log("Mouse2d" + mousePos2D);
 Debug.Log("WorldObj:" + WorldObject_ScreenPosition);
 Debug.Log("Mouse:" + MouseRaw);
 Debug.Log("attempt:" + newPos2D_Cav);
 // UI_Element.anchoredPosition = WorldObject_ScreenPosition;
 //END OF TESTS
 */

//now you can set the position of the ui element