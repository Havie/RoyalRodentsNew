using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{

    public CharacterControllerTMP controller;
    public Animator _animator;


    private float _moveSpeed;
    [SerializeField] private float _horizontalMove = 0f;
    private bool jump = false;
    [SerializeField]
    private bool _InGround = false;
    [SerializeField] private bool _AttackDelay;
    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _isHealing;
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

    [SerializeField] private bool isDead;
    [SerializeField]  bool _controlled;
    private bool _mobileMoveDelay;

    private enum eSwipeDirection { Up, Left, Right, Down, Default };
    private eSwipeDirection _swipeDir= eSwipeDirection.Default;
    Vector2 firstPressPos;
    Vector2 secondPressPos;

    //Debug
    public UIDebuggPrints _debugger;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public void LoadData()
    {
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
                if (_debugger)
                    _debugger.Log("TOUCH COUNT =" + count);
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                    input = touch.position;
            }
            else
            {
                if(_debugger)
                    _debugger.Log("TOUCH COUNT =" + count);
            }

            GameObject go = MVCController.Instance.CheckClick(input);
            if (!_controlled)
                StopMoving();

            if (go && _controlled)
            {
                // possibly move toward it with normalized direction
                if (go != MVCController.Instance._dummyObj)
                {
                   // Debug.Log("Location for " + go + "   is " + go.transform.position);
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
                            else if (go.transform.parent.GetComponent<BuildableObject>().getTeam() == 500)
                            {

                                //clicked a searchable object without a rodent assigned 
                                if (go.transform.parent.GetComponent<Searchable>())
                                {
                                    //check if its in range
                                    if (_InRange.Contains(go.transform.parent.gameObject))
                                        StopMoving();

                                    else
                                    {
                                        StartCoroutine(MoveDelay(input));
                                    }
                                }
                                else //do nothing - clicked a dirt mound

                                    StopMoving();

                            }
                            else // enemy team move to it ( no such thing as neutral buildings?)
                            {
                                //To:Do will have to handle in range  just like searchable object above for enemy
                                if (_InRange.Contains(go.transform.parent.gameObject))
                                {
                                    //decide if we need to flip to face in case we walked past
                                    DecideIfNeedToFlip(go.gameObject.transform.position);
                                    _AttackTarget = go.transform.parent.gameObject;
                                    Attack();
                                }
                                else
                                {
                                    //Debug.LogWarning("move to attck target" + go.gameObject);
                                    // Debug.Log("Move toward Rodent on Team:" + go.GetComponent<Rodent>().getTeam());
                                    //and set goal to attack it
                                    _wantToAttack = true;
                                    _AttackTarget = go.transform.parent.gameObject;
                                    //move towards it
                                    StartCoroutine(MoveDelay(input, go.transform.position));
                                    _MoveLocation.transform.position = go.transform.position;
                                    //this might be completely extra?
                                    _horizontalMove = (_MoveLocation.transform.position - this.transform.position).normalized.x * _moveSpeed;
                                }
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
                                    Debug.Log("In range contains, so Attack!");
                                    _AttackTarget = go.gameObject;
                                    Attack();
                                }
                                else
                                {
                                    Debug.LogWarning("move to attck target");
                                    // Debug.Log("Move toward Rodent on Team:" + go.GetComponent<Rodent>().getTeam());
                                    //and set goal to attack it
                                    _wantToAttack = true;
                                    _AttackTarget = go.gameObject;
                                    //move towards it
                                    StartCoroutine(MoveDelay(input, go.transform.position));

                                    _MoveLocation.transform.position = go.transform.position;
                                    //this might be completely extra?
                                    _horizontalMove = (_MoveLocation.transform.position - this.transform.position).normalized.x * _moveSpeed;
                                    //Should this be Horiz move=
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
                else
                {
                    //To:Do Figure when this happens?
                    //print("go was dummy.. does this happen?");
                    //StopMoving();
                }
            }
            else if (_controlled)
            {
                //Debug.Log("No go, so move to mouse loc , which will need to change for touch");
                if (_debugger)
                    _debugger.Log("No go, so move to  loc");

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
        if (!_mobileMoveDelay)
        {
            _mobileMoveDelay = true;
            if (_debugger)
            {
                _debugger.Log("Current position=" + this.transform.position);
                _debugger.Log("Dummy position=" + _MoveLocation.transform.position);
                _debugger.Log("position to move=" + input);
                _debugger.Log("CameraToWorld   =" + new Vector3(Camera.main.ScreenToWorldPoint(input).x, _MoveLocation.transform.position.y, 0));

            }

            //Keep track of old Y
            float _oldY = _MoveLocation.transform.position.y;
            //first move the _Move Location somewhere absurd to reset collision enter with DummyObj
            _MoveLocation.transform.position = new Vector3(0, 3200, 0);
            //wait a split second to reset collision
            yield return new WaitForSeconds(0.05f);
            // pick the actual correct location to move
            _MoveLocation.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(input).x, _oldY, 0);
            float _moveDis = (_MoveLocation.transform.position - this.transform.position).normalized.x;

            // Debug.Log("MoveDis:: " + _moveDis);

            // an extra layer so we dont move if the click is too close
            if (Mathf.Abs(_moveDis) > 0.6f)
                _horizontalMove = _moveDis * _moveSpeed;

            if (_debugger)
            {
                _debugger.Log("Move dis: " + _moveDis);
                _debugger.Log("horizMove: " + _horizontalMove);

            }
            _mobileMoveDelay = false;
        }
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

        Swipe();

        if (GameManager.Instance.getCurrentZone() != 1) // cant dig in own territory
        {
            if (!_InGround && (Input.GetKeyDown(KeyCode.DownArrow) || _swipeDir==eSwipeDirection.Down))
            {
                if (_CurrentSoilTile)
                {
                    StartCoroutine(DigDelay(Vector2.down, _CurrentSoilTile));
                }

            }
            else if (_InGround && _horizontalMove == 0)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) || _swipeDir == eSwipeDirection.Right)
                {
                    //Need to check tile to the right
                    if (CheckTile("right"))
                    {

                    }
                    return true;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || _swipeDir == eSwipeDirection.Left)
                {
                    //Need to check tile to the left
                    if (CheckTile("left"))
                    {

                    }
                    return true;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) || _swipeDir == eSwipeDirection.Up)
                {
                    if (CheckTile("up"))
                    {

                    }
                    return true;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || _swipeDir == eSwipeDirection.Down)
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
        }
        return false;
    }
    private void Swipe()
    {
        Vector2 currentSwipe;

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe upwards
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
             {
                    Debug.Log("up swipe");
                    _swipeDir = eSwipeDirection.Up;
                    return;
                }
                //swipe down
                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
             {
                    Debug.Log("down swipe");
                    _swipeDir = eSwipeDirection.Down;
                    return;
                }
                //swipe left
                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
             {
                    Debug.Log("left swipe");
                    _swipeDir = eSwipeDirection.Left;
                    return;
                }
                //swipe right
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
             {
                    Debug.Log("right swipe");
                    _swipeDir = eSwipeDirection.Right;
                    return;
                }
            }
        }
        //print("swipe=default");
        _swipeDir = eSwipeDirection.Default;
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
                   // Debug.Log("DT:" + dt.gameObject + "  @" + dt.transform.position.y);
                   // Debug.Log("OUR:" + "king" + "  @" + this.transform.position.y);

                    //Calculate distance because of weird anchor points? - keep player in middle of tile
                    float newY = (this.transform.position.y - dt.transform.position.y)/2; //can Div by 2 to alter if sprite is wrong pos
                    if (dir == Vector2.up)
                         newY = newY-2; 

                    this.transform.position = new Vector3(dt.transform.position.x, this.transform.position.y - newY, 0);

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
                            else // up and down
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
       // print("Starting Attack Routine");
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
            //print("Attack Target is: " + _AttackTarget.name + "Going to DMG");
            if (_AttackTarget.GetComponent<Rodent>())
            {
                _AttackTarget.GetComponent<Rodent>().Damage(_damage);
               // print("Dmg Rodent:");
            }
            else if (_AttackTarget.GetComponent<BuildableObject>())
            {
                _AttackTarget.GetComponent<BuildableObject>().Damage(_damage);
            }
            SoundManager.Instance.PlayCombat();
        }
        else
            Debug.LogError("some how attack target is null");

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
        //print("die called");
        if (isDead)
            return;

        isDead = true;
        _animator.SetTrigger("Dead");
        GameManager.Instance.youLose();
    }

    public void Heal()
    {
    }
    public void Move(float move, bool crouch, bool jump)
    {
        if(move>=45)
             Debug.Log("we are moving This much:" + move);

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

    public void setAttacking(bool cond) => _isAttacking = cond;
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


         if (collision.transform.GetComponent<DiggableTile>())
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
                //print("Collided with a  base hitbox");
                if (collision.transform.parent.GetComponent<BuildableObject>())
                {
                   // print("collded with a buildng hitbox");
                    //Add to our list of interactable things in range
                    _InRange.Add(collision.transform.parent.gameObject);
                    //print("Added: " + collision.transform.parent.gameObject);
                    //Can we search it?
                    if (collision.transform.parent.GetComponent<Searchable>())
                    {
                        Searchable s = collision.transform.parent.GetComponent<Searchable>();
                        {
                            s.setActive(true);
                            // Debug.LogWarning("Players in range");
                        }
                    }
                }

                else if (collision.transform.parent.GetComponent<Rodent>())
                {
                    //Add to our list of interactable things in range
                    Rodent r = collision.transform.parent.GetComponent<Rodent>();
                    if(r.getTeam()==2 && _InRange.Contains(collision.transform.parent.gameObject) ==false)
                         _InRange.Add(collision.transform.parent.gameObject);
                }
                
            }
        }

        ///Old Game Jam code, could be reused for pickups 
        else if (collision.transform.GetComponent<CoinResource>())
        {
            // if (collision.transform.GetComponent<CoinResource>().isActive())
            {
                ResourceManagerScript.Instance.incrementResource(ResourceManagerScript.ResourceType.Trash, 1);
                Destroy(collision.gameObject);
            }
        }

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<DiggableTile>())
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
                    //Add to our list of interactable things in range
                    _InRange.Remove(collision.transform.parent.gameObject);
                    //Can we search it?
                    if (collision.transform.parent.GetComponent<Searchable>())
                    {
                        Searchable s = collision.transform.parent.GetComponent<Searchable>();
                        {
                            s.setActive(false);
                            // Debug.LogWarning("Players in range");
                        }
                    }
                }

                else if (collision.transform.parent.GetComponent<Rodent>())
                {
                    if(_InRange.Contains(collision.transform.parent.gameObject))
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





