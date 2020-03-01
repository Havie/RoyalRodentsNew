using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildableObject : MonoBehaviour, IDamageable<float>
{
    [SerializeField] private Sprite _sStatedefault;
    [SerializeField] private Sprite _sStateHighlight;
    [SerializeField] private Sprite _sStateConstruction;
    [SerializeField] private Sprite _sStateDamaged;
    [SerializeField] private Sprite _sStateDestroyed;
    [SerializeField] private Sprite _sOnHover;
    [SerializeField] private Sprite _sNotification;
    [SerializeField] private Sprite _sEmptyPortrait;
    [SerializeField] private Sprite _sWorker;
    [SerializeField] private Sprite _sRedX;
    [SerializeField] private Sprite _sBuildingHammer;

    [SerializeField] private GameObject _NotificationObject;
    [SerializeField] private GameObject _WorkerObject;
    [SerializeField] private GameObject _PortraitOutlineObject;
    [SerializeField] private GameObject _RedXObject;

    [SerializeField] private Animator _animator;
    [SerializeField] private HealthBar _HealthBar;

    [SerializeField] private Rodent _Worker;


    [SerializeField]
    private BuildingState eState;

    [SerializeField]
    private BuildingType eType;

    private int _level = 0;

    private SpriteRenderer _sr;
    private SpriteRenderer _srNotify;
    private SpriteRenderer _srWorker;
    private SpriteRenderer _srPortrait;
    private SpriteRenderer _srRedX;
    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private MVCController _controller;

    [SerializeField]
    private float _hitpoints = 0;
    private float _hitpointsMax = 0;

    public enum BuildingState { Available, Idle, Building, Built };
    public enum BuildingType { House, Farm, Tower, Wall, TownCenter, Vacant}




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
    /** End interface stuff*/


    // Start is called before the first frame update
    void Start()
    {
        _sr = this.transform.GetComponent<SpriteRenderer>();
        _sStatedefault= Resources.Load<Sprite>("Buildings/DirtMound/dirt_mound_concept");
        if (eType != BuildingType.TownCenter)
            _sr.sprite = _sStatedefault;

        //SetUp the NotifyObj
        _srNotify = _NotificationObject.transform.GetComponent<SpriteRenderer>();
        _srNotify.sprite = _sNotification;

        //SetUp the Portrait BG
        _srPortrait = _PortraitOutlineObject.transform.GetComponent<SpriteRenderer>();
        _srPortrait.sprite = _sEmptyPortrait;

        //Set Up Worker to be empty
        _srWorker = _WorkerObject.transform.GetComponent<SpriteRenderer>();
        _sWorker = _sEmptyPortrait;
        _srWorker.sprite = _sWorker;


        //SetUp the RedX
        _srRedX = _RedXObject.transform.GetComponent<SpriteRenderer>();
        _srRedX.sprite = _sRedX;
        ShowRedX(false);

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

        o = GameObject.FindGameObjectWithTag("MVC");
        if (o)
        {
            if (o.GetComponent<MVCController>())
                _controller = o.GetComponent<MVCController>();
            else
                Debug.LogError("UI Costs cant find MVC Controller");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            this.GetComponent<SpriteRenderer>().sprite = _sStateHighlight;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            this.GetComponent<SpriteRenderer>().sprite = _sStatedefault;
        }
        switch (eState)
        {
            case BuildingState.Available:
                {
                    _srNotify.sprite = _sNotification;
                    _srNotify.enabled = true;
                    _srPortrait.enabled = false;
                    _srWorker.enabled = false;
                    _animator.SetBool("Notify", true);
                    _animator.SetBool("Building", false);
                    break;
                }
            case BuildingState.Building:
                {
                    _srNotify.sprite = _sBuildingHammer;
                    _srWorker.sprite = _sWorker; // update to be empty elsewhere later on
                    _srNotify.enabled = true;
                    _srPortrait.enabled = true;
                    _srWorker.enabled = true;
                    _animator.SetBool("Building", true);
                    break;
                }
            case BuildingState.Idle:
                {
                    _srWorker.sprite = _sWorker;
                    _srNotify.enabled = false;
                    _srPortrait.enabled = true;
                    _srWorker.enabled = true;
                    _animator.SetBool("Notify", false);
                    _animator.SetBool("Building", false);
                    break;
                }
            case BuildingState.Built:
                {
                    _srWorker.sprite = _sWorker;
                    _srNotify.enabled = false;
                    _srPortrait.enabled = true;
                    _srWorker.enabled = true;
                    _animator.SetBool("Notify", false);
                    _animator.SetBool("Building", false);
                    break;
                }
        }

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

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something In Collider Range");
    }

    // Called from MVC controller to let the building know its been clicked
    public void imClicked()
    {
        if (eState == BuildingState.Built)
        {
            //Create a new menu interaction on a built object, downgrade? Demolish? Show resource output etc. Needs Something
        }
       else if (eState == BuildingState.Available || eState == BuildingState.Idle)
        {
            // Turns off the "notification exclamation mark" as the player is now aware of obj
            eState = BuildingState.Idle;

            //Disconnect here, MVC controller is now responsible for talking to UI
        }
        else
        {
            //Default
            eState = BuildingState.Idle;
        }
  

    }

    // Called from MVC controller
    public virtual void BuildSomething(string type)
    {
       // Debug.Log("Time to Build Something type=" + type);
        switch (type)
        {
            case ("house"):
                this.gameObject.AddComponent<bHouse>();
                eType = BuildingType.House;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level=1;
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
            case ("wall"):
                this.gameObject.AddComponent<bWall>();
                eType = BuildingType.Wall;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                // Debug.Log("Made a Wall");
                break;
            case ("tower"):
                this.gameObject.AddComponent<bTower>();
                eType = BuildingType.Tower;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                _level = 1;
                // Debug.Log("Made a Tower");
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
        _BuildMenu.showMenu(false, Vector3.zero,null, this);
        StartCoroutine(BuildCoroutine());
    }

    // Called from MVC controller
    public  void DemolishSomething()
    {
       // Debug.Log("Time to Destroy Something" );
        switch (eType)
        {
            case (BuildingType.House):
                bHouse house = this.GetComponent<bHouse>();
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
            case (BuildingType.Wall):
                bWall wall = this.GetComponent<bWall>();
                Destroy(wall);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a Wall");
                break;
            case (BuildingType.Tower):
                bTower tower = this.GetComponent<bTower>();
                Destroy(tower);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                _sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a Tower");
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
        _DestroyMenu.showMenu(false, Vector3.zero, null, this);
        StartCoroutine(DemolishCoroutine());
    }

    public void Upgrade()
    {
        ++_level;
        StartCoroutine(BuildCoroutine());
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
        if(eType== BuildingType.House)
        {
            _hitpoints+=  this.GetComponent<bHouse>().BuildingComplete(_level);
        }
       else if (eType == BuildingType.Farm)
        {
            _hitpoints += this.GetComponent<bFarm>().BuildingComplete();
        }
       else if (eType == BuildingType.Wall)
        {
            _hitpoints += this.GetComponent<bWall>().BuildingComplete();
        }
       else if (eType == BuildingType.Tower)
        {
            _hitpoints += this.GetComponent<bTower>().BuildingComplete();
        }
       else if (eType == BuildingType.TownCenter)
        {
            _hitpoints += this.GetComponent<bTownCenter>().BuildingComplete();
        }

        //Resets it so we can click again without clicking off first
        if(_controller.getLastClicked()==this.gameObject)
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
    public void AssignWorker(Rodent r)
    {
       Debug.Log("AssignWorker!" + r.getName());
        _Worker = r;
        bWorkerScript ws=_PortraitOutlineObject.GetComponent<bWorkerScript>();
        if(ws)
        {
            ws.setWorker(_Worker);
            _sWorker = r.GetPortrait();
           // Debug.LogError(_sWorker.ToString());
        }
        //To-Do: Something not being handled here is the status of Building to Built.

    }
    public void DismissWorker(Rodent r)
    {
       // Debug.Log("DismissWorker!");
        if (r != _Worker)
            Debug.LogError("Rodents dont match:Uh-Oh?");

        //Tell the worker to fuck off
        if (_Worker)
            _Worker.setTarget(null);
        else
            Debug.LogError("Trying to dismiss a worker thats not there??");

        eState = BuildingState.Idle;
        _Worker = null;
        _sWorker = _sEmptyPortrait;
        //Resets the assignment window to get the available worker
        //appears it works well enough to call here, instead of _Worker.setTarget(null)
        UIAssignmentMenu.Instance.ResetButtons();

    }
    public bool CheckOccupied()
    {
        //If we want to have multiple workers, this needs to change
        // can always check the workScript if its occupied? 
        // or get all children of type bWorkerScript and see if any arent occupied

        return (_Worker!=null);
    }
    public void ShowRedX(bool cond)
    {
        if (cond)
        {
            _srRedX.enabled = true;
            MVCController.Instance.setLastRedX(this);
        }
        else
        {
            _srRedX.enabled = false;
            //Turn back on the collider possible hack
            if(_PortraitOutlineObject)
                _PortraitOutlineObject.GetComponent<bWorkerScript>().ToggleCollider(true);
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