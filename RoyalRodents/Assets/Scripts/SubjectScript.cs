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

    // Start is called before the first frame update
    void Start()
    {
        facingRight = false;
        // a backup condition to get the right speed
        Rodent r = this.GetComponent<Rodent>();
        if (r)
            moveSpeed=r.getSpeed();

        
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
                
                if (!coroutineStarted)
                {
                    royalGuardBehavior();
                    
                }
               
            }
            else if (worker) {
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

    public void setWorker(){
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

    // Finds target object position, and then commands the rat to move if it is not very close to it already
    void Move(GameObject target)
    {
        Vector3 pos = new Vector3(target.transform.position.x, 0, 0);

        if (anims)
        {
            anims.SetBool("isMoving", true);
        }
        
        if(Mathf.Abs(pos.x - transform.position.x) > 2.5f)
        {
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
        else
        {
            if (anims)
            {
                // On finishing movement, return to idle
                anims.SetBool("isMoving", false);
                
            }
            StartCoroutine(idleDelay());
        }
        
    }

    void idleInRadius(){
        Vector3 pos = new Vector3(Random.Range((currentTarget.transform.position.x - 1.5f), (currentTarget.transform.position.x +1.5f)), 0 , 0);

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

        if (anims)
        {
            // On finishing movement, return to idle
            anims.SetBool("isMoving", false);
        }
        StartCoroutine(idleDelay());

    }

    IEnumerator idleDelay()
    {
        coroutineStarted = true;
        Debug.Log("Your enter Coroutine at" + Time.time);
        yield return new WaitForSeconds(5.0f);
        Debug.Log("Your exit oroutine at" + Time.time);
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
        Move(currentTarget);
        idleInRadius();
        
    }

    private void workerBehavior()
    {
        // Walk to their assigned building
        // Idle in the area of it
        // Future: Be able to work occupy the building and deliver resources to the town center
        Move(currentTarget);
        
    }

    private void builderBehavior()
    {
        // Walk to their assigned building
        // Future: Be able to carry resources from the town center to the building being constructed
        Move(currentTarget);

    }
}
