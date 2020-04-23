﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beaver : MonoBehaviour
{
    public Sprite _Default;

    private float _Hp = 135f;
    private float _HpMax = 135f;
    [Range(0, 10f)]
    private float _MoveSpeed = 2.4f;
    private float _AttackDamage = 3.7f;
    private int _buildRate = 3;
    private int _gatherRate = 5;
    private Vector2 _BoxColliderSize = new Vector2(2, 1.5f);
    private Vector2 _BoxColliderOffset = new Vector2(0, 0.5f);
    [SerializeField]
    private Sprite _Portrait;

    private int _RecruitmentCost = 1;

    [SerializeField]
    private Animator _Animator;
    [SerializeField]
    private RuntimeAnimatorController _NeutralController;
    private RuntimeAnimatorController _AlliedController;
    private RuntimeAnimatorController _EnemyController;

    private bool _AnimsSet;



    private void Awake()
    {
        _Default = Resources.Load<Sprite>("Rodent/FatRat/Recruit_Beaversprite_Idle_0");
        _Portrait = Resources.Load<Sprite>("UI/RodentIcons/BeaverIcon");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = _Default;
        setUpAnimators();
        Rodent r = this.GetComponent<Rodent>();
        if(r)
        {
            r.setRodentType(Rodent.eRodentType.Beaver); 
            r.setSpeed(_MoveSpeed);
            r.setHpMax(_HpMax);
            r.setHp(_Hp);
            r.setAttackDmg(_AttackDamage);
            r.setPortrait(_Portrait);
            r.setBuildRate(_buildRate);
            r.setGatherRate(_gatherRate);
            setUpProperBoxCollider();

            int curr=ResourceManagerScript.Instance.getCurrentPopulation();
            r.setRecruitmentCost(_RecruitmentCost+curr);

        }
    }

   private void setUpAnimators()
    {
        _Animator = this.GetComponent<Animator>();
        if (_Animator == null)
            Debug.LogWarning("Cant Find Animator on Beaver??");

        _NeutralController = Resources.Load<RuntimeAnimatorController>("Rodent/Beaver/NeutralController");
        if (_NeutralController == null)
            Debug.LogWarning("Cant Find Neutral Controller on Beaver");

        _AlliedController = Resources.Load<RuntimeAnimatorController>("Rodent/Beaver/AlliedController");
        if (_AlliedController == null)
            Debug.LogWarning("Cant Find Allied Controller on Beaver");

        _EnemyController = Resources.Load<RuntimeAnimatorController>("Rodent/Beaver/EnemyController");
        if (_EnemyController == null)
            Debug.LogWarning("Cant Find Enemy Controller on Beaver");

        _AnimsSet = true;
    }

    public void setAnimatorByTeam(int team)
    {
        //mini check - need this because things get called out of order on start
        if (!_AnimsSet)
            setUpAnimators();


        if (team == 0)
        {
            _Animator.runtimeAnimatorController = _NeutralController;
        }
        else if (team == 1)
        {
            _Animator.runtimeAnimatorController = _AlliedController;
        }
        else if (team == 2)
        {
            _Animator.runtimeAnimatorController = _EnemyController;
        }
        else
            Debug.LogWarning("Wrong team passed in");
    }

    private void setUpProperBoxCollider()
    {
        //To-Do: Figure out the proper size for this species box Collider, then set it
       BaseHitBox hitBox= this.gameObject.GetComponentInChildren<BaseHitBox>();
        if(hitBox)
        {
            BoxCollider2D collider= hitBox.GetComponent<BoxCollider2D>();
            if(collider)
            {
                collider.size = _BoxColliderSize;
                collider.offset = _BoxColliderOffset;
            }
        }
    }
}
