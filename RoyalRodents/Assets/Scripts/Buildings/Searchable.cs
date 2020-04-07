using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManagerScript;

public class Searchable : MonoBehaviour
{
    [SerializeField]
    private bool _MainCharacterInRange;
    private bool _Hovered;  //unused
    [SerializeField] private bool _Empty;   //means its on cool down
    [SerializeField] private bool _SearchMe; // the MC has clicked this and is in range
    private float _Delay = 1f; //used by coroutine
    [SerializeField] private bool _Searching;  //used by coroutine
    [SerializeField] private float _SearchTime;
    [SerializeField] private float _SearchTimeMax= 1*5;
    [SerializeField] private float _CooldownTime = 0.5f;
    [SerializeField] private bool _CoolingDown;
    [SerializeField] private bool _CoolDownOveride;

    //Gathering Data
    public ResourceType _gatherResourceType = ResourceType.Food;
    public int _gatherResourceAmount = 1 ;
    private int _gathering = 0;
    private int _gatheringMax = 100;

    //Gathering Resource Icon Data
    public GameObject _ResourceIconAnimObject;
    public Animator _ResourceIconAnimController;
    SpriteRenderer _ResourceIconSpriteRenderer;
    Sprite _IconSprite;
    Sprite _IconSprite2; //only needed if Trash Resource Gather Type

    private int _StaminaCost = 5;

    private GameObject _ProgressBarObj;
    private HealthBar _ProgressBar;

    private GameObject _MainCharacter;
    private bool _MobileMode;

    private void Start()
    {

        _MobileMode = GameManager.Instance.getMobileMode();

        _MainCharacter = GameObject.FindGameObjectWithTag("Player");
        if (_MainCharacter == null)
            Debug.LogError("Cant Find Player Tag");


        _SearchTimeMax = _Delay * 5;

        //Get the Prefab
        if (_ProgressBarObj == null)
            _ProgressBarObj = Resources.Load<GameObject>("UI/ProgressBarSearch");
        SetUpProgressBar(_ProgressBarObj);

        //Find ResourceIconAnimObject
        _ResourceIconAnimObject = this.gameObject.transform.GetChild(4).gameObject;

        //SetUPAnimControllers
        if (_ResourceIconAnimObject)
        {
            //Get Gather Icon Animator Component
            _ResourceIconAnimController = _ResourceIconAnimObject.GetComponent<Animator>();

            //Set Gather Icon Sprite
            _IconSprite = Resources.Load<Sprite>(ResourceManagerScript.GetIconPath(_gatherResourceType));
            if (_gatherResourceType == ResourceType.Trash)
                _IconSprite2 = Resources.Load<Sprite>(ResourceManagerScript.GetIconPath(ResourceType.Food));

            _ResourceIconSpriteRenderer = _ResourceIconAnimObject.GetComponent<SpriteRenderer>();
            if (_ResourceIconSpriteRenderer)
                _ResourceIconSpriteRenderer.sprite = _IconSprite;
        }
        //if (_ResourceIconAnimController == null)
            //Debug.LogError("ResourceIconAnimController Null");

    }

    public void setGatherResource(ResourceType type, int amnt)
    {
        _gatherResourceType = type;
        _gatherResourceAmount = amnt;
        //Debug.Log("ResourceType set to " + type.ToString() + ", count = " + amnt);
    }

    //Change Gather Stats
    public void incrementGathering(int amnt)
    {
        //increment gathering safely
        if (amnt > 0)
        {
            if (_gathering + amnt <= _gatheringMax)
                _gathering += amnt;
            else
                _gathering = _gatheringMax;
        }
        else
        {
            if (_gathering + amnt > 0)
                _gathering += amnt;
            else
            {
                _Empty = true;
                if (_Searching)
                    _CooldownTime = 0.5f;
            }
        }

        //check if progress is full, then do action and reset progress
        if (_gathering >= _gatheringMax)
        {
            _Empty = true;
            if(_Searching)
                _CooldownTime = 0.5f;
            GainSpecifiedResource();
        }

        //update Gather Bar
        UpdateProgressBar();

        //Debug.Log("Gathering: " + _gathering + "/" + _gatheringMax);
    }

    public void GatherAction(int gatheramnt)
    {
        incrementGathering(gatheramnt);
    }

    public void SetUpProgressBar(GameObject go)
    {
        if (_ProgressBarObj != null)
        {
            _ProgressBarObj = Instantiate(go);
            _ProgressBarObj.gameObject.transform.SetParent(this.transform);
            _ProgressBar = _ProgressBarObj.GetComponentInChildren<HealthBar>();
            if (!_ProgressBar)
                Debug.LogError("Cant Find Progress bar");
            _ProgressBarObj.transform.SetParent(this.transform);
            _ProgressBarObj.transform.localPosition = new Vector3(0, 0.55f, 0);
        }
        else
            Debug.LogError("Cant Find Progress bar Prefab");


        UpdateProgressBar();
    }

    public void UpdateProgressBar()
    {
        if (_ProgressBar)
            _ProgressBar.SetFillAmount((float)_gathering / _gatheringMax);
    }

    private void Update()
    {
        if(_SearchMe)
        {

            if (!_MobileMode && Input.GetMouseButtonUp(0))
                _SearchMe = false;

            else if (_MobileMode && Input.touchCount == 0)
                _SearchMe = false;

            if (!_Searching && !_Empty)
                StartCoroutine(Search());
            else if(_Empty)
            {
                if (!_CoolingDown)
                    StartCoroutine(Cooldown(_CooldownTime));
            }

        }
    }

    public void setActive(bool cond)
    {
        _MainCharacterInRange = cond;
       // print("Set Active= " + cond);
    }


    public void ImClicked()
    {
        // Debug.Log("Clicked:SearchableObject");
        if (_MainCharacterInRange)
        {
            _SearchMe = true;
           // print("In range");
        }

    }
    /** Please Do Not Change SearchTime variables
     * by using a search time limits the player to less resources than rodent method
     * if you want to change how many times player can search and how often just edit
     * the global variables _SearchTimeMax and _Delay
     */
    IEnumerator Search()
    {
        _Searching = true;
        bool _okayToSearch = true;

        //Decrease stamina
        if (_MainCharacter)
        {
            PlayerStats _PlayerStats = _MainCharacter.GetComponent<PlayerStats>();
            if (_PlayerStats)
            {
                if (_PlayerStats.getStamina() >= _StaminaCost)
                {
                    _PlayerStats.IncrementStamina(-_StaminaCost);
                    //Tell MC to play Animation
                    Animator am = _MainCharacter.GetComponent<Animator>();
                    if(am)
                        am.SetTrigger("doDig");

                }
                else
                    _okayToSearch = false;
            }
            else
                _okayToSearch = false;
        }
        else
            _okayToSearch = false;

        if (_okayToSearch)
        {

            yield return new WaitForSeconds(_Delay);

            //Increment Progress bar
            _SearchTime += _Delay;

            //Gain Resource
            //GainSpecifiedResource(); //- old system
            incrementGathering(100);  // - new system
            // trick the progress bar into thinking were using the new system
            //_gathering = (int)_SearchTime * 20;


            UpdateProgressBar();

            //print("search time= " + _SearchTime + "  max= " + _SearchTimeMax);

            if (_SearchTime >= _SearchTimeMax)
            {
                //Debug.LogError("Starting manual over ride");
                _Empty = true;
                StartCoroutine(Cooldown(5));
                _CoolDownOveride = true;
                //To-Do : Make progress bar red? or indicate somehow to player it is on CD
            }
        }
        _Searching = false;
    }
    IEnumerator Cooldown(float time)
    {
        _CoolingDown = true;
        yield return new WaitForSeconds(time);

            _CoolingDown = false;
            _Empty = false;
            _gathering = 0;
            UpdateProgressBar();

        if(_CoolDownOveride)
        {
            _SearchTime = 0;
            _CoolDownOveride = false;
        }

    }

    //unused
    public void GainRandomResource()
    {
        int _ResourceNumber = Random.Range(0, 10);
        int _Amount = Random.Range(0, 100);

        //Cut down to the real amount
        if (_Amount > 80)
            _Amount = 2;
        else
            _Amount = 1;

        if (_ResourceNumber < 7)
        {
            ResourceManagerScript.Instance.incrementResource(ResourceManagerScript.ResourceType.Trash, _Amount);
            _ResourceIconAnimController.SetTrigger("Pop");
        }
        else
        {
            ResourceManagerScript.Instance.incrementResource(ResourceManagerScript.ResourceType.Food, _Amount);
            _ResourceIconAnimController.SetTrigger("Pop");
        }

    }

    public void GainSpecifiedResource()
    {
        //if set Resource is Trash, then randomize between Trash and Food
        if (_gatherResourceType == ResourceType.Trash)
        {
            int _ResourceNumber = Random.Range(0, 10);
            if (_ResourceNumber <= 6) //70% chance of trash
            {
                ResourceManagerScript.Instance.incrementResource(ResourceType.Trash, _gatherResourceAmount);
                if (_ResourceIconSpriteRenderer)
                    _ResourceIconSpriteRenderer.sprite = _IconSprite;
            }
            else
            {
                ResourceManagerScript.Instance.incrementResource(ResourceType.Food, _gatherResourceAmount);
                if (_ResourceIconSpriteRenderer)
                    _ResourceIconSpriteRenderer.sprite = _IconSprite2;
            }
        }
        else
            ResourceManagerScript.Instance.incrementResource(_gatherResourceType, _gatherResourceAmount); //Give Player Resource

        //Animate Icon
        if (_ResourceIconAnimController)
            _ResourceIconAnimController.SetTrigger("Pop");
        else
            Debug.Log("Icon Animator is Null!!!");

        //Update Bar
        UpdateProgressBar();
    }
}
