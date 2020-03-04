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
    public Animator anims;
    public float moveSpeed = 0.5f;
    public GameObject currentTarget;
    public GameObject savedTarget;
    public Vector3 IdlePos;
    private bool facingRight;
    private bool royalGuard = false;
    private bool worker = false;
    private bool builder = true;
    private bool coroutineStarted = false;
    private bool ShouldIdle = false;
    private bool MovingInIdle = false;
    private float WaitDuration;

    private bool _printStatements = false;

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
        // Check if a target exists
        if (currentTarget)
        {
            // TODO: branches with each class's behavior built in.
            if (royalGuard)
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
                idleInRadius(IdlePos,5); 
            }
        }
        else
        {
            //free movement for rats with no target
            idleInRadius(IdlePos, 5);
        }
    }

    /** Sets rodent roles, ensuring there is only 1 active at a time */
    public void setRoyalGuard()
    {
        royalGuard = true;
        worker = false;
        builder = false;

        currentTarget = GameObject.FindGameObjectWithTag("Player");
    }

    public void setWorker()
    {
        royalGuard = false;
        worker = true;
        builder = false;

        //Get TownCenter location
        GameObject centerLocation = GameManager.Instance.getTownCenter().transform.gameObject;
        savedTarget = centerLocation;
    }

    public void setBuilder()
    {
        royalGuard = false;
        worker = false;
        builder = true;

        //Get TownCenter location
        GameObject centerLocation = GameManager.Instance.getTownCenter().transform.gameObject;
        savedTarget = centerLocation;
    }
    public void setIdle()
    {
        royalGuard = false;
        worker = false;
        builder = false;
        //changeTarget(this.gameObject);  // shouldnt need to do this
        IdlePos = this.transform.position;


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
       if(!ShouldIdle)
            Move(target.transform.position);
    }
    /** Will move to location if far enough away, otherwise will try to idle */
    void Move(Vector3 loc)
    {
        if (_printStatements)
            Debug.Log("Told to Move to Loc  " + loc);

        //LINE 133 is Bugged
        //Line 134 is bugged
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

            //Responsible for starting Coroutine
            if (!coroutineStarted)
            {
                StartCoroutine(idleDelay());
                if (builder)
                {
                    //swap current loc with staged location
                    swapTarget();
                }

                if (worker)
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

        if(currentTarget==null)
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
            if(_printStatements)
                Debug.LogError("Told to start timer");
            ShouldIdle = true;

            float currentTime = Time.time;
            float ExitTime = currentTime + WaitDuration;

            if(builder)
            {
                // set anim bool/trigger to true
            }
            else if(worker)
            {
                //set anim /bool trigger to true
            }


            while (currentTime < ExitTime)
            {
                currentTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(WaitDuration);
            ShouldIdle = false;

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
        //No Print Statements Here 290
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
        for( int i=0; i<index; ++i)
        {
            Transform t = this.transform.GetChild(i);
           ScaleKeeper sk= t.GetComponent<ScaleKeeper>();
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
            // FindAttackTarget();
            Move(currentTarget);
            if (_printStatements)
                Debug.LogError("RoyalMove");
        }
        else
            idleInRadius(8);

    }

    public void FindAttackTarget(Collision2D collision)
    {
        // Code assumes that currentTarget will always be an enemy rodent or building if it is not the player
        float closestDistance = 10f;

        // Check if current target is not king. if so, closest distance = current target
        if (!(currentTarget.tag == "Player"))
        {
            closestDistance = currentTarget.transform.position.x;
        }
        // Look at the new collision, check if it's an enemy, and compare it to closest distance

        // Rodent case
        if (collision.transform.gameObject.GetComponent<Rodent>())
        {
            // Check for enemy team
            if (collision.transform.gameObject.GetComponent<Rodent>().getTeam() == 2)
            {
                // Compare distance
                if (Mathf.Abs(this.transform.position.x - collision.transform.position.x) < Mathf.Abs(this.transform.position.x - currentTarget.transform.position.x))
                {
                    // Set new target
                    currentTarget = collision.transform.gameObject;
                    Debug.Log("Target changed to " + currentTarget.ToString());
                }
            }
        }

        //Building case
        else if (collision.transform.gameObject.GetComponent<BuildableObject>())
        {
            // Check for enemy team
            //if (collision.transform.gameObject.GetComponent<BuildableObject>().getTeam() == 2)
            //{
            // Compare distance
                //if (Mathf.Abs(this.transform.position.x - collision.transform.position.x) < Mathf.Abs(this.transform.position.x - currentTarget.transform.position.x))
                //{
                //// Set new target
                //currentTarget = collision.transform.gameObject;
                //}
            //}

        }

        // When target dies, find new target? Or reset to king and let the collider try to find a new one

    }

    // Resets the rodent's target back to the original current, now saved in the secondary target
    public void swapTarget()
    {
        GameObject tempTarget = currentTarget;
        currentTarget = savedTarget;
        savedTarget = tempTarget;
        Debug.Log("Target changed to " + currentTarget.ToString());
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
        else {
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
                return Random.Range(1, 4f);
            else if (builder)
                return Random.Range(5, 10f);
        }
        else if(state.Equals("idle"))
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
}
