﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rodent : MonoBehaviour, IDamageable<float>, DayNight
{
    public GameObject _HealthBarObj;
    public GameObject _RecruitMenuPrefab;
    private HealthBar _HealthBar;

    [SerializeField]
    private float _Hp = 50f;
    [SerializeField]
    private float _HpMax = 100f;
    [SerializeField]
    [Range(0, 10f)]
    private float _MoveSpeed = 2f;
    [SerializeField]
    private float _AttackDamage = 1f;
    [SerializeField]
    private string _Name = "Rodent";
    [SerializeField]
    private eRodentType _Type = eRodentType.Default;
    [SerializeField]
    private eStatus _Status = eStatus.Available;

    private int _RecruitmentCost = 1;
    private int _PopulationCost = 1;

    [SerializeField]
    private int _Team = 0; // 0 is neutral, 1 is player, 2 is enemy

    public enum eRodentType { Rat, Badger, Beaver, Raccoon, Mouse, Porcupine, Default };
    public enum eStatus { Busy, Available, Building, Working, Army, Default };

    private SubjectScript _SubjectScript;
    private UIRecruitMenu _RecruitMenu;
    [SerializeField] private Employee _Job;

    [SerializeField]
    private Sprite _Portrait;

    [SerializeField]
    private GameObject _NotificationObject;
    [SerializeField]
    private Animator _NotifyAnimator;
    [SerializeField]
    private GameObject _placeOfWork;
    [SerializeField]
    private int _ID;

    private bool _isDead;
    private bool _isRanged;

    /**Begin Interface Stuff */
    public void Damage(float damageTaken)
    {
        if (_Hp - damageTaken > 0)
        {
            _Hp -= damageTaken;
            if (_SubjectScript)
                _SubjectScript.UnderAttack(true);
        }
        else
        {
            _Hp = 0;
            Die();
            if (_SubjectScript)
                _SubjectScript.setDead();
        }
       // Debug.LogWarning("@ " + Time.time + "   HP= " + _Hp);
        UpdateHealthBar();
       
    }

    public void SetUpHealthBar(GameObject go)
    {
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("UI/HealthBarCanvas");
        if (_HealthBarObj != null)
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


        UpdateHealthBar();
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
    /** End Interface Stuff */

    private void SetUpRecruitMenu()
    {
        if (_RecruitMenuPrefab)
        {
            GameObject go = GameObject.Instantiate(_RecruitMenuPrefab, this.transform.position, this.transform.rotation);
            if (go)
            {
                _RecruitMenu = go.GetComponent<UIRecruitMenu>();
                if (_RecruitMenu == null)
                    Debug.LogError("Cant Find RecruitMenu on " + this.gameObject + " " + _Name);
                go.transform.SetParent(this.transform);
            }
        }
        else
        {
            _RecruitMenuPrefab = Resources.Load<GameObject>("UI/RecruitMenu");
            if (_RecruitMenuPrefab)
                SetUpRecruitMenu();
            else
                Debug.LogError("Cant Find RecruitMenu Prefab");
        }
    }
    private void setUpNotifyObj()
    {
        // done via inspector 
        if (_NotificationObject == null)
            Debug.Log("NotifyObj not set from inspector");
    }
    private bool PickRanged()
    {
        int _chanceToMove = Random.Range(0, 9);
        if (_chanceToMove > 4)
        {
            Animator a = this.GetComponent<Animator>();
            if (a)
                a.SetBool("isRanged", true);
            return true;
        }
        return false;
    }

    public void LoadData(int id, int team, int type, int WorkID, float xPos)
    {
        if (_ID != id)
            Debug.LogWarning("Rodent IDs do not match, save data failure");
        //Set the Position
        this.transform.position = new Vector3(xPos, this.transform.position.y, 0);
        //Set the Team
        _Team = team;
        //Set the Species
        setRodentType((eRodentType)type);

        //Figure out place of employment 
        if (WorkID == -1)
            setTarget(null);
        else if (WorkID == -2) // is a royal guard
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            setTarget(player);
            player.GetComponent<PlayerStats>().AssignWorker(this);
        }
        else
        {
            BuildableObject b = BuildingSlotManager.Instance.getBuildingFromID(WorkID);
            setTarget(b.gameObject);
            b.AssignWorker(this);
            if (b == null)
                Debug.LogWarning("Rodent should work at building but its null, Possible Save Game Corruption");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("UI/HealthBarCanvas");

        SetUpHealthBar(_HealthBarObj.gameObject);
        SetUpRecruitMenu();

        setTeam(_Team);
        _SubjectScript = this.GetComponent<SubjectScript>();
        if (_SubjectScript == null)
            Debug.LogError("Warning No SubjectScript found for Rodent");

        //get a name
        _Name = RodentNames.getRandomName();


        //Add Day/Night Cycle Stuff
        SetUpDayNight();

        setUpNotifyObj();
        setTarget(null);

        _ID = GameManager.Instance.getRodentIdex();
       // Debug.Log(this.gameObject + " ID is: " + _ID);
        GameManager.Instance.AddtoRodents(this);

        //Rename Prefab 
        this.gameObject.name =_ID+ " Rodent: " + _Name + " ";

        _isRanged = PickRanged();

    }


    public void setHp(float val) { _Hp = val; UpdateHealthBar(); }// use sparingly, should call Damage
    public void setHpMax(float val) { _HpMax = val; UpdateHealthBar(); }
    public void setSpeed(float val) { _MoveSpeed = val; if (_SubjectScript) _SubjectScript.setSpeed(_MoveSpeed); }
    public void setAttackDmg(float val) { _AttackDamage = val; if (_SubjectScript) _SubjectScript.setAttackDamage(val); }
    public void setName(string s) => _Name = s;
    public void setRodentType(eRodentType type)
    {
        _Type = type;
        setTeam(_Team); // why is this here? does removing it break anything?? too lazy to check - might be for rodents that start in the scene?

        switch (_Type)
        {
            case eRodentType.Rat:
                {
                    //Debug.Log("Told to set Type of Rat");
                    if (this.GetComponent<Rat>() == null)
                        this.gameObject.AddComponent<Rat>();
                    break;
                }
        }

        this.gameObject.name += "(" +_Type+")";
    }
    public void setRodentStatus(eStatus status) => _Status = status;
    public void setPortrait(Sprite s) => _Portrait = s;

    public bool isRanged() => _isRanged;
    public bool isDead() => _isDead;
    public float getHp() { return _Hp; }
    public float getHpMax() { return _HpMax; }
    public float getSpeed() { return _MoveSpeed; }
    public float getAttackDmg() { return _AttackDamage; }
    public string getName() { return _Name; }
    public eRodentType GetRodentType() { return _Type; }
    public eStatus GetRodentStatus() { return _Status; }
    public Sprite GetPortrait() { return _Portrait; }
    public GameObject getPlaceOfWork() => _placeOfWork;
    public int getID() => _ID;

    private void Update()
    {
        //TMP
        if(Input.GetKeyDown(KeyCode.D))
        {
            Die();
        }
    }

    public void Die()
    {
        print(_Name + " is dead");
        //Should this be in Rodent or in AIController which holds the Animator?
        // the player script does this that way but it feels weird 
        //HACK
        _isDead = true;
        this.GetComponent<Animator>().SetBool("isDead", true);
        StartCoroutine(DeathDelay());
    }
    IEnumerator DeathDelay()
    {
        //Fun Way to use event system
        //EventSystem.Instance.IDied(this);
        //but this was already here and much cleaner/easier..lmao, oh well i learned alot
        if(_Job)
            _Job.Dismiss(this);  //Unassign self from job 
        if(_NotifyAnimator)
            Destroy(_NotifyAnimator.gameObject);
        yield return new WaitForSeconds(5f);
       Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        if(_Team==1)
            GameManager.Instance.RemovePlayerRodent(this);

        GameManager.Instance.RemoveFromRodents(this);

    }
    /** Responsible for giving SubjectScript new Target and Updating our Status  */
    public void setTarget(GameObject o)
    {
        _placeOfWork = o;
        //need proper getter/setter someday
        SubjectScript s = this.GetComponent<SubjectScript>();
        if (s)
            s.changeTarget(o);

        if (o == null)
        {
            _Status = eStatus.Available;
            s.setIdle();
            _NotificationObject.SetActive(true);
            _NotifyAnimator.SetBool("Notify", true);
            return;
        }


        if (o.GetComponent<BuildableObject>())
        {
            BuildableObject bo = o.GetComponent<BuildableObject>();
            if (bo.getState() == BuildableObject.BuildingState.Building)
            {

                //Tell subject script to behave like a builder
                s.setBuilder();
                _Status = eStatus.Building;

                _NotificationObject.SetActive(false);
                // Debug.Log("Updated State to Builder");
                //OR
                // Tell them to defend a location when that script arrives
                // _Status = eStatus.Army;
            }
            else if (bo.getState() == BuildableObject.BuildingState.Built || bo.getState() == BuildableObject.BuildingState.Idle)
            {
                //Unknown if state IDLE could cause a unique problem, can a building be
                // idle but not built? i forget
                _NotificationObject.SetActive(false);

                // Tell Subject Script to behave like a Worker 
                if (bo.getType() == BuildableObject.BuildingType.Outpost)
                    s.setDefender();
                else if (bo.getType() == BuildableObject.BuildingType.Farm)
                    s.setFarmer();
                else
                    s.setGatherer();

                _Status = eStatus.Working;
                // Debug.Log("Updated State to Worker");

            }
        }
        else if (o.GetComponent<PlayerStats>())
        {
            //Debug.Log("Was told to go to RoyalGuard");
            // Tell Subject script to behave like a bodyguard
            _NotificationObject.SetActive(false);
            s.setRoyalGuard();
            _Status = eStatus.Army; // for all intensive purposes army can behave same for player and defense structure

        }
        else
        {
            Debug.Log("We dont know this behavior");
            s.setIdle();
        }
    }
    public void Recruit()
    {
        _Status = eStatus.Available;
        setTeam(1);
        // No new Behavior?

        // Go to Town Center? 

        //What if not in Zone?
    }
    public Employee GetJob()
    {
        return _Job;
    }
    public void SetJob(Employee e)
    {
        _Job = e;
    }
    /**Sets the ID for the team
     * 0 = neutral
     * 1 = player
     * 2 = enemy
     * Also handles updating the Animator based on Type*/
    public void setTeam(int id)
    {
        int oldTeam = _Team;

        if (id > -1 && id < 3)
            _Team = id;

        if (_SubjectScript)
            _SubjectScript.setTeam(_Team);

        if (_Team == 1)
            GameManager.Instance.addToPlayerRodents(this);
        else if (oldTeam == 1)
            GameManager.Instance.RemovePlayerRodent(this);


        switch (_Type)
        {
            case (eRodentType.Rat):
                this.GetComponent<Rat>().setAnimatorByTeam(id);
                break;
        }


    }
    public int getTeam()
    {
        return _Team;
    }

    public void setRecruitmentCost(int cost)
    {
        _RecruitmentCost = cost;
    }
    public int getRecruitmentCost()
    {
        return _RecruitmentCost;
    }
    public int getPopulationCost()
    {
        return _PopulationCost;
    }

    public void imClicked()
    {
        // Debug.Log("Heard Click Rodent:  " + _Name);

        //Tell any old Menu To close
        _RecruitMenu.showMenu(false, Vector3.zero, null, 0, 0);


        if (_Status == eStatus.Available && _Team == 0)
        {
            if (_RecruitMenu)
            {
                _RecruitMenu.showMenu(true, this);
            }
            else
                Debug.LogError("No RecruitMenu");
        }
        else if (_Status != eStatus.Available && _Team == 1)
        {
            //Show Dismiss Button
            if (_RecruitMenu)
                _RecruitMenu.showDismissMenu(true, this);
        }
        else if (_Status == eStatus.Available && _Team == 1)
        {

            // Debug.Log("Show AssignmentMenu");


            // Show the Royal guard above players head
            // by activating the assignment menu! - might not want this
            UIAssignmentMenu.Instance.showMenu(true, this.transform.gameObject);

            //TO-DO: Need to phase Out
            UIAssignmentMenu.Instance.CreateButtons(GameManager.Instance.getPlayerRodents());


        }
    }

}

