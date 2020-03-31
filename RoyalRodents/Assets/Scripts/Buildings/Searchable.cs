using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManagerScript;

public class Searchable : MonoBehaviour
{
    [SerializeField]
    private bool _MainCharacterInRange;
    private bool _Hovered;  //unused
    private bool _Empty;   //means its on cool down
    private bool _SearchMe; // the MC has clicked this and is in range
    private float _Delay = 1f; //used by coroutine
    private bool _Searching;  //used by coroutine
    private float _SearchTime;
    private float _SearchTimeMax= 1*5;
    private float _CooldownTime = 5f;
    private bool _CoolingDown;

    //Gathering Data
    public ResourceType _gatherResourceType = ResourceType.Food;
    public int _gatherResourceAmount = 1 ;
    private int _gathering = 0;
    private int _gatheringMax = 100;

    //Gathering Resource Icon Data
    public GameObject _ResourceIconAnimObject;
    public Animator _ResourceIconAnimController;
    Sprite _IconSprite;

    private int _StaminaCost = 1;

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
            SpriteRenderer _ResourceIconSpriteRenderer = _ResourceIconAnimObject.GetComponent<SpriteRenderer>();
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
            if (_gathering + amnt < _gatheringMax)
                _gathering += amnt;
            else
                _gathering = _gatheringMax;
        }
        else
        {
            if (_gathering + amnt > 0)
                _gathering += amnt;
            else
                _gathering = 0;
        }

        //check if progress is full, then do action and reset progress
        if (_gathering >= _gatheringMax)
        {
            _gathering = 0;
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
                    StartCoroutine(Cooldown());
            }

        }
    }

    public void setActive(bool cond)
    {
        _MainCharacterInRange = cond;
    }


    /** for some unknown reason, the OnCollisonEnter2D only works on the playerMovement.cs   */
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //doesn't work
        Debug.Log("Enter Garbage Can");
    }

// 
//     private void OnMouseDown()
//     {
//        // Debug.Log("Search on mouse down");
//         //This work for Touch gestures..?
//         if(_MainCharacterInRange)
//         {
//             _SearchMe = true;
//            
//         }
//        
//     }
    public void ImClicked()
    {
        // Debug.Log("Search on mouse down");
        //This work for Touch gestures..?
        if (_MainCharacterInRange)
        {
            _SearchMe = true;

        }

    }

    IEnumerator Search()
    {
        _Searching = true;
        bool _okayToSearch = true;

        //TO-DO:  handle wtf the MVC thinks is going on?

        //Decrease stamina
        if (_MainCharacter)
        {
            PlayerStats _PlayerStats = _MainCharacter.GetComponent<PlayerStats>();
            if (_PlayerStats)
            {
                if (_PlayerStats.getStamina() >= _StaminaCost)
                {
                    _PlayerStats.IncrementStamina(-_StaminaCost);
                    //TO-DO: Tell MC to play Animation

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


            //Gain Resource
            GainSpecifiedResource();

            //Increment Progress bar
            _SearchTime += _Delay;
            UpdateProgressBar();
            if (_SearchTime >= _SearchTimeMax)
                _Empty = true;
        }
        _Searching = false;
    }
    IEnumerator Cooldown()
    {
        _CoolingDown = true;
        yield return new WaitForSeconds(_CooldownTime);
        _Empty = false;
        _SearchTime = 0;
        UpdateProgressBar();
        _CoolingDown = false;
    }

    //unused
    public void GainRandomResource()
    {
        int _ResourceNumber = Random.Range(0, 10);
        int _Amount = Random.Range(0, 100);

        //Cut down to a the amount
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
        //Give Player Resource
        ResourceManagerScript.Instance.incrementResource(_gatherResourceType, _gatherResourceAmount);

        //Animate Icon
        if (_ResourceIconAnimController)
            _ResourceIconAnimController.SetTrigger("Pop");
        else
            Debug.Log("Icon Animator is Null!!!");

        //Update Bar
        UpdateProgressBar();
    }
}
