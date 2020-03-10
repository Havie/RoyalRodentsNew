using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bTownCenter : MonoBehaviour
{
    [SerializeField]
	private Sprite _builtSpriteLevel1;
	private Sprite _builtSpriteLevel2;
	private Sprite _builtSpriteLevel3;
	private Sprite _builtSpriteLevel4;
	private Sprite _builtSpriteLevel5;
	private static int maxLevel = 5;

	private float _hitpoints = 250;
	private float _hitPointGrowth = 10;

	private static bool _isSet;

	//create strucutre costs (costLevel1 is used to BUILD TO level 1, not ON level 1)
	public static Dictionary<string, int> _costLevel1 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel2 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel3 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel4 = new Dictionary<string, int>();
    public static Dictionary<string, int> _costLevel5 = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        _builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/TownCenter/trash_town_center");
		_builtSpriteLevel2 = Resources.Load<Sprite>("Buildings/TownCenter/wood_town_center");
		_builtSpriteLevel3 = Resources.Load<Sprite>("Buildings/TownCenter/stone_town_center");
		_builtSpriteLevel4 = Resources.Load<Sprite>("Buildings/TownCenter/trash_town_center");
		_builtSpriteLevel5 = Resources.Load<Sprite>("Buildings/TownCenter/trash_town_center");
	}

    // Update is called once per frame
    void Update()
    {

    }

	private static void setupCosts()
	{
		if (!_isSet)
		{
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

			_isSet = true;
		}
	}

	public float BuildingComplete(int level)
	{
		if (level == 1)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
		else if (level == 2)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel2;
		else if (level == 3)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel3;
		else if (level == 4)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel4;
		else if (level == 5)
			this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel5;

		return (_hitpoints + (_hitPointGrowth * level));
	}

	public void StartingBuildComplete()
    {
        if(!_builtSpriteLevel1)
        {
            _builtSpriteLevel1 = Resources.Load<Sprite>("Buildings/TownCenter/trash_town_center");
        }
        this.transform.GetComponent<BuildableObject>().SetType("TownCenter");
        this.transform.GetComponent<BuildableObject>().Damage(0 - _hitpoints);
        this.transform.GetComponent<SpriteRenderer>().sprite = _builtSpriteLevel1;
        //MEGA hack of all Hacks
        this.transform.GetComponentInChildren<Employee>().gameObject.GetComponentInChildren<eWorkerOBJ>().gameObject.GetComponent<SpriteRenderer>().sprite = null;
       // Debug.Log("Created Initial TownCenter with Sprite:::" + _built);
    }

	public static Dictionary<string, int> getCost(int level)
	{

		if (_costLevel1.Count == 0)
		{
			setupCosts();
		}

		if (level == 1)
		{
			return _costLevel1;
		}
		else if (level == 2)
		{
			return _costLevel2;
		}
		else if (level == 3)
		{
			return _costLevel3;
		}
		else if (level == 4)
		{
			return _costLevel4;
		}
		else if (level == 4)
		{
			return _costLevel4;
		}
		else if (level == 5)
		{
			return _costLevel5;
		}
		else
			return null;
	}

	public static int getMaxLevel()
	{
		return maxLevel;
	}
}
