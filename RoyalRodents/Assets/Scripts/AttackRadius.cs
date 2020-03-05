using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadius : MonoBehaviour
{
    [SerializeField]
    private SubjectScript _subjectScript;
    private BoxCollider2D _range;

    // Start is called before the first frame update
    void Start()
    {
        _range = this.GetComponent<BoxCollider2D>();
       
        _subjectScript = transform.parent.GetComponent<SubjectScript>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onCollisionEnter(Collision2D collision)
    {
        Debug.Log("Collided with " + collision.transform.gameObject.ToString());

        // Send collider, check for properties in script
        _subjectScript.FindAttackTarget(collision); 
    }
    private void OnCollisionExit(Collision collision)
    {
        _subjectScript.removefromRange(collision);
    }
}
