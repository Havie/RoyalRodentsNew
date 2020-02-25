using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : MonoBehaviour
{
    public Sprite _Default;
    public RuntimeAnimatorController _AnimatorController;

    private float _Hp = 100f;
    private float _HpMax = 100f;
    [Range(0, 10f)]
    private float _MoveSpeed = 3f;
    private float _AttackDamage = 1f;
    [SerializeField]
    private Sprite _Portrait;



    private void Awake()
    {
        _Default = Resources.Load<Sprite>("Rodent/FatRat/RatSprite_0");
        _AnimatorController = Resources.Load<RuntimeAnimatorController>("Rodent/FatRat/RatController");
        _Portrait = Resources.Load<Sprite>("TMPAssests/tmpRat");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = _Default;
        this.GetComponent<Animator>().runtimeAnimatorController = _AnimatorController;

        Rodent r = this.GetComponent<Rodent>();
        if(r)
        {
            r.setSpeed(_MoveSpeed);
            r.setHpMax(_HpMax);
            r.setHp(_Hp);
            r.setAttackDmg(_AttackDamage);
            r.setPortrait(_Portrait);
        }


        //TMP Test - Finds and follows the player
        this.GetComponent<SubjectScript>().currentTarget = GameObject.FindObjectOfType<PlayerStats>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
