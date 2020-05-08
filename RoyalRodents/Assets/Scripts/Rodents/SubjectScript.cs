using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Behavior script for rats in the player base.
 * - CURRENT FUNCTIONALITY -
 *      - Rats will run to whatever object is assigned and idle there.
 *      - Now has a function that changes targets
 *      - Rats can be assigned up to two targets, one remembered and one being acted on
 *      
 * - PLANNED FUTURE FUNCTIONALITY -
 *      - If not assigned a building, rats will run around the base.
 *      - Different behaviors for different types of rodents
 */
public class SubjectScript : MonoBehaviour
{
    #region Attributes
    public Animator anims;
    public float moveSpeed = 0.5f;
    public GameObject currentTarget;
    public GameObject savedTarget;
    public GameObject savedTarget2;
    public Vector3 IdlePos;
    private bool facingRight;
    public bool royalGuard = true;
    public bool farmer = false;
    public bool gatherer = false;
    public bool builder = false;
    public bool defender = false;
    public bool tmpFighter = false;
    public bool rangedAttacker = false;
    private bool coroutineStarted = false;
    private bool ShouldIdle = false;
    private bool MovingInIdle = false;
    private float WaitDuration;
    private bool canAttack = true;
    private float attackDamage = 1;
    private int team = 0;
    private bool _underAttack = false;
    public float _underAttackTime;
    private bool _underAttackCoroutineOn = false;
    private bool _isDead;
    private string _oldJob;
    private bool _approachingTownCenterasWorker;
    private GameObject townCenterLoc;
    public bool isRanged;
    private BuildableObject job;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    private const string MOVING_ANIMATION_BOOL = "isMoving";
    private const string ARMED_ANIMATION_BOOL = "isArmed";
    private const string BUILDING_ANIMATION_BOOL = "isBuilding";
    private const string FARMING_ANIMATION_BOOL = "isFarming";
    private const string GATHERHING_ANIMATION_BOOL = "isGathering";
    private const string ATK_ANIMATION_TRIGGER = "doAttack";


    private List<GameObject> _inRange = new List<GameObject>();

    private bool _printStatements = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        getAnimator();
        facingRight = false;
        // a backup condition to get the right speed
        Rodent r = this.GetComponent<Rodent>();
        if (r)
            moveSpeed = r.getSpeed();

        WaitDuration = 10f;

        testSwap();
        townCenterLoc = GameManager.Instance.getTownCenter().transform.gameObject;
        isRanged = this.GetComponent<Rodent>().isRanged();
        projectileSpawnPoint = gameObject.transform.Find("ProjectileSpawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDead)
        {
            // Check if a target exists
            if (currentTarget)
            {
                // branches with each class's behavior built in.
                if (royalGuard || tmpFighter)
                {
                    royalGuardBehavior();
                }
                else if (farmer)
                {
                    farmerBehavior();
                }
                else if (builder)
                {
                    builderBehavior();
                }
                else if (gatherer)
                {
                    gatherBehavior();
                }
                else if (defender) {
                    // Do the defender thing
                    defenderBehavior();
                }
                else
                {
                    //Shouldnt happen?
                    // Debug.LogWarning("This shouldn't happen, if it does, I want to know about it");
                    idleInRadius(IdlePos, 5);
                }
            }
            else
            {
                //free movement for rats with no target
               // print("IDLE1");
                idleInRadius(IdlePos, 5);
            }
        }
    }
    public void getAnimator()
    {
        anims = this.GetComponent<Animator>();
        if (anims == null)
            Debug.LogError("No Animator found on : " + this.gameObject.ToString());
    }
    public void setAnim(string s, bool b, bool isTrigger)
    {
        if (anims == null)
            getAnimator();

        if (anims)
        {
            if (!isTrigger)
            {
               //print("enter " + s + "  cond: " +b  +"  for" + this.gameObject.name);
                anims.SetBool(s, b);
                if (s.Equals("isMoving") == false && _printStatements)
                    Debug.Log("We set bool " + s + "  to  " + b + "  for " + this.gameObject.name);
            }
            else
            {
                anims.SetTrigger(s);
                if (_printStatements)
                    Debug.Log("We set trigger" + s);
            }
        }
        
    }
    public void setIsRanged(bool cond)
    {
        isRanged = cond;
    }
    /** Sets rodent roles, ensuring there is only 1 active at a time */
    public void setRoyalGuard()
    {
        royalGuard = true;
        farmer = false;
        builder = false;
        gatherer = false;
        setAnim(ARMED_ANIMATION_BOOL, true, false);

        // Always should have the king in Saved Target if it is not the current target
        currentTarget = GameObject.FindGameObjectWithTag("Player");
        savedTarget = currentTarget;
    }
    public void setAttacker()
    {
        royalGuard = true;
        farmer = false;
        builder = false;
        gatherer = false;
        setAnim(ARMED_ANIMATION_BOOL, true, false);

        //Should be players town center
        savedTarget = currentTarget;

    }

    public void setFarmer()
    {
        royalGuard = false;
        farmer = true;
        builder = false;
        gatherer = false;
        setAnim(ARMED_ANIMATION_BOOL, false, false);

        job = currentTarget.GetComponent<BuildableObject>();
        //Get TownCenter location
        GameObject centerLocation = townCenterLoc;
        savedTarget = centerLocation;
    }
    public void setGatherer()
    {
        royalGuard = false;
        farmer = false;
        builder = false;
        gatherer = true;
        setAnim(ARMED_ANIMATION_BOOL, false, false);

        job = currentTarget.GetComponent<BuildableObject>();
        //Get TownCenter location
        GameObject centerLocation = townCenterLoc;
        savedTarget = centerLocation;
    }

    public void setBuilder()
    {
        royalGuard = false;
        farmer = false;
        builder = true;
        gatherer = false;
        setAnim(ARMED_ANIMATION_BOOL, false, false);

        job = currentTarget.GetComponent<BuildableObject>();
        //Get TownCenter location
        GameObject centerLocation = townCenterLoc;
        savedTarget = centerLocation;

        swapTarget();
    }
    public void setDefender()
    {
        //Debug.Log("Rodent Assigned to Defender");
        defender = true;
        royalGuard = false;
        builder = false;
        farmer = false;
        setAnim(ARMED_ANIMATION_BOOL, true, false);
        

        //Get TownCenter location
        GameObject centerLocation = townCenterLoc;
        savedTarget = centerLocation;
    }
    public void setIdle()
    {
       // print("set idle called");
        royalGuard = false;
        farmer = false;
        builder = false;
        gatherer = false;

        setAnim(FARMING_ANIMATION_BOOL, false, false);
        setAnim(BUILDING_ANIMATION_BOOL, false, false);
        setAnim(GATHERHING_ANIMATION_BOOL, false, false);

        IdlePos = this.transform.position;
    }
    public void setDead()
    {
        _isDead = true;
    }
    public void setSpeed(float nSpeed)
    {
        this.moveSpeed = nSpeed;
    }

    float getSpeed()
    {
        return moveSpeed;
    }
    /** find objects Position and pass it along */
    private void Move(GameObject target)
    {
        if (_printStatements)
            Debug.Log("Told to Move to Target" + target);
        if (!ShouldIdle)
            Move(target.transform.position);
    }
    /** Will move to location if far enough away, otherwise will try to idle */
    void Move(Vector3 loc)
    {
        if (_printStatements)
            Debug.Log("Told to Move to Loc  " + loc);

        Vector3 pos = new Vector3(loc.x, 0, 0);

        //If we are far enough away
        if (Mathf.Abs(pos.x - transform.position.x) > 1.5f)

        {

            setAnim("isMoving", true, false);
        

            if (transform.position.x > pos.x)
            {
                // Flip if facing right
                if (facingRight)
                {
                    flipDirection();
                }
                // Account for double negatives
                if (pos.x >= 0)
                {
                    transform.position -= pos.normalized * Time.deltaTime * moveSpeed;
                }
                else
                {
                    transform.position += pos.normalized * Time.deltaTime * moveSpeed;
                }
            }
            else
            {
                // Flip if facing left
                if (!facingRight)
                {
                    flipDirection();
                }
                // Account for double negatives
                if (pos.x >= 0)
                {
                    transform.position += pos.normalized * Time.deltaTime * moveSpeed;
                }
                else
                {
                    transform.position -= pos.normalized * Time.deltaTime * moveSpeed;
                }
            }
        }
        else //We have Reached our destination 
        {

           // On finishing movement, return to idle
           setAnim("isMoving", false, false);


            //Check if we need to attack something as royal guard
            if (royalGuard || defender)
            {
                //print(this.gameObject.name + "  Wants to attack");
                bool guardShouldIdle = true;
                // If target is enemy, attack. Add coroutine for attacking
                if (_inRange.Count > 0 && currentTarget)
                {
                    if (team == 1 && currentTarget.tag != "Player") // And can attack
                    {
                        if (isRanged)
                        {
                            StartCoroutine(Shoot(currentTarget.transform.position));
                        }
                        else
                        {
                            StartCoroutine(Attack());
                        }


                        guardShouldIdle = false;
                    }
                    else if (team == 2)
                    {
                        if (isRanged)
                        {
                            StartCoroutine(Shoot(currentTarget.transform.position));
                        }
                        else
                        {
                            StartCoroutine(Attack());
                        }

                        
                        guardShouldIdle = false;
                    }
                }
                //no target to attack so we can idle
                if (!coroutineStarted && guardShouldIdle)
                {
                    StartCoroutine(idleDelay());
                }

            }
            // TODO: Else if for defenders attacking any targets in range (Different distance check)

            //Idle for workers
            else if (!coroutineStarted)
            {
                StartCoroutine(idleDelay());
                if (builder)
                {
                    //Possible issues of building anim at TC, doesnt seem to be big deal
                    // however this script doesnt seem to make him move toward TC as he builds TC
                    // so we might have an issue with resource collection at TC once implemented
                    //if current is not town center prepare for search anim
                    if (currentTarget != townCenterLoc)
                    {
                        // next time We work, to use gather anim
                        _approachingTownCenterasWorker = true;
                    }
                    //else swap back to proper anim
                    else
                    {
                        //go back to normal anim
                        _approachingTownCenterasWorker = false;
                    }

                    swapTarget();
                    IncrementBuilding();
                }

                else if (farmer || gatherer)
                {
                    //if current is not town center prepare for search anim
                    if (currentTarget != townCenterLoc)
                    {
                        // next time We work, to use gather anim
                        _approachingTownCenterasWorker = true;
                    }
                    //else swap back to proper anim
                    else
                    {
                        //go back to normal anim
                        _approachingTownCenterasWorker = false;
                    }

                    swapTarget();
                    // This is probably where we should send the signal for the progress bars
                    IncrementGathering();
                }
            }

            //Responsible for ending Coroutine
            if (MovingInIdle)
                MovingInIdle = false;

                                                                        

        }

    }
    private void IncrementBuilding()
    {
        if (currentTarget == townCenterLoc)
        {
            //Debug.Log("INcrement On" + savedTarget.gameObject);
            Rodent r = this.GetComponent<Rodent>(); // no null check cuz would never happen
            savedTarget.GetComponent<BuildableObject>().IncrementConstruction(r.getBuildRate());
        }

    }
    private void IncrementGathering()
    {
        if (currentTarget == townCenterLoc)
        {
            Debug.Log("INcrement On" + savedTarget.gameObject);
            Rodent r = this.GetComponent<Rodent>(); // no null check cuz would never happen
            savedTarget.GetComponent<BuildableObject>().IncrementGathering(r.getGatherRate());
        }
        else
            Debug.Log("current target is" + currentTarget.gameObject);
    }

    private bool CheckIfRat()
    {
        Rodent r = GetComponent<Rodent>();
        if(r)
        {
            if(r.GetRodentType() == Rodent.eRodentType.Rat || r.GetRodentType() == Rodent.eRodentType.Mouse)
            {
                return true;
            }
        }
        return false;
    }
    /**  Finds the right Position to idle in */
    public void idleInRadius(int radius)
    {

        if (currentTarget == null)
        {
            Debug.LogError("Cant Idle, Current Target is Null");
            return;
        }
        idleInRadius(currentTarget.transform.position, radius);
    }
    /** This will pick a location nearby to move to and start a coroutine if one isn't already started 
    * we dont use Current Target here in case we dont have one, and want to idle on our own location internally */
    private void idleInRadius(Vector3 loc, int radius)
    {


        if (!coroutineStarted)
        {
           // Debug.Log("Told to Idle in Radius , and coroutine started=" +coroutineStarted);
            int _chanceToMove = Random.Range(-radius, radius);
            if (_chanceToMove > 2)
            {
                //Move Right
                Vector3 pos = new Vector3(Random.Range((loc.x + 1f), (loc.x + 6.5f)), 0, 0);

                if (_printStatements)
                    Debug.Log("Move Positively to::" + pos);
                StartCoroutine(IdleMovetoLoc(pos));
                WaitDuration = SetWaitDuration("move");
            }
            else if (_chanceToMove < -2)
            {
                //Move Left
                Vector3 pos = new Vector3(Random.Range((loc.x - 6.5f), (loc.x - 1f)), 0, 0);
                if (_printStatements)
                    Debug.Log("Move Negatively to::" + pos);
                StartCoroutine(IdleMovetoLoc(pos));
                WaitDuration = SetWaitDuration("move");
            }
            else
            {
                //Stand Still
                if (_printStatements)
                    Debug.Log("StandStill::" + transform.position);
                StartCoroutine(IdleMovetoLoc(transform.position));
                WaitDuration = SetWaitDuration("move");

            }
        }
    }
    /** Responsible for the overall time spent idling */
    IEnumerator idleDelay()
    {
        //if its already started, we leave it alone
        if (ShouldIdle)
        {
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            ShouldIdle = true;
            //Debug.LogWarning("Start Idle");

            float currentTime = Time.time;
            float ExitTime = currentTime + WaitDuration;



            if (builder)
            {
                   // Fix for builder
                   if(CheckIfRat())
                        flipDirection();
                    if (!_approachingTownCenterasWorker)
                    {
                        setAnim(BUILDING_ANIMATION_BOOL, true, false);
                        setAnim(GATHERHING_ANIMATION_BOOL, false, false);
                    }
                    else
                    {
                        setAnim(BUILDING_ANIMATION_BOOL, false, false);
                        setAnim(GATHERHING_ANIMATION_BOOL, true, false);
                    }

            }
            else if (farmer)
            {

                    if (!_approachingTownCenterasWorker)
                    {
                         print("turned back on farming");
                        setAnim(FARMING_ANIMATION_BOOL, true, false);
                        setAnim(GATHERHING_ANIMATION_BOOL, false, false);
                    }
                    else
                    {
                        setAnim(FARMING_ANIMATION_BOOL, false, false);
                        setAnim(GATHERHING_ANIMATION_BOOL, true, false);
                    }
 
            }
            else if (gatherer)
            {
  
                    setAnim(GATHERHING_ANIMATION_BOOL, true, false);
                
            }


            while (currentTime < ExitTime)
            {
                currentTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(WaitDuration);
            //Debug.LogWarning("Stop Idle");
            ShouldIdle = false;

            //set false

            if (builder)
            {
                if(CheckIfRat())
                    flipDirection();
                setAnim(BUILDING_ANIMATION_BOOL, false, false);
                setAnim(GATHERHING_ANIMATION_BOOL, false, false);
            }  
            else if (farmer)
            {

                setAnim(FARMING_ANIMATION_BOOL, false, false);
                setAnim(GATHERHING_ANIMATION_BOOL, false, false);
                
            }
            else if (gatherer)
            {

                setAnim(GATHERHING_ANIMATION_BOOL, false, false);
                
            }

            if (_printStatements)
                Debug.Log("Exit Idle Timer");

        }
    }
    /** While this is active, the subject will repeatedly move to a location, Upon Reaching that location will wait again before exiting */
    IEnumerator IdleMovetoLoc(Vector3 pos)
    {
        if (!coroutineStarted)
        {


            // Debug.LogWarning("Enter Actual Move Coroutine");
            MovingInIdle = true;
            coroutineStarted = true;
            while (MovingInIdle)
            {
                //print("1");
                Move(pos);
                yield return new WaitForSeconds(Time.deltaTime);

            }
           // print("wait for " + WaitDuration + " seconds on " + this.gameObject.name);
            yield return new WaitForSecondsRealtime(WaitDuration);
           // print("wait is over " + WaitDuration + " " + this.gameObject.name);
            coroutineStarted = false;
        }
        else
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }


    void flipDirection()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);


        //Fix Children Health bar and RecruitMenu
        int index = this.transform.childCount;
        for (int i = 0; i < index; ++i)
        {
            Transform t = this.transform.GetChild(i);
            ScaleKeeper sk = t.GetComponent<ScaleKeeper>();
            if (sk)
            {
                Vector3 _properScale = sk.getScale();

                if (facingRight)
                    _properScale = new Vector3(-_properScale.x, _properScale.y, _properScale.z);

                t.localScale = _properScale;
            }
        }


    }

    // Assign rodent's current target. 
    public void changeTarget(GameObject nTarget)
    {
        //Debug.Log("Changing Target to " + nTarget);
        this.currentTarget = nTarget;
    }

    // Assign the rodent's saved target
    public void setSavedTarget(GameObject nTarget)
    {
        savedTarget = nTarget;
    }

    private void royalGuardBehavior()
    {
       // print("RoyalguardBehavior,  shoulIdle= " + ShouldIdle);
        // Follow the king at all times.
        // Future: Attack enemies within a radius of the king
        if (!ShouldIdle)
        {
            Vector3 moveTo = currentTarget.transform.position;


            // Prevent the rodent from moving if it's ranged and targetting something to attack
            // Target exists & Ranged Unit & within a range (aggro range)
            if (currentTarget && isRanged && Mathf.Abs(moveTo.x - transform.position.x) < 10f)
            {
                if(team == 1 && currentTarget.tag == "Player")
                {
                    // Do nothing if Allied and following the King
                   // print("2");
                    Move(moveTo);
                }
                else
                {
                    //print("Start shoot to move??" + moveTo);
                    StartCoroutine(Shoot(moveTo));
                }

            }
            else
            {
               // print("3");
                Move(moveTo);
            }
           

        }
        else
            idleInRadius(2);

    }
    IEnumerator Shoot(Vector3 shootTargetCoordinate)
    {
        if (canAttack)
        {
            Debug.LogWarning("SHOOT");
            //Flip if needed
            float dist = shootTargetCoordinate.x - transform.position.x;
            if(dist > 0 && !facingRight)
            {
               flipDirection();
            }
            else if(dist < 0 && facingRight){
               flipDirection();
            }
            canAttack = false;
            if (anims)
            {
                anims.SetTrigger(ATK_ANIMATION_TRIGGER);
            }
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            projectile.GetComponent<Projectile>().setDamage(attackDamage);
            projectile.GetComponent<Projectile>().setEnemyTeam(getEnemyTeam());
            projectile.GetComponent<Projectile>().setTarget(shootTargetCoordinate);


            yield return new WaitForSeconds(1.16f);
            canAttack = true;
        }

    }

    public void AgroRadiusTrigger(Collider2D collision)
    {

        // Add a target to the list based on collisions
        //Debug.Log("Collided with " + collision.gameObject.ToString());

        //check that it HAS a parent

        if (collision.transform.parent)
        {
            Rodent unknownRodent = collision.transform.parent.gameObject.GetComponent<Rodent>();
            PlayerStats king = collision.transform.parent.gameObject.GetComponent<PlayerStats>();
            BuildableObject unknownBuilding = collision.transform.parent.gameObject.GetComponent<BuildableObject>();

            if (unknownRodent)
            {
                // Debug.LogWarning("Found Rodent" + unknownRodent.getName() + " on team:  " + unknownRodent.getTeam());
                // Rodent case
                if (unknownRodent.getTeam() == getEnemyTeam())
                {
                    _inRange.Add(unknownRodent.gameObject);
                    //Debug.Log("Rodent added to targets in range " + unknownRodent.getName() + unknownRodent.getTeam());
                    // print("called from AgroRadiusTrigger");
                    if (_inRange.Count == 1 && royalGuard)
                    {
                        print("Newest target added to queue: " + unknownRodent.gameObject);
                        currentTarget = unknownRodent.gameObject;
                    }
                }


            }
            else if (unknownBuilding)
            {
                if (unknownBuilding.getTeam() == getEnemyTeam())
                {
                    // Ensure this isn't a natural resource
                    if (unknownBuilding.getType() != BuildableObject.BuildingType.WoodPile && unknownBuilding.getType() != BuildableObject.BuildingType.StonePile
                        && unknownBuilding.getType() != BuildableObject.BuildingType.GarbageCan)
                    {
                        _inRange.Add(unknownBuilding.gameObject);
                        if (_inRange.Count == 1 && royalGuard)
                        {
                            print("Newest target added to queue: " + currentTarget.ToString());
                            currentTarget = unknownBuilding.gameObject;
                        }
                    }

                }
            }

            // Special case: Finding King as an attack target
            else if (team == 2 && king)
            {
                _inRange.Add(king.gameObject);
                if (_inRange.Count == 1)
                {
                    print("Newest target added to queue: " + king.gameObject);
                    currentTarget = king.gameObject;
                    //Should probably update state to stop moving, and start attacking
                    MovingInIdle = false;

                }
            }
        }
    }
    private int getEnemyTeam()
    {
        int enemyTeam;

        // If neutral, not applicable
        if (team == 0)
            enemyTeam = -1;
        // If Allied, Enemy
        else if (team == 1)
            enemyTeam = 2;
        else
            enemyTeam = 1;

        //Debug.Log("enemyTeam is: " + enemyTeam);
        return enemyTeam;
    }


    // Handles the rat attacking an enemy
    IEnumerator Attack()
    {
        // Play animation
        //Debug.Log("Attack!Rat@" + Time.time);

        if (canAttack)
        {
            canAttack = false;
            if (anims)
            {
                // Put attack animation here
                anims.SetTrigger(ATK_ANIMATION_TRIGGER);
            }
            // For rodents
            Rodent _EnemyRodent = currentTarget.GetComponent<Rodent>();
            PlayerStats king = currentTarget.GetComponent<PlayerStats>();

            if (_EnemyRodent)
            {
                if (!_EnemyRodent.isDead())
                {// Reduce enemy health
                    _EnemyRodent.Damage(attackDamage);
                    SoundManager.Instance.PlayCombat();
                }
                else
                {
                    _inRange.Remove(currentTarget);
                    // print("Called from Dead Rodent found");
                    FindNextTargetInRange();
                }
            }
            else if (king)
            {
                if (!king.isDead())
                {
                    king.Damage(attackDamage);
                    SoundManager.Instance.PlayCombat();
                }
                else
                {
                    _inRange.Remove(currentTarget);
                    FindNextTargetInRange();
                }
            }
            else
            {
                BuildableObject _enemyBuilding = currentTarget.GetComponent<BuildableObject>();
                if(_enemyBuilding.getHP() > 0 && _enemyBuilding.getType() != BuildableObject.BuildingType.TownCenter)
                {
                    _enemyBuilding.Damage(attackDamage);
                    SoundManager.Instance.PlayCombat();
                }
                else
                {
                    _inRange.Remove(currentTarget);
                    // print("Called from Dead Building found");
                    FindNextTargetInRange();
                }
            }


            yield return new WaitForSeconds(1.16f); //Length of attack animation
            canAttack = true;

        }


    }


    // Removes a target from the list if it exits the rodent's range
    public void removefromAgroRange(Collider2D c)
    {
        // Remove objects from the list
        GameObject go = c.gameObject;
        // TODO: UNset removed target from currentTarget
        if(c == currentTarget)
        {
            currentTarget = savedTarget;
        }
        if (_inRange.Contains(go))
        {

            if (go == currentTarget || currentTarget == null)
            {
                print("Called from Remove from AgroRange");
                FindNextTargetInRange();
            }

            _inRange.Remove(go);
            Debug.Log("Rodent removed from targets");
            Debug.Log("Rodent removed from targets");
        }
        //else debug error 

        

    }

    // Parses the list and finds the next suitable target
    // Sets back to king if none
    private void FindNextTargetInRange()
    {
        //parse the list for closest target and make next target
        if (_inRange.Count > 0)
        {
            // Priming read
            GameObject currentClosest = _inRange[0];
            if (currentClosest != null) // do we need an is dead check here?
            {

                float closestDist = Mathf.Abs(transform.position.x - _inRange[0].transform.position.x);

                foreach (GameObject go in _inRange)
                {
                    if (go != null) // do we need an is dead check here?
                    {
                        float tempDist = Mathf.Abs(transform.position.x - go.transform.position.x);
                        if (tempDist < closestDist)
                        {
                            closestDist = tempDist;
                            currentClosest = go;
                        }
                    }

                }

            }
            currentTarget = currentClosest;
        }
        // If no valid targets, set to King
        else
        {
            currentTarget = savedTarget;
        }
    }


    // Resets the rodent's target back to the original current, now saved in the secondary target
    public void swapTarget()
    {
        GameObject tempTarget = currentTarget;
        currentTarget = savedTarget;
        savedTarget = tempTarget;

    }

    public void defenderBehavior()
    {
        Vector3 targetPos;
        // Check which side of the map the rodent is on
        if (currentTarget.transform.position.x - townCenterLoc.transform.position.x < 0)
        {
            targetPos = new Vector3(currentTarget.transform.position.x + 5, this.transform.position.y, 0);
            
        }
        else
        {
           targetPos = new Vector3(currentTarget.transform.position.x - 5, this.transform.position.y, 0);
            
        }

        if (!ShouldIdle)
        {
            // Skip this if ranger with an enemy as a target
            if(isRanged && Mathf.Abs(transform.position.x - currentTarget.transform.position.x) < 10f)
            {
                if(team == 1 && currentTarget.tag == "Player" || currentTarget.GetComponent<BuildableObject>().getTeam() == team)
                {
                    // Do nothing if Allied and targetting king
                }
                else
                {
                    StartCoroutine(Shoot(currentTarget.transform.position));

                }

            }
            else
            {
                print("4");
                Move(targetPos);
            }
            
        }

        // Maybe don't include idle so they seem more attentive at the wall?

        // Wait 15 units behind the wall they're assigned to
        

    }

    private void farmerBehavior()
    {

        // Walk to their assigned building
        // Idle in the area of it
        // Future: Be able to work occupy the building and deliver resources to the town center
        if (!ShouldIdle)
        {

            Move(currentTarget);
            if (_printStatements)
                Debug.LogError("WorkerMove");
        }
        else
        {
            idleInRadius(3);
        }
    }
    public void gatherBehavior()
    {
        if (!ShouldIdle)
        {

            Move(currentTarget);
            if (_printStatements)
                Debug.LogError("WorkerMove");
        }
        else
        {
            idleInRadius(3);
        }
    }

    private void builderBehavior()
    {
        // Walk to their assigned building
        // Future: Be able to carry resources from the town center to the building being constructed


        // Move(currentTarget);
        if (!ShouldIdle)
        {

            Move(currentTarget);
            if (_printStatements)
                Debug.LogError("BuilderMove");


        }

        // not sure this one will idle
        // instead it might reach here and play an animation
        // or Idle in radius 0 and play an anim in there?
    }


    /**Figures out how long to idle for based on Occupation and State */
    private float SetWaitDuration(string state)
    {
      //  print("set wait");
        if (state.Equals("move"))
        {
            if (farmer || gatherer)
                if(currentTarget != townCenterLoc)
                {
                   //Debug.Log("Setting delay for TownCenter work");
                    float delay = Random.Range(1, 2);
                    //Debug.Log("Delay of: " + delay);
                    return delay;
                }
                else
                {
                   // Debug.Log("Setting delay for place of Employment work");
                    float delay = Random.Range(4, 10);
                    //Debug.Log("Delay of: " + delay);
                    return delay;
                }
                
            else if (royalGuard || defender)
                return Random.Range(1, 2.5f);
            else if (builder)
                if (currentTarget != townCenterLoc)
                {
                    //Debug.Log("Setting delay for TownCenter work");
                    float delay = Random.Range(1, 2.5f);
                    //Debug.Log("Delay of: " + delay);
                    return delay;
                }
                else
                {
                    //Debug.Log("Setting delay for place of Employment work");
                    float delay = Random.Range(5, 10);
                    //Debug.Log("Delay of: " + delay);
                    return delay;
                }
        }
        else if (state.Equals("idle"))
        {
            if (farmer || gatherer)
                return Random.Range(0.5f, 1f);
            else if (royalGuard || defender)
                return Random.Range(1, 2f);  // will follow player a lot better
            else if (builder)
                return Random.Range(0.1f, 1f);
        }

        return Random.Range(2, 5);
    }

    //As a rodent type changes, we figure out from here
    public void setAttackDamage(float damage)
    {
        attackDamage = damage;
    }
    //Doing too many get component calls when attacking, this will help
    public void setTeam(int id)
    {
        team = id;
    }
    public void UnderAttack(bool cond)
    {
        _underAttack = cond;
        _underAttackTime = Time.time;
        //Debug.Log("NEW TIME= " + _underAttackTime);
        StartCoroutine(UnderAttackDelay(_underAttackTime - 5.0f));

    }
    IEnumerator UnderAttackDelay(float _WaitDuration)
    {
        if (!_underAttackCoroutineOn)
        {
            //Debug.LogWarning("Were under attack @!" + _underAttackTime + " , waiting until: " + _WaitDuration);
            _underAttackCoroutineOn = true;
            //Tell villager to pull out Weapons
            if (royalGuard == false)
            {
                setAnim(ARMED_ANIMATION_BOOL, true, false);

                SaveLastJob();
            }
            //If no damage taken in X time, go back to normal
            while (_underAttackTime > _WaitDuration)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                _underAttackTime -= Time.deltaTime;
            }

            _underAttack = false;
            _underAttackCoroutineOn = false;
            restoreLastJob();
           // Debug.LogError("Were NOT under attack @" + Time.time + "  " + this.gameObject);
        }
    }

    public void SaveLastJob()
    {
        if (farmer)
            _oldJob = "farmer";
        else if (builder)
            _oldJob = "builder";
        else if (gatherer)
            _oldJob = "gatherer";
        else if (royalGuard)
            _oldJob = "royalGuard";
        else
            _oldJob = "idle";

        savedTarget2 = currentTarget;
        royalGuard = true;
        farmer = false;
        builder = false;
        gatherer = false;
        FindNextTargetInRange();
    }
    private void restoreLastJob()
    {
        switch (_oldJob)
        {
            case "farmer":
                {
                    royalGuard = false;
                    farmer = true;
                    currentTarget = savedTarget2;
                    //Tell villagers to put weapons away
                    setAnim(ARMED_ANIMATION_BOOL, false, false);
                    break;
                }
            case "gatherer":
                {
                    royalGuard = false;
                    gatherer = true;
                    currentTarget = savedTarget2;
                    //Tell villagers to put weapons away
                    setAnim(ARMED_ANIMATION_BOOL, false, false);
                    break;
                }
            case "builder":
                {
                    royalGuard = false;
                    builder = true;
                    currentTarget = savedTarget2;
                    //Tell villagers to put weapons away
                    setAnim(ARMED_ANIMATION_BOOL, false, false);
                    break;
                }
            case "royalGuard":
                {
                    //Do nothing? rest of behavior should be handled above
                    break;
                }
            case "idle":
                {
                    royalGuard = false;
                    currentTarget = null;
                    //Tell villagers to put weapons away
                    setAnim(ATK_ANIMATION_TRIGGER, false, false);
                    break;
                }


        }


    }

    //Unused but found this cool way to turn off ALL parameters EXCEPT one in controller
    private void DisableOtherAnimations(Animator animator, string animation)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name != animation)
            {
                setAnim(parameter.name, false, false);
            }
        }
    }

    private void testSwap()
    {
        /*
        //UnityEngine.AnimationClip clip 
        foreach (var a in anims.GetCurrentAnimatorClipInfo(0))
        {
          //  print(a.clip.ToString());
        }

        AnimatorOverrideController aoc = new AnimatorOverrideController(anims.runtimeAnimatorController);

        foreach(var a in aoc.animationClips)
        {
            print(a.name);
        }
        */


    }
}
