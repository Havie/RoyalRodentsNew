﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : MonoBehaviour
{
    public Sprite _Default;

    private float _Hp = 100f;
    private float _HpMax = 100f;
    [Range(0, 10f)]
    private float _MoveSpeed = 3f;
    private float _AttackDamage = 3f;
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
        _Default = Resources.Load<Sprite>("Rodent/FatRat/RatSprite_0");
         _Portrait = Resources.Load<Sprite>("TMPAssests/tmpRat");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = _Default;
        setUpAnimators();
        Rodent r = this.GetComponent<Rodent>();
        if(r)
        {
            r.setRodentType(Rodent.eRodentType.Rat); 
            r.setSpeed(_MoveSpeed);
            r.setHpMax(_HpMax);
            r.setHp(_Hp);
            r.setAttackDmg(_AttackDamage);
            r.setPortrait(_Portrait);
            r.setRecruitmentCost(_RecruitmentCost);
           
        }


        //TMP Test - Finds and follows the player
        // this.GetComponent<SubjectScript>().currentTarget = GameObject.FindObjectOfType<PlayerStats>().gameObject;
    }

   private void setUpAnimators()
    {
        _Animator = this.GetComponent<Animator>();
        if (_Animator == null)
            Debug.LogWarning("Cant Find Animator on Rat??");

        _NeutralController = Resources.Load<RuntimeAnimatorController>("Rodent/FatRat/NeutralRatController");
        if (_NeutralController == null)
            Debug.LogWarning("Cant Find Neutral Controller on Rat");

        _AlliedController = Resources.Load<RuntimeAnimatorController>("Rodent/FatRat/AlliedRatContoller");
        if (_AlliedController == null)
            Debug.LogWarning("Cant Find Allied Controller on Rat");

        _EnemyController = Resources.Load<RuntimeAnimatorController>("Rodent/FatRat/EnemyRatController");
        if (_EnemyController == null)
            Debug.LogWarning("Cant Find Enemy Controller on Rat");

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
    }
}