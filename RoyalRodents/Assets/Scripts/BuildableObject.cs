using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildableObject : MonoBehaviour, IDamageable<float>
{
    public Sprite _sStatedefault;
    public Sprite _sStateHighlight;
    public Sprite _sStateConstruction;
    public Sprite _sStateDamaged;
    public Sprite _sStateDestroyed;
    public Sprite _sOnHover;
    public Sprite _sNotification;
    public Sprite _sEmptyWorker;
    public Sprite _sWorker;
    public Sprite _sBuildingHammer;

    public GameObject _NotificationObject;
    public GameObject _WorkerObject;

    public Animator _animator;
    public HealthBar _HealthBar;

    private Rodent _Worker;


    [SerializeField]
    private BuildingState eState;

    [SerializeField]
    private BuildingType eType;

    private SpriteRenderer sr;
    private SpriteRenderer srNotify;
    private SpriteRenderer srWorker;
    private UIBuildMenu _BuildMenu;
    private UIBuildMenu _DestroyMenu;
    private MVCController controller;

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
            _HealthBar.SetSize(_hitpoints / _hitpointsMax);

        if (_hitpoints == 0)
            _HealthBar.gameObject.SetActive(false);
    }
    /** End interface stuff*/


    // Start is called before the first frame update
    void Start()
    {
        sr = this.transform.GetComponent<SpriteRenderer>();
        _sStatedefault= Resources.Load<Sprite>("Buildings/DirtMound/dirt_mound_concept");
        sr.sprite = _sStatedefault;

        srNotify = _NotificationObject.transform.GetComponent<SpriteRenderer>();
        srWorker = _WorkerObject.transform.GetComponent<SpriteRenderer>();
        srWorker.sprite = _sEmptyWorker;

        eState =BuildingState.Available;
        eType = BuildingType.Vacant;
        _animator = GetComponentInChildren<Animator>();
        


        GameObject o=GameObject.FindGameObjectWithTag("BuildMenu");
        _BuildMenu = o.GetComponent<UIBuildMenu>();
        o = GameObject.FindGameObjectWithTag("DestroyMenu");
        _DestroyMenu = o.GetComponent<UIBuildMenu>();

        o = GameObject.FindGameObjectWithTag("MVC");
        if (o)
        {
            if (o.GetComponent<MVCController>())
                controller = o.GetComponent<MVCController>();
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
                    srNotify.sprite = _sNotification;
                    srNotify.enabled = true;
                    srWorker.enabled = false;
                    _animator.SetBool("Notify", true);
                    _animator.SetBool("Building", false);
                    break;
                }
            case BuildingState.Building:
                {
                    srNotify.sprite = _sBuildingHammer;
                    srWorker.sprite = _sEmptyWorker;
                    srNotify.enabled = true;
                    srWorker.enabled = true;
                    _animator.SetBool("Building", true);
                    break;
                }
            case BuildingState.Idle:
                {
                    srNotify.enabled = false;
                    srWorker.enabled = false;
                    _animator.SetBool("Notify", false);
                    _animator.SetBool("Building", false);
                    break;
                }
            case BuildingState.Built:
                {
                    srNotify.enabled = false;
                    srWorker.enabled = true;
                    _animator.SetBool("Notify", false);
                    _animator.SetBool("Building", false);
                    break;
                }
        }

    }

    public BuildingState getState()
    {
        return eState;
    }
    public BuildingType getType()
    {
        return eType;
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
                sr.sprite = _sStateConstruction;
               // Debug.Log("Made a house");
                break;
            case ("farm"):
                this.gameObject.AddComponent<bFarm>();
                eType = BuildingType.Farm;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
               // Debug.Log("Made a Farm");
                break;
            case ("wall"):
                this.gameObject.AddComponent<bWall>();
                eType = BuildingType.Wall;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
               // Debug.Log("Made a Wall");
                break;
            case ("tower"):
                this.gameObject.AddComponent<bTower>();
                eType = BuildingType.Tower;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
               // Debug.Log("Made a Tower");
                break;
            case ("towncenter"):
                this.gameObject.AddComponent<bTownCenter>();
                eType = BuildingType.TownCenter;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
               // Debug.Log("Made a TownCenter");
                break;

            case null:
                break;
        }
        _BuildMenu.showMenu(false, Vector3.zero,null);
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
                sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a house");
                break;
            case (BuildingType.Farm):
                bFarm farm = this.GetComponent<bFarm>();
                Destroy(farm);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a Farm");
                break;
            case (BuildingType.Wall):
                bWall wall = this.GetComponent<bWall>();
                Destroy(wall);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a Wall");
                break;
            case (BuildingType.Tower):
                bTower tower = this.GetComponent<bTower>();
                Destroy(tower);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a Tower");
                break;
            case (BuildingType.TownCenter):
                bTownCenter btc = this.GetComponent<bTownCenter>();
                Destroy(btc);
                eType = BuildingType.Vacant;
                eState = BuildingState.Building;
                sr.sprite = _sStateConstruction;
                // Debug.Log("Destroyed a TownCenter");
                break;

        }
        _DestroyMenu.showMenu(false, Vector3.zero, null);
        StartCoroutine(DemolishCoroutine());
    }



    //Temporary way to delay construction
    IEnumerator BuildCoroutine()
    {
        yield return new WaitForSeconds(5f);
        BuildComplete();

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
            _hitpoints+=  this.GetComponent<bHouse>().BuildingComplete();
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
        if(controller.getLastClicked()==this.gameObject)
            controller.clearLastClicked();
    }
    public void DemolishComplete()
    {
        eState = BuildingState.Available;
        sr.sprite = _sStatedefault;
        if (controller.getLastClicked() == this.gameObject)
            controller.clearLastClicked();
    }

    //Temp hack/work around for GameManager to create your town center on launch, must be updated later on
    public void SetType(string type)
    {
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