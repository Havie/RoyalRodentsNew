using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bTownCenter : MonoBehaviour
{
    [SerializeField]
    private Sprite _built;
    private float _hitpoints = 250;



    public bTownCenter() // calls BuildObjectConstructor by default
    {

    }



    // Start is called before the first frame update
    void Start()
    {
<<<<<<< HEAD
        _built = Resources.Load<Sprite>("Buildings/TownCenter/trash_town_center");
=======
        _built = Resources.Load<Sprite>("TmpAssests/Alex/monolith restored6");
>>>>>>> parent of 16935df... Resource System Work

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
        if(!_built)
        {
            _built = Resources.Load<Sprite>("Buildings/TownCenter/trash_town_center");

        }
        this.transform.GetComponent<BuildableObject>().SetType("TownCenter");
        this.transform.GetComponent<BuildableObject>().Damage(0 - _hitpoints);
        this.transform.GetComponent<SpriteRenderer>().sprite = _built;
        Debug.Log("Created Initial TownCenter with Sprite:::" + _built);
    }


}
