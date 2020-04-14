using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float hSpeed = 1f;
    private float vSpeed = 5f;
    public Rigidbody2D rb;
    private Vector3 targetPosition;
    public int enemyTeam;
    public float attack;
    
    void Start()
    {
        //set default targetposition
        targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void Update()
    {
        // If the projectile goes under the map, destroy it
        
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Put proper hit detection code here
        Rodent r = hitInfo.gameObject.GetComponent<Rodent>();
        
        // Check for enemy rodent
        if (r)
        {
            if(r.getTeam() == enemyTeam)
            {
                Debug.Log("Call Damage");
                r.Damage(attack);
                Destroy(gameObject);
            }
        }
        else
        {
            // Check for enemy building
            BuildableObject b = hitInfo.gameObject.GetComponent<BuildableObject>();
            if (b)
            {
                if (b.getTeam() == enemyTeam)
                {
                    b.Damage(attack);
                    Destroy(gameObject);
                }
            }
        }
        
    }

    public void setTarget(Vector3 pos)
    {
        targetPosition = pos;
        setVelocity();
    }

    public void setVelocity()
    {
        //Calculate time in air based on vertical velocity
        float time = vSpeed / rb.gravityScale;

        //Calculate correct horizonal velocity based on time in air
        float deltaX = -1 * (targetPosition.x - transform.position.x);
        //Debug.Log("PositionX is " + transform.position.x);
        //Debug.Log("TargetX is " + targetPosition.x);
        
        hSpeed = deltaX / time;

        //Set velocity from hSpeed and vSpeed
        rb.velocity = (transform.right * hSpeed) + (transform.up * vSpeed);
    }
}
