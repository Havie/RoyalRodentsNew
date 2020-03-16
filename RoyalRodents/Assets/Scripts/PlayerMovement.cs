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
    [SerializeField]
    private bool _InGround = false;
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

    private bool _wantToAttack;
    private GameObject _AttackTarget;
    [SerializeField]
    private DiggableTile _CurrentSoilTile;
    [SerializeField]
    private DiggableTile _CurrentTunnelTile;
    private float _YHeight;
    private float _YHeightDummy;

    PlayerStats _PlayerStats;
    private int _AttackCost = 4;
    private int _TunnelCost = 15;

    private bool isDead;
    private bool _controlled;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public void LoadData()
    {
        Debug.Log("Load Called ");
        sPlayerData data = sSaveSystem.LoadPlayerData();
        if (data != null)
        {
            _InGround = false;
           // m_FacingRight = data._FacingRight;
            _YHeight = data._YHeight;

            //Spawn Above Ground for now
            this.transform.position = new Vector3(data.position[0], _YHeight, 0);
            _MoveLocation.transform.position = new Vector3(data.position[0], _YHeightDummy, 0);

            StopMoving();

            // Cant save Diggable Tiles, could be an issue
            // _CurrentSoilTile = data._CurrentTopTile;
            //  _CurrentTunnelTile = data._CurrentTunnelTile;

        }
        else
            Debug.LogError("no SaveData to Load");
    }
    public bool getInGround() =>  _InGround;
    public bool getIsAttacking() => _isAttacking;
    public bool getIsFacingRight() => m_FacingRight;
    public bool getIsControlled() => _controlled;
    public float getYHeight() => _YHeight;
    public DiggableTile getCurrentSoilTile() => _CurrentSoilTile;
    public DiggableTile getCurrentTunnelTile() => _CurrentTunnelTile;
    public Vector3 getLastAboveGroundLoc()
    {
        if (_CurrentSoilTile == null)
            return this.transform.position;
        else
            return _CurrentSoilTile.transform.position;}
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(" Called ");
        _PlayerStats = this.GetComponent<PlayerStats>();
        if (_PlayerStats)
        {
            _moveSpeed = _PlayerStats.getMoveSpeed();
            _damage = _PlayerStats.getAttackDamage();
        }
        else
            Debug.LogError("NoPlayerStatsFound");

        _animator = this.GetComponent<Animator>();
        _YHeight = this.transform.position.y;
        _YHeightDummy = _MoveLocation.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if (Input.GetMouseButton(1))
        {
            //old code from game jam
            Heal();
        }
        if (!CheckDig() && (Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !_InGround)
        {

            Vector3 input = Input.mousePosition;

            int count = Input.touchCount;
            if (count > 0)
            {

                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                    input = touch.position;
            }

            GameObject go = MVCController.Instance.checkClick(input);
            if (go && _controlled)
            {

                // possibly move toward it with normalized direction
                if (go != MVCController.Instance._dummyObj)
                {
                    Debug.Log("Location for " + go + "   is " + go.transform.position);
                    //figure out if the collider is on a building we own
                    if (go.transform.parent)
                    {
                        //check if its a building
                        if (go.transform.parent.GetComponent<BuildableObject>())
                        {
                            // Debug.Log("Found a BuildableObject");
                            //check team
                            //player team - do not move
                            if (go.transform.parent.GetComponent<BuildableObject>().getTeam() == 1)
                            {
                                //do nothing - this is our building
                                StopMoving();
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
                            //Debug.Log("Found a Rodent w parent");
                            //check team
                            if (go.GetComponent<Rodent>().getTeam() == 1 || go.GetComponent<Rodent>().getTeam() == 0)
                            {
                                //do nothing  //player team - do not move
                                StopMoving();
                            }
                            else //enemy
                            {
                                //check in range
                                if (_InRange.Contains(go.gameObject))
                                {
                                    //decide if we need to flip to face in case we walked past
                                    DecideIfNeedToFlip(go.gameObject.transform.position);
                                    // Debug.Log("Attack!");
                                    Attack();
                                }
                                else
                                {
                                    // Debug.Log("Move toward Rodent on Team:" + go.GetComponent<Rodent>().getTeam());
                                    //and set goal to attack it
                                    _wantToAttack = true;
                                    _AttackTarget = go.gameObject;
                                    //move towards it
                                    StartCoroutine(MoveDelay(input, go.transform.position));

                                    _MoveLocation.transform.position = go.transform.position;
                                    float _MoveAmnt = (_MoveLocation.transform.position - this.transform.position).normalized.x * _moveSpeed;

                                }
                            }
                        }
                    }
                    //check if its a rodent Place 2 - no parent? possible?
                    else if (go.GetComponent<Rodent>())
                    {
                        Debug.LogWarning("Found a Rodent with no parent shouldn't happen, check Inspector");
                    }


                }
            }
            else if (_controlled)
            {
                Debug.Log("No go, so move to mouse loc , which will need to change for touch");
                //make sure the click is far enough away from us 
                StartCoroutine(MoveDelay(input));
                _wantToAttack = false;

            }

        }


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

            Move(_horizontalMove * Time.fixedDeltaTime, _InGround, jump);
        }
        else
            _animator.SetBool("IsMoving", false);
    }

    IEnumerator MoveDelay(Vector3 input)
    {
        //Keep track of old Y
        float _oldY = _MoveLocation.transform.position.y;
        //first move the _Move Location somewhere absurd to reset collision enter with DummyObj
        _MoveLocation.transform.position = new Vector3(0, 3200, 0);
        //wait a split second to reset collision
        yield return new WaitForSeconds(0.05f);
        // pick the actual correct location to move
        _MoveLocation.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(input).x, _oldY, 0);
        float _moveDis = (_MoveLocation.transform.position - this.transform.position).normalized.x;

        Debug.Log("MoveDis:: " + _moveDis);
        Debug.Log("This trans:: " + this.transform.position);
        Debug.Log("Dummy trans:: " + _MoveLocation.transform.position);

        // an extra layer so we dont move if the click is too close
        if (Mathf.Abs(_moveDis) > 0.6f)
            _horizontalMove = _moveDis * _moveSpeed;
        else
            Debug.Log("not far");
    }
    IEnumerator MoveDelay(Vector3 input, Vector3 _movePos)
    {
        //Keep track of old Y
        float _oldY = _MoveLocation.transform.position.y;
        //first move the _Move Location somewhere absurd to reset collision enter with DummyObj
        _MoveLocation.transform.position = new Vector3(0, 3200, 0);
        //wait a split second to reset collision
        yield return new WaitForSeconds(0.1f);
        // pick the actual correct location to move
        _MoveLocation.transform.position = _movePos;
        float _MoveAmnt = (_MoveLocation.transform.position - this.transform.position).normalized.x; // if we add a multiply here we get a charge speed effect

        // Debug.Log("MoveDis:: " + _moveDis);

        // an extra layer so we dont move if the click is too close
        if (Mathf.Abs(_MoveAmnt) > 0.6f)
            _horizontalMove = _MoveAmnt * _moveSpeed;
    }
    public void StopMoving()
    {
        _horizontalMove = 0;
    }
    private bool CheckDig()
    {
       
        if (!_InGround && Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_CurrentSoilTile )
            {
              StartCoroutine(DigDelay(Vector2.down, _CurrentSoilTile));
            }

        }
        else if (_InGround && _horizontalMove==0)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Need to check tile to the right
                if (CheckTile("right"))
                {
                    
                }
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //Need to check tile to the left
                if (CheckTile("left"))
                {
                   
                }
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (CheckTile("up"))
                {

                }
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_CurrentSoilTile)
                {
                    if (CheckTile("down"))
                    {

                    }
                     return true;
                }

            }
        }
        return false;
    }
    IEnumerator DigDelay(Vector2 dir, DiggableTile dt)
    {
        bool _okayToMove=true;
        StopMoving();
        if (dt.isDiggable())
        {
            if (!dt.isOpen())
            {
                if (_PlayerStats.getStamina() >=_TunnelCost)
                {
                    _PlayerStats.IncrementStamina(-_TunnelCost);
                    //play Anim
                    _animator.SetTrigger("doDig");
                    yield return new WaitForSeconds(2f);
                    dt.DigTile();
                }
                else
                    _okayToMove = false;
            }
            if (_okayToMove)
            {
                if (!_InGround)
                {
                    _animator.SetBool("InGround", true);
                    _InGround = true;
                }
                _CurrentTunnelTile = dt;
                if (dir == Vector2.right)
                {
                    _horizontalMove = 0.75f;
                }
                else if (dir == Vector2.left)
                {
                    _horizontalMove = -0.75f;
                }
                else if (dir == Vector2.down || dir == Vector2.up)
                {
                    //Calculate distance because of weird anchor points? - keep player in middle of tile
                    float newY = (this.transform.position.y - dt.transform.position.y); //can Div by 2 to alter if sprite is wrong pos
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - newY, 0);
                }
            }
        }
    }
    public bool CheckTile(string Direction)
    {
        // Debug.Log("ourPos:" + this.transform.position);
        Vector3 location;
        Vector2 directionVector = Vector2.zero;
        if (_CurrentTunnelTile != null)
            location = _CurrentTunnelTile.transform.position;
        else
            location = _CurrentSoilTile.transform.position;




        if (Direction.Equals("right"))
        {
            directionVector = Vector2.right;
           // location += new Vector3(-1f, 0, 0);
        }
        else if (Direction.Equals("left"))
        {
            directionVector = Vector2.left;
            //Character is oddly offset due to tail?
            //location += new Vector3(1, 0, 0);
        }
        else if (Direction.Equals("up"))
        {
            directionVector = Vector2.up;
          // location += new Vector3(0.8f, 0, 0);
        }
        else if (Direction.Equals("down"))
        {
            directionVector = Vector2.down;
            // location += new Vector3(0.8f, 0, 0);
        }
        LayerMask _LayerMask = (1 << 11);

        // initial hit has to stay right.. or it misses left.. idk wtf its doing
        RaycastHit2D initialHit = Physics2D.Raycast(location, directionVector, 2f, _LayerMask);
        RaycastHit2D[] hits = Physics2D.RaycastAll(location, directionVector, 8f, _LayerMask);

        //Debug stuff

        Vector2 LocRaised = new Vector2(location.x -0.1f, location.y + 0.2f);
        Vector3 pos = (LocRaised + (directionVector * 2));
        Debug.DrawLine(LocRaised, pos, Color.blue, 3f);
        Vector3 pos2 = (new Vector2(location.x, location.y) + (directionVector * 8));
        Debug.DrawLine(location, pos2, Color.red, 3f);




        GameObject localCurrentTile;
        //Have to be able to find the current tile were on first (Note: _CurrentTile Global is next tile at this point)
        if (initialHit.collider)
        {
            //Debug.Log("Local current tile=" + initialHit.collider.gameObject);
            localCurrentTile = initialHit.collider.gameObject;

            foreach (RaycastHit2D h in hits)
            {
                if (h.collider.gameObject != localCurrentTile)
                {
                    if (h.collider.gameObject.GetComponent<DiggableTile>())
                    {
                      //  Debug.Log("Found DiggableTile:" + h.collider.gameObject);
                        DiggableTile dt = h.collider.gameObject.GetComponent<DiggableTile>();
                        if (dt.isDiggable())
                        {
                           // Debug.Log("Its True:" + h.collider.gameObject);
                            if ((directionVector == Vector2.right || directionVector == Vector2.left))
                            {
                                if (!dt.isTopSoil())
                                {
                                    StartCoroutine(DigDelay(directionVector, dt));
                                    _MoveLocation.transform.position = h.collider.gameObject.transform.position;
                                    return true;
                                }
                            }
                            else //if(DirectionVector==Vector2.up) up and down
                            {
                                StartCoroutine(DigDelay(directionVector, dt));
                                return true;
                            }
                        }

                    }

                }
            }
        }

            if(directionVector == Vector2.up && _CurrentTunnelTile.isTopSoil() && _CurrentTunnelTile.isDiggable())
            {
           // Debug.Log("IsPassable Top tile:" +_CurrentTopTile);
            //go up to initial ground level
            this.transform.position = new Vector3(this.transform.position.x, _YHeight, 0);
                _MoveLocation.transform.position= new Vector3(this.transform.position.x, _YHeight, 0);
                _InGround = false;
                _animator.SetBool("InGround", false);
            }
        


        return false;
    }
    public void Attack()
    {
        StopMoving();
        if (!_AttackDelay)
        {
            //Drain stamina && CHECK if enough
            PlayerStats ps = this.transform.GetComponent<PlayerStats>();
            if (ps)
            {
                if (ps.getStamina() > _AttackCost)
                {
                    ps.IncrementStamina(-_AttackCost);
                    _isAttacking = true;
                    _animator.SetTrigger("Attack");
                    StartCoroutine(AttackRoutine());
                }
            }

        }

    }
    /** A Coroutine that can set a delay that is partly responsible for how long till we can attack again
    * also handles our damage output via ray casting in front of us */
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

        if (_AttackTarget != null)
        {
            if (_AttackTarget.GetComponent<Rodent>())
            {
                _AttackTarget.GetComponent<Rodent>().Damage(_damage);
            }
            else if (_AttackTarget.GetComponent<BuildableObject>())
            {
                _AttackTarget.GetComponent<BuildableObject>().Damage(_damage);
            }
        }


        /* This logic is only finding the portrait outline?? perhaps my transform height is off
        // Defines a layer mask that only looks at the "buildings" and "Player" Layer(s)
        LayerMask _LayerMask = (1 << 8) | (1 << 9);
        RaycastHit2D hit = Physics2D.Raycast(_startPos, _ourDir, 3.75f, _LayerMask);
        RaycastHit2D[] hits = Physics2D.RaycastAll(_startPos, _ourDir, 3.75f, _LayerMask);

        //Drawing a Ray doesnt work?
        //Debug.DrawRay(_startPos, _ourDir, Color.red);

        foreach (var h in hits)
        {
            Debug.Log(h.collider.gameObject);

        }

        Debug.Log("Hit Dis:" + hit.distance);

        if (hit.collider != null)
        {
            Debug.Log("Found :" + hit.collider.gameObject.name);
            GameObject go = hit.collider.gameObject;
            if (go == _AttackTarget)
            {
               if(go.GetComponent<Rodent>())
                {
                    go.GetComponent<Rodent>().Damage(_damage);
                }
               else if (go.GetComponent<BuildableObject>())
                {
                    go.GetComponent<BuildableObject>().Damage(_damage);
                }
            }

           
            AIController ai = hit.collider.GetComponent<AIController>();
            if (ai)
            {
                ai.Damage(_damage);
            }
        }
        */
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
                FlipDirection();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                FlipDirection();
            }
        }
        else
            _animator.SetBool("IsMoving", false);
    }

    private void DecideIfNeedToFlip(Vector3 targetPos)
    {
        if (transform.position.x > targetPos.x)
        {
            // Flip if facing right
            if (m_FacingRight)
            {
                FlipDirection();
            }

        }
        else
        {
            // Flip if facing left
            if (!m_FacingRight)
            {
                FlipDirection();
            }

        }
    }

    private void FlipDirection()
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
    public GameObject getDummy()
    {
        return _MoveLocation;
    }

    //Collect Pickups and search and attack things
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Enter Trigger with" + collision.transform.gameObject);

        if (_wantToAttack && _AttackTarget != null)
        {
            if (collision.gameObject.transform.parent)
            {
                if (collision.gameObject.transform.parent.gameObject == _AttackTarget)
                    Attack();
            }
        }
        else if (_MoveLocation == collision.gameObject)
        {
            _horizontalMove = 0;
        }

        if (collision.transform.GetComponent<Searchable>())
        {
            Searchable s = collision.transform.GetComponent<Searchable>();
            {
                s.setActive(true);
                //Do not add to our list of objects in range?
            }
        }
        else if (collision.transform.GetComponent<DiggableTile>())
        {
            // Debug.Log("Collider w diggable tile");
            if(!_InGround) // only keep track of top soil tiles
              _CurrentSoilTile = collision.transform.GetComponent<DiggableTile>();
        }
        else if (collision.transform.parent)
        {
            // handle if collider is agro range or base range
            if (collision.transform.GetComponent<BaseHitBox>())
            {

                if (collision.transform.parent.GetComponent<BuildableObject>())
                {
                    //Add to our list of interactable things in range
                    _InRange.Add(collision.transform.parent.gameObject);
                }

                else if (collision.transform.parent.GetComponent<Rodent>())
                {
                    //Add to our list of interactable things in range
                    _InRange.Add(collision.transform.parent.gameObject);
                }
            }
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

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Searchable>())
        {
            Searchable s = collision.transform.GetComponent<Searchable>();
            {
                s.setActive(false);
            }
        }
        else if (collision.transform.GetComponent<DiggableTile>())
        {
            // Debug.Log("Exited Collision w diggable tile");

            //Possible to collided with a New Tile Before Exit is called so need this check
            if (!_InGround) // only keep track of top soil tiles
            {
                if (_CurrentSoilTile == collision.transform.GetComponent<DiggableTile>())
                    _CurrentSoilTile = null;
            }

        }
        else if (collision.transform.parent)
        {
            // handle if collider is agro range or base range
            if (collision.transform.GetComponent<BaseHitBox>())
            {
                if (collision.transform.parent.GetComponent<BuildableObject>())
                {
                    _InRange.Remove(collision.transform.parent.gameObject);
                }

                else if (collision.transform.parent.GetComponent<Rodent>())
                {
                    _InRange.Remove(collision.transform.parent.gameObject);
                }
            }
        }

    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("ENTER Collision with " + collision.gameObject);


    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.Log("EXIT Collision with " + collision.gameObject);

    }
}





