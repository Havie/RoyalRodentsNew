using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHitBox : MonoBehaviour
{
    [SerializeField]
    private bool _OnPlayer;
    [SerializeField]
    private bool _OnBuilding;
    [SerializeField]
    private bool _DigCollider;

    private GameObject _MoveDummy;
    private void Start()
    {
        FigureWhoWereOn();

    }
    private void FigureWhoWereOn()
    {
        PlayerMovement player = this.transform.GetComponentInParent<PlayerMovement>();
        BuildableObject buiding = this.transform.GetComponentInParent<BuildableObject>();
        if (player)
        {
            _OnPlayer = true;
            _OnBuilding = false;
            _MoveDummy = player.getDummy();
        }

        if (buiding)
        {
            _OnPlayer = false;
            _OnBuilding = true;
        }

    }

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
        if (_OnPlayer && _OnBuilding)
            Debug.LogWarning("Cant have a collider set to building and player!...");

        if (_OnPlayer)
        {
            if (this.transform.parent)
            {
                Transform t = this.transform.parent;

                //This all gets double checked in PM, but not refactoring till this is final.

                if (collision.gameObject == _MoveDummy && !_DigCollider)
                    t.GetComponent<PlayerMovement>().OnTriggerEnter2D(collision);

                else if (collision.transform.GetComponent<Searchable>() && !_DigCollider)
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
        else if (_OnBuilding)
        {
            if (this.transform.parent)
            {
                Transform t = this.transform.parent;
                if (t)
                {
                    bBanner banner = t.GetComponent<bBanner>();
                    if (banner)
                    {
                        // handle if collider is agro range or base range
                        if (collision.transform.GetComponent<AttackRadius>())
                        {
                            Transform colliderParent = collision.gameObject.transform.parent;
                            if (colliderParent)
                            {
                               // Debug.Log(banner.gameObject.name
                                    //    + " Collided with :" + colliderParent.gameObject);

                                Rodent r = colliderParent.GetComponent<Rodent>();
                                if (r)
                                {
                                    //Set Max HP
                                    //Debug.Log("HP was:" + r.getHpMax());
                                    r.setHpMax(r.getHpMax() * banner.getHPBonus());
                                    // Debug.Log("HP now:" + r.getHpMax());

                                    // Set HP
                                    r.setHp(r.getHp() * banner.getHPBonus());

                                    //set Gathering Bonuses
                                   // print(r.name + " gather was: " + r.getGatherRate());
                                    r.setGatherRate((int) (r.getGatherRate() * banner.getGatherBonus()));
                                  //  print(r.name + " gather is now: " + r.getGatherRate());

                                }
                            }
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
        else if (_OnBuilding)
        {
            if (this.transform.parent)
            {
                Transform t = this.transform.parent;
                if (t)
                {
                    bBanner banner = t.GetComponent<bBanner>();
                    if (banner)
                    {
                        // handle if collider is agro range or base range
                        if (collision.transform.GetComponent<AttackRadius>())
                        {
                            Transform colliderParent = collision.gameObject.transform.parent;
                            if (colliderParent)
                            {
                               // Debug.Log(banner.gameObject.name
                                    //    + " Collided with :" + colliderParent.gameObject);

                                Rodent r = colliderParent.GetComponent<Rodent>();
                                if (r)
                                {
                                    //Set Max HP
                                   // Debug.Log("HP was:" + r.getHpMax());
                                    r.setHpMax(r.getHpMax() / banner.getHPBonus());
                                   // Debug.Log("HP now:" + r.getHpMax());

                                    // Set HP
                                    r.setHp(r.getHp() / banner.getHPBonus());

                                    //Undo Gather bonus
                                    r.setGatherRate((int)(r.getGatherRate() / banner.getGatherBonus()));

                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
