using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{

    public CharacterControllerTMP controller;
    public Animator _animator;


    private float _moveSpeed ;
    private float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;
    private bool _AttackDelay;
    private bool _isAttacking;
    private bool _isHealing;
    private float _damage;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;


    private bool isDead;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _moveSpeed = this.GetComponent<PlayerStats>()._Move_Speed;
        _damage = this.GetComponent<PlayerStats>()._AttackDamage;
        _animator = this.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * _moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            crouch = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            crouch = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(MVCController.Instance.checkIfAttackable(Input.mousePosition))
                Attack();
        }
        if (Input.GetMouseButton(1))
        {
            Heal();
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            // move our character
            if (horizontalMove != 0)
                _animator.SetBool("IsMoving", true);
            else
                _animator.SetBool("IsMoving", false);

            Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        }
        else
            _animator.SetBool("IsMoving", false);
    }

    public void Attack()
    {
        
        if (!_AttackDelay)
        {
            _isAttacking = true;
            _animator.SetTrigger("Attack");
            StartCoroutine(AttackRoutine());
        }

    }
    // A Couroutine that can set a delay that is partly responsible for howlong till we can attack again
    // also handles our damage output via raycasting in front of us
    IEnumerator AttackRoutine()
    {

        _AttackDelay = true;
        yield return new WaitForSeconds(0.1f);

        //Add to the starting pos so we dont target ourself
        Vector3 _startPos = this.transform.position;
        Vector3 _ourDir = Vector2.left;
       
            if (m_FacingRight)
            {
                _startPos += new Vector3(1, 0, 0);
                _ourDir = -Vector2.left;
            }
            else
                _startPos -= new Vector3(1, 0, 0);


        // Defines a layer mask that only looks at the "builings" and "Player" Layer(s)
        LayerMask _LayerMask = (1 << 8) | (1 << 9);
        RaycastHit2D hit = Physics2D.Raycast(_startPos, _ourDir, 0.75f, _LayerMask);

        //Drawing a Ray doesnt work?
        //Debug.DrawRay(_startPos, _ourDir, Color.red);

        //Debug.Log("Hit Dis:" + hit.distance);

        if (hit.collider != null)
        {
           // Debug.Log("Found :" + hit.collider.gameObject.name);
            AIController ai = hit.collider.GetComponent<AIController>();
            if (ai)
            {
                ai.Damage(_damage);
            }
        }

        yield return new WaitForSeconds(0.85f);
        _AttackDelay = false;
    }

    //Called from Engine-Animation Event
    public void attackDone()
    {
        StartCoroutine(AttackDoneC());
    }
    IEnumerator AttackDoneC()
    {
        yield return new WaitForSeconds(0.5f);
        _isAttacking = false;
    }

    public void Die()
    {
        isDead = true;
        _animator.SetTrigger("Dead");
        GameManager.Instance.youLose();
    }

    public void Heal()
    {
        if (GameManager.Instance._gold > 0)
        {
            this.GetComponent<PlayerStats>().Damage(-5);
            GameManager.Instance.incrementGold(-1);
            _animator.SetTrigger("Dead");

        }
    }
    public void Move(float move, bool crouch, bool jump)
    {
        if (!_isAttacking)
        {
            // Move the character by finding the target velocity
            //Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            // m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);


            this.transform.position += new Vector3( move , 0, 0);


            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        else
            _animator.SetBool("IsMoving", false);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<CoinResource>())
        {
            // if (collision.transform.GetComponent<CoinResource>().isActive())
            {
                GameManager.Instance.incrementGold(1);
                Destroy(collision.gameObject);
            }
        }
    }



}


