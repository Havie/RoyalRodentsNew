using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            Debug.LogError("Camera has no target will crash");

        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
    }
}
