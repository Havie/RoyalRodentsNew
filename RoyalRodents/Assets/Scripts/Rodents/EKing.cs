using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKing : MonoBehaviour
{
    public Sprite _Default;

    private float _Hp = 300f;
    private float _HpMax = 300f;
    [Range(0, 10f)]
    private float _MoveSpeed = 2.7f;
    private float _AttackDamage = 3.5f;
    private int _buildRate = 1;
    private int _gatherRate = 10;
    private Vector2 _BoxColliderSize = new Vector2(1.8f, 1f);
    private Vector2 _BoxColliderOffset = new Vector2(0, 1f);
    [SerializeField]
    private Sprite _Portrait;

    private int _RecruitmentCost = 1;

    [SerializeField]
    private Animator _Animator;
    [SerializeField]
    private RuntimeAnimatorController _EnemyController;

    private bool _AnimsSet;



    private void Awake()
    {
        _Default = Resources.Load<Sprite>("Rodent/King_Enemy/EnemySprite_Idle_1_0");
        _Portrait = Resources.Load<Sprite>("UI/RodentIcons/EnemyIcon"); //dont need one?
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = _Default;
        setUpAnimators();
        Rodent r = this.GetComponent<Rodent>();
        if(r)
        {
            r.setRodentType(Rodent.eRodentType.EKing); 
            r.setSpeed(_MoveSpeed);
            r.setHpMax(_HpMax);
            r.setHp(_Hp);
            r.setAttackDmg(_AttackDamage);
            r.setPortrait(_Portrait);
            r.setRecruitmentCost(_RecruitmentCost);
            r.setBuildRate(_buildRate);
            r.setGatherRate(_gatherRate);

            setUpProperBoxCollider();
        }

        //TMP Test - Finds and follows the player
        // this.GetComponent<SubjectScript>().currentTarget = GameObject.FindObjectOfType<PlayerStats>().gameObject;
    }

   private void setUpAnimators()
    {
        _Animator = this.GetComponent<Animator>();
        if (_Animator == null)
            Debug.LogWarning("Cant Find Animator on Rat??");


        _EnemyController = Resources.Load<RuntimeAnimatorController>("Rodent/King_Enemy/EnemyKingAnims");
        if (_EnemyController == null)
            Debug.LogWarning("Cant Find Enemy Controller on EKing");

        _AnimsSet = true;
    }

    public void setAnimatorByTeam(int team)
    {
        //mini check - need this because things get called out of order on start
        if (!_AnimsSet)
            setUpAnimators();

        if (team == 2)
        {
            _Animator.runtimeAnimatorController = _EnemyController;
        }
        else
            Debug.LogWarning("Wrong team passed in");
    }
    private void setUpProperBoxCollider()
    {
        //To-Do: Figure out the proper size for this species box Collider, then set it
        BaseHitBox hitBox = this.gameObject.GetComponentInChildren<BaseHitBox>();
        if (hitBox)
        {
            BoxCollider2D collider = hitBox.GetComponent<BoxCollider2D>();
            if (collider)
            {
                collider.size = _BoxColliderSize;
                collider.offset = _BoxColliderOffset;
            }
        }
    }
}
