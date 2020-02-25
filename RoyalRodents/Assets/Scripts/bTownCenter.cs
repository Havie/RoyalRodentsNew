using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bTownCenter : MonoBehaviour
{
    [SerializeField]
    private Sprite _built;
    private float _hitpoints = 250;

    //create strucutre costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
    public static Dictionary<string, int> _costLevel1 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel2 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel3 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel4 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel5 = new Dictionary<string, int>();


    public bTownCenter() // calls BuildObjectConstructor by default
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        _built = Resources.Load<Sprite>("Buildings/TownCenter/trash_town_center");

        //Set Upgrade/Build Costs (1-5 levels)
        _costLevel1.Add("Trash", 2);

        _costLevel2.Add("Trash", 4);
        _costLevel2.Add("Wood", 2);

        _costLevel3.Add("Trash", 6);
        _costLevel3.Add("Wood", 4);
        _costLevel3.Add("Metal", 2);

        _costLevel4.Add("Trash", 6);
        _costLevel4.Add("Wood", 4);
        _costLevel4.Add("Metal", 2);

        _costLevel5.Add("Trash", 6);
        _costLevel5.Add("Wood", 4);
        _costLevel5.Add("Metal", 2);
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
