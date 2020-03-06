using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable<float>, DayNight
{
    public float _Hp = 50f;
    public float _HpMax = 100f;
    [Range(0, 10f)]
    public float _MoveSpeed = 40f; 
    public float _AttackDamage = 10f;
    public GameObject _HealthBarObj;
    private HealthBar _HealthBar;

    public GameObject[] _RoyalGuards = new GameObject[3];
    private Transform _RoyalGuardParent;


    //Dont think we even need this
     private Rodent _Worker;



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

    public void SetUpDayNight()
    {
        if (this.transform.gameObject.GetComponent<Register2DDN>() == null)
            this.transform.gameObject.AddComponent<Register2DDN>();
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
    public void LateUpdate()
    {
        if(_RoyalGuardParent)
             _RoyalGuardParent.position = this.transform.position;
    }

    public void setUpRoyalGuard()
    {
        //How to check if _RoyalGuards is initialized?

      

        for (int i=0; i<_RoyalGuards.Length; ++i)
        {
            if(i==0)
                _RoyalGuards[0].GetComponent<Employee>().Lock(false);
            else
                _RoyalGuards[i].GetComponent<Employee>().Lock(true);
        }

        if(_RoyalGuards.Length>0)
            _RoyalGuardParent= _RoyalGuards[0].transform.parent;

        ShowRoyalGuard(false);



    }
    private int findAvailableSlot()
    {
        int _count = 0;

        foreach (GameObject g in _RoyalGuards)
        {
            Employee e = g.GetComponent<Employee>();
            if (e)
            {
                if (!e.isOccupied() && !e.isLocked())
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
        //Debug.Log("AssignWorker!" + r.getName());

        int index = findAvailableSlot();
        if (index > -1)         //This is kind of a hack
        {
            _RoyalGuards[index].GetComponent<Employee>().Assign(r);
            r.setTarget(this.gameObject);
        }
      //  else
          //  Debug.Log("no Empty");

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
                       //Debug.Log("We found the right Employee");
                       e.Dismiss(r);
                       break;
                    }
                }
            }
        }
    }
    public void ShowRedX(bool cond)
    {
        bool foundAtLeastOne = false;
       // Debug.LogWarning("ShowRedX RoyalGuard" + cond);

        // Debug.Log("Called Players Set Red x to " +cond);

        //Tell any occupied Employees to show x or tell all to not show it
        foreach (GameObject g in _RoyalGuards)
            {
                Employee e = g.GetComponent<Employee>();
                if (e)
                {

                    if (e.isOccupied() && cond == true)
                    {
                        e.ShowRedX(true);
                        foundAtLeastOne = true;
                    }
                    else
                        e.ShowRedX(false);

                }
            }

    }

    public void ShowRoyalGuard(bool cond)
    {
        foreach(GameObject g in _RoyalGuards)
        {
            g.SetActive(cond);
        }
    }
}
