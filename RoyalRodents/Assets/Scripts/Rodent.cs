using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rodent : MonoBehaviour , IDamageable<float>
{

    public float _Hp = 50f;
    public float _HpMax = 100f;
    [Range(0, 10f)]
    public float _MoveSpeed = 2f;
    public float _AttackDamage = 1f;
    public HealthBar _HealthBar;

    public string _Name="Rodent";

    [SerializeField]
    private RodentType _Type = RodentType.Default;

    public enum RodentType { Rat, Badger, Beaver, Raccoon, Mouse, Porcupine, Default };



    /**Interface Stuff */
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

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Die()
    {
        //Should this be in Rodent or in AIController which holds the Animator?
    }
}

