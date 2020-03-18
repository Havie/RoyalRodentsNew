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


    private void OnTriggerEnter2D(Collider2D collision)
    {
       //Debug.Log("Collided with " + collision.transform.gameObject.ToString());

        // Send collider, check for properties in script
        _subjectScript.AgroRadiusTrigger(collision); 
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
       // Debug.Log("Target exited aggro range");
        _subjectScript.removefromAgroRange(collision);
    }
}
