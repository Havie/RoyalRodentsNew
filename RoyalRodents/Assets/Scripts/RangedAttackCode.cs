using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackCode : MonoBehaviour
{
    public GameObject projectilePrefab; //currently called RangeProjectile

    void Update()
    {
        //public GameObject projectile;
        if (Input.GetKeyDown(KeyCode.F))
        {
            //targetLocation is the location of the enemy for the projectile to track, change the x position based on the enemy x pos, keep the same y as the thing shooting
            Vector2 targetLocation = new Vector2(transform.position.x-20, transform.position.y);

            Shoot(targetLocation);
        }
    }

    void Shoot(Vector2 shootTargetCoordinate)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.transform.parent = gameObject.transform;
        projectile.GetComponent<Projectile>().setTarget(shootTargetCoordinate);
    }
}
 