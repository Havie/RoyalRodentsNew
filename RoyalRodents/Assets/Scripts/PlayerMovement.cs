using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{

    public CharacterControllerTMP controller;
    public Animator _animator;


    private float _moveSpeed;
    private float _horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;
    private bool _AttackDelay;
    private bool _isAttacking;
    private bool _isHealing;
    private float _damage;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [SerializeField]
    private List<GameObject> _InRange = new List<GameObject>();
    [SerializeField]
    private GameObject _MoveLocation;



    private bool isDead;
    private bool _controlled;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _moveSpeed = this.GetComponent<PlayerStats>()._MoveSpeed;
        _damage = this.GetComponent<PlayerStats>()._AttackDamage;
        _animator = this.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

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
        if (Input.GetMouseButton(1))
        {
            //old code from gamejam
            Heal();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // if (MVCController.Instance.checkIfAttackable(Input.mousePosition))
            // Attack();

            // Check 
            //Touch touch = Input.GetTouch(0);
            //Vector2 pos= touch.position;

            Vector3 input = Input.mousePosition;

            int count = Input.touchCount;
            if(count>0)
            {
               
               Touch touch = Input.GetTouch(0);

               if( touch.phase == TouchPhase.Began )
                    input = touch.position;
            }

            GameObject go = MVCController.Instance.checkClick(input);
            if (go && _controlled)
            {
               
                if (go == MVCController.Instance._dummyObj)
                {
                    //if the item returned is a UI element do nothing?
                    Debug.Log("Received Dummy OBJ in playerMove");

                }
                // possibly move toward it with normalized direction
                else
                {

                    Debug.Log("Location for " + go + "   is " + go.transform.position);

                    //figure out if the collider is on a building we own
                    if(go.transform.parent)
                    {
                        //check if its a building
                        if(go.transform.parent.GetComponent<BuildableObject>())
                        {
                            Debug.Log("Found a BuildableObject");
                            //check team
                            //player team - do not move
                            if(go.transform.parent.GetComponent<BuildableObject>().getTeam()==1)
                            {
                                //do nothing 
                            }
                            else // enemy team move to it ( no such thing as neutral buildings?)
                            {
                                _MoveLocation.transform.position = go.transform.position;
                                _horizontalMove = (_MoveLocation.transform.position - this.transform.position).normalized.x * _moveSpeed;
                            }
                        }
                        //check if its a rodent place 1 - parent could be the spawn volume or Player Rodent list
                        else if (go.GetComponent<Rodent>())
                        {

                            Debug.Log("Found a Rodent w parent");
                            //check team
                            //player team - do not move
                            if (go.GetComponent<Rodent>().getTeam() == 1 || go.GetComponent<Rodent>().getTeam() == 0)
                            {
                                //do nothing 
                            }
                            else //enemy
                            {
                                //check in range

                                // else move toward it? 
                                Debug.Log("Move toward Rodent on Team:" + go.GetComponent<Rodent>().getTeam());

                                _MoveLocation.transform.position = go.transform.position;
                                _horizontalMove = (_MoveLocation.transform.position - this.transform.position).normalized.x * _moveSpeed;
                            }
                        }
                    }
                    //check if its a rodent Place 2 - no parent? possible?
                    else if (go.GetComponent<Rodent>())
                    {
                        Debug.LogWarning("Found a Rodent no parent shouldnt happen"); 
                    }


                }
            }
            else if (_controlled)
            {
                Debug.Log("No go, so move to mouse loc , which will need to change for touch");
                //make sure the click is far enough away from us 
                StartCoroutine(MoveDelay());
               
            }

        }

    }

    IEnumerator MoveDelay()
    {
        //Keep track of old Y
        float _oldY = _MoveLocation.transform.position.y;
        //first move the _Move Location somewhere absurd to reset collision enter with DummyObj
        _MoveLocation.transform.position = new Vector3(0, 3200, 0);
        //wait a split second to reset collision
        yield return new WaitForSeconds(0.1f);
        // pick the actual correct location to move
        _MoveLocation.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, _oldY, 0);
        float _moveDis = (_MoveLocation.transform.position - this.transform.position).normalized.x;
       
        // Debug.Log("MoveDis:: " + _moveDis);

        // an extra layer so we dont move if the click is too close
        if (Mathf.Abs(_moveDis) > 0.6f)
            _horizontalMove = _moveDis * _moveSpeed;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            // move our character
            if (_horizontalMove != 0)
                _animator.SetBool("IsMoving", true);
            else
                _animator.SetBool("IsMoving", false);

            Move(_horizontalMove * Time.fixedDeltaTime, crouch, jump);
        }
        else
            _animator.SetBool("IsMoving", false);
    }
    public void StopMoving()
    {
        _horizontalMove = 0;
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
    // A Coroutine that can set a delay that is partly responsible for how long till we can attack again
    // also handles our damage output via ray casting in front of us
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


        // Defines a layer mask that only looks at the "buildings" and "Player" Layer(s)
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
            ResourceManagerScript.Instance.incrementTrash(-1);
            _animator.SetTrigger("Dead");

        }
    }
    public void Move(float move, bool crouch, bool jump)
    {
        //Debug.Log("we are moving This much:" + move);

        if (!_isAttacking)
        {
            // Move the character by finding the target velocity
            //Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            // m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);


            this.transform.position += new Vector3(move, 0, 0);


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
        // Switch the way the player is labeled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        //Fix Children HealthBar
        int index = this.transform.childCount;
        for (int i = 0; i < index; ++i)
        {
            Transform t = this.transform.GetChild(i);
            ScaleKeeper sk = t.GetComponent<ScaleKeeper>();
            if (sk)
            {
                Vector3 _properScale = sk.getScale();

                if (!m_FacingRight)
                    _properScale = new Vector3(-_properScale.x, _properScale.y, _properScale.z);

                t.localScale = _properScale;
            }
        }
    }

    public void setControlled(bool cond)
    {
        // Debug.Log("Player Is Controlled=" + cond);
        _controlled = cond;
    }


    //Collect Pickups and search things
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Enter Collision with" + collision.transform.gameObject);

        if (_MoveLocation == collision.gameObject)
            _horizontalMove = 0;

        if (collision.transform.GetComponent<Searchable>())
        {
            Searchable s = collision.transform.GetComponent<Searchable>();
            {
                s.setActive(true);
                //Do not add to our list of objects in range?
            }
        }
        else if (collision.transform.GetComponent<BuildableObject>())
        {
            //Add to our list of interactable things in range
            _InRange.Add(collision.gameObject);
        }

        else if (collision.transform.GetComponent<Rodent>())
        {
            //Add to our list of interactable things in range
            _InRange.Add(collision.gameObject);
        }

        ///Old Game Jam code, could be reused for pickups 
        else if (collision.transform.GetComponent<CoinResource>())
        {
            // if (collision.transform.GetComponent<CoinResource>().isActive())
            {
                ResourceManagerScript.Instance.incrementTrash(1);
                Destroy(collision.gameObject);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Searchable>())
        {
            Searchable s = collision.transform.GetComponent<Searchable>();
            {
                s.setActive(false);
            }
        }
        else if (collision.transform.GetComponent<BuildableObject>())
        {
            _InRange.Remove(collision.gameObject);
        }

        else if (collision.transform.GetComponent<Rodent>())
        {
            _InRange.Remove(collision.gameObject);
        }
    }



}


