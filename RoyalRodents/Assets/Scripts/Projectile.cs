﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float hSpeed = 1f;
    private float vSpeed = 20f;
    public Rigidbody2D rb;
    private Vector2 targetPosition;
    
    void Start()
    {
        //set default targetposition
        targetPosition = new Vector2(transform.position.x, transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        //Debug.Log(hitInfo.name);
        Destroy(gameObject);
    }

    public void setTarget(Vector2 pos)
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
        rb.velocity = (transform.right * hSpeed) + (transform.up * vSpeed);
    }
}