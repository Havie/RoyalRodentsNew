using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bTownCenter : MonoBehaviour
{
    private Sprite _built;
    private float _hitpoints = 150;



    public bTownCenter() // calls BuildObjectConstructor by default
    {

    }



    // Start is called before the first frame update
    void Start()
    {
        _built = Resources.Load<Sprite>("TmpAssests/Alex/monolith restored6");

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

    public void StartingBuildComplete()
    {
        this.transform.GetComponent<BuildableObject>().SetType("TownCenter");
        this.transform.GetComponent<BuildableObject>().Damage(0 - _hitpoints);
        this.transform.GetComponent<SpriteRenderer>().sprite = _built;
    }


}
