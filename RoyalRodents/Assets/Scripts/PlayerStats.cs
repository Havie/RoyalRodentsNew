using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable<float>
{
    public float _Hp = 50f;
    public float _HpMax = 100f;
    [Range(0, 10f)]
    public float _MoveSpeed = 3.5f;  //used to be 2
    public float _AttackDamage = 10f;
    public GameObject _HealthBarObj;
    private HealthBar _HealthBar;

    public GameObject[] _RoyalGuards = new GameObject[3];



    [SerializeField] private GameObject[] _NotificationObjects;
    [SerializeField] private GameObject[] _WorkerObjects;
    [SerializeField] private GameObject[] _PortraitOutlineObjects;
    [SerializeField] private GameObject[] _RedXObjects;

    [SerializeField] private Rodent _Worker;



    /**Begin Interface stuff*/
    public void Damage(float damageTaken)
    {
        if (_Hp - damageTaken > 0)
            _Hp -= damageTaken;
        else
        {
            _Hp = 0;
            //Sloppy?
            PlayerMovement pm = this.GetComponent<PlayerMovement>();
            if (pm)
                pm.Die();
            else
                Debug.LogError("Should have died but cant find PlayerMovement");

        }
        //Debug.LogWarning("HP=" + _Hp);
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
            _HealthBar.SetFillAmount(_Hp / _HpMax);
    }
    /**End Interface stuff*/


    // Start is called before the first frame update
    void Start()
    {
        _Hp = 50f;
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("UI/HealthBarCanvas");
        SetUpHealthBar(_HealthBarObj.gameObject);
        UpdateHealthBar();
        setUpRoyalGuard();
    }

    public void setUpRoyalGuard()
    {
        //How to check if _RoyalGuards is initialized?

        //set up our arrays 
        _NotificationObjects = new GameObject[_RoyalGuards.Length];
        _WorkerObjects = new GameObject[_RoyalGuards.Length];
        _PortraitOutlineObjects = new GameObject[_RoyalGuards.Length];
        _RedXObjects = new GameObject[_RoyalGuards.Length];

        _RoyalGuards[0].GetComponent<Employee>().Lock(false);



    }
    private int findAvailableSlot()
    {
        int _count = 0;

        foreach (GameObject g in _RoyalGuards)
        {
            Employee e = g.GetComponent<Employee>();
            if (e)
            {
                if (!e.isOccupied())
                {
                    return _count;
                }
                ++_count;

            }
        }

        return -1;
    }

    //I worry were gonna be passed in a employee Object, not a rodent? or both
    public void AssignWorker(Rodent r)
    {
        Debug.Log("AssignWorker!" + r.getName());

        int index = findAvailableSlot();
        if (index > -1)         //This is kind of a hack
            _RoyalGuards[index].GetComponent<Employee>().Assign(r);
        else
            Debug.Log("no Empty");

    }
    public void DismissWorker(Rodent r)
    {
        foreach (GameObject g in _RoyalGuards)
        {
            Employee e = g.GetComponent<Employee>();
            if (e)
            {
                if (e.isOccupied())
                {
                    if (e.getCurrentRodent() == r)
                    {
                       Debug.Log("We found the right Employee");
                       e.Dismiss();
                       // break;
                    }
                }


            }
        }

        //Resets the assignment window to get the available worker
        //appears it works well enough to call here, instead of _Worker.setTarget(null)
        UIAssignmentMenu.Instance.ResetButtons();

    }
    public void ShowRedX(bool cond)
    {
        bool foundAtLeastOne = false;
        Debug.Log("Called Players Set Red x to " +cond);

            //Tell any occupied Employees to show x
            foreach (GameObject g in _RoyalGuards)
            {
                Employee e = g.GetComponent<Employee>();
                if (e)
                {
                    Debug.Log("We Found an Employee");
                    if (e.isOccupied() && cond == true)
                    {
                        e.ShowRedX(true);
                        foundAtLeastOne = true;
                    }
                    else
                        e.ShowRedX(false);

                }
            }
            if (foundAtLeastOne)
                MVCController.Instance.setLastRedX(this.gameObject);
        
    }
}
