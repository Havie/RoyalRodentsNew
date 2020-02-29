using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{

    public Animator _Animator;
    public GameObject _target;
    public float _MovementSpeed = 2f;


    private HealthBar _HealthBar;

    public Vector3 testGoalPos;

    public GameObject _skull;

    private float minDistance = 0.5f;
    private bool _FacingRight;
    private bool _isAttacking;
    private bool _AttackDelay;


 

    // Start is called before the first frame update
    void Start()
    {
        _Animator = this.GetComponent<Animator>();
        if (!_Animator)
            Debug.LogError("AI Controller Missing Animator");

     
    }

    // Update is called once per frame
    void Update()
    {
        if (_target)
        {
            Vector3 _goalPos;
            _goalPos = new Vector3(_target.transform.position.x, 0, 0);
            if (Mathf.Abs(transform.position.x - _goalPos.x) >= minDistance )
            {
                
                float rand= Random.Range(-1, 1);
                if(rand>0 && _goalPos != testGoalPos)
                    MoveToTarget(0.2f*_goalPos);
                else if(rand==0)
                    _Animator.SetBool("IsMoving", false);
                else if(rand <0 && this.transform.position.x -_goalPos.x <15f)
                    MoveToTarget((-0.5f*_goalPos));

                testGoalPos = _goalPos;
            }
            else
                _Animator.SetBool("IsMoving", false);

        }

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
            _target = Instantiate(_skull, this.transform.position, Quaternion.identity);
            Debug.Log("SkullMade@" + _target.transform.position);
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
