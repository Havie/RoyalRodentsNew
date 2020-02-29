using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour ,IDamageable<float>
{
    public float _Hp=50f;
    public float _HpMax = 100f;
    [Range(0, 10f)]
    public float _MoveSpeed = 3.5f;  //used to be 2
    public float _AttackDamage = 10f;
    public GameObject _HealthBarObj;
    private HealthBar _HealthBar;

    /**Begin Interface stuff*/
    public void Damage(float damageTaken)
    {
        if (_Hp - damageTaken > 0)
            _Hp -= damageTaken;
        else
        {
            _Hp = 0;
            //Sloppy?
            PlayerMovement pm = this.GetComponent<PlayerMovement>();
            if (pm)
                pm.Die();
            else
                Debug.LogError("Should have died but cant find PlayerMovement");
            
        }
        //Debug.LogWarning("HP=" + _Hp);
        UpdateHealthBar();
    }

    public void SetUpHealthBar(GameObject go)
    {
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("HealthBarCanvas");
        if (_HealthBarObj)
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
    }

    public void UpdateHealthBar()
    {
        if (_HealthBar)
            _HealthBar.SetHealth(_Hp / _HpMax);
    }
    /**End Interface stuff*/


    // Start is called before the first frame update
    void Start()
    {
        _Hp = 50f;
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("UI/HealthBarCanvas");
        SetUpHealthBar(_HealthBarObj.gameObject);
        UpdateHealthBar();
    }

    void LateUpdate()
    {
    
    }
}
