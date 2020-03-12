using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHitBox : MonoBehaviour
{
    [SerializeField]
    private bool _OnPlayer;
    [SerializeField]
    private bool _DigCollider;

    //OLD - unused at the moment
    public void turnOnCollider(bool cond)
    {
       // Debug.Log("Turn collider " + cond + " FOR:" + this.transform.gameObject);
        var _collider= this.transform.GetComponent<Collider2D>();
        _collider.enabled = cond;
    }
    //COLLISION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.transform.parent)
        {
            Transform t = this.transform.parent;

            if (t)
            {
                // Debug.Log("Enter T");
                if (t.GetComponent<PlayerMovement>())
                {
                    //Will have to refactor these to Check the type here, then based on bools send in to PlayerMovement
                    if (_OnPlayer && _DigCollider)
                        t.GetComponent<PlayerMovement>().OnCollisionEnter2D(collision);
                }
               else if (t.GetComponent<BuildableObject>())
                {
                    // Debug.Log("Enter BO");
                    if (_OnPlayer && _DigCollider)
                        t.GetComponent<PlayerMovement>().OnCollisionEnter2D(collision);

                }
                else if (t.GetComponent<Rodent>())
                {
                    //  Debug.Log("Enter SubjectScript");
                    if(_OnPlayer && _DigCollider)
                        t.GetComponent<PlayerMovement>().OnCollisionEnter2D(collision);
                    //To:Do else
                    // Tell the Subject Script 

                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (this.transform.parent)
        {
            Transform t = this.transform.parent;

            if (t)
            {
                // Debug.Log("Enter T");
                if (t.GetComponent<PlayerMovement>())
                {
                    if (_OnPlayer && _DigCollider)
                        t.GetComponent<PlayerMovement>().OnCollisionExit2D(collision);
                }
                else if (t.GetComponent<BuildableObject>())
                {
                    // Debug.Log("Enter BO");
                    if (_OnPlayer && _DigCollider)
                        t.GetComponent<PlayerMovement>().OnCollisionExit2D(collision);

                }
                else if (t.GetComponent<Rodent>())
                {
                    //  Debug.Log("Enter SubjectScript");
                    if (_OnPlayer && _DigCollider)
                        t.GetComponent<PlayerMovement>().OnCollisionExit2D(collision);
                    //To:Do else
                    // Tell the Subject Script 

                }
            }
        }
    }
    //TRIGGER
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.transform.parent)
        {
            Transform t = this.transform.parent;

            if (t)
            {
                // Debug.Log("Enter T");
                if (t.GetComponent<PlayerMovement>())
                {
                    if (_OnPlayer)
                        t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);

                }
                else if (t.GetComponent<BuildableObject>())
                {
                    // Debug.Log("Enter BO");
                    if (_OnPlayer)
                        t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);

                }
                else if (t.GetComponent<Rodent>())
                {
                    //  Debug.Log("Enter SubjectScript");
                    if (_OnPlayer)
                        t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);
                    //To:Do
                    // Tell Subject script
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (this.transform.parent)
        {
            Transform t = this.transform.parent;

            if (t)
            {
                // Debug.Log("Enter T");
                if (t.GetComponent<PlayerMovement>())
                {
                    if (_OnPlayer)
                        t.GetComponent<PlayerMovement>().OnTriggerExit2D(collision);

                }
                else if (t.GetComponent<BuildableObject>())
                {
                    // Debug.Log("Enter BO");
                    if (_OnPlayer)
                        t.GetComponent<PlayerMovement>().OnTriggerExit2D(collision);

                }
                else if (t.GetComponent<Rodent>())
                {
                    //  Debug.Log("Enter SubjectScript");
                    if (_OnPlayer)
                        t.GetComponent<PlayerMovement>().OnTriggerExit2D(collision);
                    //To:Do
                    // Tell Subject script
                }
            }
        }
    }
}
