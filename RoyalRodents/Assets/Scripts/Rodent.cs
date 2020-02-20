using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rodent : MonoBehaviour, IDamageable<float>
{
    public HealthBar _HealthBar;

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
    private eStatus _Status = eStatus.Available;

    public enum eRodentType { Rat, Badger, Beaver, Raccoon, Mouse, Porcupine, Default };
    public enum eStatus { Busy, Available, Building, Working, Army, Default};

    private SubjectScript subjectScript;

    [SerializeField]
    private Sprite _Portrait;


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
        //which comes first the chicken or the egg...
        _HealthBar = Instantiate(go).GetComponent<HealthBar>();
        _HealthBar.gameObject.transform.SetParent(this.transform);
    }

    public void UpdateHealthBar()
    {
        if (_HealthBar)
            _HealthBar.SetSize(_Hp / _HpMax);
    }
    /** End Interface Stuff */


    // Start is called before the first frame update
    void Start()
    {
        SetUpHealthBar(_HealthBar.gameObject);
        subjectScript = this.GetComponent<SubjectScript>();
        if (subjectScript == null)
            Debug.LogError("Warning No SubjectScript found for Rodent");

        //get a name
        _Name = RodentNames.getRandomName();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LateUpdate()
    {
        _HealthBar.transform.position = this.transform.position + new Vector3(0, 1, 0);
    }

    public void setHp(float val) { _Hp = val; UpdateHealthBar(); }// use sparingly, should call Damage
    public void setHpMax(float val) { _HpMax = val; UpdateHealthBar(); }
    public void setSpeed(float val)  { _MoveSpeed = val; if(subjectScript)subjectScript.setSpeed(_MoveSpeed);  }
    public void setAttackDmg(float val) => _AttackDamage = val;
    public void setName(string s) => _Name = s;
    public void setRodentType(eRodentType type) => _Type = type;
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

        //need proper getter/setter someday
       SubjectScript s= this.GetComponent<SubjectScript>();
        if (s)
            s.changeTarget(o);

        if(o==null)
        {
            _Status = eStatus.Available;
            return;
        }


        if (o.GetComponent<BuildableObject>())
        {
            BuildableObject bo = o.GetComponent<BuildableObject>();
            if (bo.getState() == BuildableObject.BuildingState.Building)
            {
                //Tell subject script to behave like a builder
                _Status = eStatus.Building;
                //OR
                // Tell them to defend a location when that script arrives
                _Status = eStatus.Army;
            }
            else if (bo.getState() == BuildableObject.BuildingState.Built)
            {
                // Tell Subject Script to behave like a Worker 
                _Status = eStatus.Working;
            }
        }
        else if (o.GetComponent<PlayerStats>())
        {
            // Tell Subject script to behave like a bodyguard
            _Status = eStatus.Army; // for all intentive purposes army can behave same for player and defense structure
        }
        else
        {
            Debug.Log("We dont know this behavior");
        }
    }
}

