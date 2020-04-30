using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float hSpeed = 1f;
    private float vSpeed = 5f;
    private float lifespan = 5f;
    public Rigidbody2D rb;
    private Vector3 targetPosition;
    private int enemyTeam;
    private float attackDamage;
    
    void Start()
    {
        //set default targetposition
        targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void Update()
    {
        // Destroy in 5 seconds, if nothing is hit in that timespan.
        Destroy(gameObject, lifespan);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Put proper hit detection code here
        if (hitInfo.transform.parent)
        {
            Rodent unknownRodent = hitInfo.transform.parent.gameObject.GetComponent<Rodent>();
            PlayerStats king = hitInfo.transform.parent.gameObject.GetComponent<PlayerStats>();
            BuildableObject unknownBuilding = hitInfo.transform.parent.gameObject.GetComponent<BuildableObject>();
            if (unknownRodent)
            {
                if(unknownRodent.getTeam() == enemyTeam)
                {
                    unknownRodent.Damage(attackDamage);
                    Destroy(gameObject);
                }
                
            }
            else if (unknownBuilding)
            {
                if (unknownBuilding.getTeam() == enemyTeam && unknownBuilding.getType() != BuildableObject.BuildingType.TownCenter)
                {
                    unknownBuilding.Damage(attackDamage);
                    Destroy(gameObject);
                }
            }
            else if (king)
            {
                if (enemyTeam == 1)
                {
                    king.Damage(attackDamage);
                    Destroy(gameObject);
                }
            }
        }

        
    }

    public void setEnemyTeam(int team)
    {
        enemyTeam = team;
    }

    public void setDamage(float damage)
    {
        attackDamage = damage;
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
        float deltaX = targetPosition.x - transform.position.x;
        //Debug.Log("PositionX is " + transform.position.x);
        //Debug.Log("TargetX is " + targetPosition.x);
        //Debug.Log("DeltaX is " + deltaX);
        hSpeed = deltaX / time;

        //Set velocity from hSpeed and vSpeed
        rb.velocity = (transform.right * hSpeed * 5f) + (transform.up * vSpeed);
        //Debug.Log("Shooting at: " + deltaX);
    }
}
