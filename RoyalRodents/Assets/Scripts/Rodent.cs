﻿ using System.Collections;
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


    /**Begin Interface Stuff */
    public void Damage(float damageTaken)
    {
        if (_Hp - damageTaken > 0)
            _Hp -= damageTaken;
        else
        {
            _Hp = 0;
            Die();
        }
        //Debug.LogWarning("HP=" + _Hp);
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
        if(this.transform.gameObject.GetComponent<Register2DDN>() == null)
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
    }


    public void setHp(float val) { _Hp = val; UpdateHealthBar(); }// use sparingly, should call Damage
    public void setHpMax(float val) { _HpMax = val; UpdateHealthBar(); }
    public void setSpeed(float val) { _MoveSpeed = val; if (_SubjectScript) _SubjectScript.setSpeed(_MoveSpeed); }
    public void setAttackDmg(float val) => _AttackDamage = val;
    public void setName(string s) => _Name = s;
    public void setRodentType(eRodentType type){_Type = type; setTeam(_Team); }
    public void setRodentStatus(eStatus status) => _Status = status;
    public void setPortrait(Sprite s) => _Portrait = s;

    public float getHp() { return _Hp; }
    public float getHpMax() { return _HpMax; }
    public float getSpeed() { return _MoveSpeed; }
    public float getAttackDmg() { return _AttackDamage; }
    public string getName() { return _Name; }
    public eRodentType GetRodentType() { return _Type; }
    public eStatus GetRodentStatus() { return _Status; }
    public Sprite GetPortrait() { return _Portrait; }

    public void Die()
    {
        //Should this be in Rodent or in AIController which holds the Animator?
        // the player script does this that way but it feels weird 
    }
    /** Responsible for giving SubjectScript new Target and Updating our Status  */
    public void setTarget(GameObject o)
    {

		if (o != null)
			_Job = o.GetComponentInChildren<Employee>();
	

	   //need proper getter/setter someday
	   SubjectScript s= this.GetComponent<SubjectScript>();
        if (s)
            s.changeTarget(o);

        if(o==null)
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
                s.setWorker();
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
        GameManager.Instance.addToPlayerRodents(this);
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

    /**Sets the ID for the team
     * 0 = neutral
     * 1 = player
     * 2 = enemy
     * Also handles updating the Animator based on Type*/
    public void setTeam(int id)
    {
        if(id> -1 && id<3)
            _Team = id;

        switch (_Type)
        {
            case (eRodentType.Rat):
               this.GetComponent<Rat>().setAnimatorByTeam(id);
               // Debug.Log("Rat");
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

