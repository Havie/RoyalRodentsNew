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
        var _collider = this.transform.GetComponent<Collider2D>();
        _collider.enabled = cond;
    }
    //COLLISION
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_OnPlayer)
        {
            if (this.transform.parent)
            {
                Transform t = this.transform.parent;


                if (collision.transform.GetComponent<Searchable>() && !_DigCollider)
                {
                    t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);
                }
                else if (collision.transform.GetComponent<DiggableTile>() && _DigCollider)
                {
                    t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);
                }
                else if (collision.transform.parent && !_DigCollider)
                {
                    // handle if collider is agro range or base range
                    if (collision.transform.GetComponent<BaseHitBox>())
                    {

                        if (collision.transform.parent.GetComponent<BuildableObject>())
                        {
                            //Add to our list of interactable things in range
                            t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);
                        }

                        else if (collision.transform.parent.GetComponent<Rodent>())
                        {
                            //Add to our list of interactable things in range
                            t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);
                        }
                    }
                }

            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_OnPlayer)
        {
            if (this.transform.parent)
            {
                Transform t = this.transform.parent;


                if (collision.transform.GetComponent<Searchable>() && !_DigCollider)
                {
                    t.GetComponent<PlayerMovement>().OnTriggerExit2D(collision);
                }
                else if (collision.transform.GetComponent<DiggableTile>() && _DigCollider)
                {
                    t.GetComponent<PlayerMovement>().OnTriggerExit2D(collision);
                }
                else if (collision.transform.parent && !_DigCollider)
                {
                    // handle if collider is agro range or base range
                    if (collision.transform.GetComponent<BaseHitBox>())
                    {

                        if (collision.transform.parent.GetComponent<BuildableObject>())
                        {
                            //Add to our list of interactable things in range
                            t.GetComponent<PlayerMovement>().OnTriggerExit2D(collision);
                        }

                        else if (collision.transform.parent.GetComponent<Rodent>())
                        {
                            //Add to our list of interactable things in range
                            t.GetComponent<PlayerMovement>().OnTriggerExit2D(collision);
                        }
                    }
                }

            }

        }
    }

}
