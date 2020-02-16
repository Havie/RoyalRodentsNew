using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour ,IDamageable<float>
{
    public float _Hp=50f;
    public float _HpMax = 100f;
    [Range(0, 10f)]
    public float _Move_Speed = 2f;
    public float _AttackDamage = 10f;
    public HealthBar _HealthBar;

    //Interface Stuff
    public void Damage(float damageTaken)
    {
        if (_Hp - damageTaken > 0)
            _Hp -= damageTaken;
        else
        {
            _Hp = 0;
            //Super slop::
            this.GetComponent<PlayerMovement>().Die();
            
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

    // Start is called before the first frame update
    void Start()
    {
        _Hp = 50f;
        Debug.Log("HP=" +_Hp);
        SetUpHealthBar(_HealthBar.gameObject);
        UpdateHealthBar();
    }

    void LateUpdate()
    {
        _HealthBar.transform.position = this.transform.position + new Vector3(0, 1, 0);
    }
}
