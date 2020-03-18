using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleKeeper : MonoBehaviour
{

    public Vector3 _Scale;



    private void Awake()
    {
        _Scale = this.transform.localScale;
    }


    public Vector3 getScale()
    {
        return _Scale;
    }
}
