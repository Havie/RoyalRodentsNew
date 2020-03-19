﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable<float>, DayNight
{

    [SerializeField] private float _Hp = 50f;
    [SerializeField] private float _HpMax = 100f;
    [SerializeField] [Range(0, 10f)] private float _MoveSpeed = 40f;
    [SerializeField] private float _AttackDamage = 30f;
    [SerializeField] private float _Stamina = 60f;
    [SerializeField] private float _StaminaMax = 100f;



    public GameObject _HealthBarObj;
    private HealthBar _HealthBar;
    private HealthBar _StaminaBar;

    public GameObject[] _RoyalGuards = new GameObject[3];
    private Transform _RoyalGuardParent;


    /**Begin Interface stuff*/
    public void Damage(float damageTaken)
    {
        if (_Hp - damageTaken > 0 && _Hp- damageTaken <= _HpMax)
            _Hp -= damageTaken;
        else
        {
            if (_Hp - damageTaken < 0)
            {
                _Hp = 0;
                //Sloppy?
                PlayerMovement pm = this.GetComponent<PlayerMovement>();
                if (pm)
                    pm.Die();
                else
                    Debug.LogError("Should have died but cant find PlayerMovement");
            }
            else if (_Hp - damageTaken >= _HpMax)
                _Hp = _HpMax;

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

    public void LoadData()
    {
        sPlayerData data = sSaveSystem.LoadPlayerData();
        if(data!=null)
        {
            _Hp = data._Health;
            _Stamina = data._Stamina;

            UpdateHealthBar();
            UpdateStaminaBar();
        }
        else
            Debug.LogError("noSaveData to Load");
    }
    // Start is called before the first frame update
    void Start()
    {
        _Hp = 50f;
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("UI/HealthBarCanvas");
        SetUpHealthBar(_HealthBarObj.gameObject);
        UpdateHealthBar();
        SetUpStaminaBar();
        setUpRoyalGuard();
    }
    public void LateUpdate()
    {
       if(_RoyalGuardParent)
             _RoyalGuardParent.position = this.transform.position;

        //Player will trickle restore HP based on stamina
        if(!InOwnTerritory())
        {
            if(_Hp<_HpMax)
                 Damage(-_Stamina/5000f);
        }
    }
    public void SetUpStaminaBar()
    {
        GameObject sb=GameObject.FindGameObjectWithTag("StaminaBar");
        if (sb)
            _StaminaBar = sb.GetComponentInChildren<HealthBar>();

        if (_StaminaBar == null)
            Debug.LogError("Stamina Bar is not found");

        UpdateStaminaBar();
    }
    public void UpdateStaminaBar()
    {
        if (_StaminaBar)
            _StaminaBar.SetFillAmount(_Stamina / _StaminaMax);
    }
    public void IncrementStamina(float amnt)
    {
        if (amnt > 0)
        {
            if (_Stamina + amnt < _StaminaMax)
                _Stamina += amnt;
            else
                _Stamina = _StaminaMax;
        }
        else
        {
            if(_Stamina +amnt > 0)
                _Stamina += amnt;
            else
                _Stamina = 0;
        }

        UpdateStaminaBar();
    }
    public float getStamina()
    {
        return _Stamina;
    }
    public float getStaminaMax()
    {
        return _StaminaMax;
    }
    //getters
    public float getMoveSpeed()
    {
        return _MoveSpeed;
    }
    public float getAttackDamage()
    {
        return _AttackDamage;
    }
    public float getHealth()
    {
        return _Hp;
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
    private bool InOwnTerritory()
    {
        //To-Do: implement 
        return true;
    }
}