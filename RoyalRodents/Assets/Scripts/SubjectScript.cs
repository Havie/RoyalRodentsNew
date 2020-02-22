using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Behavior script for rats in the player base.
 * - CURRENT FUNCTIONALITY -
 *      - Rats will run to whatever object is assigned and wait there.
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
    private bool facingRight;
    private bool royalGuard = true;
    private bool worker = false;
    private bool builder = false;
    private bool coroutineStarted = false;
    private bool ShouldIdle = false;
    private bool MovingInIdle = false;
    private Vector3 IdlePos;
    private float WaitDuration;

    private bool _printStatements = false;

    // Start is called before the first frame update
    void Start()
    {
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

        }
        else
        {
            //TODO: free movement for rats with no target
        }
    }

    // Set rodent roles, ensuring there is only 1 active at a time
    public void setRoyalGuard()
    {
        royalGuard = true;
        worker = false;
        builder = false;
    }

    public void setWorker()
    {
        royalGuard = false;
        worker = true;
        builder = false;
    }

    public void setBuilder()
    {
        royalGuard = false;
        worker = false;
        builder = true;
    }

    public void setSpeed(float nSpeed)
    {
        this.moveSpeed = nSpeed;
    }

    float getSpeed()
    {
        return moveSpeed;
    }
    private void Move(GameObject target)
    {
        if (_printStatements)
            Debug.Log("Told to Move to Target" + target);
       if(!ShouldIdle)
            Move(target.transform.position);
    }
    // Finds target object position, and then commands the rat to move if it is not very close to it already
    void Move(Vector3 loc)
    {
        if (_printStatements)
            Debug.Log("Told to Move to Loc  " + loc);

        Vector3 pos = new Vector3(loc.x, 0, 0);
        float _ranDistance = Random.Range(0.1f, 1.5f); //might make global and unique to role

        //If we are far enough away
        if (Mathf.Abs(pos.x - transform.position.x) > _ranDistance)
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
                StartCoroutine(idleDelay());
            //Responsible for ending Coroutine
            if (MovingInIdle)
                MovingInIdle = false;

        }

    }

    /**If there character should Idle, This will pick a location nearby and start a coroutine if one isnt already started */
    void idleInRadius(int radius)
    {
        if (!coroutineStarted)
        {
            // Debug.Log("Told to Idle in Radius");
            int _chanceToMove = Random.Range(-radius, radius);
            if (_chanceToMove > 2)
            {
                //Move Right
                Vector3 pos = new Vector3(Random.Range((currentTarget.transform.position.x +1f), (currentTarget.transform.position.x + 6.5f)), 0, 0);
                
                    if (_printStatements)
                        Debug.Log("Move Positively to::" + pos);
                StartCoroutine(IdleMovetoLoc(pos));
                WaitDuration = Random.Range(5, 10);
            }
            else if (_chanceToMove < -2)
            {
                //Move Left
                Vector3 pos = new Vector3(Random.Range((currentTarget.transform.position.x - 6.5f), (currentTarget.transform.position.x-1f)), 0, 0);
                if (_printStatements)
                    Debug.Log("Move Negatively to::" + pos);
                StartCoroutine(IdleMovetoLoc(pos));
                WaitDuration = Random.Range(5, 10);
            }
            else
            {
                //Stand Still
                if (_printStatements)
                    Debug.Log("StandStill::" + transform.position);
                StartCoroutine(IdleMovetoLoc(transform.position));
                WaitDuration = Random.Range(2, 5);
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

            while (currentTime < ExitTime)
            {
                currentTime += Time.deltaTime;
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
        while (MovingInIdle)
        {
            Move(pos);
            yield return new WaitForSeconds(0.05f);

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
    }

    // Assign rodent's current target. 
    public void changeTarget(GameObject nTarget)
    {
        Debug.Log("Changing Target to " + nTarget);
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
        { Move(currentTarget);
            if (_printStatements)
                Debug.LogError("RoyalMove"); }
        else
            idleInRadius(8);

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
                Debug.LogError("RoyalMove");
        }
        else
            idleInRadius(10);

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
                Debug.LogError("RoyalMove");
        }
            // not sure this one will idle
            // instead it might reach here and play an animation
            // or Idle in radius 0 and play an anim in there?
    }
}
