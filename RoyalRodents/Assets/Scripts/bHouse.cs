using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bHouse :MonoBehaviour
{
    private Sprite _built;
    private float _hitpoints = 50;
	public static Dictionary<string, int> _costLevel1 = new Dictionary<string, int>();


    // Start is called before the first frame update
    void Start()
    {
        _built = Resources.Load<Sprite>("TmpAssests/Alex/monolith restored_y");
		_costLevel1.Add("Trash", 4);

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
