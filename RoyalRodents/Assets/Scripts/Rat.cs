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
    private float _MoveSpeed = 5f;
    private float _AttackDamage = 1f;



    private void Awake()
    {
        _Default = Resources.Load<Sprite>("Rodent/FatRat/RatSprite_0");
        _AnimatorController = Resources.Load<RuntimeAnimatorController>("Rodent/FatRat/RatSprite_1");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = _Default;
        //this.GetComponent<Animator>().runtimeAnimatorController = _AnimatorController;

        //Tmp until getter/setter
        this.GetComponent<SubjectScript>().moveSpeed = _MoveSpeed;
        //this.GetComponent<Rodent>().set= _MoveSpeed;

        //TMP Test - Finds and follows the player
        this.GetComponent<SubjectScript>().target = GameObject.FindObjectOfType<PlayerStats>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
