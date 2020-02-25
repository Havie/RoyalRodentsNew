using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bFarm : MonoBehaviour
{
    private Sprite _built;
    private float _hitpoints = 50;



    public bFarm() // calls BuildObjectConstructor by default
    {

    }



    // Start is called before the first frame update
    void Start()
    {
        _built = Resources.Load<Sprite>("Buildings/Farm/trash_farm");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public float BuildingComplete()
    {
        this.transform.GetComponent<SpriteRenderer>().sprite = _built;
        return _hitpoints;
    }


}
