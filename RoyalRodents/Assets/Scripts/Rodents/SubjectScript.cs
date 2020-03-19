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
    public bool worker = false;
    public bool builder = false;
    public bool tmpFighter = false;
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

    private const string MOVING_ANIMATION_BOOL = "isMoving";
    private const string ARMED_ANIMATION_BOOL = "isArmed";
    private const string ATK_ANIMATION_TRIGGER = "doAttack";

    private List<GameObject> _inRange = new List<GameObject>();

    private bool _printStatements = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        anims = this.GetComponent<Animator>();
        facingRight = false;
        // a backup condition to get the right speed
        Rodent r = this.GetComponent<Rodent>();
        if (r)
            moveSpeed = r.getSpeed();

        WaitDuration = 10f;
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
                else if (worker)
                {
                    workerBehavior();
                }
                else if (builder)
                {
                    builderBehavior();
                }
                else
                {
                    //Shouldnt happen?
                    //  Debug.LogWarning("This shouldn't happen, if it does, I want to know about it");
                    idleInRadius(IdlePos, 5);
                }
            }
            else
            {
                //free movement for rats with no target
                idleInRadius(IdlePos, 5);
            }
        }
    }

    /** Sets rodent roles, ensuring there is only 1 active at a time */
    public void setRoyalGuard()
    {
        royalGuard = true;
        worker = false;
        builder = false;

        if (anims)
            anims.SetBool(ATK_ANIMATION_TRIGGER, true);

        // Always should have the king in Saved Target if it is not the current target
        currentTarget = GameObject.FindGameObjectWithTag("Player");
        savedTarget = currentTarget;
    }

    public void setWorker()
    {
        royalGuard = false;
        worker = true;
        builder = false;
        if (anims)
            anims.SetBool(ATK_ANIMATION_TRIGGER, false);


        //Get TownCenter location
        GameObject centerLocation = GameManager.Instance.getTownCenter().transform.gameObject;
        savedTarget = centerLocation;
    }

    public void setBuilder()
    {
        royalGuard = false;
        worker = false;
        builder = true;
        if (anims)
            anims.SetBool(ATK_ANIMATION_TRIGGER, false);


        //Get TownCenter location
        GameObject centerLocation = GameManager.Instance.getTownCenter().transform.gameObject;
        savedTarget = centerLocation;
    }
    public void setDefender()
    {
        //To-Do:
        //Debug.Log("Rodent Assigned to Defender");
    }
    public void setIdle()
    {
        royalGuard = false;
        worker = false;
        builder = false;
        if (anims)
            anims.SetBool(ATK_ANIMATION_TRIGGER, false);
        //changeTarget(this.gameObject);  // shouldnt need to do this
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

            if (anims)
            {
                anims.SetBool("isMoving", true);

            }

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
            if (anims)
            {
                // On finishing movement, return to idle
                anims.SetBool("isMoving", false);

            }

            //Check if we need to attack something as royal guard
            if (royalGuard)
            {
                bool guardShouldIdle = true;
                // If target is enemy, attack. Add coroutine for attacking
                if (_inRange.Count > 0 && currentTarget)
                {
                    if (team == 1 && currentTarget.tag != "Player") // And can attack
                    {
                        StartCoroutine(Attack());
                        guardShouldIdle = false;
                    }
                    else if (team == 2)
                    {
                        StartCoroutine(Attack());
                        guardShouldIdle = false;
                    }
                }
                //no target to attack so we can idle
                if (!coroutineStarted && guardShouldIdle)
                {
                    StartCoroutine(idleDelay());
                }

            }

            //Idle for workers
           else  if (!coroutineStarted)
            {
                StartCoroutine(idleDelay());
                if (builder)
                {
                    //swap current loc with staged location
                    swapTarget();
                }

                else if (worker)
                {
                    swapTarget();
                }
            }

            //Responsible for ending Coroutine
            if (MovingInIdle)
                MovingInIdle = false;



        }

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
            // Debug.Log("Told to Idle in Radius");
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
                //To:Do 
                //If Working Play A working Animation while standing still
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
            if (_printStatements)
                Debug.LogError("Told to start timer");
            ShouldIdle = true;

            float currentTime = Time.time;
            float ExitTime = currentTime + WaitDuration;



            if (builder)
            {
                // set anim bool/trigger to true
                if (anims)
                {
                    // Fix for builder
                    flipDirection();
                    anims.SetBool("isBuilding", true);
                }

            }
            else if (worker)
            {
                if (anims)
                {
                    anims.SetBool("isWorking", true);
                }
            }


            while (currentTime < ExitTime)
            {
                currentTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(WaitDuration);
            ShouldIdle = false;

            //set false

            if (builder)
            {
                flipDirection();
                anims.SetBool("isBuilding", false);
            }
            else if (worker)
            {
                if (anims)
                {
                    anims.SetBool("isWorking", false);
                }
            }

            if (_printStatements)
                Debug.Log("Exit Idle Timer");

        }
    }
    /** While this is active, the subject will repeatedly move to a location, Upon Reaching that location will wait again before exiting */
    IEnumerator IdleMovetoLoc(Vector3 pos)
    {

        //Debug.LogWarning("Enter Actual Move Coroutine");
        MovingInIdle = true;
        coroutineStarted = true;
        while (MovingInIdle)
        {
            Move(pos);
            yield return new WaitForSeconds(Time.deltaTime);

        }
        yield return new WaitForSeconds(WaitDuration);
        coroutineStarted = false;

    }


    void flipDirection()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;


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

    // TODO: Cases for Worker, RoyalGuard, and Builder specific behavior
    private void royalGuardBehavior()
    {
        // Follow the king at all times.
        // Future: Attack enemies within a radius of the king
        if (!ShouldIdle)
        {
            //FindNextTargetInRange();
            Move(currentTarget);

        }
        else
            idleInRadius(2);

    }

    public void AgroRadiusTrigger(Collider2D collision)
    {

        // Add a target to the list based on collisions
        //Debug.Log("Collided with " + collision.gameObject.ToString());

        //check that it HAS a parent

        if (collision.transform.parent)
        {
            Rodent unknownRodent = collision.transform.parent.gameObject.GetComponent<Rodent>();
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
                        print("Newest target added to queue");
                        currentTarget = unknownRodent.gameObject;
                    }
                }


            }
            // Do building case when functional

        }
    }
    private int getEnemyTeam()
    {
        int enemyTeam;

        if (team == 0)
            enemyTeam = -1;
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
           Rodent  _EnemyRodent = currentTarget.GetComponent<Rodent>();

            if (_EnemyRodent)
            {
                if (!_EnemyRodent.isDead())
                {// Reduce enemy health
                    currentTarget.GetComponent<Rodent>().Damage(attackDamage);
                }
                else
                {
                    _inRange.Remove(currentTarget);
                   // print("Called from Dead Rodent found");
                    FindNextTargetInRange();
                }
            }
            // Building here


            yield return new WaitForSeconds(1.16f); //Length of attack animation
            canAttack = true;

        }


    }


    // Removes a target from the list if it exits the rodent's range
    public void removefromAgroRange(Collider2D c)
    {
        // Remove objects from the list
        GameObject go = c.gameObject;
        if (_inRange.Contains(go))
        {

            if (go == currentTarget || currentTarget == null)
            {
                print("Called from Remove from AgroRange");
                FindNextTargetInRange();
            }

            _inRange.Remove(go);
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
            Debug.LogWarning("InRange[0] =" + currentClosest);
            float closestDist = Mathf.Abs(transform.position.x - _inRange[0].transform.position.x);

            foreach (GameObject go in _inRange)
            {
                float tempDist = Mathf.Abs(transform.position.x - go.transform.position.x);
                if (tempDist < closestDist)
                {
                    closestDist = tempDist;
                    currentClosest = go;
                    Debug.Log("Found new target.");
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

    private void workerBehavior()
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
        if (state.Equals("move"))
        {
            if (worker)
                return Random.Range(4, 10);
            else if (royalGuard)
                return Random.Range(1, 2.5f);
            else if (builder)
                return Random.Range(5, 10f);
        }
        else if (state.Equals("idle"))
        {
            if (worker)
                return Random.Range(0.5f, 1f);
            else if (royalGuard)
                return Random.Range(1, 2f);  // will follow player a lot better
            else if (builder)
                return Random.Range(0.1f, 1f);
        }

        return Random.Range(2, 5);
    }

    //As a rodent type changes, we figure out from here
    public void setAttackDamage(float damage)
    {
        attackDamage = damage+15;
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
                if (anims)
                    anims.SetBool(ATK_ANIMATION_TRIGGER, true);

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
           Debug.LogError("Were NOT under attack @" + Time.time + "  " + this.gameObject);
        }
    }

    public void SaveLastJob()
    {
        if (worker)
            _oldJob = "worker";
        else if (builder)
            _oldJob = "builder";
        else if (royalGuard)
            _oldJob = "royalGuard";
        else
            _oldJob = "idle";

        savedTarget2 = currentTarget;
        royalGuard = true;
        worker = false;
        builder = false;
        FindNextTargetInRange();
    }
    private void restoreLastJob()
    {
        switch(_oldJob)
        {
            case "worker":
                {
                    royalGuard = false;
                    worker = true;
                    currentTarget = savedTarget2;
                    //Tell villagers to put weapons away
                    if (anims)
                        anims.SetBool(ATK_ANIMATION_TRIGGER, false);
                    break;
                }
            case "builder":
                {
                    royalGuard = false;
                    builder = true;
                    currentTarget = savedTarget2;
                    //Tell villagers to put weapons away
                    if (anims)
                        anims.SetBool(ATK_ANIMATION_TRIGGER, false);
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
                    if (anims)
                        anims.SetBool(ATK_ANIMATION_TRIGGER, false);
                    break;
                }


        }


    }

    //Unused but found this cool way to turn off ALL parameters EXCEPT one in controller
    private void DisableOtherAnimations(Animator animator, string animation)
    {
        foreach(AnimatorControllerParameter parameter in animator.parameters)
        {
            if(parameter.name != animation)
            {
                animator.SetBool(parameter.name, false);
            }
        }
    }
}
