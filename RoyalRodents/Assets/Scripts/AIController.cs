using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IDamageable<float>
{

    public Animator _Animator;
    public GameObject _target;
    public float _MovementSpeed = 2f;

    public float _health = 25f;
    public float _healthMax = 25.5f;
    public float _damage = 3f;
    public GameObject _HealthBarObj;
    private HealthBar _HealthBar;

    public Vector3 testGoalPos;

    public GameObject _skull;

    private float minDistance = 1f;
    private bool _FacingRight;
    private bool _isAttacking;
    private bool _AttackDelay;


    //Interface Stuff
    public void Damage(float damageTaken)
    {
        if (_health - damageTaken > 0)
            _health -= damageTaken;
        else
        {
            _health = 0;
            Die();
        }

       // Debug.LogWarning("HP=" + _health);
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
            _HealthBar.SetHealth(_health / _healthMax);
    }

    // Start is called before the first frame update
    void Start()
    {
        _Animator = this.GetComponent<Animator>();
        if (!_Animator)
            Debug.LogError("AI Controller Missing Animator");
        _health = 25.5f;
        _healthMax = 25.5f;
        _damage = 3f;
        if (_HealthBarObj == null)
            _HealthBarObj = Resources.Load<GameObject>("UI/HealthBarCanvas");
        SetUpHealthBar(_HealthBarObj);
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
       if(_target)
        {
                Vector3 _goalPos;
                _goalPos = new Vector3(_target.transform.position.x,0,0);
            if (Mathf.Abs(transform.position.x - _goalPos.x) >= minDistance)
            {
                MoveToTarget(_goalPos);
                testGoalPos = _goalPos;
            }
            else
                Attack();
        }

    }
    void LateUpdate()
    {
        
    }


    public void MoveToTarget(Vector3 pos)
    {
        if (!_isAttacking)
        {

            if (pos.x > 0)
            {
                if (pos.x > transform.position.x)
                {
                    transform.position += pos.normalized * Time.deltaTime * _MovementSpeed;
                  //  Debug.LogError("case1");
                    if (!_FacingRight)
                        Flip();
                }
                else if (pos.x < transform.position.x)
                {
                    transform.position -= pos.normalized * Time.deltaTime * _MovementSpeed;
                   // Debug.LogError("case2");
                    if (_FacingRight)
                        Flip();
                }

            }
            else if (pos.x <= 0)
            {
                if (pos.x > transform.position.x)
                {
                    transform.position -= pos.normalized * Time.deltaTime * _MovementSpeed;
                    //Debug.LogError("case3");
                    if (!_FacingRight)
                        Flip();
                }
                else if (pos.x < transform.position.x)
                {
                    transform.position += pos.normalized * Time.deltaTime * _MovementSpeed;
                   // Debug.LogError("case4");
                    if (_FacingRight)
                        Flip();
                }
            }
            _Animator.SetBool("IsMoving", true);
        }
            

    }
    public void Attack()
    {
        if (!_AttackDelay)
        {

            _isAttacking = true;

            _Animator.SetTrigger("Attack");

            _target.GetComponent<PlayerStats>().Damage(_damage);
            StartCoroutine(AttackEnd());
        }

    }
    IEnumerator AttackEnd()
    {
        _AttackDelay = true;
        yield return new WaitForSeconds(1.5f);
        _isAttacking = false;
        _AttackDelay = false;
    }
    public void Die()
    {
        //Play an Anim
        _Animator.SetTrigger("Dead");
        StartCoroutine(Death());
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(1f);
        if (_skull)
        {
            _target=    Instantiate(_skull, this.transform.position, Quaternion.identity);
            //Debug.Log("SkullMade@" + _target.transform.position);
        }
        else
            Debug.LogWarning("noSkull");
        
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<PlayerMovement>())
        {
            _target = collision.gameObject;
            //MAJOR HACK needs fixing:
            this.transform.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _FacingRight = !_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
