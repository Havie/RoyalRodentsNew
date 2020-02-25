using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bTower : MonoBehaviour
{
    private Sprite _built;
    private float _hitpoints = 50;

    //create strucutre costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
    public static Dictionary<string, int> _costLevel1 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel2 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel3 = new Dictionary<string, int>();

    public bTower() // calls BuildObjectConstructor by default
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        _built = Resources.Load<Sprite>("TmpAssests/Alex/monolith restored5");

        //Set Upgrade/Build Costs (1-3 levels)
        _costLevel1.Add("Trash", 2);

        _costLevel2.Add("Trash", 4);
        _costLevel2.Add("Wood", 2);

        _costLevel3.Add("Trash", 6);
        _costLevel3.Add("Wood", 4);
        _costLevel3.Add("Metal", 2);
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
