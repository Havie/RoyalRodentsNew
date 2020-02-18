using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Behavior script for rats in the player base.
 * - CURRENT FUNCTIONALITY -
 *      - Rats will run to whatever object is assigned and wait there.
 *      
 * - PLANNED FUTURE FUNCTIONALITY -
 *      - Rats will be assigned buildings to run towards.
 *      - After reaching their target, they will occupy that building.
 *      - If not assigned a building, rats will run around the base.
 */
public class SubjectScript : MonoBehaviour
{
    public Animator anims;
    public float health = 30f;
    public float moveSpeed = 0.5f;
    public GameObject target;
    private bool facingRight;

    // Start is called before the first frame update
    void Start()
    {
        facingRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if a target exists
        if (target)
        {
            // Finds target object position, and then commands the rat to move if it is not very close to it already
            Vector3 targetLocation = new Vector3(target.transform.position.x, 0, 0);
            if(Mathf.Abs(targetLocation.x - transform.position.x) > 0.5f)
            {
                Move(targetLocation);
            }
            else
            {
                anims.SetFloat("currentSpeed", 0);

            }
        }
    }

    // Moves the rat towards its target
    void Move(Vector3 pos)
    {
        anims.SetFloat("currentSpeed", 1);
        if (transform.position.x > pos.x)
        {

            if (facingRight)
            {
                flipDirection();
            }

            if(pos.x >= 0)
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
            if (!facingRight)
            {
                flipDirection();
            }
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

    void flipDirection()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // Reassign rodent's target
    void changeTarget(GameObject nTarget)
    {
        this.target = nTarget;
    }
}
