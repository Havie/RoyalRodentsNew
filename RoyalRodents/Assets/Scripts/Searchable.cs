using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject _Trash;
    public GameObject _Food;

    public Animator _FoodController;
    public Animator _TrashController;


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


        //SetUPAnimControllers
        if (_Food)
        {
            _FoodController = _Food.GetComponent<Animator>();
            
        }
        if (_FoodController == null)
            Debug.LogError("FoodController Null");
        
        //SetUPAnimControllers
        if (_Trash)
        {
            _TrashController = _Trash.GetComponent<Animator>();

        }
        if (_TrashController == null)
            Debug.LogError("TrashController Null");

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
            _ProgressBar.SetFillAmount(_SearchTime/ _SearchTimeMax);
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


    private void OnMouseDown()
    {

        Debug.Log("Search on mouse down");
        //This work for gestures?
        if(_MainCharacterInRange)
        {
            _SearchMe = true;
           
        }
       
    }

    IEnumerator Search()
    {
        _Searching = true;

        //TO-DO: Tell MC to play Animation // handle wtf the MVC thinks is going on

        yield return new WaitForSeconds(_Delay);


        //Gain Resource
        GainRandomResource();

         //Increment Progress bar
         _SearchTime += _Delay;
        UpdateProgressBar();
        if (_SearchTime >= _SearchTimeMax)
            _Empty = true;

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
            ResourceManagerScript.Instance.incrementTrash(_Amount);
            _TrashController.SetTrigger("Pop");
        }
        else
        {
            ResourceManagerScript.Instance.incrementFood(_Amount);
            _FoodController.SetTrigger("Pop");
        }

    }
}
